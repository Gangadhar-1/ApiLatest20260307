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
    public class RaiseTicketController : ControllerBase
    {
        private readonly ICosmosDbService<RaiseTicket> _cosmosDbService;

        // Constructor to initialize dependencies
        public RaiseTicketController(ICosmosDbService<RaiseTicket> cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;

        }




        [HttpPost("CreateRaiseTicket")]
        public async Task<IActionResult> CreateTicket([FromBody] RaiseTicket RaiseTicket)
        {
            if (RaiseTicket == null)
            {
                return BadRequest("Ticket data cannot be null.");
            }

            string ticketId = GenerateRaiseTicketId();


            RaiseTicket.id = Guid.NewGuid().ToString();
            RaiseTicket.status = "Open";
            RaiseTicket.Date = DateTime.UtcNow;
            RaiseTicket.RaiseTicketId = ticketId;

            // Insert the support ticket into Cosmos DB
            await _cosmosDbService.AddItemAsync(RaiseTicket);
            return Ok(new { Message = "Raise ticket created successfully", RaiseTicketId = RaiseTicket.id, TicketId = RaiseTicket.RaiseTicketId });
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
        public async Task<IActionResult> DeleteBuyProduct(string id)
        {
            var existingaddress = await _cosmosDbService.GetItemAsync(id);
            if (existingaddress == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync(id);
            return Ok("Successfully  deleted  RaiseTickets  Item. ");
        }

        // GET: api/RaiseTicket/{ticketId}
        [HttpGet("GetTicket/{ticketId}")]
        public async Task<IActionResult> GetTicket(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                return BadRequest("Ticket ID cannot be null or empty.");
            }

            var ticket = await _cosmosDbService.GetItemAsync(ticketId);
            if (ticket == null)
            {
                return NotFound($"Ticket with ID {ticketId} not found.");
            }

            return Ok(ticket);
        }
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<TrackTickets>> GetTicketByCustomerId(string customerId)
        {
            try
            {
                // Call the Cosmos DB service to get a ticket
                var ticket = await _cosmosDbService.GetRaiseTicketsAsync(customerId);

                // Check if a ticket was found
                if (ticket == null)
                {
                    return NotFound($"No tickets found for CustomerId: {customerId}.");
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                // Return internal server error if an exception occurs
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // POST: api/RaiseTicket/EditTicket
        [HttpPost("EditTicket")]
        public async Task<IActionResult> EditTicket([FromBody] RaiseTicket updatedTicket)
        {
            if (updatedTicket == null || string.IsNullOrEmpty(updatedTicket.RaiseTicketId))
            {
                return BadRequest("Invalid ticket data.");
            }

            // Retrieve the existing ticket
            var existingTicket = await _cosmosDbService.GetItemAsync(updatedTicket.RaiseTicketId);
            if (existingTicket == null)
            {
                return NotFound($"Ticket with ID {updatedTicket.RaiseTicketId} not found.");
            }

            // Update the ticket fields
            existingTicket.Subject = updatedTicket.Subject ?? existingTicket.Subject;
            existingTicket.Category = updatedTicket.Category ?? existingTicket.Category;
            existingTicket.Address = updatedTicket.Address ?? existingTicket.Address;
            existingTicket.AssignedTo = updatedTicket.AssignedTo ?? existingTicket.AssignedTo;
            existingTicket.Attachments = updatedTicket.Attachments ?? existingTicket.Attachments; // Set attachments list

            // Save the updated ticket
            await _cosmosDbService.UpdateItemAsync(existingTicket);

            return Ok(new { Message = "Ticket updated successfully", Ticket = existingTicket });
        }


        [HttpGet("GetTicketsNotifications")]
        public async Task<IActionResult> GetTrackTicketDetails()
        { 
            try
            {
                var RaiseTicket = await _cosmosDbService.GetTrackTicketDetailsAsync();

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }


        [HttpGet("GetTicketsNotificationsForTechnician")]
        public async Task<IActionResult> GetTicketsNotificationsForTechnician()
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketsNotificationsForTechnician();

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }


        [HttpGet("GetTicketsNotificationsForTechnicianForSMS")]
        public async Task<IActionResult> GetTicketsNotificationsForTechnicianForSMS()
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketsNotificationsForTechnicianForSMS();

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }


        [HttpGet("GetTicketsNotificationsForLowestTechnicianSMS")]
        public async Task<IActionResult> GetTicketsNotificationsLowestForTechnicianSMS()
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketsNotificationsForLowestTechnicianForSMS();

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }



        [HttpGet("GetRaiseTicketsForDealers")]
        public async Task<IActionResult> GetRaiseTicketsForDealers()
        {
            try
            {
                var RaiseRaiseTickets = await _cosmosDbService.GetRaiseTicketsForDealer();

                // Return 200 OK with tickets
                return Ok(RaiseRaiseTickets);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }



        [HttpGet("GetRaiseTicketsForDealersForSMS")]
        public async Task<IActionResult> GetRaiseTicketsForDealersForSMS()
        {
            try
            {
                var RaiseRaiseTickets = await _cosmosDbService.GetRaiseTicketsForDealerForSMS();

                // Return 200 OK with tickets
                return Ok(RaiseRaiseTickets);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }







        [HttpGet("GetRaiseTicketsOfLowestDealersForSMS")]
        public async Task<IActionResult> GetRaiseTicketsForLowestDealersForSMS()
        {
            try
            {
                var RaiseRaiseTickets = await _cosmosDbService.GetRaiseTicketsForDealerForSMS();

                // Return 200 OK with tickets
                return Ok(RaiseRaiseTickets);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }
        [HttpGet("GetRaiseTicketsForCustomer")]
        public async Task<IActionResult> GetRaiseTicketsForCustomer()
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketsForCustomer();

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }

        [HttpGet("GetNotificationsByDistrict")]

        public async Task<IActionResult> GetNotificationsByDistrict(string district, string category)
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketNotificationsByDistrict(district, category);

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }

        [HttpGet("GetRaiseTicketNotificationsByStateAndDistrictForDealer")]

        public async Task<IActionResult> GetRaiseTicketNotificationsByStateAndDistrict(string district, string category)
        {
            try
            {
                var raiseAQuotebyDealer = await _cosmosDbService.GetRaiseTicketNotificationsByStateAndDistrict(district, category);

                // Return 200 OK with tickets
                return Ok(raiseAQuotebyDealer);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }


        }





        [HttpGet("GetTicketIdAndSubjectByStateAndDistrict")]
        public async Task<IActionResult> getTicketIdAndSubjectByStateAndDistrict(string state, string district)
        {
            try
            {
                var raisetickets = await _cosmosDbService.GetRaiseTicketForTechnicians(state, district);

                var response = raisetickets.Select(n => new
                {
                    n.RaiseTicketId,
                    n.Date,
                    n.Subject
                }).ToList();

                return Ok(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error  fetching raisetickets: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching raisetickets.");
            }
        }


        [HttpGet("GetRecentNotifications")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<object>))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecentNotifications()
        {
            try
            {
                var notifications = await _cosmosDbService.GetRecentNotifications();

                var response = notifications.Select(n => new
                {
                    n.RaiseTicketId,
                    n.Date,
                    n.Subject
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }

        //[HttpGet("GetTotalCountOfRaiseTicket")]
        //public async Task<IActionResult> GetTotalCountOfRaiseTicket()
        //{
        //    try
        //    {
        //        // Call the service method to get ticket counts by status
        //        var ticketCounts = await _cosmosDbService.GetTotalCountsOfRaiseTicket();

        //        if (ticketCounts == null)
        //        {
        //            return StatusCode(500, "Error fetching ticket counts.");
        //        }

        //        return Ok(ticketCounts); // Return the dictionary as a JSON response
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and return a generic error response
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return StatusCode(500, "Unexpected error occurred.");
        //    }
        //}

        [HttpGet("GetRaiseTicketsBystateAndDistrict")]
        public async Task<ActionResult> GetRaiseTicketBystateAndDistrict(string state, string district)
        {
            try
            {
                string normalisedstate = state.ToUpper();
                string normaliseddistrict = district.ToUpper();
                var raisetickets = await _cosmosDbService.GetRaiseTicketForTechnician(state, district);


                if (raisetickets == null)
                {
                    return StatusCode(500, "Error fetching raisetickets.");
                }
                return Ok(raisetickets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error Occurred.");
            }

        }

        [HttpGet("GetTotalCountOfRaiseTicketBystateWise")]
        public async Task<ActionResult> GetTotalCountOfRaiseTicketByStateWise(string state)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCount = await _cosmosDbService.GetTotalCountOfRaiseTicketsByStateWise(normalisedstate);

                if (totalCount == null)
                {
                    return StatusCode(500, "Error fetching total counts .");
                }
                return Ok(totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected  error occurred.");
            }
        }




        [HttpGet("GetTotalCountOfRaiseTicketBystateWiseAndDistrictWise")]
        public async Task<IActionResult> GetTotalCountOfSupportTickeByStateWiseAndDistrictWise(string state, string district)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCount = await _cosmosDbService.GetTotalCountOfRaiseTicketStateWiseAndDistrictWise(normalisedstate, district);

                // Return the total count in the desired format
                if (totalCount == null)
                {
                    return StatusCode(500, "Error fetching  total counts.");
                }

                return Ok(totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }

        [HttpGet("GetTotalCountOfSupportTickeBystateWiseAndDistrictWiseAndZipCodeWise")]
        public async Task<IActionResult> GetTotalCountOfSupportTickeByStateWiseAndDistrictWiseAndZipcodeWise(string state, string district, string zipCode)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCount = await _cosmosDbService.GetTotalCountOfRaiseTicketByStateWiseAndDistrictWiseAndZipcodeWise(normalisedstate, district, zipCode);

                if (totalCount == null)
                {
                    return StatusCode(500, "Error fetching total counts");
                }
                return Ok(totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected  error occurred.");
            }
        }



        //Updating  RaiseTicket
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRaiseTicket(string id, [FromBody] RaiseTicket raiseTicket)
        {
            if (raiseTicket == null || raiseTicket.id != id)
            {
                return BadRequest("Product information is incorrect.");
            }

            var existingProduct = await _cosmosDbService.GetItemAsync(id);
            if (existingProduct == null )
            {
                existingProduct.RaiseTicketId = raiseTicket.RaiseTicketId;
                existingProduct.Attachments = raiseTicket.Attachments;


            }
            existingProduct.LowestBidderTechnicainId.Replace("/",string.Empty); 
            await _cosmosDbService.UpdateItemAsync(raiseTicket);
            return Ok($"RaiseTicket Data Updated Successfully. At with respectiveId {id}.");




        }




        [HttpGet("GetTotalCountOfRaiseTicket")]
        public async Task<IActionResult> GetTotalCountOfRaiseTicket()
        {
            try
            {
                var countsFromUser = await _cosmosDbService.GetAllRaiseTicketsCounts();

                if (countsFromUser == null)
                {
                    return StatusCode(500, "Error retrieving counts from Cosmos DB.");
                }

                var totalCount = countsFromUser.Values.Sum();

                return Ok(new
                {
                    Open = countsFromUser.GetValueOrDefault("Open", 0),
                    Pending = countsFromUser.GetValueOrDefault("Pending", 0),
                    Assigned = countsFromUser.GetValueOrDefault("Assigned", 0),
                    Closed = countsFromUser.GetValueOrDefault("Closed", 0),

                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }


        [HttpGet("GetRaiseTicketNotificationsByCustomerId")]

        public async Task<IActionResult> GetRaiseTicketNotificationsByCustomerId(string customerId)
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketNotificationsByCustomerId(customerId);

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }






        [HttpGet("GetRaiseTicketsByTechnicalAgency")]
        public async Task<IActionResult> GetRaiseTicketsByTechnicalAgency()
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetRaiseTicketsByTechnicalAgency();


                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }





        [HttpGet("GetPendingActions")]
        public async Task<IActionResult> GetPendingActions(string state = null, string district = null, string ZipCode = null)
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetPendingActionsAsync(state, district, ZipCode);

                // Return 200 OK with ticketss
                return Ok(RaiseTicket);
            }
            catch (Exception ex)
            {
                // Log and return 500 Internal Server Error
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }



        [HttpGet("GetNotificationsByExistingTechnicianId")]
        public async Task<IActionResult> GetNotificationsByExistingTechnicianId(string district, string category, string technicianId)
        {
            try
            {
                var raiseTickets = await _cosmosDbService.GetNotificationsByExistingTechnicianId(district, category, technicianId);

                Console.WriteLine($"Technician ID: {technicianId}");
                Console.WriteLine($"Retrieved Tickets Count: {raiseTickets.Count}");

                if (raiseTickets.Count == 0)
                {
                    return NotFound(new { message = "No RaiseTickets found matching the criteria.", technicianId, district, category });
                }

                return Ok(new { tickets = raiseTickets });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }


        [HttpGet("GetNotificationsByNotExistTechnicianId")]
        public async Task<IActionResult> GetNotificationsByNotExistTechnicianId(string district, string category, string technicianId)
        {
            try
            {
                Console.WriteLine($"[INFO] Fetching RaiseTickets for Technician ID: {technicianId}, District: {district}, Category: {category}");

                var raiseTickets = await _cosmosDbService.GetRaiseTicketNotificationsByNotExistTechnicianId(district, category, technicianId);

                Console.WriteLine($"[INFO] Retrieved Tickets Count: {raiseTickets.Count}");

                if (raiseTickets == null || raiseTickets.Count == 0)
                {
                    return NotFound(new
                    {
                        message = "No RaiseTickets found matching the criteria.",
                        technicianId,
                        district,
                        category
                    });
                }

                return Ok(new
                {
                    message = "RaiseTickets retrieved successfully.",
                    technicianId,
                    district,
                    category,
                    totalTickets = raiseTickets.Count,
                    tickets = raiseTickets
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error retrieving tickets: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An internal server error occurred while retrieving tickets.",
                    error = ex.Message
                });
            }
        }


        [HttpGet("GetNotificationsByExistingDealerId")]
        public async Task<IActionResult> GetNotificationsByExistingDealerId(string category, string district, string dealerId)
        {
            try
            {
                var raiseTickets = await _cosmosDbService.GetNotificationsByExistingDealerId(category, district, dealerId);

                Console.WriteLine($"Technician ID: {dealerId}");
                Console.WriteLine($"Retrieved Tickets Count: {raiseTickets.Count}");

                if (raiseTickets.Count == 0)
                {
                    return NotFound(new { message = "No RaiseTickets found matching the criteria.", dealerId, district, category });
                }

                return Ok(new { tickets = raiseTickets });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving tickets.");
            }
        }


        [HttpGet("GetNotificationsByNotExistDealerId")]
        public async Task<IActionResult> GetNotificationsByNotExistDeaerId(string category, string district, string dealerId)
        {
            try
            {
                Console.WriteLine($"[INFO] Fetching RaiseTickets for Technician ID: {dealerId}, District: {district}, Category: {category}");

                var raiseTickets = await _cosmosDbService.GetRaiseTicketNotificationsByNotExistDealerId(category, district, dealerId);

                Console.WriteLine($"[INFO] Retrieved Tickets Count: {raiseTickets.Count}");

                if (raiseTickets == null || raiseTickets.Count == 0)
                {
                    return NotFound(new
                    {
                        message = "No RaiseTickets found matching the criteria.",
                        dealerId,
                        district,
                        category
                    });
                }

                return Ok(new
                {
                    message = "RaiseTickets retrieved successfully.",
                    dealerId,
                    district,
                    category,
                    totalTickets = raiseTickets.Count,
                    tickets = raiseTickets
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error retrieving tickets: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An internal server error occurred while retrieving tickets.",
                    error = ex.Message
                });
            }

        }
            [HttpGet("GetTechnicianOrders")]
            public async Task<IActionResult> GetNotificationsByExistingDealerId( string District)
            {
                try
                {
                    var raiseTickets = await _cosmosDbService.GetTechnicianOrders( District);

                    
                    Console.WriteLine($"Retrieved Tickets Count: {raiseTickets.Count}");

                    if (raiseTickets.Count == 0)
                    {
                        return NotFound(new { message = "No RaiseTickets found matching the criteria.",  District });
                    }

                    return Ok(new { tickets = raiseTickets });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving tickets: {ex.Message}");
                    return StatusCode(500, "An error occurred while retrieving tickets.");
                }
            }



        [HttpGet("GetTrackTicketsByCustomerId")]

        public async Task<IActionResult> GetTrackTicketsByCustomerId(string customerId)
        {
            try
            {
                var RaiseTicket = await _cosmosDbService.GetTrackTicketsByCustomerId(customerId);

                // Return 200 OK with tickets
                return Ok(RaiseTicket);
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





 