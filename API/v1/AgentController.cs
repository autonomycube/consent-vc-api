using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/agents")]
    public class AgentController : ControllerBase {
        #region Private Variables
        private readonly IAgentService _agentService;
        private readonly IProvisioningService _provisionService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public AgentController(
            IAgentService agentService, 
            IProvisioningService provisionService,
            IMapper mapper
        ) {
            _agentService = agentService;
            _provisionService = provisionService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        [HttpPost]
        public async Task CreateAgent([FromBody] CreateAgentRequest request)
        {
            var agentOptions = _mapper.Map<AgentOptions>(request);
            agentOptions.EndpointUri = $"http://localhost:5000/api/v1/agents/{agentOptions.WalletConfiguration.Id}";
            await _provisionService.ProvisionAgentAsync(agentOptions);
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