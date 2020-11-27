using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Records;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/cred_defs")]
    public class CredDefController : ControllerBase {
        #region Private Variables
        private readonly ISchemaService _schemaService;
        private readonly IProvisioningService _provisionService;
        private readonly IAgentService _agentService;
        #endregion

        #region Constructor
        public CredDefController(ISchemaService schemaService,
                IAgentService agentService,
                IProvisioningService provisionService) {
                    _schemaService = schemaService;
                    _agentService = agentService;
                    _provisionService = provisionService;
                }
        #endregion

        [HttpPost("{name}/{id}")]
        public async Task<object> Create([FromRoute] string name, [FromRoute] string id) {
            Console.WriteLine(id);
            var context = await _agentService.GetAgentContext(name, name);
            var issuer = await _provisionService.GetProvisioningAsync(context.Wallet);
            var credDefId = await _schemaService.CreateCredentialDefinitionAsync(context, new CredentialDefinitionConfiguration {
                SchemaId = id,
                Tag = "default",
                EnableRevocation = false,
                RevocationRegistrySize = 0,
                RevocationRegistryBaseUri = string.Empty,
                RevocationRegistryAutoScale = false,
                IssuerDid = issuer.IssuerDid
            });
            return new { credDefId };
        }

        [HttpGet("{name}")]
        public async Task<List<DefinitionRecord>> Get([FromRoute] string name) {
            var context = await _agentService.GetAgentContext(name, name);
            return await _schemaService.ListCredentialDefinitionsAsync(context.Wallet);
        }
    }
}