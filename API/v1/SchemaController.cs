using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Infrastructure.Utils;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 
{
    [Route("/api/v1/schemas")]
    public class SchemaController : ControllerBase 
    {
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
        public async Task<ApiResponse> Create([FromBody] CreateConsentSchemaRequest request) 
        {
            var agentName = ConsentUtils.agentName(HttpContext);
            
            try
            {
                var context = await _agentService.GetAgentContext(agentName, agentName);
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

                var credDefId = await _schemaService.CreateCredentialDefinitionAsync(context, new CredentialDefinitionConfiguration {
                    SchemaId = schemaId,
                    Tag = string.IsNullOrEmpty(request.Tag) ? "latest" : request.Tag,
                    EnableRevocation = request.EnableRevocation,
                    RevocationRegistrySize = 1,
                    // RevocationRegistryBaseUri = string.Empty,
                    RevocationRegistryAutoScale = request.EnableRevocation,
                    IssuerDid = issuer.IssuerDid
                });

                return new ApiResponse("Schema created successfully", new { schemaId, credDefId }, 200);
            }
            catch (WalletNotFoundException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentNullException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        [HttpGet("{name}")]
        public async Task<ApiResponse> Get([FromRoute] string name) 
        {
            try
            {
                var context = await _agentService.GetAgentContext(name, name);
                var schemas = await _schemaService.ListSchemasAsync(context.Wallet);
                return new ApiResponse("Schemas retrieved", schemas, 200);
            }
            catch (WalletNotFoundException ex)
            {
                throw new ApiException(ex.Message, 400);
            }
            catch (ArgumentNullException ex)
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