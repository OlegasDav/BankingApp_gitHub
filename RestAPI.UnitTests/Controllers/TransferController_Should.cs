using AutoFixture.Xunit2;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
    public class TransferController_Should
    {
        private readonly Mock<ITransferService> _transferServiceMock = new Mock<ITransferService>();

        private readonly TransferController _sut;

        public TransferController_Should()
        {
            _sut = new TransferController(_transferServiceMock.Object);
        }

        [Theory, AutoData]
        public async Task GetAllTopUps_ReturnsTopUps_When_AllParametersArePassed(
            string accountIban,
            IEnumerable<ShortTopUpsResponse> shortTopUpsResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetAllTopUpsAsync(localId, accountIban))
                .ReturnsAsync(shortTopUpsResponse);

            // Act
            var result = await _sut.GetAllTopUps(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<ShortTopUpsResponse>>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(shortTopUpsResponse);

            _transferServiceMock
                .Verify(transferService => transferService.GetAllTopUpsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetAllTopUps_ReturnsNotFound_When_AccountDoesNotExist(
            string accountIban)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetAllTopUpsAsync(localId, accountIban))
                .ThrowsAsync(new UserException($"You do not have account with IBAN: {accountIban}", 404));

            // Act
            var result = await _sut.GetAllTopUps(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<ShortTopUpsResponse>>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have account with IBAN: {accountIban}");

            _transferServiceMock
                .Verify(transferService => transferService.GetAllTopUpsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetTopUp_ReturnsTopUp_When_AllParametersArePassed(
            Guid id,
            ShortTopUpResponse shortTopUpResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetTopUpAsync(localId, id))
                .ReturnsAsync(shortTopUpResponse);

            // Act
            var result = await _sut.GetTopUp(id);

            // Assert
            result.Should().BeOfType<ActionResult<ShortTopUpResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(shortTopUpResponse);

            _transferServiceMock
                .Verify(transferService => transferService.GetTopUpAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetTopUp_ReturnsNotFound_When_TopUpDoesNotExist(
            Guid id)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetTopUpAsync(localId, id))
                .ThrowsAsync(new UserException($"You do not have TopUP with Id: {id}", 404));

            // Act
            var result = await _sut.GetTopUp(id);

            // Assert
            result.Should().BeOfType<ActionResult<ShortTopUpResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have TopUP with Id: {id}");

            _transferServiceMock
                .Verify(transferService => transferService.GetTopUpAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetAllTransfers_ReturnsTransfers_When_AllParametersArePassed(
            string accountIban,
            IEnumerable<ShortTransferResponse> shortTransferResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetAllTransfersAsync(localId, accountIban))
                .ReturnsAsync(shortTransferResponse);

            // Act
            var result = await _sut.GetAllTransfers(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<ShortTransferResponse>>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(shortTransferResponse);

            _transferServiceMock
                .Verify(transferService => transferService.GetAllTransfersAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetAllTransfers_ReturnsNotFound_When_AccountDoesNotExist(
            string accountIban)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetAllTransfersAsync(localId, accountIban))
                .ThrowsAsync(new UserException($"You do not have account with IBAN: {accountIban}", 404));

            // Act
            var result = await _sut.GetAllTransfers(accountIban);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<ShortTransferResponse>>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have account with IBAN: {accountIban}");

            _transferServiceMock
                .Verify(transferService => transferService.GetAllTransfersAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetTransfer_ReturnsTransfer_When_AllParametersArePassed(
            Guid id,
            TransferResponse transferResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetTransferAsync(localId, id))
                .ReturnsAsync(transferResponse);

            // Act
            var result = await _sut.GetTransfer(id);

            // Assert
            result.Should().BeOfType<ActionResult<TransferResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(transferResponse);

            _transferServiceMock
                .Verify(transferService => transferService.GetTransferAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task GetTransfer_ReturnsNotFound_When_TransferDoesNotExist(
            Guid id)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.GetTransferAsync(localId, id))
                .ThrowsAsync(new UserException($"You do not have the transfer with Id: {id}", 404));

            // Act
            var result = await _sut.GetTransfer(id);

            // Assert
            result.Should().BeOfType<ActionResult<TransferResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have the transfer with Id: {id}");

            _transferServiceMock
                .Verify(transferService => transferService.GetTransferAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task TopUpAccount_ReturnsTopUpResponse_When_AllParametersArePassed(
            TopUpRequest request,
            TopUpResponse topUpResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.UpdateAsync(localId, request.TopUp, request.AccountIban))
                .ReturnsAsync(topUpResponse);

            // Act
            var result = await _sut.TopUpAccount(request);

            // Assert
            result.Should().BeOfType<ActionResult<TopUpResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(topUpResponse);

            _transferServiceMock
                .Verify(transferService => transferService.UpdateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task TopUpAccount_ReturnsNotFound_When_AccountDoesNotExist(
            TopUpRequest request)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.UpdateAsync(localId, request.TopUp, request.AccountIban))
                .ThrowsAsync(new UserException($"You do not have account with IBAN: {request.AccountIban}", 404));

            // Act
            var result = await _sut.TopUpAccount(request);

            // Assert
            result.Should().BeOfType<ActionResult<TopUpResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have account with IBAN: {request.AccountIban}");

            _transferServiceMock
                .Verify(transferService => transferService.UpdateAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task MakeTransfer_ReturnsTransferResponse_When_AllParametersArePassed(
            TransferRequest request,
            TransferResponse transferResponse)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.UpdateAsync(localId, request))
                .ReturnsAsync(transferResponse);

            // Act
            var result = await _sut.MakeTransfer(request);

            // Assert
            result.Should().BeOfType<ActionResult<TransferResponse>>()
                .Which.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(transferResponse);

            _transferServiceMock
                .Verify(transferService => transferService.UpdateAsync(It.IsAny<string>(), It.IsAny<TransferRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task MakeTransfer_ReturnsNotFound_When_AccountDoesNotExist(
            TransferRequest request)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.UpdateAsync(localId, request))
                .ThrowsAsync(new UserException($"You do not have account with IBAN: {request.SenderAccountIban}.", 404));

            // Act
            var result = await _sut.MakeTransfer(request);

            // Assert
            result.Should().BeOfType<ActionResult<TransferResponse>>()
                .Which.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"You do not have account with IBAN: {request.SenderAccountIban}.");

            _transferServiceMock
                .Verify(transferService => transferService.UpdateAsync(It.IsAny<string>(), It.IsAny<TransferRequest>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task MakeTransfer_ReturnsBadRequest_When_NotEnoughMoney(
            TransferRequest request)
        {
            // Arrange
            var user = SetupHttpContent();
            var localId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            _transferServiceMock
                .Setup(transferService => transferService.UpdateAsync(localId, request))
                .ThrowsAsync(new UserException($"There is not enough money in your account with IBAN: {request.SenderAccountIban}", 400));

            // Act
            var result = await _sut.MakeTransfer(request);

            // Assert
            result.Should().BeOfType<ActionResult<TransferResponse>>()
                .Which.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo($"There is not enough money in your account with IBAN: {request.SenderAccountIban}");

            _transferServiceMock
                .Verify(transferService => transferService.UpdateAsync(It.IsAny<string>(), It.IsAny<TransferRequest>()), Times.Once);
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
