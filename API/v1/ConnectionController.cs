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
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.AspNetCore.Mvc;

namespace Consent_Aries_VC.API.v1 {
    [Route("/api/v1/connections")]
    public class ConnectionController : ControllerBase {
        #region Private Variables
        private readonly IAgentService _agentService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IProvisioningService _provisionService;
        private readonly IConnectionService _connectionService;
        private readonly IMessageService _messageService;
        #endregion

        #region Constructor
        public ConnectionController(
            IAgentService agentService,
            IAgentProvider agentContextProvider,
            IProvisioningService provisioningService,
            IConnectionService connectionService,
            IMessageService messageService) {
                _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
                _agentContextProvider = agentContextProvider;
                _provisionService = provisioningService;
                _connectionService = connectionService;
                _messageService = messageService;
        }
        #endregion

        [HttpPost("invite")]
        public async Task<ApiResponse> Invite([FromBody] CreateConnectionInvitationRequest request) 
        {
            //var context = await _agentContextProvider.GetContextAsync();
            try
            {
                var agentName = ConsentUtils.agentName(HttpContext);
                var context = await _agentService.GetAgentContext(agentName, agentName);

                (var connectionInvitationMessage, var connectionRecord) = 
                await _connectionService.CreateInvitationAsync(context, new InviteConfiguration {
                    AutoAcceptConnection = true,
                    MyAlias = request.Alias
                });

                var qrUrl = $"{(await _provisionService.GetProvisioningAsync(context.Wallet)).Endpoint.Uri}?c_i={connectionInvitationMessage.ToJson().ToBase64()}";

                return new ApiResponse("Connection invitation sent", qrUrl, 200);
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

        [HttpPost("accept")]
        public async Task<ApiResponse> AcceptInvitation([FromBody] AcceptConnectionRequest connectionReqeust) 
        {
            try
            {
                var agentName = ConsentUtils.agentName(HttpContext);
                var context = await _agentService.GetAgentContext(agentName, agentName);
                var invite = MessageUtils.DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(connectionReqeust.InvitationDetails);
                var (request, record) = await _connectionService.CreateRequestAsync(context, invite);
                await _messageService.SendAsync(context.Wallet, request, record);
                return new ApiResponse("Invitation accepted", true, 200);
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
        public async Task<ApiResponse> Get() 
        {
            try
            {
                var context = await _agentContextProvider.GetContextAsync();
                var invitedConnections = await _connectionService.ListAsync(context);
                return new ApiResponse("Connections retrieved", invitedConnections, 200);
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
        public async Task<ApiResponse> GetByName([FromRoute] string name) 
        {
            try
            {
                var context = await _agentService.GetAgentContext(name, name);

                var connections = await _connectionService.ListAsync(context);
                return new ApiResponse("connections retrieved", connections, 200);
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