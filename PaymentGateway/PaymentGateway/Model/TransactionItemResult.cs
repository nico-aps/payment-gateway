using System.Net;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model
{
    public class TransactionItemResult
    {
        public double AuthorisedAmount { get; set; }
        public double AmountCaptured { get; set; }
        public double AmountRefunded { get; set; }
        public string Currency { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}