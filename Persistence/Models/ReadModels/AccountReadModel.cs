using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.ReadModels
{
    public class AccountReadModel
    {
        public Guid Id { get; set; }

        public string AccountIban { get; set; }

        public Guid UserId { get; set; }

        public decimal Balance { get; set; }

        public DateTime DateOpened { get; set; }
    }
}
