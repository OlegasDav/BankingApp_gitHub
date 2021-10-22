using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface ISqlClient
    {
        Task<int> ExecuteAsync(string sql, object param = null);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);

        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null);
    }
}
