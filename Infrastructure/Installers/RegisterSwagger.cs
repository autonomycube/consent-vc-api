using System;
using Consent_Aries_VC.Contracts;
using Consent_Aries_VC.Infrastructure.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Consent_Aries_VC.Infrastructure.Installers {
    public class RegisterSwagger : IDIContainerRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Consent VC API",
                    Version = "v1",
                });
                options.DocumentFilter<SwaggerAriesApiHideFilter>();
                options.OperationFilter<AddHeadersInSwaggerFilter>();
            });
        }
    }
}