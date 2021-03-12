using System;
using System.Threading.Tasks;

namespace PaymentGateway.Model.Adapters
{
    public interface IAuthRequestAdapter
    {
        public Task AddTransaction(Transaction request);
        public Task<Transaction> GetTransaction(Guid id);
        public Task<Transaction> AddCapture(Guid id, double amount);
        public Task<Transaction> AddRefund(Guid id, double amount);
        public Task<Transaction> VoidTransaction(Guid id);


    }
}