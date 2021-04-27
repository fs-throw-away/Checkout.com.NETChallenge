using System;
using Gateway.Domain;
using Gateway.Repository;
using Gateway.Test.utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Gateway.Test.Repository
{
    public class PaymentRepositoryTest : IDisposable
    {
        private readonly SqliteConnection _sqliteConnection;
        private readonly PaymentRepository _paymentRepository;
        
        public PaymentRepositoryTest()
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();

            var paymentDbContext = new PaymentDbContext(new DbContextOptionsBuilder<PaymentDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options);
            
            paymentDbContext.Database.EnsureCreated();

            _paymentRepository =
                new PaymentRepository(new Logger<PaymentRepository>(new LoggerFactory()), paymentDbContext);
        }

        [Fact]
        public async void can_persist_and_retrieve_payment_request()
        {
            var paymentId = Guid.NewGuid();
            var paymentRequest = Factory.ValidPaymentRequest();

            await _paymentRepository.InsertPaymentRequest(paymentId, paymentRequest);

            var payment = await _paymentRepository.GetPayment(paymentId);

            payment?.Id.ShouldBe(paymentId);
            payment?.Card.Number.ShouldBe(paymentRequest.Card.Number);
            payment?.Card.ExpiryYear.ShouldBe(paymentRequest.Card.ExpiryYear);
            payment?.Card.ExpiryMonth.ShouldBe(paymentRequest.Card.ExpiryMonth);
            payment?.Card.Cvv.ShouldBe(paymentRequest.Card.Cvv);
            payment?.Amount.ShouldBe(paymentRequest.Amount);
            payment?.Currency.ShouldBe(paymentRequest.Currency);
            payment?.BankPaymentId.ShouldBe(null);
            payment?.Status.ShouldBe(null);
        }
        
        [Fact]
        public async void return_null_for_retrieve_unexisting_payment()
        {
            var paymentId = Guid.NewGuid();
            var payment = await _paymentRepository.GetPayment(paymentId);

            payment.ShouldBeNull();
        }
        
        [Fact]
        public async void can_insert_and_retrieve_bank_responsne()
        {
            var paymentId = Guid.NewGuid();
            var paymentRequest = Factory.ValidPaymentRequest();
            var bankPaymentResponse = Factory.SuccessfulBankPaymentResponse();
            
            await _paymentRepository.InsertPaymentRequest(paymentId, paymentRequest);
            await _paymentRepository.SetBankPaymentResponse(paymentId, bankPaymentResponse);

            var payment = await _paymentRepository.GetPayment(paymentId);

            payment?.Id.ShouldBe(paymentId);
            payment?.Card.Number.ShouldBe(paymentRequest.Card.Number);
            payment?.Card.ExpiryYear.ShouldBe(paymentRequest.Card.ExpiryYear);
            payment?.Card.ExpiryMonth.ShouldBe(paymentRequest.Card.ExpiryMonth);
            payment?.Card.Cvv.ShouldBe(paymentRequest.Card.Cvv);
            payment?.Amount.ShouldBe(paymentRequest.Amount);
            payment?.Currency.ShouldBe(paymentRequest.Currency);
            payment?.BankPaymentId.ShouldBe(bankPaymentResponse.BankPaymentId);
            payment?.Status.ShouldBe(PaymentStatus.Success);
        }
        
        public void Dispose()
        {
            _sqliteConnection.Close();
        }
    }
}