using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public interface IUsersRepository
    {
        Task<int> SaveAsync(UserWriteModel model);

        Task<UserReadModel> GetAsync(string firebaseId);
    }
}
