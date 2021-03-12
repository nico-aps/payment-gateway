using System;
using FluentValidation;
using PaymentGateway.Model.DTO;

namespace PaymentGateway.Validators
{
    public class TransactionItemValidator : AbstractValidator<TransactionItemDto>
    {
        public TransactionItemValidator()
        {
            RuleFor(req => req.Amount).NotNull().NotEmpty().GreaterThan(0);
            RuleFor(req => req.Id).NotEqual(Guid.Empty);
        }
    }
}