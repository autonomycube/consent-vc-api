using System;
using Consent_Aries_VC.Infrastructure.Protocols.BasicMessage;
using Consent_Aries_VC.Infrastructure.Protocols.TrustPing;
using Consent_Aries_VC.Services;
using Hyperledger.Aries.Agents;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class ConsentAgent : AgentBase {
        public ConsentAgent(IServiceProvider serviceProvider) : base(serviceProvider) {}

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddForwardHandler();
            AddHandler<BasicMessageHandler>();
            AddHandler<TrustPingMessageHandler>();
            AddDiscoveryHandler();
            AddTrustPingHandler();
            AddCredentialHandler();
            AddProofHandler();
        }
    }
}