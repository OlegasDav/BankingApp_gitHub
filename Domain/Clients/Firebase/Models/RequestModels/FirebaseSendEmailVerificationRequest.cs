using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Clients.Firebase.Models.RequestModels
{
    public class FirebaseSendEmailVerificationRequest
    {
        [JsonPropertyName("requestType")]
        public string RequestType { get; } = "VERIFY_EMAIL";

        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }
}
