namespace Consent_Aries_VC.Data.DTO.Request {
    public class CredentialOfferRequest {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string SchemaId { get; set; }
        public string CredentialDefinitionId { get; set; }
        public string CredentialAttributes { get; set; }
    }
}