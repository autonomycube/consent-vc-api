using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/credentials")]
    public class CredentialsController : ControllerBase {
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
            IConnectionService connectionService
        ) {
            _messageService = messageService;
            _credentialService = credentialService;
            _agentService = agentService;
            _provisioningService = provisioningService;
            _connectionService = connectionService;
        }
        #endregion

        [HttpPost("issue")]
        public async Task IssueCredentials([FromBody] CredentialOfferRequest offerRequest) {
            var context = await _agentService.GetAgentContext(offerRequest.Name, offerRequest.Name);
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
            
            await _messageService.SendAsync(context.Wallet, offer, connection);
        }

        [HttpGet("{name}")]
        public async Task<List<CredentialRecord>> GetCredential([FromRoute] string name) {
            var context = await _agentService.GetAgentContext(name, name);
            return await _credentialService.ListAsync(context);
        }
    }
}