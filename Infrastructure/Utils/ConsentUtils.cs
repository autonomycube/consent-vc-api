using System;
using System.Text;
using Hyperledger.Aries.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Consent_Aries_VC.Infrastructure.Utils 
{
    public static class ConsentUtils
    {
        public static string agentName(HttpContext httpCtx) {
            var agentName = httpCtx.Request.Headers[HttpHeaders.AgentHeader];
            return agentName.ToString();
        } 

        public static T fromBase64<T>(string b64Data) 
        {
            try
            {
                var stringifiedData = b64Data.FromBase64();
                // var stringifiedData = Encoding.UTF8.GetString(bytesData);
                var data = JsonConvert.DeserializeObject<T>(stringifiedData);
                return data;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}