using System;
using System.Linq;
using Consent_Aries_VC.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consent_Aries_VC.Infrastructure.Extensions {
    public static class DIExtensions {
        public static void AddServicesInAssembly(this IServiceCollection services, 
                IConfiguration configuration) 
        {
            var appServices = typeof(Startup).Assembly.DefinedTypes
                                .Where(x => typeof(IDIContainerRegistration)
                                    .IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                    .Select(Activator.CreateInstance)
                                    .Cast<IDIContainerRegistration>().ToList();

            appServices.ForEach(svc => svc.RegisterAppServices(services, configuration));
        }
    }
}