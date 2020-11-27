using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Consent_Aries_VC.Infrastructure.Protocols.BasicMessage {
    public class BasicMessage : AgentMessage {
        public BasicMessage() {
            Id = Guid.NewGuid().ToString();
            Type = CustomMessageTypes.BasicMessageType;
        }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("sent_time")]
        [JsonPropertyName("sent_time")]
        public string SentTime { get; set; }
    }
}