using Contracts.Models.Response;
using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountReadModel>> GetAllAsync(string localId);

        Task<AccountReadModel> GetAsync(string localId, string accountIban);

        Task<AccountResponse> CreateAsync(string localId);

        Task<int> DeleteAsync(string localId, string accountIban);
    }
}
