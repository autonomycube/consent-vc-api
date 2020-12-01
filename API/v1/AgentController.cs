using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/agents")]
    public class AgentController : ControllerBase {
        #region Private Variables
        private readonly IAgentService _agentService;
        private readonly IProvisioningService _provisionService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public AgentController(
            IAgentService agentService, 
            IProvisioningService provisionService,
            IWalletService walletService,
            IMapper mapper
        ) {
            _agentService = agentService;
            _provisionService = provisionService;
            _walletService = walletService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CreateAgentRequest request)
        {
            if (request.IsInvalid())
            {
                return new BadRequestObjectResult("Invalid request");
            }

            try
            {
                // if a wallet exists - then the user is real - so assume he logged in.
                Wallet wallet = null;
                try
                {
                    wallet = await _walletService.GetWalletAsync(new WalletConfiguration
                    {
                        Id = request.WalletConfigurationId
                    }, 
                    new WalletCredentials
                    {
                        Key = request.WalletKey
                    });
                }
                catch (Exception) {
                    
                }

                if (wallet != null)
                {
                    // if a provision record is there - agent can be assumed to be logged in
                    var provision = await _provisionService.GetProvisioningAsync(wallet);
                    if (provision != null)
                        return new OkObjectResult(new {
                            result = true
                        });
                }

                // if we reach this point - user does not exists and a wallet and provision record is created
                // for him and will be logged in.
                var agentOptions = _mapper.Map<AgentOptions>(request);
                agentOptions.EndpointUri = $"http://api-ssi.consentwallets.com/api/v1/agents/{agentOptions.WalletConfiguration.Id}";
                await _provisionService.ProvisionAgentAsync(agentOptions);

                return new OkObjectResult(new {
                    result = true
                });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
            }
        }

        [HttpPost("{name}")]
        public async Task ProcessMessage([FromRoute] string name, [FromBody] object data) 
        {
            if (HttpContext.Request.ContentLength == null)
                throw new Exception("Empty content length");
            
            using var stream = new StreamReader(HttpContext.Request.Body);
            string body = await stream.ReadToEndAsync();

            IAgent agent = await _agentService.GetAgentAsync(name);

            MessageContext response =
                await agent.ProcessAsync
                (
                    context: await _agentService.GetContextAsync(),
                    messageContext: new PackedMessageContext(body.GetUTF8Bytes())
                );

            HttpContext.Response.StatusCode = 200;

            if (response != null)
            {
                HttpContext.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
                await HttpContext.Response.WriteAsync(response.Payload.GetUTF8String());
            }
            else
            {
                await HttpContext.Response.WriteAsync(string.Empty);
            }
        }
    }
}