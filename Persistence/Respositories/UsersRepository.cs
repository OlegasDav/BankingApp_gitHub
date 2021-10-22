using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Respositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TableName = "user";

        public UsersRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<UserReadModel> GetAsync(string localId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE LocalId = @LocalId";

            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new { LocalId = localId });
        }

        public Task<int> SaveAsync(UserWriteModel user)
        {
            var sql = $"INSERT INTO {TableName} (Id, Username, Email, LocalId, DateCreated) VALUES(@Id, @Username, @Email, @LocalId, @DateCreated)";

            return _sqlClient.ExecuteAsync(sql, user);
        }
    }
}
