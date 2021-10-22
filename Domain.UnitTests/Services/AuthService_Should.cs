using AutoFixture.Xunit2;
using Contracts.Models.Request;
using Domain.Clients.Firebase;
using Domain.Clients.Firebase.Models.RequestModels;
using Domain.Clients.Firebase.Models.ResponseModels;
using Domain.Services;
using FluentAssertions;
using Moq;
using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using Persistence.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper.Attributes;
using Xunit;

namespace Domain.UnitTests.Services
{
    public class AuthService_Should
    {
        [Theory]
        [AutoMoqData]
        public async Task SignUpAsync_WithSingUpRequest_ReturnsSignUpResponse(
            SignUpRequest signUpRequest,
            FirebaseSignUpResponse firebaseSingUpResponse,
            [Frozen] Mock<IFirebaseClient> fireBaseClientMock,
            [Frozen] Mock<IUsersRepository> userRepositoryMock,
            AuthService sut)
        {
            // Arrange
            firebaseSingUpResponse.Email = signUpRequest.Email;

            fireBaseClientMock
                .Setup(firebaseClient => firebaseClient
                .SignUpAsync(signUpRequest.Email, signUpRequest.Password))
                .ReturnsAsync(firebaseSingUpResponse);

            // Act
            var result = await sut.SignUpAsync(signUpRequest);

            //result.Should().BeEquivalentTo(result, options => options.ComparingByMembers<SignUpResponse>());

            result.IdToken.Should().BeEquivalentTo(firebaseSingUpResponse.IdToken);
            result.Email.Should().BeEquivalentTo(firebaseSingUpResponse.Email);
            result.Email.Should().BeEquivalentTo(signUpRequest.Email);
            result.Username.Should().BeEquivalentTo(signUpRequest.Username);
            result.DateCreated.GetType().Should().Be<DateTime>();
            result.Id.GetType().Should().Be<Guid>();

            fireBaseClientMock
                .Verify(firebaseClient => firebaseClient
                .SignUpAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            fireBaseClientMock
                .Verify(firebaseClient => firebaseClient
                .SendEmailAsync(It.Is<FirebaseSendEmailVerificationRequest>(model => model.IdToken.Equals(firebaseSingUpResponse.IdToken))), Times.Once);

            userRepositoryMock
                .Verify(userRepository => userRepository
                .SaveAsync(It.Is<UserWriteModel>(user => user.LocalId.Equals(firebaseSingUpResponse.LocalId) &&
                user.Username.Equals(signUpRequest.Username) &&
                user.Email.Equals(firebaseSingUpResponse.Email))), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task SignInAsync_WithSingInRequest_ReturnsSignInResponse(
            [Frozen] Mock<IFirebaseClient> fireBaseClientMock,
            [Frozen] Mock<IUsersRepository> userRepositoryMock,
            SignInRequest signInRequest,
            FirebaseSignInResponse firebaseSingInResponse,
            UserReadModel userReadModel,
            AuthService sut)
        {
            // Arrange
            firebaseSingInResponse.Email = signInRequest.Email;

            userReadModel.LocalId = firebaseSingInResponse.LocalId;
            userReadModel.Email = firebaseSingInResponse.Email;

            fireBaseClientMock
                .Setup(firebaseClient => firebaseClient
                .SignInAsync(signInRequest.Email, signInRequest.Password))
                .ReturnsAsync(firebaseSingInResponse);

            userRepositoryMock
                .Setup(userRepository => userRepository
                .GetAsync(firebaseSingInResponse.LocalId))
                .ReturnsAsync(userReadModel);

            // Act
            var result = await sut.SignInAsync(signInRequest);

            // Assert
            result.Username.Should().BeEquivalentTo(userReadModel.Username);
            result.Email.Should().BeEquivalentTo(userReadModel.Email);
            result.IdToken.Should().BeEquivalentTo(firebaseSingInResponse.IdToken);

            fireBaseClientMock
                .Verify(firebaseClient => firebaseClient
                .SignInAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            userRepositoryMock
                .Verify(userRepository => userRepository
                .GetAsync(firebaseSingInResponse.LocalId), Times.Once);
        }
    }
}
