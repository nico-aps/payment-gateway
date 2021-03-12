using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Model;
using PaymentGateway.Model.Adapters;
using PaymentGateway.Model.Enum;
using Xunit;

namespace PaymentGatewayTest.Model
{
    public class AdapterTests
    {
        private AuthRequestAdapter _adapter;

        public AdapterTests()
        {
            var context = GetNewInMemoryDb();
            _adapter = new AuthRequestAdapter(context);
        }

        private static PaymentGatewayContext GetNewInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<PaymentGatewayContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PaymentGatewayContext(options);
        }


        [Theory, AutoData]
        public async void TransactionCreation(Transaction request)
        {
            await _adapter.AddTransaction(request);

            var result = await _adapter.GetTransaction(request.Id);
            result.Should().Be(request);
        }

        [Fact, AutoData]
        public async Task WrongId()
        {
            await _adapter.Invoking(f => f.AddCapture(Guid.Empty, 5)).Should().ThrowAsync<ArgumentException>();
            await _adapter.Invoking(f => f.AddRefund(Guid.Empty, 5)).Should().ThrowAsync<ArgumentException>();
        }

        #region Capture
        
        [MemberData(nameof(CaptureData))]
        [Theory]
        public async void SuccessfulCapture(Transaction request, int amountToCapture)
        {
            var originalAmountCaptured = request.AmountCaptured;
            await _adapter.AddTransaction(request);

            var result = await _adapter.AddCapture(request.Id, amountToCapture);
            result.AmountCaptured.Should().Be(originalAmountCaptured + amountToCapture);
        }

        [Theory, AutoData]
        public async Task CaptureAfterRefund(Transaction request)
        {
            request.Status = TransactionStatus.Refund;

            await _adapter.AddTransaction(request);

            await _adapter.Invoking(f => f.AddCapture(request.Id, 0)).Should().ThrowAsync<InvalidOperationException>();
        }

        [MemberData(nameof(CaptureData))]
        [Theory]
        public async Task CaptureTooMuch(Transaction request, int amountToCapture)
        {
            await _adapter.AddTransaction(request);

            await _adapter.Invoking(f => f.AddCapture(request.Id, request.Amount + amountToCapture ))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
        #endregion
        
        #region Refund

        [MemberData(nameof(RefundData))]
        [Theory]
        public async void SuccessfulRefund(Transaction request, int amountToCapture, int amountToRefund)
        {
            var originalAmountRefunded = request.AmountRefunded;
            await _adapter.AddTransaction(request);
            await _adapter.AddCapture(request.Id, amountToCapture);
            
            var result = await _adapter.AddRefund(request.Id, amountToRefund);
            
            result.AmountCaptured.Should().Be(originalAmountRefunded + amountToCapture);
        }

        [MemberData(nameof(RefundData))]
        [Theory]
        public async Task RefundTooMuch(Transaction request, int amountToCapture, int amountToRefund)
        {
            await _adapter.AddTransaction(request);
            await _adapter.AddCapture(request.Id, amountToCapture);

            await _adapter.Invoking(f => f.AddCapture(request.Id, request.Amount + amountToRefund ))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
        
        #endregion

        #region Void

        [Theory, AutoData]
        public async void SuccessfulVoid(Transaction request)
        {
            await _adapter.AddTransaction(request);

            var result = await _adapter.VoidTransaction(request.Id);
            result.Status.Should().Be(TransactionStatus.Void);
        }

        #endregion


        public static IEnumerable<object[]> CaptureData =>
            new List<object[]>
            {
                new object[]
                    {GetTransaction(), 5},
                new object[]
                    {GetTransaction(), 10}
            };
        public static IEnumerable<object[]> RefundData =>
            new List<object[]>
            {
                new object[]
                    {GetTransaction(), 5, 3},
                new object[]
                    {GetTransaction(), 10, 10}
            };

        private static Transaction GetTransaction() => new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = 10,
            AmountCaptured = 0,
            AmountRefunded = 0,
            CardNumber = "4111111111111111", // random valid card number
            Status = TransactionStatus.Active,
            Currency = Currency.EUR,
            CVV = 999,
            ExpMonth = 08,
            ExpYear = 22
        };
    }
}