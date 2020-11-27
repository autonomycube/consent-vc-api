using System;
using System.IO;
using System.Threading.Tasks;
using Consent_Aries_VC.Services.Abstract;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Storage;
using Microsoft.AspNetCore.Http;

namespace Consent_Aries_VC.Infrastructure.Middlewares {
    public class AgentMiddleware {
        private readonly RequestDelegate NextRequestDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentMiddleware"/> class
        /// </summary>
        /// <param name="aNext">Context Provider.</param>
        public AgentMiddleware(RequestDelegate aNext) 
        {
            NextRequestDelegate = aNext;
        }

        /// <summary>
        /// Invokes the agent processing pipeline.
        /// </summary>
        /// <param name="aHttpContext"></param>
        /// <param name="aAgentProvider"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext aHttpContext, IAgentService aAgentProvider, IWalletService aWalletService) 
        {
            if (
                !HttpMethods.IsPost(aHttpContext.Request.Method) ||
                !(aHttpContext.Request.ContentType?.Equals(DefaultMessageService.AgentWireMessageMimeType) ?? false)
            ) {
                await NextRequestDelegate(aHttpContext);
                return;
            }

            if (aHttpContext.Request.ContentLength == null)
                throw new Exception("Empty content length");
            
            using var stream = new StreamReader(aHttpContext.Request.Body);
            string body = await stream.ReadToEndAsync();
            string name = string.Empty;

            if (aHttpContext.Request.Path.HasValue && aHttpContext.Request.Path.StartsWithSegments("/api/v1/agents")) {
                var nIndex = aHttpContext.Request.Path.Value.LastIndexOf("/");
                name = aHttpContext.Request.Path.Value.Substring(nIndex + 1);
            }

            IAgent agent = await aAgentProvider.GetAgentAsync();
            IAgentContext context = await aAgentProvider.GetContextAsync();
            if (!string.IsNullOrEmpty(name)) {
                context = await aAgentProvider.GetAgentContext(name, name);
                agent = context.Agent;
            }

            MessageContext response =
                await agent.ProcessAsync
                (
                    context: context,
                    messageContext: new PackedMessageContext(body.GetUTF8Bytes())
                );

            aHttpContext.Response.StatusCode = 200;

            if (response != null)
            {
                aHttpContext.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
                await aHttpContext.Response.WriteAsync(response.Payload.GetUTF8String());
            }
            else
            {
                await aHttpContext.Response.WriteAsync(string.Empty);
            }
        }
    }
}