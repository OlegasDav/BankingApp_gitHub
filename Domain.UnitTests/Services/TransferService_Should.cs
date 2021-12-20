using AutoFixture.Xunit2;
using Contracts.Models.Request;
using Domain.Exceptions;
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
    public class TransferService_Should
    {
        private readonly Random _random = new Random();

        [Theory]
        [AutoMoqData]
        public async Task GetAllTopUpsAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetAllTopUpsAsync(localId, accountIban))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have account with IBAN: {accountIban}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetAllTopUpsAsync_ReturnsTopUpsList_When_AccountExists(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            IEnumerable<TopUpReadModel> topUpReadModels,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetAllTopUpAsync(accountIban))
                .ReturnsAsync(topUpReadModels);

            // Act 
            var result = await sut.GetAllTopUpsAsync(localId, accountIban);

            // Assert
            result.Should().BeEquivalentTo(topUpReadModels, option => option
                .ExcludingMissingMembers());

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetAllTopUpAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTopUpAsync_ReturnsNotFound_When_TopUpDoesNotExist(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTopUpAsync(id))
                .ReturnsAsync((TopUpReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetTopUpAsync(localId, id))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have TopUP with Id: {id}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTopUpAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTopUpAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            TopUpReadModel topUpReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTopUpAsync(id))
                .ReturnsAsync(topUpReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(topUpReadModel.AccountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetTopUpAsync(localId, id))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have TopUP with Id: {id}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTopUpAsync(It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTopUpAsync_ReturnsTopUp_When_AllParametersArePassed(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            TopUpReadModel topUpReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTopUpAsync(id))
                .ReturnsAsync(topUpReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(topUpReadModel.AccountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            // Act
            var result = await sut.GetTopUpAsync(localId, id);

            // Assert
            result.Should().BeEquivalentTo(topUpReadModel, option => option
                .ExcludingMissingMembers());

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTopUpAsync(It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetAllTransfersAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetAllTransfersAsync(localId, accountIban))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have account with IBAN: {accountIban}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetAllTransfersAsync_ReturnsTransfersList_When_AccountExists(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            IEnumerable<TransferReadModel> transferReadModels,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetAllTransfersAsync(accountIban))
                .ReturnsAsync(transferReadModels);

            // Act 
            var result = await sut.GetAllTransfersAsync(localId, accountIban);

            // Assert
            result.Should().BeEquivalentTo(transferReadModels, option => option
                .ExcludingMissingMembers());

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetAllTransfersAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTransferAsync_ReturnsNotFound_When_TransferDoesNotExist(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTransferAsync(id))
                .ReturnsAsync((TransferReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetTransferAsync(localId, id))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have the transfer with Id: {id}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTransferAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTransferAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            TransferReadModel transferReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTransferAsync(id))
                .ReturnsAsync(transferReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferReadModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferReadModel.ReceiverAccountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetTransferAsync(localId, id))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have the transfer with Id: {id}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTransferAsync(It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public async Task GetTransferAsyncc_ReturnsTransfer_When_AllParametersArePassed(
            string localId,
            Guid id,
            UserReadModel userReadModel,
            TransferReadModel transferReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            transfersRepositoryMock
                .Setup(transfersRepository => transfersRepository
                .GetTransferAsync(id))
                .ReturnsAsync(transferReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferReadModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferReadModel.ReceiverAccountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act
            var result = await sut.GetTransferAsync(localId, id);

            // Assert
            result.Should().BeEquivalentTo(transferReadModel, option => option
                .ExcludingMissingMembers());

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .GetTransferAsync(It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            decimal topUp,
            string accountIban,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.UpdateAsync(localId, topUp, accountIban))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have account with IBAN: {accountIban}");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_TopUpAccount_When_AccountExists(
            string localId,
            decimal topUp,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            // Act
            var result = await sut.UpdateAsync(localId, topUp, accountIban);

            // Assert
            result.AccountIban.Should().BeEquivalentTo(accountReadModel.AccountIban);
            result.Sum.Should().Be(topUp);
            result.Balance.Should().Be(accountReadModel.Balance + topUp);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .SaveOrUpdateAsync(It.Is<AccountWriteModel>(model => model.Id.Equals(accountReadModel.Id) 
                && model.AccountIban.Equals(accountReadModel.AccountIban)
                && model.UserId.Equals(accountReadModel.UserId)
                && model.Balance.Equals(accountReadModel.Balance + topUp)
                && model.DateOpened.Equals(accountReadModel.DateOpened))), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .SaveAsync(It.Is<TopUpWriteModel>(model => model.AccountIban.Equals(accountReadModel.AccountIban)
                && model.Sum.Equals(topUp))), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_ReturnsNotFound_When_SenderAccountDoesNotExist(
            string localId,
            TransferRequest transferRequestModel,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel)null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.UpdateAsync(localId, transferRequestModel))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have account with IBAN: {transferRequestModel.SenderAccountIban}.");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_ReturnsNotFound_When_ReceiverAccountDoesNotExist(
            string localId,
            TransferRequest transferRequestModel,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.ReceiverAccountIban))
                .ReturnsAsync((AccountReadModel) null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.UpdateAsync(localId, transferRequestModel))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"The receiver with account IBAN: {transferRequestModel.ReceiverAccountIban} does not exist.");

            result.Which.StatusCode.Should().Be(404);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_ReturnsBadRequest_When_ThereIsNotEnoughMoney(
            string localId,
            TransferRequest transferRequestModel,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel1,
            AccountReadModel accountReadModel2,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            accountReadModel1.Balance = _random.Next(1, 100);
            transferRequestModel.Sum = _random.Next(101, int.MaxValue);

            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel1);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.ReceiverAccountIban))
                .ReturnsAsync(accountReadModel2);

            // Act & assert
            var result = await sut.Invoking(sut => sut.UpdateAsync(localId, transferRequestModel))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"There is not enough money in your account with IBAN: {transferRequestModel.SenderAccountIban}");

            result.Which.StatusCode.Should().Be(400);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateAsync_MakeTransfer_When_AllParametersArePassed(
            string localId,
            TransferRequest transferRequestModel,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel1,
            AccountReadModel accountReadModel2,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransactionsRepository> transfersRepositoryMock,
            TransactionService sut)
        {
            // Arrange
            accountReadModel1.Balance = _random.Next(101, int.MaxValue);
            transferRequestModel.Sum = _random.Next(1, 100);

            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.SenderAccountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel1);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(transferRequestModel.ReceiverAccountIban))
                .ReturnsAsync(accountReadModel2);

            // Act
            var result = await sut.UpdateAsync(localId, transferRequestModel);

            // Assert
            result.SenderName.Should().BeEquivalentTo(userReadModel.Username);
            result.SenderAccountIban.Should().BeEquivalentTo(transferRequestModel.SenderAccountIban);
            result.ReceiverName.Should().BeEquivalentTo(transferRequestModel.ReceiverName);
            result.ReceiverAccountIban.Should().BeEquivalentTo(transferRequestModel.ReceiverAccountIban);
            result.Purpose.Should().BeEquivalentTo(transferRequestModel.Purpose);
            result.Sum.Should().Be(transferRequestModel.Sum);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .SaveAsync(It.Is<TransferWriteModel>(model => model.SenderName.Equals(userReadModel.Username)
                && model.SenderAccountIban.Equals(transferRequestModel.SenderAccountIban)
                && model.ReceiverAccountIban.Equals(transferRequestModel.ReceiverAccountIban)
                && model.ReceiverName.Equals(transferRequestModel.ReceiverName)
                && model.Purpose.Equals(transferRequestModel.Purpose)
                && model.Sum.Equals(transferRequestModel.Sum))), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .SaveOrUpdateAsync(It.Is<AccountWriteModel>(model => model.Id.Equals(accountReadModel1.Id)
                && model.AccountIban.Equals(accountReadModel1.AccountIban)
                && model.UserId.Equals(accountReadModel1.UserId)
                && model.Balance.Equals(accountReadModel1.Balance - transferRequestModel.Sum)
                && model.DateOpened.Equals(accountReadModel1.DateOpened))), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .SaveOrUpdateAsync(It.Is<AccountWriteModel>(model => model.Id.Equals(accountReadModel2.Id)
                && model.AccountIban.Equals(accountReadModel2.AccountIban)
                && model.UserId.Equals(accountReadModel2.UserId)
                && model.Balance.Equals(accountReadModel2.Balance + transferRequestModel.Sum)
                && model.DateOpened.Equals(accountReadModel2.DateOpened))), Times.Once);
        }
    }
}
