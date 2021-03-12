using System;
using FluentValidation;
using PaymentGateway.Model.DTO;

namespace PaymentGateway.Validators
{
    public class AuthoriseRequestValidator : AbstractValidator<AuthoriseRequestDto>
    {
        public AuthoriseRequestValidator()
        {
            RuleFor(req => req.ExpYear)
                .NotEmpty().InclusiveBetween(DateTime.Now.Year, DateTime.Now.Year + 4);
            RuleFor(req => req.ExpMonth)
                .NotEmpty().InclusiveBetween(1, 12);
            When(req => req.ExpYear.Equals(DateTime.Now.Year),
                () => {
                    RuleFor(req => req.ExpMonth).NotEmpty().InclusiveBetween(DateTime.Now.Month, 12);
                });
            RuleFor(req => req.CVV).NotEmpty().InclusiveBetween(100, 999);
            RuleFor(req => req.Currency).IsInEnum();
            RuleFor(req => req.CardNumber).CreditCard();
        }
    }
}