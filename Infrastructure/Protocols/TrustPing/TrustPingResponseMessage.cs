using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Consent_Aries_VC.Infrastructure.Protocols.TrustPing {
    public class TrustPingResponseMessage : AgentMessage {
        public TrustPingResponseMessage() {
            Id = Guid.NewGuid().ToString();
            Type = CustomMessageTypes.TrustPingResponseMessageType;
        }

        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}