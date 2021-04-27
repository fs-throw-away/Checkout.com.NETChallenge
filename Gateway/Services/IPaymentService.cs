using System;
using System.Threading.Tasks;
using Gateway.Domain;

namespace Gateway.Services
{
    public interface IPaymentService
    {
        Task<Payment> Process(PaymentRequest paymentRequest);
        
        Task<Payment?> Retrieve(Guid paymentId);
    }
}