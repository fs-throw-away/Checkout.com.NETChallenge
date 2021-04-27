using System;
using Gateway.DTOs;

namespace Gateway.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }
        
        public Card Card { get; set; }
        
        public decimal Amount { get; set; }
        
        public Currency Currency { get; set; }
        
        public Guid? BankPaymentId { get; set; }
        
        public PaymentStatus? Status { get; set; }
    }
}