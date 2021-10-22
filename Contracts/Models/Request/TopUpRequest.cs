using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Request
{
    public class TopUpRequest
    {
        public string AccountIban { get; set; }

        [Range(10, int.MaxValue, ErrorMessage = "The minimum top-up sum 10 Eur.")]
        public decimal TopUp { get; set; }
    }
}
