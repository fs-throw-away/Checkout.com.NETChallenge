using System;
using System.Text.Json.Serialization;

namespace Gateway.Domain
{
    public class BankPaymentResponse
    {
        public Guid BankPaymentId { get; set; }
        
        public BankPaymentStatus BankPaymentStatus { get; set; }
    }
}