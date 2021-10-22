﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.Request
{
    public class SignUpRequest
    {
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
