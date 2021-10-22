using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Response
{
    public class TopUpResponse
    {
        public string AccountIban { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTransferred { get; set; }

        public decimal Balance { get; set; }
    }
}
