
using Gateway.Domain;

namespace Gateway.DTOs
{
    public class PaymentRequestDtoV1
    {
        public Card Card { get; set; }
                
        public decimal Amount { get; set; }
        
        public Currency Currency { get; set; }
    }
}