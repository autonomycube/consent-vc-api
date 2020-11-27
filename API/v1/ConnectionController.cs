using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Utils;
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
        public async Task<object> Invite([FromBody] CreateConnectionInvitationRequest request) {
            //var context = await _agentContextProvider.GetContextAsync();
            var context = await _agentService.GetAgentContext(request.WalletConfigurationId, request.WalletKey);

            (var connectionInvitationMessage, var connectionRecord) = 
            await _connectionService.CreateInvitationAsync(context, new InviteConfiguration {
                AutoAcceptConnection = true,
                MyAlias = request.Alias
            });

            return new {
                qrUrl = $"{(await _provisionService.GetProvisioningAsync(context.Wallet)).Endpoint.Uri}?c_i={connectionInvitationMessage.ToJson().ToBase64()}"
            };
        }

        [HttpPost("accept")]
        public async Task AcceptInvitation([FromBody] AcceptConnectionRequest connectionReqeust) {
            //var context = await _agentContextProvider.GetContextAsync();
            var context = await _agentService.GetAgentContext(connectionReqeust.WalletConfigurationId, connectionReqeust.WalletKey);

            var invite = MessageUtils.DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(connectionReqeust.InvitationDetails);
            var (request, record) = await _connectionService.CreateRequestAsync(context, invite);
            await _messageService.SendAsync(context.Wallet, request, record);
        }

        [HttpGet]
        public async Task<List<ConnectionRecord>> Get() {
            var context = await _agentContextProvider.GetContextAsync();

            var invitedConnections = await _connectionService.ListAsync(context);
            return invitedConnections;
        }

        [HttpGet("{name}")]
        public async Task<List<ConnectionRecord>> GetByName([FromRoute] string name) {
            var context = await _agentService.GetAgentContext(name, name);

            var connections = await _connectionService.ListAsync(context);
            return connections;
        }
    }
}