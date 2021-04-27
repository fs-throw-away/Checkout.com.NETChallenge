using System.Threading.Tasks;
using Gateway.Domain;
using Gateway.DTOs;

namespace Gateway.Services
{
    public interface IAcquiringBankService
    {
        Task<BankPaymentResponse> Process(PaymentRequest paymentRequest);
    }
}