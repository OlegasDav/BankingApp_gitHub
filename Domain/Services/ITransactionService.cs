using Contracts.Models;
using Contracts.Models.Request;
using Contracts.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<ShortTopUpsResponse>> GetAllTopUpsAsync(string localId, string accountIban);

        Task<ShortTopUpResponse> GetTopUpAsync(string localId, Guid id);

        Task<IEnumerable<ShortTransferResponse>> GetAllTransfersAsync(string localId, string accountIban);

        Task<TransferResponse> GetTransferAsync(string localId, Guid id);

        Task<TopUpResponse> UpdateAsync(string localId, decimal topUp, string accountIban);

        Task<TransferResponse> UpdateAsync(string localId, TransferRequest model);
    }
}
