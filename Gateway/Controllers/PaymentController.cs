using System;
using System.Threading.Tasks;
using Gateway.Domain;
using Gateway.DTOs;
using Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        
        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(PaymentRequestDtoV1 paymentRequestDtoV1)
        {
            _logger.LogInformation($"Received {nameof(paymentRequestDtoV1)}.");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ValidationState);

            var payment = await _paymentService.Process(PaymentRequest.FromDto(paymentRequestDtoV1));

            if (payment.Status == null)
                return StatusCode(500, "Unexpected payment status field value.");
                    
            _logger.LogInformation($"Returning PaymentId {payment.Id}, PaymentStatus {payment.Status}.");

            return new CreatedResult("payment", new PaymentResponseDto()
            {
                PaymentId = payment.Id,
                PaymentStatus = (PaymentStatus) payment.Status
            });
        }

        [HttpGet("/payment/{paymentId:guid}")]
        [ProducesResponseType(typeof(PaymentRetrievalResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid paymentId)
        {
            _logger.LogInformation($"Received {nameof(paymentId)} paymentId.");

            var payment = await _paymentService.Retrieve(paymentId);

            if (payment?.Status == null)
            {
                _logger.LogInformation($"Returning 404 Not Found.");
                return NotFound();
            }

            _logger.LogInformation($"Returning payment status {payment.Status}.");

            return new OkObjectResult(new PaymentRetrievalResponseDto()
            {
                MaskedCardDto = MaskedCardDto.fromCard(payment.Card),
                PaymentStatus = (PaymentStatus) payment.Status
            });
        }
    }
}