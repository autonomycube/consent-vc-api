using System.IO;
using Consent_Aries_VC.Infrastructure.Utils;

namespace Consent_Aries_VC.Data.DTO.Request {
    public class CreateAgentRequest {
        public string AgentName { get; set; } = NameGenerator.GetRandomName();
        public string EndpointUri { get; set; } = "http://localhost:5000/";
        public string WalletConfigurationId { get; set; }
        public string WalletKey { get; set; }
        public string GenesisFilename { get; set; } = Path.GetFullPath("pool_genesis.txn");
        public string PoolName { get; set; } = "TestPool";
    }
}