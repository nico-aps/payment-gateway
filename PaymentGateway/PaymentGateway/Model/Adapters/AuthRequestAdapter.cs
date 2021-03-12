using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Model.Enum;

namespace PaymentGateway.Model.Adapters
{
    public class AuthRequestAdapter : IAuthRequestAdapter
    {
        private readonly PaymentGatewayContext _context;

        public AuthRequestAdapter(PaymentGatewayContext context)
        {
            _context = context;
        }

        public async Task AddTransaction(Transaction request)
        {
            if (request.CardNumber == "40000000000119")
                throw new InvalidOperationException("Generic Authorisation failure.");
            await _context.AuthRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<Transaction> GetTransaction(Guid id) =>
            await _context.AuthRequests.SingleOrDefaultAsync(t => t.Id == id);

        public async Task<Transaction> AddCapture(Guid id, double amount)
        {
            var result = await GetTransaction(id);

            if (result == null) throw new ArgumentException($"Cannot find transaction {id}.");
            if (result.CardNumber == "40000000000259")
                throw new InvalidOperationException("Generic Capture failure.");
            if (result.Status != TransactionStatus.Active)
                throw new InvalidOperationException($"Transaction {id} does not allow captures.");
            var amountToCapture = result.AmountCaptured + amount;
            if (amountToCapture > result.Amount)
                throw new ArgumentOutOfRangeException(
                    $"Amount to capture ({amount}) is higher than " + 
                    $"the remaining amount allowed to capture ({result.Amount - result.AmountCaptured}).");

            result.AmountCaptured += amount;
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<Transaction> AddRefund(Guid id, double amount)
        {
            var result = await GetTransaction(id);

            if (result == null) throw new ArgumentException($"Cannot find transaction {id}.");
            if (result.CardNumber == "4000000000003238")
                throw new InvalidOperationException("Generic Refund failure.");
            if (!(result.Status == TransactionStatus.Active || result.Status == TransactionStatus.Refund))
                throw new InvalidOperationException($"Transaction {id} does not allow refunds.");

            var amountToRefund = result.AmountRefunded + amount;
            if (amountToRefund > result.AmountCaptured)
                throw new ArgumentOutOfRangeException(
                    $"Amount to refund ({amount}) is higher than the remaining amount allowed to refund ({amountToRefund}).");


            if (result.AmountRefunded == 0) result.Status = TransactionStatus.Refund;
            result.AmountRefunded += amount;
            if (Math.Abs(result.AmountRefunded - result.AmountCaptured) < 0.0001) result.Status = TransactionStatus.Finalised;
            
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<Transaction> VoidTransaction(Guid id)
        {
            var result = await GetTransaction(id);

            if (result == null) throw new ArgumentException($"Cannot find transaction {id}.");
            if (result.Status == TransactionStatus.Finalised)
                throw new InvalidOperationException($"Cannot void a Finalised transaction.");

            result.Status = TransactionStatus.Void;
            await _context.SaveChangesAsync();

            return result;
        }
    }
}