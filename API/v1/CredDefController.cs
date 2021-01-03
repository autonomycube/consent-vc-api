using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Consent_Aries_VC.Data.DTO.Generic;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Records;
using Hyperledger.Indy.WalletApi;
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

        [HttpGet("{name}")]
        public async Task<ApiResponse> Get([FromRoute] string name) 
        {
            try
            {
                var context = await _agentService.GetAgentContext(name, name);
                var credDefs = await _schemaService.ListCredentialDefinitionsAsync(context.Wallet);
                return new ApiResponse("credential definitions retrieved" ,credDefs, 200);
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