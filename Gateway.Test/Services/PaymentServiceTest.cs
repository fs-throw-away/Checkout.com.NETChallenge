using System;
using System.Threading.Tasks;
using Gateway.Domain;
using Gateway.Repository;
using Gateway.Services;
using Gateway.Test.utils;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Gateway.Test.Services
{
    public class PaymentServiceTest
    {
        private static readonly Mock<IAcquiringBankService> MockAcquiringBankService = new();
        private static readonly Mock<IPaymentRepository> MockPaymentRepository = new();

        private readonly PaymentService _subject = new PaymentService(new Logger<PaymentService>(new LoggerFactory()),
            MockAcquiringBankService.Object, MockPaymentRepository.Object);

        [Fact]
        public async Task process_payment_calls_the_acquiring_bank_service_and_repository()
        {
            var paymentRequest = Factory.ValidPaymentRequest();

            var bankPaymentResponse = Factory.SuccessfulBankPaymentResponse();
            
            MockAcquiringBankService
                .Setup(service => service.Process(paymentRequest))
                .ReturnsAsync(bankPaymentResponse);
            
            var result = await _subject.Process(paymentRequest);

            var paymentId = result.Id;
            
            MockPaymentRepository.Verify(repo => repo.InsertPaymentRequest(paymentId, paymentRequest), Times.Once);
            
            MockAcquiringBankService.Verify(repo => repo.Process(paymentRequest), Times.Once);

            MockPaymentRepository.Verify(repo => repo.SetBankPaymentResponse(paymentId, bankPaymentResponse), Times.Once);

            result.Card.Number.ShouldBe(paymentRequest.Card.Number);
            result.Card.ExpiryYear.ShouldBe(paymentRequest.Card.ExpiryYear);
            result.Card.ExpiryMonth.ShouldBe(paymentRequest.Card.ExpiryMonth);
            result.Card.Cvv.ShouldBe(paymentRequest.Card.Cvv);
            
            result.Amount.ShouldBe(paymentRequest.Amount);
            result.Currency.ShouldBe(paymentRequest.Currency);

            result.BankPaymentId.ShouldBe(bankPaymentResponse.BankPaymentId);
            result.Status.ShouldBe(PaymentStatus.Success);
        }

        [Fact]
        public async Task return_failure_status_if_acquiring_bank_fails()
        {
            var paymentRequest = Factory.ValidPaymentRequest();

            var bankPaymentResponse = Factory.SuccessfulBankPaymentResponse();
            bankPaymentResponse.BankPaymentStatus = BankPaymentStatus.Failure;
            
            MockAcquiringBankService
                .Setup(service => service.Process(paymentRequest))
                .ReturnsAsync(bankPaymentResponse);
            
            var result = await _subject.Process(paymentRequest);

            result.Status.ShouldBe(PaymentStatus.Failure);
        }

        [Fact]
        public async Task retrieve_calls_payment_repository()
        {
            var payment = Factory.ValidPayment();

            MockPaymentRepository
                .Setup(repo => repo.GetPayment(payment.Id))
                .ReturnsAsync(payment);

            var result = await _subject.Retrieve(payment.Id);
            
            MockPaymentRepository
                .Verify(repo => repo.GetPayment(payment.Id), Times.Once);

            result.ShouldBe(payment);
        }
        
        [Fact]
        public async Task retrieve_returns_null_when_repository_returns_null()
        {
            var paymentId = Guid.NewGuid();

            MockPaymentRepository
                .Setup(repo => repo.GetPayment(paymentId))
                .ReturnsAsync((Payment?) null);

            var result = await _subject.Retrieve(paymentId);
            
            MockPaymentRepository
                .Verify(repo => repo.GetPayment(paymentId), Times.Once);

            result.ShouldBe(null);
        }
    }
}