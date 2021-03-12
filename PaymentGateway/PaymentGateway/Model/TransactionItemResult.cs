using System.Net;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model
{
    public class TransactionItemResult
    {
        public double AuthorisedAmount { get; set; }
        public double AmountCaptured { get; set; }
        public double AmountRefunded { get; set; }
        public Currency Currency { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}