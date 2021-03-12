using FluentValidation;
using FluentValidation.AspNetCore;
using PaymentGateway.Model;
using PaymentGateway.Model.Adapters;
using PaymentGateway.Model.DTO;
using PaymentGateway.Validators;

            services.AddTransient<IAuthRequestAdapter, AuthRequestAdapter>();
            services.AddTransient<IValidator<AuthoriseRequestDto>, AuthoriseRequestValidator>();
            services.AddTransient<IValidator<TransactionItemDto>, TransactionItemValidator>();
