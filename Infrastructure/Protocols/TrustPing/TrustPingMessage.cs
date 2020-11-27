using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Consent_Aries_VC.Infrastructure.Protocols.TrustPing {
    public class TrustPingMessage : AgentMessage {
        public TrustPingMessage() {
            Id = Guid.NewGuid().ToString();
            Type = CustomMessageTypes.TrustPingMessageType;
        }

        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("response_requested")]
        [JsonPropertyName("response_requested")]
        public bool ResponseRequested { get; set; }
    }
}