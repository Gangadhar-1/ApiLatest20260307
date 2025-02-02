using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;

namespace OtpAuthServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICosmosDbService<Payment> _cosmosDbService;

        public PaymentController(ICosmosDbService<Payment> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreateTicket([FromBody] Payment payment)
        {
            if (payment == null)
            {
                return BadRequest("Payment data cannot be null.");
            }

            try
            {
                string dynmaicTechrrotp = GenerateRandomOtp();
                payment.id = Guid.NewGuid().ToString();
                payment.PaymentId = Guid.NewGuid().ToString();
                //payment.PaymentDataTime = DateTime.UtcNow;
                payment.TechnicianConfirmationCode = dynmaicTechrrotp;

                await _cosmosDbService.AddItemAsync(payment);
                return Ok(new
                {
                    Message = "Payment created successfully",
                    PaymentId = payment.id,
                    TechnicianConfirmationCode = payment.TechnicianConfirmationCode
                });
            }
            catch (Exception ex)
            {
                // Log the exception here using a logging framework (e.g., Serilog, NLog, etc.)
                return StatusCode(500, new { Message = "An error occurred while creating the payment.", Details = ex.Message });
            }
        }

        private string GenerateRandomOtp()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                int randomNumber = BitConverter.ToInt32(bytes, 0);
                return (Math.Abs(randomNumber % 900000) + 100000).ToString("D6");
            }
        }



        [HttpGet("GetPaymentByPamentId")]
        public async Task<IActionResult> GetPaymentByPamentId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Payment ID cannot be null or empty.");
            }

            var ticket = await _cosmosDbService.GetItemAsync(id);
            if (ticket == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }

            return Ok(ticket);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRaiseTicket(string id, [FromBody] Payment payment)
        {
            if (payment == null || payment.id != id)
            {
                return BadRequest("Payment information is incorrect.");
            }

            var existingPayment = await _cosmosDbService.GetItemAsync(id);
            if (existingPayment == null)
            {
                existingPayment.PaymentId = payment.PaymentId;



            }

            await _cosmosDbService.UpdateItemAsync(payment);
            return Ok($"Payment Data Updated Successfully. At with respectiveId {id}.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(string id)
        {
            var existingpayment = await _cosmosDbService.GetItemAsync(id);
            if (existingpayment == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync(id);
            return Ok("Successfully  deleted  Payment  Item. ");
        }





        [HttpGet("GetPaymentDetailsByRaiseTicketId")]

        public async Task<IActionResult> GetPaymentDetailsByRaiseTicketId(string RaiseTicketId)
        {
            try
            {
                var Payment = await _cosmosDbService.GetPaymentDetailsByRaiseTicketId(RaiseTicketId)
 ;

                // Return 200 OK with tickets
                return Ok(Payment);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }

    }
}
