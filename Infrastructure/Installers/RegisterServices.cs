using Consent_Aries_VC.Contracts;
using Consent_Aries_VC.Services;
using Consent_Aries_VC.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class RegisterServices : IDIContainerRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAgentService, AgentService>();
        }
    }
}