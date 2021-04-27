using System;
using System.Threading.Tasks;
using Gateway.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MockBank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankPaymentController: ControllerBase
    {
        private readonly ILogger<BankPaymentController> _logger;

        public BankPaymentController(ILogger<BankPaymentController> logger) 
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BankPaymentResponse), StatusCodes.Status201Created)]
        public IActionResult Post(PaymentRequest paymentRequestDto)
        {
            _logger.LogInformation("Received payment request.");
            
            if (paymentRequestDto.Card.Number.Equals(670300000009999))
            {
                return new CreatedResult("bankpayment", new BankPaymentResponse()
                {
                    BankPaymentId = Guid.Parse("44F500A6-000D-4E26-B32F-9ADB0E80DAFC"),
                    BankPaymentStatus = BankPaymentStatus.Failure
                });
            }

            return new CreatedResult("bankpayment", new BankPaymentResponse()
            {
                BankPaymentId = Guid.NewGuid(),
                BankPaymentStatus = BankPaymentStatus.Success
            });
        }
    }
}