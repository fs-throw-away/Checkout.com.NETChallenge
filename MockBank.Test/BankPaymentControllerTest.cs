using System;
using System.Threading.Tasks;
using Gateway.Domain;
using Gateway.DTOs;
using Gateway.Test.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockBank.Controllers;
using Moq;
using Shouldly;
using Xunit;

namespace MockBank.Test
{
    public class BankPaymentControllerTest
    {
        private readonly BankPaymentController _subject =
            new BankPaymentController(new Logger<BankPaymentController>(new LoggerFactory()));
        
        [Fact]
        public void payment_request_returns_201()
        {
            var paymentRequest = Factory.ValidPaymentRequest();
            
            var result = _subject.Post(paymentRequest);

            var response = result.ShouldBeAssignableTo<CreatedResult>();
            var responseBody = response?.Value.ShouldBeAssignableTo<BankPaymentResponse>();
            
            responseBody?.BankPaymentId.ShouldBeOfType<Guid>();
            responseBody?.BankPaymentStatus.ShouldBe(BankPaymentStatus.Success);
        }
        
        [Fact]
        public void payment_request_for_certain_card_number_returns_faire_status()
        {
            var paymentRequest = Factory.ValidPaymentRequest();
            paymentRequest.Card.Number = 670300000009999;
            
            var result = _subject.Post(paymentRequest);

            var response = result.ShouldBeAssignableTo<CreatedResult>();
            var responseBody = response?.Value.ShouldBeAssignableTo<BankPaymentResponse>();
            
            responseBody?.BankPaymentId.ShouldBe(Guid.Parse("44F500A6-000D-4E26-B32F-9ADB0E80DAFC"));
            responseBody?.BankPaymentStatus.ShouldBe(BankPaymentStatus.Failure);
        }
    }
}