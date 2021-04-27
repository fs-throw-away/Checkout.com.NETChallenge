using System;
using Gateway.Domain;

namespace Gateway.DTOs
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        
        public PaymentStatus PaymentStatus { get; set; }
    }
}