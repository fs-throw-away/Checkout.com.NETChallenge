using System;
using System.Threading.Tasks;
using Gateway.Domain;
using Gateway.Repository;
using Microsoft.Extensions.Logging;

namespace Gateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IAcquiringBankService _acquiringBankService;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(ILogger<PaymentService> logger, IAcquiringBankService acquiringBankService, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _acquiringBankService = acquiringBankService;
            _paymentRepository = paymentRepository;
        }
        
        public async Task<Payment> Process(PaymentRequest paymentRequest)
        {
            _logger.LogInformation($"Entering {nameof(Process)}");
            
            var paymentId = Guid.NewGuid();
            _logger.LogInformation($"Assigned {paymentId}");
            
            await _paymentRepository.InsertPaymentRequest(paymentId, paymentRequest);
            
            var bankPaymentResponse = await _acquiringBankService.Process(paymentRequest);

            await _paymentRepository.SetBankPaymentResponse(paymentId, bankPaymentResponse);

            _logger.LogInformation($"Exiting {nameof(Process)}");

            return new Payment()
            {
                Id = paymentId,
                Card = paymentRequest.Card,
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
                BankPaymentId = bankPaymentResponse.BankPaymentId,
                Status = bankPaymentResponse.BankPaymentStatus.Equals(BankPaymentStatus.Success)
                    ? PaymentStatus.Success
                    : PaymentStatus.Failure
            };
        }

        public async Task<Payment?> Retrieve(Guid paymentId)
        {
            _logger.LogInformation($"Entering {nameof(Retrieve)} paymentId {paymentId}");
            
            var payment = await _paymentRepository.GetPayment(paymentId);

            if (payment?.Status == null)
                return null;

            _logger.LogInformation($"Exiting {nameof(Retrieve)} paymentId {paymentId}");
            return payment;
        }
    }
}