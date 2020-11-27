using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Contracts {
    public interface IDIContainerRegistration {
        void RegisterAppServices(IServiceCollection services, IConfiguration configuration);
    }
}