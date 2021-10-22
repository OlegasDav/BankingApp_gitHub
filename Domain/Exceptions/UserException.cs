using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserException : Exception
    {
        public override string Message { get; }
        public int StatusCode { get; }

        public UserException(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
