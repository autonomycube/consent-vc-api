using System.Collections.Generic;
using System.Threading.Tasks;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Services.Abstract;
// using Consent_Aries_VC.Data.DTO.Request;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.AspNetCore.Features.Schemas;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Records;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/schemas")]
    public class SchemaController : ControllerBase {
        #region Private Variables
        private readonly ISchemaService _schemaService;
        private readonly IAgentService _agentService;
        private readonly IProvisioningService _provisionService;
        #endregion

        #region Constructor
        public SchemaController(ISchemaService schemaService,
             IAgentService agentService,
             IProvisioningService provisionService) {
            _schemaService = schemaService;
            _provisionService = provisionService;
            _agentService = agentService;
        }
        #endregion
        
        [HttpPost]
        public async Task<object> Create([FromBody] CreateConsentSchemaRequest request) {
            var context = await _agentService.GetAgentContext(request.AgentName, request.AgentName);
            var issuer = await _provisionService.GetProvisioningAsync(context.Wallet);
            var Trustee = await Did.CreateAndStoreMyDidAsync(context.Wallet,
                new { seed = "000000000000000000000000Steward1" }.ToJson());
            
            await Ledger.SignAndSubmitRequestAsync(await context.Pool, context.Wallet, Trustee.Did,
            await Ledger.BuildNymRequestAsync(Trustee.Did, issuer.IssuerDid, issuer.IssuerVerkey, null, "ENDORSER"));
            
            var schemaId = await _schemaService.CreateSchemaAsync(
                    context: context,
                    issuerDid: issuer.IssuerDid,
                    name: request.Name,
                    version: request.Version,
                    attributeNames: request.AttributeNames.ToArray());

            return new { schemaId };
        }

        [HttpGet("{name}")]
        public async Task<List<SchemaRecord>> Get([FromRoute] string name) {
            var context = await _agentService.GetAgentContext(name, name);
            var schemas = await _schemaService.ListSchemasAsync(context.Wallet);
            return schemas;
        }
    }
}