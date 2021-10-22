using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public interface IAccountsRepository
    {
        Task<int> SaveOrUpdateAsync(AccountWriteModel model);

        Task<IEnumerable<AccountReadModel>> GetAllAsync(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountIban"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<AccountReadModel> GetAsync(string accountIban, Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountIban"></param>
        /// <returns></returns>
        Task<AccountReadModel> GetAsync(string accountIban);

        Task<int> DeleteAsync(string accountIban);
    }
}
