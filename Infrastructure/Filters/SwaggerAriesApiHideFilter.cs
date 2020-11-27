using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Consent_Aries_VC.Infrastructure.Filters {
    public class SwaggerAriesApiHideFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Keys.ToList();
            foreach (var path in paths) {
                if (path.StartsWith("/aries")) {
                    swaggerDoc.Paths.Remove(path);
                }
            }
        }
    }
}