using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Request
{
    public class TransferRequest
    {
        public string SenderAccountIban { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverAccountIban { get; set; }

        public string Purpose { get; set; }

        [Range(0.01, int.MaxValue, ErrorMessage = "Incorrect amount.")]
        public decimal Sum { get; set; }
    }
}
