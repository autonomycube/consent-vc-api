using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoWrapper.Wrappers;
using Consent_Aries_VC.Data.DTO.Generic;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Data.DTO.Response;
using Consent_Aries_VC.Infrastructure.Utils;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Service;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Indy.WalletApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    /// <summary>
    /// API for verifiable presentations and proofs
    /// </summary>
    [Route("/api/v1/proofs")]
    public class ProofController : ControllerBase 
    {
        #region Private Variables
        private readonly IProofService _proofService;
        private readonly IAgentService _agentService;
        private readonly IConnectionService _connectionService;
        private readonly IMessageService _messageService;

        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public ProofController(IProofService proofService,
                               IAgentService agentService,
                               IConnectionService connectionService,
                               IMessageService messageService,
                               IMapper mapper) 
        {
            _proofService = proofService
                ?? throw new ArgumentNullException(nameof(proofService));
            _agentService = agentService
                ?? throw new ArgumentNullException(nameof(agentService));
            _connectionService = connectionService
                ?? throw new ArgumentNullException(nameof(connectionService));
            _messageService = messageService
                ?? throw new ArgumentNullException(nameof(messageService));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        /// <summary>
        /// Verifier sends a Request to Holder for a `proof`.
        /// </summary>
        [HttpPost("request/{connId}")]
        public async Task<ApiResponse> SendPresentationRequest([FromRoute] string connId, [FromBody] ProofRequest request) 
        {
            try 
            {
                var context = await getAgentContext(HttpContext);
                var connectionRecord = await _connectionService.GetAsync(context, connId);
                var (requestPresentationMessage, proofRecord) = await _proofService.CreateRequestAsync(context,
                                                                                                       request,
                                                                                                       connId);
                return new ApiResponse("Proof request sent", 
                new 
                {
                    presentationMessageB64= requestPresentationMessage.ToJson().ToBase64(),
                    presentationMessage = requestPresentationMessage,
                    proofRecord
                }, 200);
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

        /// <summary>
        /// Holder processes the Proof request sent by the Verifier.
        /// </summary>
        [HttpPost("process/{connId}")]
        public async Task<ApiResponse> ProcessPresentationRequest([FromRoute] string connId, [FromBody] RequestPresentationMessage request) 
        {
            try
            {
                var agentContext = await getAgentContext(HttpContext);
                var connectionRecord = await _connectionService.GetAsync(agentContext, connId);
                var response = await _proofService.ProcessRequestAsync(
                    agentContext,
                    request,
                    connectionRecord);
                return new ApiResponse("Presentation processed", response, 200);
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

        /// <summary>
        /// step 1 - response
        /// Receive request - Presentation
        /// Do we want to reveal this data?
        /// YES -> SendPresentation
        /// NO  -> Do we want to continue?
        ///     -> YES -> Send Propose - Presentation
        ///     -> NO  -> Send Problem Report
        /// </summary>
        [HttpPost("create/{connId}")]
        public async Task<ApiResponse> CreatePresentation([FromRoute] string connId, [FromBody] CreatePresentationRequestDTO request) 
        {
            try
            {
                var agentContext = await getAgentContext(HttpContext);
                var (presentationMessage, proofRecord) = await _proofService.CreatePresentationAsync(
                    agentContext,
                    proofRecordId: request.proofRecID,
                    request.RequestedCredentials);

                var connRec = await _connectionService.GetAsync(agentContext, connId);
                await _messageService.SendAsync(agentContext.Wallet, presentationMessage, connRec);

                return new ApiResponse("Presentation created",
                                       new { presentationMessage, proofRecord },
                                       200);
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

        /// <summary>
        /// </summary>
        [HttpPost("verify/{proofRecId}")]
        public async Task<ApiResponse> VerifyPresentation([FromRoute] string proofRecId)
        {
            try
            {
                var agentContext = await getAgentContext(HttpContext);

                var isVerified = await _proofService.VerifyProofAsync(agentContext, proofRecId);
                var message = isVerified ? "Verification success" : "Verification failed";
                return new ApiResponse(message, isVerified, 200);
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

        [HttpGet]
        public async Task<ApiResponse> GetPresentations()
        {
            try
            {
                var context = await getAgentContext(HttpContext);
                var presentations = await _proofService.ListAsync(context);
                var response = _mapper.Map<List<ProofRecordDTO>>(presentations);
                return new ApiResponse("Proof records retrieved", response, 200);
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

        [HttpPost("creds/supported/{itemRef}")]
        public async Task<ApiResponse> ListSupportedCredentials([FromRoute] string itemRef, [FromBody] ProofRequest proofRequest)
        {
            try
            {
                var agentContext = await getAgentContext(HttpContext);
                var creds = await _proofService.ListCredentialsForProofRequestAsync(agentContext, proofRequest, itemRef);
                return new ApiResponse("Credentials retrieved successfully", creds, 200);
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

        #region Helpers
        private async Task<IAgentContext> getAgentContext(HttpContext context) {
            var agentName = ConsentUtils.agentName(context);
            if (string.IsNullOrEmpty(agentName))
            {
                throw new ArgumentNullException("Invalid request");
            }
            var ctx = await _agentService.GetAgentContext(agentName, agentName);
            return ctx;
        }
        #endregion
    }
}