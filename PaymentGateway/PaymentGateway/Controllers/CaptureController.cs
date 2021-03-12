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
    public class CaptureController : ControllerBase
    {
        private readonly IAuthRequestAdapter _adapter;
        private readonly ILogger<CaptureController> _logger;

        public CaptureController(IAuthRequestAdapter adapter, ILogger<CaptureController> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        // POST: api/Auth
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TransactionItemResult>> CaptureTransactionItem(
            [FromBody] TransactionItemDto request)
        {
            try
            {
                _logger.LogInformation($"Validating request: {request}");
                await new TransactionItemValidator().ValidateAsync(request);
                _logger.LogTrace("Validation successful.");

                var transaction = await _adapter.AddCapture(request.Id, request.Amount);
                _logger.LogInformation("Capture successful.");

                return Ok(new TransactionItemResult
                {
                    StatusCode = HttpStatusCode.OK,
                    AuthorisedAmount = transaction.Amount,
                    AmountCaptured = transaction.AmountCaptured,
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