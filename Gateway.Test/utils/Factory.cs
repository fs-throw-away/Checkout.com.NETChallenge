using System;
using Gateway.Domain;
using Gateway.DTOs;

namespace Gateway.Test.utils
{
    public static class Factory
    {
        public static Card ValidCard()
        {
            return new Card()
            {
                Number = 4035501000000008,
                ExpiryMonth = 11,
                ExpiryYear = 2023,
                Cvv = 433
            };
        }

        public static PaymentRequestDtoV1 ValidPaymentRequestDto()
        {
            return new PaymentRequestDtoV1()
            {
                Card = ValidCard(),
                Amount = (decimal) 34.11,
                Currency = Currency.USD
            };
        }

        public static PaymentRequest ValidPaymentRequest()
        {
            return new PaymentRequest()
            {
                Card = ValidCard(),
                Amount = (decimal) 34.11,
                Currency = Currency.USD
            };
        }
        
        public static Payment ValidPayment(string paymentId = "33E35EBC-47FB-4B76-A803-D46594E3D467")
        {
            return new Payment()
            {
                Id = Guid.Parse(paymentId),
                Card = ValidCard(),
                Amount = (decimal) 34.11,
                Currency = Currency.USD,
                BankPaymentId = Guid.NewGuid(),
                Status = PaymentStatus.Success
            };
        }

        public static BankPaymentResponse SuccessfulBankPaymentResponse()
        {
            return new BankPaymentResponse()
            {
                BankPaymentId = Guid.Parse("FF66BA38-C7A2-4FED-9766-A9D5EF6D286A"),
                BankPaymentStatus = BankPaymentStatus.Success
            };
        }
    }
}