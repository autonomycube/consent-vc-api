using System;

namespace Consent_Aries_VC.Data.DTO.Response
{
    public class ProofRecordDTO 
    {
        public string Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string TypeName { get; set; }
        public string ProposalJson { get; set; }
        public string RequestJson { get; set; }
        public string ProofJson { get; set; }
        public string ConnectionId { get; set; }
        public string State { get; set; }
    }
}