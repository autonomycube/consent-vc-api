using System.Collections.Generic;
using System.Linq;
using Hyperledger.Aries.Features.PresentProof;

namespace Consent_Aries_VC.Infrastructure.Utils
{
    public static class RequestedCredentialsUtils
    {
        public static IEnumerable<string> GetCredentialIdentifiers(this RequestedCredentials requestedCredentials)
        {
            var credIds = new List<string>();
            credIds.AddRange(requestedCredentials.RequestedAttributes.Values.Select(x => x.CredentialId));
            credIds.AddRange(requestedCredentials.RequestedPredicates.Values.Select(x => x.CredentialId));
            return credIds.Distinct();
        }
    }
}