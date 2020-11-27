using Consent_Aries_VC.Contracts;
using Consent_Aries_VC.Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class RegisterAppSettings : IDIContainerRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            var appConfig = configuration.GetSection("AppConfig");
            services.Configure<AppConfig>(appConfig);

            var mongoConfig = configuration.GetSection("MongoDbSettings");
            services.Configure<MongoDbSettings>(mongoConfig);
        }
    }
}