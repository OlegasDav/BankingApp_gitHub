using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Authorize]
    [EmailVerification]
    [ApiController]
    [Route("accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll()
        {
            var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var accounts = await _accountService.GetAllAsync(localId);

            return Ok(accounts.Select(account => new AccountResponse
            {
                AccountIban = account.AccountIban,
                Balance = account.Balance,
                DateOpened = account.DateOpened
            }));
        }

        [HttpGet]
        [Route("{accountIban}")]
        public async Task<ActionResult<AccountResponse>> Get(string accountIban)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var account = await _accountService.GetAsync(localId, accountIban);

                return Ok(new AccountResponse
                {
                    AccountIban = account.AccountIban,
                    Balance = account.Balance,
                    DateOpened = account.DateOpened
                });
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountResponse>> OpenAccount()
        {
            var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var account = await _accountService.CreateAsync(localId);

            return CreatedAtAction(nameof(Get), new { account.AccountIban }, account);
        }

        [HttpDelete]
        [Route("{accountIban}")]
        public async Task<ActionResult> CloseAccount(string accountIban)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _accountService.DeleteAsync(localId, accountIban);

                return NoContent();
            }
            catch (UserException exception)
            {
                switch (exception.StatusCode)
                {
                    case 404:
                        return NotFound(exception.Message);
                    case 400:
                        return BadRequest(exception.Message);
                    default: throw;
                }
            }
        }
    }
}
