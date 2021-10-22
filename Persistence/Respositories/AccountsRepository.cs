using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TableName = "account";

        public AccountsRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<IEnumerable<AccountReadModel>> GetAllAsync(Guid userId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserId = @UserId";

            return _sqlClient.QueryAsync<AccountReadModel>(sql, new
            {
                UserId = userId
            });
        }

        public Task<AccountReadModel> GetAsync(string accountIban, Guid userId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE AccountIban = @AccountIban AND UserId = @UserId";

            return _sqlClient.QuerySingleOrDefaultAsync<AccountReadModel>(sql, new
            {
                AccountIban = accountIban,
                UserId = userId
            });
        }

        public Task<AccountReadModel> GetAsync(string accountIban)
        {
            var sql = $"SELECT * FROM {TableName} WHERE AccountIban = @AccountIban";

            return _sqlClient.QuerySingleOrDefaultAsync<AccountReadModel>(sql, new
            {
                AccountIban = accountIban,
            });
        }

        public Task<int> SaveOrUpdateAsync(AccountWriteModel model)
        {
            var sql = @$"INSERT INTO {TableName} (Id, AccountIban, UserId, Balance, DateOpened) 
                        VALUES (@Id, @AccountIban, @UserId, @Balance, @DateOpened)
                        ON DUPLICATE KEY UPDATE Balance = @Balance";

            return _sqlClient.ExecuteAsync(sql, model);
        }

        public Task<int> DeleteAsync(string accountIban)
        {
            var sql = $"DELETE FROM {TableName} WHERE AccountIban = @AccountIban";

            return _sqlClient.ExecuteAsync(sql, new 
            {
                AccountIban = accountIban
            });
        }
    }
}
