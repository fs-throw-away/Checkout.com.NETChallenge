using System;

namespace Gateway.Repository
{
    public class PaymentRepositoryException: Exception
    {
        public PaymentRepositoryException(string message) : base(message)
        {
        }
    }
}