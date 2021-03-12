using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model.DTO
{
    public class AuthoriseRequestDto
    {
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public int CVV { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        
        
    }
}