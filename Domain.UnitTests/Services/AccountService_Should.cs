using AutoFixture.Xunit2;
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
    public class AccountService_Should
    {
        private readonly Random _random = new Random();

        [Theory]
        [AutoMoqData]
        public async Task GetAllAsync_ReturnsAccounts(
            string localId,
            UserReadModel userReadModel,
            IEnumerable<AccountReadModel> accountReadModels,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAllAsync(userReadModel.Id))
                .ReturnsAsync(accountReadModels);

            // Act
            var result = await sut.GetAllAsync(localId);

            //Assert
            result.Should().BeEquivalentTo(accountReadModels);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAllAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync((AccountReadModel) null);

            // Act & assert
            var result = await sut.Invoking(sut => sut.GetAsync(localId, accountIban))
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
        public async Task GetAsync_ReturnsAccount_When_AccountIbanExist(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
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
            var result = await sut.GetAsync(localId, accountIban);

            // Assert
            result.Should().BeEquivalentTo(accountReadModel);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task CreateAsync_ReturnsAccountResponse(
            string localId,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            // Act
            var result = await sut.CreateAsync(localId);

            // Assert
            result.Balance.Should().Be(0);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .SaveOrUpdateAsync(It.Is<AccountWriteModel>(model => model.UserId.Equals(userReadModel.Id) && model.Balance.Equals(0))), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteAsync_ReturnsNotFound_When_AccountDoesNotExist(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
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
            var result = await sut.Invoking(sut => sut.DeleteAsync(localId, accountIban))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You do not have the account with IBAN: {accountIban}");

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
        public async Task DeleteAsync_ReturnsBadRequest_When_AccountIsNotEmpty(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            AccountService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountReadModel.Balance = _random.Next(1, int.MaxValue);

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            // Act & assert
            var result = await sut.Invoking(sut => sut.DeleteAsync(localId, accountIban))
                .Should().ThrowAsync<UserException>()
                .WithMessage($"You cannot close the account with IBAN: {accountIban} till there is some money");

            result.Which.StatusCode.Should().Be(400);

            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteAsync_DeleteAccount_When_AccountExistsAndIsEmpty(
            string localId,
            string accountIban,
            UserReadModel userReadModel,
            AccountReadModel accountReadModel,
            [Frozen] Mock<IUsersRepository> usersRepositoryMock,
            [Frozen] Mock<IAccountsRepository> accountsRepositoryMock,
            [Frozen] Mock<ITransfersRepository> transfersRepositoryMock,
            AccountService sut)
        {
            // Arrange
            usersRepositoryMock
                .Setup(usersRepository => usersRepository
                .GetAsync(localId))
                .ReturnsAsync(userReadModel);

            accountReadModel.Balance = 0;

            accountsRepositoryMock
                .Setup(accountsRepository => accountsRepository
                .GetAsync(accountIban, userReadModel.Id))
                .ReturnsAsync(accountReadModel);

            // Act 
            var result = await sut.DeleteAsync(localId, accountIban);

            // Assert
            usersRepositoryMock
                .Verify(usersRepository => usersRepository
                .GetAsync(It.IsAny<string>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .GetAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);

            accountsRepositoryMock
                .Verify(accountsRepository => accountsRepository
                .DeleteAsync(accountIban), Times.Once);

            transfersRepositoryMock
                .Verify(transfersRepository => transfersRepository
                .DeleteTopUpAsync(accountIban), Times.Once);
        }
    }
}
