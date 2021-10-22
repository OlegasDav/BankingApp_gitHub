using Domain.Clients.Firebase.Models;
using Domain.Clients.Firebase.Models.RequestModels;
using Domain.Clients.Firebase.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Clients.Firebase
{
    public interface IFirebaseClient
    {
        Task<FirebaseSignUpResponse> SignUpAsync(string email, string password);

        Task<FirebaseSignInResponse> SignInAsync(string email, string password);

        Task<FirebaseSendEmailVerificationResponse> SendEmailAsync(FirebaseSendEmailVerificationRequest user);
    }
}
