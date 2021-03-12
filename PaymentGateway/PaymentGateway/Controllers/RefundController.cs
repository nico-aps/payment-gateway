using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Model;
using PaymentGateway.Model.Adapters;
using PaymentGateway.Model.DTO;
using PaymentGateway.Model.Generic;
using PaymentGateway.Validators;

namespace PaymentGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IAuthRequestAdapter _adapter;
        private readonly ILogger<RefundController> _logger;


        public RefundController(IAuthRequestAdapter adapter, ILogger<RefundController> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        // POST: api/Auth
        [HttpPost]
        public async Task<ActionResult<TransactionItemResult>> RefundTransactionItem(
            [FromBody] TransactionItemDto request)
        {
            try
            {
                _logger.LogInformation($"Validating request: {request}");
                await new TransactionItemValidator().ValidateAndThrowAsync(request);
                _logger.LogTrace("Validation successful.");

                var transaction = await _adapter.AddRefund(request.Id, request.Amount);
                _logger.LogInformation("Refund successful.");

                return Ok(new TransactionItemResult
                {
                    StatusCode = HttpStatusCode.OK,
                    AuthorisedAmount = transaction.Amount,
                    AmountCaptured = transaction.AmountCaptured,
                    AmountRefunded = transaction.AmountRefunded,
                    Currency = request.Currency.ToString()
                });
            }
            catch (ArgumentOutOfRangeException ae)
            {
                _logger.LogError($"Argument exception: {ae.Message}");
                return BadRequest(new ErrorMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ae.Message
                });
            }
            catch (ArgumentException ae)
            {
                _logger.LogError($"Argument exception: {ae.Message}");
                return NotFound(new ErrorMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = ae.Message
                });
            }
            catch (InvalidOperationException ie)
            {
                _logger.LogError($"Invalid operation: {ie.Message}");
                return BadRequest(new ErrorMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ie.Message
                });
            }
            catch (ValidationException ve)
            {
                _logger.LogError($"Request was invalid with the following errors: {ve.Errors}");
                return BadRequest(new ErrorMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ve.Message
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error: {e.Message}");
                return StatusCode(500, new ErrorMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = e.Message
                });
            }
        }
    }
}