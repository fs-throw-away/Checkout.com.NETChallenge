using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Gateway.Services;
using Gateway.Test.utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace Gateway.Test.Services
{
    public class AcquiringBankServiceTest
    {
        private static readonly IConfiguration MockConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                {"AcquiringBankUrl", "http://localhost:7201"},
            })
            .Build();
            
        [Fact]
        public async void process_calls_the_acquiring_bank()
        {
            var mockHttp = new MockHttpMessageHandler();
            var subject = new AcquiringBankService(new Logger<AcquiringBankService>(new LoggerFactory()), mockHttp.ToHttpClient(),
                    MockConfig);
            
            var response = Factory.SuccessfulBankPaymentResponse();
            
            mockHttp.When("http://localhost:7201/bankpayment")
                .Respond(
                    HttpStatusCode.Created,
                    "application/json", 
                    JsonSerializer.Serialize(response));

            var result = await subject.Process(Factory.ValidPaymentRequest());

            result.BankPaymentId.ShouldBe(response.BankPaymentId);
            result.BankPaymentStatus.ShouldBe(response.BankPaymentStatus);
        }
        
        [Fact]
        public async void throw_exception_if_bank_returns_error_code()
        {
            var mockHttp = new MockHttpMessageHandler();
            var subject = new AcquiringBankService(new Logger<AcquiringBankService>(new LoggerFactory()), mockHttp.ToHttpClient(),
                MockConfig);
            
            mockHttp.When("http://localhost:7201/bankpayment")
                .Respond(HttpStatusCode.InternalServerError, "application/text","Cobol bug.");

            var exception = await Assert.ThrowsAsync<AcquiringBankServiceException>(async () => 
                await subject.Process(Factory.ValidPaymentRequest()));
            
            exception.Message.ShouldBe("Received response code InternalServerError from http://localhost:7201/: Cobol bug.");
        }
    }
}