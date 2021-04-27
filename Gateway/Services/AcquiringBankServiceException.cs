using System;

namespace Gateway.Services
{
    public class AcquiringBankServiceException : Exception
    {
        public AcquiringBankServiceException(string message) : base(message)
        {
        }
    }
}