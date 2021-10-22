using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Response
{
    public class ReceiveResponse
    {

        public string SenderName { get; set; }

        public string SenderAccountIban { get; set; }

        public string Purpose { get; set; }

        public decimal Sum { get; set; }

        public DateTime DateTransferred { get; set; }
    }
}
