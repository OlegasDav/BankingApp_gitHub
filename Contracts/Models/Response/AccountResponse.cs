using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Response
{
    public class AccountResponse
    {
        public string AccountIban { get; set; }

        public decimal Balance { get; set; }

        public DateTime DateOpened { get; set; }
    }
}
