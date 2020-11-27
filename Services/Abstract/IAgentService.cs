using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Consent_Aries_VC.Services.Abstract {
    public interface IAgentService : IAgentProvider
    {
        Task<IAgentContext> GetAgentContext(string walletConfigurationId, string walletKey);
    }
}