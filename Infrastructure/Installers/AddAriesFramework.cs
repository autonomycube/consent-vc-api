using System;
using System.IO;
using Consent_Aries_VC.Contracts;
using Consent_Aries_VC.Infrastructure.Protocols.BasicMessage;
using Consent_Aries_VC.Infrastructure.Protocols.TrustPing;
using Consent_Aries_VC.Infrastructure.Utils;
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
                    c.AgentName = Environment.GetEnvironmentVariable("AGENT_NAME") ?? NameGenerator.GetRandomName();
                    //c.EndpointUri = "http://5e751170ce64.ngrok.io/";
                    c.EndpointUri = "http://localhost:5000/";
                    //c.EndpointUri = Environment.GetEnvironmentVariable("ENDPOINT_HOST") ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
                    c.WalletConfiguration = new WalletConfiguration { Id = "ClaimerWallet" };
                    c.WalletCredentials = new WalletCredentials { Key = "MyWalletKey" };
                    c.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                    c.PoolName = "TestPool";
                });
            });

            services.AddSingleton<BasicMessageHandler>();
            services.AddSingleton<TrustPingMessageHandler>();
        }
    }
}