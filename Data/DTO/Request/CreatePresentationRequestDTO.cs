using Hyperledger.Aries.Features.PresentProof;

namespace Consent_Aries_VC.Data.DTO.Request
{
    public class CreatePresentationRequestDTO
    {
        public string proofRecID { get; set; }
        public RequestedCredentials RequestedCredentials { get; set; }
    }
}