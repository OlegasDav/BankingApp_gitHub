using AutoFixture.Xunit2;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Persistence.Models.ReadModels;
using RestApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestAPI.UnitTests.Controllers
{
    public class AccountController_Should
    {
        private readonly Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        private readonly AccountController _sut;

        public AccountController_Should()
        {
            _sut = new AccountController(_accountServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task GetAllTodoItems_When_GetAllIsCalled(
            List<AccountReadModel> accountReadModels)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.GetAllAsync(localId))
                .ReturnsAsync(accountReadModels);

            // Act
            var result = await _sut.GetAll();

            // Assert

            result.Should().BeOfType<ActionResult<IEnumerable<AccountResponse>>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(accountReadModels, option => option
                .ExcludingMissingMembers());

            _accountServiceMock
                .Verify(accountService => accountService.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task Get_ReturnsAccountResponse_When_AllParametersArePassed(
            string accountIban,
            AccountReadModel accountReadModel)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.GetAsync(localId, accountIban))
                .ReturnsAsync(accountReadModel);

            // Act
            var result = await _sut.Get(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<AccountResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(accountReadModel, option => option
                .ExcludingMissingMembers());

            _accountServiceMock
                .Verify(accountService => accountService.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task Get_ReturnsNotFound_When_AccountoesNotExist(
            string accountIban)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.GetAsync(localId, accountIban))
                .ThrowsAsync(new UserException($"You do not have account with IBAN: {accountIban}", 404));

            // Act
            var result = await _sut.Get(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<AccountResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have account with IBAN: {accountIban}");

            _accountServiceMock
                .Verify(accountService => accountService.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task OpenAccount_ReturnsAccountResponse(
            AccountResponse accountResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.CreateAsync(localId))
                .ReturnsAsync(accountResponse);

            // Act
            var result = await _sut.OpenAccount();

            // Assert
            result.Should().BeOfType<ActionResult<AccountResponse>>()
                .Which.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeEquivalentTo(accountResponse);

            _accountServiceMock
                .Verify(accountService => accountService.CreateAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task CloseAccount_ReturnsNoContent_When_AllParametersArePassed(
            string accountIban,
            int rowsAffected)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.DeleteAsync(localId, accountIban))
                .ReturnsAsync(rowsAffected);

            // Act
            var result = await _sut.CloseAccount(accountIban);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _accountServiceMock
                .Verify(accountService => accountService.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task CloseAccount_ReturnsNotFound_When_AccountDoesNotExist(
            string accountIban)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.DeleteAsync(localId, accountIban))
                .ThrowsAsync(new UserException($"You do not have the account with IBAN: {accountIban}", 404));

            // Act
            var result = await _sut.CloseAccount(accountIban);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have the account with IBAN: {accountIban}");

            _accountServiceMock
                .Verify(accountService => accountService.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task CloseAccount_ReturnsBadRequest_When_AccountIsNotEmpty(
            string accountIban)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _accountServiceMock
                .Setup(accountService => accountService.DeleteAsync(localId, accountIban))
                .ThrowsAsync(new UserException($"You cannot close the account with IBAN: {accountIban} till there is some money", 400));

            // Act
            var result = await _sut.CloseAccount(accountIban);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You cannot close the account with IBAN: {accountIban} till there is some money");

            _accountServiceMock
                .Verify(accountService => accountService.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        private ClaimsPrincipal SetupHttpContent()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            _sut.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            return user;
        }
    }
}
