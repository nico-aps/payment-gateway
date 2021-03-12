using System;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public int CVV { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        public double AmountCaptured { get; set; }
        public double AmountRefunded { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
