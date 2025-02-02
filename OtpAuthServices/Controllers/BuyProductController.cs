using Microsoft.AspNetCore.Mvc;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using System;
using System.Collections.Generic;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuyProductController : Controller
    {
        private readonly BlobService _blobService;
        private readonly ICosmosDbService<BuyProduct> _cosmosDbService;

        public BuyProductController(BlobService blobService, ICosmosDbService<BuyProduct> cosmosDbService)
        {
            _blobService = blobService;
            _cosmosDbService = cosmosDbService;
        }

        [HttpPost("BuyProductUpload")]
        public async Task<IActionResult> UploadAddress([FromBody] BuyProduct buyProduct)
        {



            if (buyProduct == null)
            {

                return BadRequest("BuyProduct data cannot be null.");
            }

            try
            {
                buyProduct.BuyProductId = Guid.NewGuid().ToString();
                buyProduct.id = Guid.NewGuid().ToString();
                buyProduct.status = "Open";
                await _cosmosDbService.AddItemAsync(buyProduct);  // Add item to Cosmos DB



                return Ok(new
                {
                    Message = "BuyProduct data uploaded successfully",
                    BuyProductId = buyProduct.BuyProductId.ToString()  // Return as string in the response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading address.");
                return StatusCode(500, "An error occurred while uploading the address. Please try again.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuyProduct(string id, [FromBody] BuyProduct buyProduct)
        {
            if (buyProduct == null || buyProduct.id != id)
            {
                return BadRequest("BuyProduct information is incorrect.");
            }

            var existingaddress = await _cosmosDbService.GetItemAsync(id);
            if (existingaddress == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(buyProduct);
            return Ok("BuyProduct data Updated successfully.");
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
            return   Ok("Successfully  deleted  BuyProduct  Item. ");
        }

        [HttpGet("GetBuyProductDetailsByBuyProductId/{BuyProductId}")]
        public async Task<ActionResult<List<AddressModel>>> GetBuyProductDetails(string BuyProductId)
        {
            if (string.IsNullOrEmpty(BuyProductId))
            {
                return BadRequest("BuyProductId Is required.");
            }

            try
            {
                // Fetch the address by ProfileType and UserId from Cosmos DB
                var buyProduct = await _cosmosDbService.GetBuyProductdetails(BuyProductId);

                if (buyProduct == null || buyProduct.Count == 0)
                {
                    return NotFound($"No buyproducts   BuyProductId '{BuyProductId}'.");
                }

                return Ok(buyProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching addresses by ProfileType and UserId.");
                return StatusCode(500, "An error occurred while retrieving the addresses. Please try again.");
            }
        }


        
   


        [HttpGet("GetTotalCountOfBuyProducts")]
        public async Task<IActionResult> GetTotalCountOfBuyProducts()
        {
            try
            {
                // Call the service method to get ticket counts by status
                var ticketCounts = await _cosmosDbService.GetTotalCountOfBuyProducts();

                if (ticketCounts == null)
                {
                    return StatusCode(500, "Error fetching ticket counts.");
                }

                return Ok(ticketCounts); // Return the dictionary as a JSON response
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error response
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }


        [HttpGet("GetTotalCountOfBuyProductsBystateWise")]
        public async Task<IActionResult> GetTotalCountOfBuyProductsByStateWise(string state)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCounts = await _cosmosDbService.GetTotalCountOfBuyProductsByStateWise(normalisedstate);
                if(totalCounts == null)
                {
                    return StatusCode(500, "Error fetching total counts.");
                }

                return Ok(totalCounts);
                
            }
            catch  (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }



        [HttpGet("GetTotalCountOfBuyProductsBystateWiseAndDistrictWise")]
        public async Task<IActionResult> GetTotalCountOfBuyProductsByStateWiseAndDistrictWise(string state,string  district)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCount = await _cosmosDbService.GetTotalCountOfBuyProductsByStateWiseAndDistrictWise(normalisedstate,district);

                if (totalCount == null)
                {
                    return StatusCode(500, "Error fetching total counts.");
                }

                return Ok(totalCount);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }



        [HttpGet("GetTotalCountOfBuyProductsBystateWiseAndDistrictWiseAndZipCodeWise")]
        public async Task<ActionResult> GetTotalCountOfBuyProductsByStateWiseAndDistrictWiseAndZipcodeWise(string state, string district, string  zipCode)
        {
            try
            {

                string normalisedstate = state.ToUpper();
                // Fetch the total count from the service
                var totalCount = await _cosmosDbService.GetTotalCountOfBuyProductsByStateWiseAndDistrictWiseAndZipcodeWise(normalisedstate, district,zipCode);

                if (totalCount == null)
                {
                    return StatusCode(500, "Error fetching total counts.");
                }

                return Ok(totalCount);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred.");
            }
        }



    }
}