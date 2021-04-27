using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Gateway.DTOs;

namespace Gateway.Domain
{
    public class PaymentRequest
    {
        public Card Card { get; set; }
        
        public decimal Amount { get; set; }
        
        public Currency Currency { get; set; }

        public static PaymentRequest FromDto(PaymentRequestDtoV1 paymentRequestDtoV1)
        {
            return new PaymentRequest()
            {
                Card = paymentRequestDtoV1.Card,
                Amount = paymentRequestDtoV1.Amount,
                Currency = paymentRequestDtoV1.Currency
            };
        }
    }
}