using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Model
{
    public class PaymentGatewayContext : DbContext
    {
        public PaymentGatewayContext(DbContextOptions<PaymentGatewayContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> AuthRequests { get; set; }
    }
}