using System;
using System.ComponentModel.DataAnnotations;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model.DTO
{
    public class TransactionItemDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public Currency Currency { get; set; }
    }
}