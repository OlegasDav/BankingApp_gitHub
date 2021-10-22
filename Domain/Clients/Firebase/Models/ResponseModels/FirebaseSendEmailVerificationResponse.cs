using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Clients.Firebase.Models.ResponseModels
{
    public class FirebaseSendEmailVerificationResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
