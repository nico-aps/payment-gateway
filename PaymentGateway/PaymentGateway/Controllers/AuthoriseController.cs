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
using PaymentGateway.Model.Mappers;
using PaymentGateway.Validators;

namespace PaymentGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthoriseController : ControllerBase
    {
        private readonly IAuthRequestAdapter _adapter;
        private readonly ILogger<AuthoriseController> _logger;

        public AuthoriseController(IAuthRequestAdapter adapter, ILogger<AuthoriseController> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        // POST: api/Auth
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AuthoriseResult>> PostAuthRequest([FromBody] AuthoriseRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Validating request: {request}");
                await new AuthoriseRequestValidator().ValidateAndThrowAsync(request);
                _logger.LogTrace("Validation successful.");
            
                _logger.LogTrace("Mapping to transaction object...");
                var authRequest = request.ToTransaction();
                
                await _adapter.AddTransaction(authRequest);
                _logger.LogTrace("Transaction object created.");

                var result = authRequest.ToAuthResult(HttpStatusCode.Created);

                return CreatedAtAction("PostAuthRequest", new { id = authRequest.Id }, result);
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
