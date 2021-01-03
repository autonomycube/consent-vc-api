using System.IO;
using Consent_Aries_VC.Contracts;
using Consent_Aries_VC.Infrastructure.Protocols.BasicMessage;
using Consent_Aries_VC.Infrastructure.Protocols.TrustPing;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class AddAriesFramework : IDIContainerRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAriesFramework(builder =>
            {
                builder.RegisterAgent<ConsentAgent>(c =>
                {
                    c.AgentName = "Consent";
                    // c.EndpointUri = "http://api-ssi.consentwallets.com/";
                    c.EndpointUri = "http://localhost:7000/";
                    c.WalletConfiguration = new WalletConfiguration { Id = "consent" };
                    c.WalletCredentials = new WalletCredentials { Key = "consentindia123$" };
                    c.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                    c.PoolName = "TestPool";
                    c.AutoRespondCredentialOffer = true;
                    c.AutoRespondCredentialRequest = true;
                });
            });

            services.AddSingleton<BasicMessageHandler>();
            services.AddSingleton<TrustPingMessageHandler>();
        }
    }
}