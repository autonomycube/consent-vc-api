using System.Threading.Tasks;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Options;

namespace Consent_Aries_VC.Services {
    public class AgentService : DefaultAgentProvider, IAgentService {
        #region Private Variables
        private readonly IAgent _agent;
        private readonly IWalletService _walletService;
        private readonly IPoolService _poolService;
        #endregion

        #region Constructor
        public AgentService(
            IOptions<AgentOptions> agentOptions,
            IAgent agent,
            IWalletService walletService,
            IPoolService poolService
        ) : base(agentOptions, agent, walletService, poolService) {
            _agent = agent;
            _walletService = walletService;
            _poolService = poolService;
        }
        #endregion

        public async Task<IAgentContext> GetAgentContext(
            string walletConfigurationId, string walletKey
        ) {
            var context = await base.GetContextAsync();
            context.Wallet = await _walletService.GetWalletAsync(
                new WalletConfiguration {
                    Id = walletConfigurationId
                },
                new WalletCredentials {
                    Key = walletKey
                }
            );

            return context;
        }
    }
}