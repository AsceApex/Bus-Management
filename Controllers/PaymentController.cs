// Controllers/PaymentController.cs
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using BusReservation.API.Dtos;

namespace BusReservation.API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    public class PaymentController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
        }

        // POST: api/Payment/create-order
        // Controllers/PaymentController.cs (only the action body changed)
        [Microsoft.AspNetCore.Mvc.HttpPost("create-order")]
        public Microsoft.AspNetCore.Mvc.IActionResult CreateOrder(
            [Microsoft.AspNetCore.Mvc.FromBody] PaymentRequestDto dto)
        {
            if (dto is null || dto.Amount <= 0)
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult("Amount must be > 0 paise.");

            var key = _config["Razorpay:Key"];
            var secret = _config["Razorpay:Secret"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult("Razorpay keys are not configured.");

            try
            {
                var client = new RazorpayClient(key, secret);
                var options = new Dictionary<string, object>
    {
        { "amount", dto.Amount },  // in paise
        { "currency", dto.Currency ?? "INR" },
        { "receipt", dto.ReceiptId ?? string.Empty },
        { "payment_capture", 1 }
    };

                Order order = client.Order.Create(options);

                return new Microsoft.AspNetCore.Mvc.OkObjectResult(new
                {
                    orderId = order["id"]?.ToString(),
                    amount = order["amount"],
                    currency = order["currency"]
                });
            }
            catch (System.Exception ex)
            {
                return new Microsoft.AspNetCore.Mvc.ObjectResult(new { error = ex.Message })
                {
                    StatusCode = 500
                };
            }

        }
    }
}