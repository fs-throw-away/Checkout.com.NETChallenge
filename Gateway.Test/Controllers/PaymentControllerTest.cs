using System;
using System.Threading.Tasks;
using Gateway.Controllers;
using Gateway.Domain;
using Gateway.DTOs;
using Gateway.Services;
using Gateway.Test.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace Gateway.Test.Controllers
{
    public class PaymentControllerTest
    {
        private static readonly Mock<IPaymentService> MockPaymentService = new();

        private readonly PaymentController _subject =
            new PaymentController(new Logger<PaymentController>(new LoggerFactory()), MockPaymentService.Object);

        [Fact]
        public async Task process_payment_request_and_returns_201()
        {
            var paymentId = Guid.NewGuid();

            var paymentRequestDto = Factory.ValidPaymentRequestDto();
            
            MockPaymentService
                .Setup(paymentService => paymentService.Process(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(Factory.ValidPayment(paymentId.ToString()));

            var result = await _subject.Post(paymentRequestDto);

            var response = result.ShouldBeAssignableTo<CreatedResult>();
            var responseBody = response?.Value.ShouldBeAssignableTo<PaymentResponseDto>();
            
            MockPaymentService.Verify(paymentService => paymentService.Process(It.IsAny<PaymentRequest>()), Times.Once);
            
            responseBody?.PaymentId.ShouldBe(paymentId);
            responseBody?.PaymentStatus.ShouldBe(PaymentStatus.Success);
        }
        
        [Fact]
        public async Task returns_500_when_payment_service_returns_null_value()
        {
            var paymentRequestDto = Factory.ValidPaymentRequestDto();

            var payment = Factory.ValidPayment();
            payment.Status = null;
            
            MockPaymentService
                .Setup(paymentService => paymentService.Process(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(payment);            
            
            var result = await _subject.Post(paymentRequestDto);

            var objectResult = result.ShouldBeAssignableTo<ObjectResult>();
            objectResult?.StatusCode.ShouldBe(500);
            objectResult?.Value.ShouldBe("Unexpected payment status field value.");
        }
        
        [Fact]
        public async Task process_invalid_payment_request_returns_400()
        {
            var paymentRequestDto = Factory.ValidPaymentRequestDto();
            
            _subject.ModelState.AddModelError("Card Number", "Incorrect number of digits");
            
            var result = await _subject.Post(paymentRequestDto);
            
            result.ShouldBeAssignableTo<BadRequestObjectResult>();
        }
        
        [Fact]
        public async Task get_existing_payment_request_returns_200()
        {
            var paymentId = Guid.NewGuid();
            var payment = Factory.ValidPayment(paymentId.ToString());

            MockPaymentService
                .Setup(paymentService => paymentService.Retrieve(It.IsAny<Guid>()))
                .ReturnsAsync(payment);

            var result = await _subject.Get(paymentId);

            var response = result.ShouldBeAssignableTo<OkObjectResult>();
            MockPaymentService.Verify(paymentService => paymentService.Retrieve(paymentId), Times.Once);

            var responseBody = response?.Value.ShouldBeAssignableTo<PaymentRetrievalResponseDto>();

            responseBody?.MaskedCardDto?.Number.ShouldBe("************0008");
            responseBody?.MaskedCardDto?.ExpiryMonth.ShouldBe(payment.Card.ExpiryMonth);
            responseBody?.MaskedCardDto?.ExpiryYear.ShouldBe(payment.Card.ExpiryYear);
            responseBody?.PaymentStatus.ShouldBe((PaymentStatus) payment.Status!);            
        }

        [Fact]
        public async Task get_non_existing_payment_returns_404()
        {
            var paymentId = Guid.NewGuid();

            MockPaymentService
                .Setup(paymentService => paymentService.Retrieve(It.IsAny<Guid>()))
                .ReturnsAsync((Payment?) null);

            var result = await _subject.Get(paymentId);

            var response = result.ShouldBeAssignableTo<NotFoundResult>();
            MockPaymentService.Verify(paymentService => paymentService.Retrieve(paymentId), Times.Once);
        }
        
        [Fact]
        public async Task get_payment_with_null_status_returns_404()
        {
            var paymentId = Guid.NewGuid();
            var payment = Factory.ValidPayment();
            payment.Status = null;
            
            MockPaymentService
                .Setup(paymentService => paymentService.Retrieve(It.IsAny<Guid>()))
                .ReturnsAsync(payment);

            var result = await _subject.Get(paymentId);

            result.ShouldBeAssignableTo<NotFoundResult>();
            MockPaymentService.Verify(paymentService => paymentService.Retrieve(paymentId), Times.Once);
        }
    }
}