using Hyperledger.Aries.Features.DidExchange;

namespace Consent_Aries_VC.Data.DTO.Request {
    public class CreateConnectionInvitationRequest {
        public string WalletConfigurationId { get; set; }
        public string WalletKey { get; set; }
        public ConnectionAlias Alias { get; set; }
    }
}