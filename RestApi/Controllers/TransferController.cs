using Contracts.Models;
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
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpGet]
        [Route("transfers/{accountIban}/topUps")]
        public async Task<ActionResult<IEnumerable<ShortTopUpsResponse>>> GetAllTopUps(string accountIban)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var topUps = await _transferService.GetAllTopUpsAsync(localId, accountIban);

                return Ok(topUps);
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet]
        [Route("transfers/topUps/{id}")]
        public async Task<ActionResult<ShortTopUpResponse>> GetTopUp(Guid id)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var topUp = await _transferService.GetTopUpAsync(localId, id);

                return Ok(topUp);
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet]
        [Route("{accountIban}/transfers")]
        public async Task<ActionResult<IEnumerable<ShortTransferResponse>>> GetAllTransfers(string accountIban)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transfers = await _transferService.GetAllTransfersAsync(localId, accountIban);

                return Ok(transfers);
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet]
        [Route("transfers/{id}")]
        public async Task<ActionResult<TransferResponse>> GetTransfer(Guid id)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transfer = await _transferService.GetTransferAsync(localId, id);

                return Ok(transfer);
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        [Route("transfers/topUpAccount")]
        public async Task<ActionResult<TopUpResponse>> TopUpAccount([FromBody] TopUpRequest request)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var account = await _transferService.UpdateAsync(localId, request.TopUp, request.AccountIban);

                return Ok(account);
            }
            catch (UserException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        [Route("transfers/makeTransfer")]
        public async Task<ActionResult<TransferResponse>> MakeTransfer([FromBody] TransferRequest request)
        {
            try
            {
                var localId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transfer = await _transferService.UpdateAsync(localId, request);

                return Ok(transfer);
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
