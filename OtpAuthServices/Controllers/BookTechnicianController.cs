using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using OtpAuthServices.Models;
using OtpAuthServices.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookTechnicianController : ControllerBase
    {
        private readonly ICosmosDbService<BookTechnician> _cosmosDbService;

        // Constructor to initialize dependencies
        public BookTechnicianController(ICosmosDbService<BookTechnician> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;

        }




        [HttpPost("CreateBookTechnician")]
        public async Task<IActionResult> CreateTicket([FromBody] BookTechnician BookTechnician)
        {
            if (BookTechnician == null)
            {
                return BadRequest("Ticket data cannot be null.");
            }

            string ticketId = GenerateRaiseTicketId();


            BookTechnician.id = Guid.NewGuid().ToString();
            BookTechnician.status = "Open";
            BookTechnician.Date = DateTime.UtcNow;
            BookTechnician.BookTechnicianId = ticketId;

            // Insert the support ticket into Cosmos DB
            await _cosmosDbService.AddItemAsync(BookTechnician);
            return Ok(new { Message = "Raise ticket created successfully", RaiseTicketId = BookTechnician.id, TicketId = BookTechnician.BookTechnicianId });
        }


        private string GenerateRaiseTicketId()
        {
            Random random = new Random();
            string prefix = "VSKPAKP"; // Fixed prefix
            string numbers = random.Next(1000, 9999).ToString(); // Random 4-digit number
            char letter = (char)random.Next('A', 'Z' + 1); // Random uppercase letter

            return $"{prefix}{numbers}{letter}";
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookTechnician(string id)
        {
            var existingaddress = await _cosmosDbService.GetItemAsync(id);
            if (existingaddress == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync(id);
            return Ok("Successfully  deleted BookTechnician   Item. ");
        }

        // GET: api/RaiseTicket/{ticketId}
        [HttpGet("GetBookTechnician/{id}")]
        public async Task<IActionResult> GetBookTechnician(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Ticket ID cannot be null or empty.");
            }

            var bookTechnician = await _cosmosDbService.GetItemAsync(id);
            if (bookTechnician == null)
            {
                return NotFound($"BookTechnician with ID {id} not found.");
            }

            return Ok(bookTechnician);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookTechnician(string id, [FromBody] BookTechnician BookTechnician)
        {
            if (BookTechnician == null || BookTechnician.id != id)
            {
                return BadRequest("Product information is incorrect.");
            }

            var existingProduct = await _cosmosDbService.GetItemAsync(id);
            if (existingProduct == null)
            {
                existingProduct.BookTechnicianId = BookTechnician.BookTechnicianId;
                


            }
            //existingProduct.BookTechnicianId.Replace("/", string.Empty);
            await _cosmosDbService.UpdateItemAsync(BookTechnician);
            return Ok($"BookTechnician Data Updated Successfully. At with respectiveId {id}.");




        }


    }
}

