using System.Net;

namespace PaymentGateway.Model.Generic
{
    public class ErrorMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}