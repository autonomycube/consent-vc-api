using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Consent_Aries_VC.Data.DTO.Generic;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Infrastructure.Utils;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Indy.WalletApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Consent_Aries_VC.API.v1 
{
    [Route("/api/v1/credentials")]
    public class CredentialsController : ControllerBase 
    {
        #region Private Variables
        private readonly IMessageService _messageService;
        private readonly IAgentService _agentService;
        private readonly IProvisioningService _provisioningService;
        private readonly ICredentialService _credentialService;
        private readonly IConnectionService _connectionService;
        #endregion
        #region Constructor
        public CredentialsController(
            IMessageService messageService,
            IAgentService agentService,
            IProvisioningService provisioningService,
            ICredentialService credentialService,
            IConnectionService connectionService) 
        {
            _messageService = messageService;
            _credentialService = credentialService;
            _agentService = agentService;
            _provisioningService = provisioningService;
            _connectionService = connectionService;
        }
        #endregion

        [HttpPost("issue")]
        public async Task<ApiResponse> IssueCredentials([FromBody] CredentialOfferRequest offerRequest) 
        {
            try
            {
                var agentName = ConsentUtils.agentName(HttpContext);
                var context = await _agentService.GetAgentContext(agentName, agentName);
                var issuer = await _provisioningService.GetProvisioningAsync(context.Wallet);
                var connection = await _connectionService.GetAsync(context, offerRequest.ConnectionId);

                var values = JsonConvert.DeserializeObject<List<CredentialPreviewAttribute>>(offerRequest.CredentialAttributes);

                foreach (CredentialPreviewAttribute attr in values) {
                    attr.MimeType = CredentialMimeTypes.ApplicationJsonMimeType;
                }

                var (offer, _) = await _credentialService.CreateOfferAsync(context, new OfferConfiguration {
                    CredentialDefinitionId = offerRequest.CredentialDefinitionId,
                    IssuerDid = issuer.IssuerDid,
                    CredentialAttributeValues = values
                }, offerRequest.ConnectionId);
                Console.WriteLine(offer);
                
                _ = _messageService.SendAsync(context.Wallet, offer, connection);
                return new ApiResponse("Credential offer issued", true, 200);
            }
            catch (WalletNotFoundException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentNullException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("{name}")]
        public async Task<ApiResponse> GetCredential([FromRoute] string name) 
        {
            try
            {
                var context = await _agentService.GetAgentContext(name, name);
                var credentials = await _credentialService.ListAsync(context);   
                return new ApiResponse("Credentials retrieved", credentials, 200);
            }
            catch (WalletNotFoundException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentNullException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpPut("{credId}")]
        public async Task<ApiResponse> RevokeCredential([FromRoute] string credId) 
        {
            try
            {
                var agentName = ConsentUtils.agentName(HttpContext);
                var context = await _agentService.GetAgentContext(agentName, agentName);
                await _credentialService.RevokeCredentialAsync(context, credId);   
                return new ApiResponse("Revocation successful", true, 200);
            }
            catch (WalletNotFoundException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentNullException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }
    }
}