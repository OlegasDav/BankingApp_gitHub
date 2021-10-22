using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Clients.Firebase;
using Domain.Clients.Firebase.Models.RequestModels;
using Persistence.Models.WriteModels;
using Persistence.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IFirebaseClient _firebaseClient;
        private readonly IUsersRepository _usersRepository;

        public AuthService(IFirebaseClient firebaseClient, IUsersRepository usersRepository)
        {
            _firebaseClient = firebaseClient;
            _usersRepository = usersRepository;
        }

        public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            var user = await _firebaseClient.SignUpAsync(request.Email, request.Password);

            var verificationEmail = new FirebaseSendEmailVerificationRequest
            {
                IdToken = user.IdToken
            };

            await _firebaseClient.SendEmailAsync(verificationEmail);

            var userWriteModel = new UserWriteModel
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = user.Email,
                LocalId = user.LocalId,
                DateCreated = DateTime.Now
            };

            await _usersRepository.SaveAsync(userWriteModel);

            return new SignUpResponse
            {
                Id = userWriteModel.Id,
                IdToken = user.IdToken,
                Email = userWriteModel.Email,
                Username = userWriteModel.Username,
                DateCreated = userWriteModel.DateCreated
            };
        }

        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            var firebaseSignInResponse = await _firebaseClient.SignInAsync(request.Email, request.Password);

            var user = await _usersRepository.GetAsync(firebaseSignInResponse.LocalId);

            return new SignInResponse
            {
                Username = user.Username,
                Email = user.Email,
                IdToken = firebaseSignInResponse.IdToken
            };
        }
    }
}
