using Domain.Clients.Firebase.Models;
using Domain.Clients.Firebase.Models.RequestModels;
using Domain.Clients.Firebase.Models.ResponseModels;
using Domain.Clients.Firebase.Options;
using Domain.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Clients.Firebase
{
    public class FirebaseClient : IFirebaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseOptions _firebaseOptions;

        public FirebaseClient(HttpClient httpClient, IOptions<FirebaseOptions> firebaseOptions)
        {
            _httpClient = httpClient;
            _firebaseOptions = firebaseOptions.Value;
        }

        public async Task<FirebaseSignUpResponse> SignUpAsync(string email, string password)
        {
            var url = $"{_firebaseOptions.BaseAddress}:signUp?key={_firebaseOptions.ApiKey}";

            var user = new FirebaseSignUpRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(url, user);

            if (!response.IsSuccessStatusCode)
            {
                var newError = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new FirebaseException($"{newError.Error.Message}", newError.Error.Code);
            }

            return await response.Content.ReadFromJsonAsync<FirebaseSignUpResponse>();
        }

        public async Task<FirebaseSignInResponse> SignInAsync(string email, string password)
        {
            var url = $"{_firebaseOptions.BaseAddress}:signInWithPassword?key={_firebaseOptions.ApiKey}";

            var user = new FirebaseSignInRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(url, user);

            if (!response.IsSuccessStatusCode)
            {
                var newError = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new FirebaseException($"{newError.Error.Message}", newError.Error.Code);
            }

            return await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>();
        }

        public async Task<FirebaseSendEmailVerificationResponse> SendEmailAsync(FirebaseSendEmailVerificationRequest user)
        {
            var url = $"{_firebaseOptions.BaseAddress}:sendOobCode?key={_firebaseOptions.ApiKey}";

            var response = await _httpClient.PostAsJsonAsync(url, user);

            if (!response.IsSuccessStatusCode)
            {
                var newError = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new FirebaseException($"{newError.Error.Message}", newError.Error.Code);
            }

            return await response.Content.ReadFromJsonAsync<FirebaseSendEmailVerificationResponse>();
        }
    }
}
