using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.ReadModels
{
    public class TransferReadModel
    {
        public Guid Id { get; set; }

        public string SenderName { get; set; }

        public string SenderAccountIban { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverAccountIban { get; set; }

        public string Purpose { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTransferred { get; set; }
    }
}
