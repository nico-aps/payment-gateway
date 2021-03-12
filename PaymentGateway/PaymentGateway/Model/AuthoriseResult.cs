using System;
using System.Net;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model
{
    public class AuthoriseResult
    {
        public Guid Id { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
    }
}