using Contracts.Models.Response;
using Domain.Exceptions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using Persistence.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ITransfersRepository _transfersRepository;

        public AccountService(IUsersRepository usersRepository, IAccountsRepository accountsRepository, ITransfersRepository transfersRepository)
        {
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
            _transfersRepository = transfersRepository;
        }

        public async Task<IEnumerable<AccountReadModel>> GetAllAsync(string localId)
        {
            var user = await _usersRepository.GetAsync(localId);

            var accounts = await _accountsRepository.GetAllAsync(user.Id);

            return accounts;
        }

        public async Task<AccountReadModel> GetAsync(string localId, string accountIban)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = await _accountsRepository.GetAsync(accountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have account with IBAN: {accountIban}", 404);
            }

            return account;
        }

        public async Task<AccountResponse> CreateAsync(string localId)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = new AccountWriteModel
            { 
                Id = Guid.NewGuid(),
                AccountIban = Guid.NewGuid().ToString("N"),
                UserId = user.Id,
                Balance = 0,
                DateOpened = DateTime.Now
            };

            await _accountsRepository.SaveOrUpdateAsync(account);

            return new AccountResponse 
            {
                AccountIban = account.AccountIban,
                Balance = account.Balance,
                DateOpened = account.DateOpened
            };
        }


        public async Task<int> DeleteAsync(string localId, string accountIban)
        {
            var user = await _usersRepository.GetAsync(localId);

            var account = await _accountsRepository.GetAsync(accountIban, user.Id);

            if (account is null)
            {
                throw new UserException($"You do not have the account with IBAN: {accountIban}", 404);
            }

            if (account.Balance > 0)
            {
                throw new UserException($"You cannot close the account with IBAN: {accountIban} till there is some money", 400);
            }

            var taskToDeleteAccount = _accountsRepository.DeleteAsync(accountIban);
            var taskToDeleteTopUp = _transfersRepository.DeleteTopUpAsync(accountIban);

            await Task.WhenAll(taskToDeleteAccount, taskToDeleteTopUp);

            return await taskToDeleteAccount;
        }
    }
}
