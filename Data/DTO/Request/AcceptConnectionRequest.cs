namespace Consent_Aries_VC.Data.DTO.Request {
    public class AcceptConnectionRequest {
        public string WalletConfigurationId { get; set; }
        public string WalletKey { get; set; }
        public string InvitationDetails { get; set; }
    }
}