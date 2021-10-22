using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.WriteModels
{
    public class TopUpWriteModel
    {
        public Guid Id { get; set; }

        public string AccountIban { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTransferred { get; set; }
    }
}
