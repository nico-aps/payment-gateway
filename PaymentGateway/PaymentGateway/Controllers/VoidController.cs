using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Model;
using PaymentGateway.Model.Adapters;
using PaymentGateway.Model.DTO;
using PaymentGateway.Model.Generic;

namespace PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoidController : ControllerBase
    {
        private readonly IAuthRequestAdapter _adapter;
        private readonly ILogger<VoidController> _logger;

        public VoidController(IAuthRequestAdapter adapter, ILogger<VoidController> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        // POST: api/Void
        [HttpPost]
        public async Task<ActionResult<TransactionItemResult>> VoidTransaction(TransactionItemDto request)
        {
            try
            {
                var transaction = await _adapter.VoidTransaction(request.Id);
                _logger.LogInformation("Transaction voided successfully.");

                return Ok(new TransactionItemResult
                {
                    StatusCode = HttpStatusCode.OK,
                    AuthorisedAmount = transaction.Amount,
                    AmountCaptured = transaction.AmountCaptured,
                    AmountRefunded = transaction.AmountRefunded,
                    Currency = transaction.Currency.ToString()
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error: {e}");
                return StatusCode(500, new ErrorMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = e.Message
                });
            }
        }
    }
}