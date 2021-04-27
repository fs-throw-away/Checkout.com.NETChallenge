using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gateway.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gateway.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;
        private readonly PaymentDbContext _paymentDbContext;

        public PaymentRepository(ILogger<PaymentRepository> logger, PaymentDbContext paymentDbContext)
        {
            _logger = logger;
            _paymentDbContext = paymentDbContext;
        }

        public async Task InsertPaymentRequest(Guid paymentId, PaymentRequest paymentRequest)
        {
            _logger.LogInformation($"Entering {nameof(InsertPaymentRequest)} paymentId {paymentId}");

            await _paymentDbContext.Payments.AddAsync(new PaymentDbRecord()
            {
                Id = paymentId,
                CardNumber = paymentRequest.Card.Number,
                ExpiryMonth = paymentRequest.Card.ExpiryMonth,
                ExpiryYear = paymentRequest.Card.ExpiryYear,
                Cvv = paymentRequest.Card.Cvv,
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency.ToString(),
                InsertedDateTime = DateTime.Now
            });
            
            await _paymentDbContext.SaveChangesAsync();
            
            _logger.LogInformation($"Exiting {nameof(InsertPaymentRequest)} paymentId {paymentId}");
        }

        public async Task SetBankPaymentResponse(Guid paymentId, BankPaymentResponse bankPaymentResponse)
        {
            _logger.LogInformation($"Entering {nameof(SetBankPaymentResponse)} paymentId {paymentId}");
            
            var payment = await _paymentDbContext.Payments.SingleOrDefaultAsync(p => p.Id.Equals(paymentId));
            if (payment == null)
            {
                var message = $"Cannot set payment status as {paymentId} not found.";
                _logger.LogError(message);
                throw new PaymentRepositoryException(message);
            }
            
            payment.BankPaymentId = bankPaymentResponse.BankPaymentId;
            
            payment.Status = bankPaymentResponse.BankPaymentStatus.Equals(BankPaymentStatus.Success)
                ? PaymentStatus.Success.ToString()
                : PaymentStatus.Failure.ToString();
            ;
            payment.CompletedDateTime = DateTime.Now;

            await _paymentDbContext.SaveChangesAsync();

            _logger.LogInformation($"Exiting {nameof(SetBankPaymentResponse)} paymentId {paymentId}");
        }
        
        public async Task<Payment?> GetPayment(Guid paymentId)
        {
            _logger.LogInformation($"Entering {nameof(GetPayment)} paymentId {paymentId}");

            var paymentRecord = await _paymentDbContext.Payments.SingleOrDefaultAsync(p => p.Id.Equals(paymentId));
            
            _logger.LogInformation($"Exiting {nameof(GetPayment)} paymentId {paymentId}");
            return paymentRecord == null ? null : FromPaymentDbRecord(paymentRecord);
        }

        private static Payment FromPaymentDbRecord(PaymentDbRecord paymentDbRecord)
        {
            var currencyParseSuccess = Enum.TryParse(paymentDbRecord.Currency, out Currency currency);
            if (!currencyParseSuccess)
                throw new PaymentRepositoryException($"Cannot parse {paymentDbRecord.Currency} to Currency.");

            PaymentStatus? paymentStatus = null;

            if (paymentDbRecord.Status != null) {
                var statusParseSuccess = Enum.TryParse(paymentDbRecord.Status, out BankPaymentStatus parsedBankPaymentStatus);
                if (!statusParseSuccess)
                    throw new PaymentRepositoryException($"Cannot parse {paymentDbRecord.Status} to BankPaymentStatus.");
                
                paymentStatus = parsedBankPaymentStatus.Equals(BankPaymentStatus.Success)
                    ? PaymentStatus.Success
                    : PaymentStatus.Failure;
            }
            
            return new Payment()
            {
                Id = paymentDbRecord.Id,
                Card = new Card()
                {
                    Number = paymentDbRecord.CardNumber,
                    ExpiryMonth = paymentDbRecord.ExpiryMonth,
                    ExpiryYear = paymentDbRecord.ExpiryYear,
                    Cvv = paymentDbRecord.Cvv
                },
                Amount = paymentDbRecord.Amount,
                Currency = currency,
                BankPaymentId = paymentDbRecord.BankPaymentId,
                Status = paymentStatus
            };
        }
    }
}