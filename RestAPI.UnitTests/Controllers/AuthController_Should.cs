using AutoFixture.Xunit2;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestAPI.UnitTests.Controllers
{
    public class AuthController_Should
    {
        private readonly Mock<IAuthService> _authServiceMock = new Mock<IAuthService>();

        private readonly AuthController _sut;

        public AuthController_Should()
        {
            _sut = new AuthController(_authServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task SignUp_WithSingUpRequest_ReturnsSignUpResponse(
            SignUpRequest request,
            SignUpResponse response)
        {
            // Arrange
            _authServiceMock
                .Setup(authService => authService.SignUpAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.SignUp(request);

            // Assert
            result.Should().BeOfType<ActionResult<SignUpResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);

            _authServiceMock
                .Verify(authService => authService.SignUpAsync(It.IsAny<SignUpRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task SignUp_WithSingUpRequest_ReturnsBadRequest(
            SignUpRequest request)
        {
            // Arrange
            _authServiceMock
                .Setup(authService => authService.SignUpAsync(request))
                .ThrowsAsync(new FirebaseException("Something went wrong", 400));

            // Act
            var result = await _sut.SignUp(request);

            // Assert
            result.Should().BeOfType<ActionResult<SignUpResponse>>()
                .Which.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo("Something went wrong");

            _authServiceMock
                .Verify(authService => authService.SignUpAsync(It.IsAny<SignUpRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task SignIn_WithSingInRequest_ReturnsSignInResponse(
            SignInRequest request,
            SignInResponse response)
        {
            // Arrange
            _authServiceMock
                .Setup(authService => authService.SignInAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.SignIn(request);

            // Assert
            result.Should().BeOfType<ActionResult<SignInResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(response);

            _authServiceMock
                .Verify(authService => authService.SignInAsync(It.IsAny<SignInRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task SignIn_WithSingInRequest_ReturnsBadRequest(
            SignInRequest request)
        {
            // Arrange
            _authServiceMock
                .Setup(authService => authService.SignInAsync(request))
                .ThrowsAsync(new FirebaseException("Something went wrong", 400));

            // Act
            var result = await _sut.SignIn(request);

            // Assert
            result.Should().BeOfType<ActionResult<SignInResponse>>()
                .Which.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo("Something went wrong");

            _authServiceMock
                .Verify(authService => authService.SignInAsync(It.IsAny<SignInRequest>()), Times.Once);
        }
    }
}
