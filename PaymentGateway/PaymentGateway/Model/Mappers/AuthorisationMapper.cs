using System;
using System.Net;
using PaymentGateway.Model.DTO;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model.Mappers
{
    public static class AuthorisationMapper
    {
        public static Transaction ToTransaction(this AuthoriseRequestDto dto) => new Transaction
        {
            Id = Guid.NewGuid(),
            CardNumber = dto.CardNumber,
            ExpMonth = dto.ExpMonth,
            ExpYear = dto.ExpYear,
            CVV = dto.CVV,
            Amount = dto.Amount,
            Currency = dto.Currency,
            AmountCaptured = 0,
            AmountRefunded = 0,
            Status = TransactionStatus.Active
        };

        public static AuthoriseResult ToAuthResult(this Transaction request, HttpStatusCode statusCode) => new AuthoriseResult
        {
            Id = request.Id,
            Amount = request.Amount,
            Currency = request.Currency.ToString(),
            StatusCode = statusCode
        };
    }
}