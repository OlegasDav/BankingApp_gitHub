using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public class TransfersRepository : ITransfersRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TopUpTableName = "topup";
        private const string TransferTableName = "transfer";

        public TransfersRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<IEnumerable<TransferReadModel>> GetAllTransfersAsync(string accountIban)
        {
            var sql = $"SELECT * FROM {TransferTableName} WHERE SenderAccountIban = @SenderAccountIban OR ReceiverAccountIban = @ReceiverAccountIban";

            return _sqlClient.QueryAsync<TransferReadModel>(sql, new
            {
                SenderAccountIban = accountIban,
                ReceiverAccountIban = accountIban
            });
        }

        public Task<TransferReadModel> GetTransferAsync(Guid id)
        {
            var sql = $"SELECT * FROM {TransferTableName} WHERE Id = @Id";

            return _sqlClient.QuerySingleOrDefaultAsync<TransferReadModel>(sql, new
            {
                Id = id,
            });
        }

        public Task<IEnumerable<TopUpReadModel>> GetAllTopUpAsync(string accountIban)
        {
            var sql = $"SELECT * FROM {TopUpTableName} WHERE AccountIban = @AccountIban";

            return _sqlClient.QueryAsync<TopUpReadModel>(sql, new
            {
                AccountIban = accountIban
            });
        }

        public Task<TopUpReadModel> GetTopUpAsync(Guid id)
        {
            var sql = $"SELECT * FROM {TopUpTableName} WHERE Id = @Id";

            return _sqlClient.QuerySingleOrDefaultAsync<TopUpReadModel>(sql, new
            {
                Id = id,
            });
        }

        public Task<int> SaveAsync(TopUpWriteModel model)
        {
            var sql = @$"INSERT INTO {TopUpTableName} (Id, AccountIban, Sum, DateTransferred) 
                        VALUES (@Id, @AccountIban, @Sum, @DateTransferred)";

            return _sqlClient.ExecuteAsync(sql, model);
        }

        public Task<int> SaveAsync(TransferWriteModel model)
        {
            var sql = @$"INSERT INTO {TransferTableName} (Id, SenderName, SenderAccountIban, ReceiverName, ReceiverAccountIban, Purpose, Sum, DateTransferred) 
                        VALUES (@Id, @SenderName, @SenderAccountIban, @ReceiverName, @ReceiverAccountIban, @Purpose, @Sum, @DateTransferred)";

            return _sqlClient.ExecuteAsync(sql, model);
        }

        public Task<int> DeleteTopUpAsync(string accountIban)
        {
            var sql = $"DELETE FROM {TopUpTableName} WHERE AccountIban = @AccountIban;";

            return _sqlClient.ExecuteAsync(sql, new { AccountIban = accountIban });
        }
    }
}
