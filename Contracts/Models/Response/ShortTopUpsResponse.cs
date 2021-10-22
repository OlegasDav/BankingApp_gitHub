using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Response
{
    public class ShortTopUpsResponse
    {
        public Guid Id { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTransferred { get; set; }
    }
}
