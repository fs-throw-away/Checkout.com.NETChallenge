using System;
using System.Threading.Tasks;
using Gateway.Domain;

namespace Gateway.Repository
{
    public interface IPaymentRepository
    {
        Task InsertPaymentRequest(Guid paymentId, PaymentRequest payment);
        
        Task SetBankPaymentResponse(Guid paymentId, BankPaymentResponse bankPaymentResponse);
        
        Task<Payment?> GetPayment(Guid paymentId);
    }
}