using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Consent_Aries_VC.Infrastructure.Filters
{
    public class AddHeadersInSwaggerFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter {
                Name = "x-consent-agent",
                Required = false,
                In = ParameterLocation.Header,
            });
        }
    }
}