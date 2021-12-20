using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public interface ITransactionsRepository
    {
        Task<IEnumerable<TransferReadModel>> GetAllTransfersAsync(string accountIban);

        Task<TransferReadModel> GetTransferAsync(Guid id);

        Task<IEnumerable<TopUpReadModel>> GetAllTopUpAsync(string accountIban);

        Task<TopUpReadModel> GetTopUpAsync(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveAsync(TopUpWriteModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveAsync(TransferWriteModel model);

        Task<int> DeleteTopUpAsync(string accountIban);
    }
}
