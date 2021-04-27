using Gateway.Domain;

namespace Gateway.DTOs
{
    public class PaymentRetrievalResponseDto
    {
        public MaskedCardDto MaskedCardDto { get; set; }
        
        public PaymentStatus PaymentStatus { get; set; }
    }
}