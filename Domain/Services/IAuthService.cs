using Contracts.Models.Request;
using Contracts.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);

        Task<SignInResponse> SignInAsync(SignInRequest request);
    }
}
