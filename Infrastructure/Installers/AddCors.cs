using Consent_Aries_VC.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class AddCors : IDIContainerRegistration {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration) {
            services.AddCors(options => {
                options.AddPolicy("AllowAll", builder => {
                    builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });
        }
    }
}