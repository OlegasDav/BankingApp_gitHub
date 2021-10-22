using Contracts.Models;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Persistence.Models.WriteModels;
using Persistence.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class TransferService : ITransferService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ITransfersRepository _transfersRepository;

        public TransferService(IUsersRepository usersRepository, IAccountsRepository accountsRepository, ITransfersRepository transfersRepository)
        {
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
            _transfersRepository = transfersRepository;
        }

        public async Task<IEnumerable<ShortTopUpsResponse>> GetAllTopUpsAsync(string localId, string accountIban)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = await _accountsRepository.GetAsync(accountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have account with IBAN: {accountIban}", 404);
            }

            var topUps = await _transfersRepository.GetAllTopUpAsync(accountIban);

            return topUps.Select(topUp => new ShortTopUpsResponse 
            {
                Id = topUp.Id,
                Sum = topUp.Sum,
                DateTransferred = topUp.DateTransferred
            });
        }

        public async Task<ShortTopUpResponse> GetTopUpAsync(string localId, Guid id)
        {
            var user = await _usersRepository.GetAsync(localId);

            var topUp = await _transfersRepository.GetTopUpAsync(id);

            if (topUp is null)
            {
                throw new UserException($"You do not have TopUP with Id: {id}", 404);
            }

            var account = await _accountsRepository.GetAsync(topUp.AccountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have TopUP with Id: {id}", 404);
            }

            return new ShortTopUpResponse
            {
                AccountIban = topUp.AccountIban,
                Sum = topUp.Sum,
                DateTransferred = topUp.DateTransferred
            };
        }

        public async Task<IEnumerable<ShortTransferResponse>> GetAllTransfersAsync(string localId, string accountIban)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = await _accountsRepository.GetAsync(accountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have account with IBAN: {accountIban}", 404);
            }

            var transfers = await _transfersRepository.GetAllTransfersAsync(accountIban);

            return transfers.Select(transfer => new ShortTransferResponse
            {
                Id = transfer.Id,
                Purpose = transfer.Purpose,
                Sum = transfer.Sum,
                DateTransferred = transfer.DateTransferred
            });
        }

        public async Task<TransferResponse> GetTransferAsync(string localId, Guid id)
        {
            var user = await _usersRepository.GetAsync(localId);

            var transfer = await _transfersRepository.GetTransferAsync(id);

            if (transfer is null)
            {
                throw new UserException($"You do not have the transfer with Id: {id}", 404);
            }

            var accountSender = await _accountsRepository.GetAsync(transfer.SenderAccountIban, user.Id);

            var accountReceiver = await _accountsRepository.GetAsync(transfer.ReceiverAccountIban, user.Id);

            if (accountSender is null && accountReceiver is null)
            {
                throw new UserException($"You do not have the transfer with Id: {id}", 404);
            }

            return new TransferResponse
            {
                Id = transfer.Id,
                SenderName = transfer.SenderName,
                SenderAccountIban = transfer.SenderAccountIban,
                ReceiverName = transfer.ReceiverName,
                ReceiverAccountIban = transfer.ReceiverAccountIban,
                Purpose = transfer.Purpose,
                Sum = transfer.Sum,
                DateTransferred = transfer.DateTransferred,
            };
        }

        public async Task<TopUpResponse> UpdateAsync(string localId, decimal topUp, string accountIban)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = await _accountsRepository.GetAsync(accountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have account with IBAN: {accountIban}", 404);
            }

            var accountUpdated = new AccountWriteModel
            {
                Id = account.Id,
                AccountIban = account.AccountIban,
                UserId = account.UserId,
                Balance = account.Balance + topUp,
                DateOpened = account.DateOpened
            };

            var transfer = new TopUpWriteModel
            {
                Id = Guid.NewGuid(),
                AccountIban = accountUpdated.AccountIban,
                Sum = topUp,
                DateTransferred = DateTime.Now
            };

            var taskToUpdateAccount = _accountsRepository.SaveOrUpdateAsync(accountUpdated);
            var taskToSaveTopUp = _transfersRepository.SaveAsync(transfer);

            await Task.WhenAll(taskToUpdateAccount, taskToSaveTopUp);

            return new TopUpResponse
            {
                AccountIban = accountUpdated.AccountIban,
                Sum = transfer.Sum,
                DateTransferred = transfer.DateTransferred,
                Balance = accountUpdated.Balance
            };
        }

        public async Task<TransferResponse> UpdateAsync(string localId, TransferRequest model)
        {
            var user = await _usersRepository.GetAsync(localId);

            var accountSender = await _accountsRepository.GetAsync(model.SenderAccountIban, user.Id);

            if (accountSender is null)
            {
                throw new UserException($"You do not have account with IBAN: {model.SenderAccountIban}.", 404);
            }

            var accountReceiver = await _accountsRepository.GetAsync(model.ReceiverAccountIban);

            if (accountReceiver is null)
            {
                throw new UserException($"The receiver with account IBAN: {model.ReceiverAccountIban} does not exist.", 404);
            }

            if (model.Sum > accountSender.Balance)
            {
                throw new UserException($"There is not enough money in your account with IBAN: {model.SenderAccountIban}", 400);
            }

            var transferWriteModel = new TransferWriteModel
            {
                Id = Guid.NewGuid(),
                SenderName = user.Username,
                SenderAccountIban = model.SenderAccountIban,
                ReceiverName = model.ReceiverName,
                ReceiverAccountIban = model.ReceiverAccountIban,
                Purpose = model.Purpose,
                Sum = model.Sum,
                DateTransferred = DateTime.Now
            };

            await _transfersRepository.SaveAsync(transferWriteModel);

            var accountSenderUpdated = new AccountWriteModel
            {
                Id = accountSender.Id,
                AccountIban = accountSender.AccountIban,
                UserId = accountSender.UserId,
                Balance = accountSender.Balance - model.Sum,
                DateOpened = accountSender.DateOpened
            };

            await _accountsRepository.SaveOrUpdateAsync(accountSenderUpdated);

            var accountReceiverUpdated = new AccountWriteModel
            {
                Id = accountReceiver.Id,
                AccountIban = accountReceiver.AccountIban,
                UserId = accountReceiver.UserId,
                Balance = accountReceiver.Balance + model.Sum,
                DateOpened = accountReceiver.DateOpened
            };

            await _accountsRepository.SaveOrUpdateAsync(accountReceiverUpdated);

            return new TransferResponse
            {
                Id = transferWriteModel.Id,
                SenderName = transferWriteModel.SenderName,
                SenderAccountIban = transferWriteModel.SenderAccountIban,
                ReceiverName = transferWriteModel.ReceiverName,
                ReceiverAccountIban = transferWriteModel.ReceiverAccountIban,
                Purpose = transferWriteModel.Purpose,
                Sum = transferWriteModel.Sum,
                DateTransferred = transferWriteModel.DateTransferred
            };
        }
    }
}
