using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signUp")]
        public async Task<ActionResult<SignUpResponse>> SignUp(SignUpRequest request)
        {
            try
            {
                var response = await _authService.SignUpAsync(request);

                return Ok(response);
            }
            catch (FirebaseException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<ActionResult<SignInResponse>> SignIn(SignInRequest request)
        {
            try
            {
                var response = await _authService.SignInAsync(request);

                return Ok(response);
            }
            catch (FirebaseException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
