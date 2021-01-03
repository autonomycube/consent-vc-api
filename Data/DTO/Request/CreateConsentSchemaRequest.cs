using Hyperledger.Aries.AspNetCore.Features.Schemas;

namespace Consent_Aries_VC.Data.DTO.Request {
    public class CreateConsentSchemaRequest : CreateSchemaRequest 
    {
        public string Tag { get; set; }
        public bool EnableRevocation { get; set; }
    }
}