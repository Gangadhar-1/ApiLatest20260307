using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using OtpAuthServices.AzureService;
using OtpAuthServices.Model;
using OtpAuthServices.Models;
using OtpAuthServices.Services;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.Rest.Accounts.V1.Credential;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly BlobService _blobService;
        private readonly ICosmosDbService<Customer> _cosmosDbService;
        private readonly DataGeneratorService _dataGeneratorService;

        // Modify the constructor to accept the interface ICosmosDbService<Customer>
        public CustomerController(DataGeneratorService dataGeneratorService, BlobService blobService, ICosmosDbService<Customer> cosmosDbService)
        {
            _blobService = blobService;
            _cosmosDbService = cosmosDbService;
            _dataGeneratorService = dataGeneratorService;
        }

        [HttpPost]
        //[Route("CustomerUpload")]
        //public async Task<IActionResult> UploadUserData([FromForm] Customer customer)
        //{
        //    customer.CustomerId = Guid.NewGuid();
        //    customer.CustomerPhotoId = "download (1).jpg";
        //    string jsonString = JsonSerializer.Serialize(customer);
        //    string jsonFileName = $"{customer.UserId.ToString().Replace(" ", "_").Replace("'","")}.json";

        //    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
        //    {
        //        await _blobService.UploadBlobAsync(jsonFileName, ms, "customer");
        //    }

        //    return Ok(new { Message = "Customer data uploaded successfully", JsonFile = jsonFileName });
        //}

        [Route("CustomerUpload")]
        public async Task<IActionResult> UploadUserData([FromForm] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Product cannot be null.");
            }

            // Ensure the Id is set
            customer.id = Guid.NewGuid().ToString(); // Assign a new GUID for Id

            customer.CustomerId = Guid.NewGuid().ToString();
            customer.CustomerPhotoId = "download (1).jpg";
            customer.Status = "Pending";
            await _cosmosDbService.AddItemAsync(customer);
            return Ok(new { Message = "Customer data Inserted successfully", JsonFile = customer.id });
        }


        //[HttpPost]
        //[Route("BulkUploadUsersAndCustomers")]
        //public async Task<IActionResult> BulkUploadUsersAndCustomers(int userCount = 50000, int customerCount = 50000)
        //{
        //    try
        //    {
        //        // Step 1: Generate Users
        //        var users = _dataGeneratorService.GenerateUsers(userCount);
        //        var userIds = new List<Guid>();

        //        // Insert Users into the Cosmos DB
        //        foreach (var user in users)
        //        {
        //            //await _cosmosDbService.AddItemAsync(user);
        //            userIds.Add(user.UserId); // Store the UserId to associate with customers
        //        }

        //        // Step 2: Generate Customers and associate them with Users
        //        var customers = _dataGeneratorService.GenerateCustomers(customerCount, userIds);

        //        // Insert Customers into the Cosmos DB
        //        foreach (var customer in customers)
        //        {
        //            await _cosmosDbService.AddItemAsync(customer);
        //        }

        //        return Ok(new { Message = "Bulk users and customers data inserted successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred while processing the bulk upload.", Error = ex.Message });
        //    }
        //}

        [HttpGet("getcustomerdata")]
        public async Task<IActionResult> Getcustomerdata()
        {
            var customer = await _cosmosDbService.GetItemsAsync();
            if (customer == null)
            {
                return BadRequest($"does not find ");
            }
            return Ok(customer);
        }

        [HttpGet("GetAllCustomersDetails")]

        public async Task<IActionResult> GetAllCustomersDetails()
        {

            try
            {

                var data = await _cosmosDbService.GetAllCustomersDetails();

                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        [HttpGet("customerProfileData")]
        public async Task<IActionResult> customerProfileData(string profileType, string UserId)
        {
            try
            {

                ProfileData profileData = new ProfileData();
                string sanitizedProfileType = profileType.ToLower();



                if (string.IsNullOrEmpty(UserId))
                {
                    return BadRequest("User Id cannot be emply");
                }


                var user = await _cosmosDbService.GetUserProflie(UserId, profileType);

                if (user != null)
                {




                    // Extract FirstName and LastName safely
                    profileData.FullName = user.FirstName + " " + user.LastName;
                    profileData.MobileNumber = user.MobileNumber;


                    // Extract Email and MobileNumber safely
                    profileData.Email = user.EmailAddress;

                    // string capitalizedProfileType = char.ToUpper(profileType[0]) + profileType.Substring(1);




                    profileData.PhotoAttachmentId = user.CustomerPhotoId;





                    profileData.Address = user.Address;
                    profileData.UserId = UserId;
                    profileData.UserProfileType = profileType;
                    profileData.Status = user.Status;







                    return Ok(profileData);
                }
                else
                {
                    return NotFound(new { message = "Customer not found" });
                }
            }

            catch (Exception ex)
            {
                // Handle exceptions (for example, log the error and return an internal server error)
                return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
            }
        }
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateCustomerDetails(string id, [FromBody] Customer customer)
            {
                if (customer == null || customer.id != id)
                {
                    return BadRequest("Customer information is incorrect.");
                }

                var existingaddress = await _cosmosDbService.GetItemAsync(id);
                if (existingaddress == null)
                {
                    return NotFound();
                }

                await _cosmosDbService.UpdateItemAsync(customer);
                return Ok("Customer data Updated successfully.");
            }



            [HttpGet("GetCustomerDirectory")]
        public async Task<IActionResult> GetCustomerDirectory(
     string searchQuery = null,
   string State = null,
     string District = null,
   string Zipcode=null
   )
        {
            try
            {
                var customers = await _cosmosDbService.GetCustomerDirectoryDetails(searchQuery,State, District,Zipcode);

                if (customers == null || !customers.Any())
                {
                    return NotFound("No customers found with the given criteria.");
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving customer details.");
            }
        }


        [HttpGet("{customerId:guid}")]
        public async Task<IActionResult> GetCustomerId(Guid customerId)
        {
            try
            {
                var results = await _cosmosDbService.GetCustomerByIdAsync<Customer>(customerId);

                if (results == null || results.Count == 0)
                    return NotFound(new { message = "Customer not found." });

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the data.", error = ex.Message });
            }
        }


        //[HttpGet("GetCustomerDtailsByState")]
        //public async Task<IActionResult> GetCustomerDetailsByState(string state)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(state))
        //        {
        //            return BadRequest("state cannot be null or empty.");
        //        }


        //        var data = await _cosmosDbService.GetCustomersDetailsByState(state);


        //        return Ok(data);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception here
        //        return StatusCode(500, "An error occurred while processing the request.");
        //    }
        //}


        [HttpGet("GetCustomerDtailsByUserId")]
        public async Task<IActionResult> GetUsersData(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest("UserId cannot be null or empty.");
                }

                var data = await _cosmosDbService.GetCustomerDetailsByIUserId(userId);


                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
   

    //        [HttpPost]
    //        [Route("CustomerEdit")]
    //        public async Task<IActionResult> EditUserData(string UserId, string FullName = null, string PhotoDocumentId = null)
    //        {
    //            try
    //            {
    //                if (string.IsNullOrEmpty(UserId))
    //                {
    //                    return BadRequest(new { Message = "UserId is required." });
    //                }

    //                string jsonFileName = $"{UserId.ToString().Replace(" ", "_")}.json";
    //                Stream existingDataStream = await _blobService.DownloadBlobAsync(jsonFileName, "customer");

    //                if (existingDataStream == null)
    //                {
    //                    return NotFound(new { Message = $"Customer data with UserId {UserId} not found." });
    //                }

    //                string existingData;
    //                using (StreamReader reader = new StreamReader(existingDataStream))
    //                {
    //                    existingData = await reader.ReadToEndAsync();
    //                }

    //                Customer existingCustomer;
    //                try
    //                {
    //                    existingCustomer = JsonSerializer.Deserialize<Customer>(existingData);
    //                }
    //                catch (JsonException jsonEx)
    //                {
    //                    return StatusCode(500, new { Message = "Failed to parse customer data.", Error = jsonEx.Message });
    //                }


    //                if (!string.IsNullOrEmpty(FullName))
    //                {
    //                    existingCustomer.FirstName = FullName.Replace("'", "");
    //                    existingCustomer.LastName = "";
    //                }


    //                if (!string.IsNullOrEmpty(PhotoDocumentId))
    //                {
    //                    existingCustomer.CustomerPhotoId = PhotoDocumentId.Replace("'", "");
    //                }

    //                string updatedJsonString = JsonSerializer.Serialize(existingCustomer);
    //                using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(updatedJsonString)))
    //                {
    //                    await _blobService.UploadBlobAsync(jsonFileName, ms, "customer");
    //                }

    //                return Ok(new { Message = "Customer data updated successfully", JsonFile = jsonFileName });
    //       {     }
    //            catch (Exception ex)
    //            {
    //                return StatusCode(500, new { Message = "An error occurred while updating customer data.", Error = ex.Message });
    //            }
    //        }
    //    }
    //}
    //https://otpauthservices20240928024709.azurewebsites.net/api/Customer/CustomerEdit?UserId='0c7197fc-da28-4a15-8a35-56778056f9c8'&FullName='statyanarayana'&PhotoDocumentId='download (1).jpg'
    //https://localhost:7091/api/Customer/CustomerEdit?UserId=0c7197fc-da28-4a15-8a35-56778056f9c8&FullName=statyanarayana&PhotoDocumentId=download%20%281%29.jpg
    [HttpPost]
        [Route("CustomerEdit")]
        public async Task<IActionResult> EditUserData(string UserId, string FullName = null, string PhotoDocumentId = null)
        {
            // Validate incoming data
            if (UserId == null)
            {
                return BadRequest($"Customer information is incorrect or {UserId} mismatch.");
            }

            // Fetch existing customer data from Cosmos DB
            Customer existingCustomer = null;
            try
            {
                // Assuming you are using the Cosmos DB container to fetch the existing customer by id
                existingCustomer = await _cosmosDbService.GetUserProflie(UserId, "Customer");  // Assuming this is a method to get the customer by id

                if (existingCustomer == null)
                {
                    return NotFound($"Customer with ID {UserId} not found.");
                }
            }
            catch (CosmosException ex)
            {
                // Log CosmosException (error accessing Cosmos DB)
                return StatusCode(500, $"Error retrieving data from Cosmos DB: {ex.Message}");
            }

            // Now that we have the existing customer, we can update the necessary fields
            existingCustomer.FirstName = FullName;
            existingCustomer.LastName = "";
            // Example field update
            //  existingCustomer.LastName =LastName ?? existingCustomer.LastName;
            //existingCustomer.EmailAddress = customer.EmailAddress ?? existingCustomer.EmailAddress;
            existingCustomer.CustomerPhotoId = PhotoDocumentId;
            // Update the existing customer in Cosmos DB
            try
            {
                // Use UpsertItemAsync to update or insert the customer data
                await _cosmosDbService.UpdateItemAsync(existingCustomer);

                return Ok(existingCustomer);  // Return the updated customer data
            }
            catch (Exception ex)
            {
                // Log general exception (other errors)
                return StatusCode(500, "An error occurred while updating customer data.");
            }
        }




        //[HttpPut("Status/{CustomerId}")]
        //public async Task<IActionResult> UpdateDealerStatus(Guid CustomerId, [FromQuery] string Status)
        //{
        //    // Fetch the dealer details by ID from the database
        //    var customerList = await _cosmosDbService.GetCustomerByIdAsync<Customer>(CustomerId);

        //    if (customerList == null || customerList.Count == 0)
        //    {
        //        return NotFound(new { message = "Dealer not found." });
        //    }

        //    // Assuming only one dealer should be returned, take the first one
        //    var customer = customerList.FirstOrDefault();

        //    if (customer == null)
        //    {
        //        return NotFound(new { message = "customer not found." });
        //    }

        //    // Exit early if the status is already the same
        //    if ((string.Equals(Status, "Approved", StringComparison.OrdinalIgnoreCase) && customer.Status == "Approved") ||
        //        (string.Equals(Status, "Rejected", StringComparison.OrdinalIgnoreCase) && customer.Status == "Rejected") ||
        //        (string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase) && customer.Status == "Pending"))
        //    {
        //        return Ok(); // Exit without updating
        //    }

        //    // Update the dealer's status based on the Status parameter
        //    if (string.Equals(Status, "Approved", StringComparison.OrdinalIgnoreCase))
        //    {
        //        customer.IsApproved = true;
        //        customer.IsRejected = false;
        //        customer.IsPending = false;
        //        customer.Status = "Approved"; // Update the Status field
        //    }
        //    else if (string.Equals(Status, "Rejected", StringComparison.OrdinalIgnoreCase))
        //    {
        //        customer.IsApproved = false;
        //        customer.IsRejected = true;
        //        customer.IsPending = false;
        //        customer.Status = "Rejected"; // Update the Status field
        //    }
        //    else if (string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase))
        //    {
        //        customer.IsPending = true;
        //        customer.IsRejected = false;
        //        customer.IsApproved = false;
        //        customer.Status = "Pending"; // Update the Status field
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Invalid status. Please use 'Approved', 'Rejected', or 'Pending'." });
        //    }

        //    // Save the updated dealer back to the Cosmos DB
        //    await _cosmosDbService.UpdateCustomerAsync(customer);

        //    return Ok(new { message = $"Customer status updated to {customer.Status}." });
        //}





    }
}



//        [HttpPost]
//        [Route("CustomerEdit")]
//        public async Task<IActionResult> EditUserData(string UserId, string FullName, string PhotoDocumentId)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(UserId))
//                {
//                    return BadRequest(new { Message = "UserId is required." });
//                }

//                string jsonFileName = $"{UserId.ToString().Replace(" ", "_")}.json";
//                Stream existingDataStream = await _blobService.DownloadBlobAsync(jsonFileName, "customer");

//                if (existingDataStream == null)
//                {
//                    return NotFound(new { Message = $"Customer data with UserId {UserId} not found." });
//                }

//                string existingData;
//                using (StreamReader reader = new StreamReader(existingDataStream))
//                {
//                    existingData = await reader.ReadToEndAsync();
//                }

//                Customer existingCustomer;
//                try
//                {
//                    existingCustomer = JsonSerializer.Deserialize<Customer>(existingData);
//                }
//                catch (JsonException jsonEx)
//                {
//                    return StatusCode(500, new { Message = "Failed to parse customer data.", Error = jsonEx.Message });
//                }

//                existingCustomer.FirstName = FullName.ToString() ?? existingCustomer.FirstName;
//                existingCustomer.LastName = "";
//               // existingCustomer.LastName = FullName.Split("$")[1].ToString() ?? existingCustomer.LastName;
//               if (existingCustomer.CustomerPhotoId!=null || existingCustomer.CustomerPhotoId !="")
//                existingCustomer.CustomerPhotoId = PhotoDocumentId;

//                string updatedJsonString = JsonSerializer.Serialize(existingCustomer);
//                using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(updatedJsonString)))
//                {
//                    await _blobService.UploadBlobAsync(jsonFileName, ms, "customer");
//                }

//                return Ok(new { Message = "Customer data updated successfully", JsonFile = jsonFileName });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { Message = "An error occurred while updating customer data.", Error = ex.Message });
//            }
//        }
//    }
//}






//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using OtpAuthServices.Model;
//using System.Data;
//using System.Threading.Tasks;

//namespace OtpAuthServices.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CustomerController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        private readonly string _connectionString;

//        public CustomerController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            _connectionString = _configuration.GetConnectionString("DefaultConnection");
//        }

//        [HttpPost("InsertOrUpdateCustomer")]
//        public async Task<IActionResult> InsertOrUpdateCustomer([FromBody] Customer customer)
//        {
//            if (customer == null)
//            {
//                return BadRequest("Customer data is null");
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(_connectionString))
//                {
//                    await conn.OpenAsync();

//                    using (SqlCommand cmd = new SqlCommand("Usp_InsertOrUpdateCustomer", conn))
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure;

//                        // Input parameters
//                        cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
//                        cmd.Parameters.AddWithValue("@LastName", customer.LastName);
//                        cmd.Parameters.AddWithValue("@MobileNumber", customer.MobileNumber);
//                        cmd.Parameters.AddWithValue("@MobileVerificationCode", (object)customer.MobileVerificationCode ?? DBNull.Value);
//                        cmd.Parameters.AddWithValue("@EmailAddress", customer.EmailAddress);
//                        cmd.Parameters.AddWithValue("@EmailVerificationCode", (object)customer.EmailVerificationCode ?? DBNull.Value);
//                        cmd.Parameters.AddWithValue("@AlternativeMobileNumber", (object)customer.AlternativeMobileNumber ?? DBNull.Value);
//                        cmd.Parameters.AddWithValue("@GSTNumber", (object)customer.GSTNumber ?? DBNull.Value);
//                        cmd.Parameters.AddWithValue("@Address", customer.Address);
//                        cmd.Parameters.AddWithValue("@Landmark", (object)customer.Landmark ?? DBNull.Value);
//                        cmd.Parameters.AddWithValue("@State", customer.State);
//                        cmd.Parameters.AddWithValue("@District", customer.District);
//                        cmd.Parameters.AddWithValue("@ZipCode", customer.ZipCode);
//                        cmd.Parameters.AddWithValue("@UserId", customer.UserId);
//                        // Output parameter for the message
//                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.VarChar, 600)
//                        {
//                            Direction = ParameterDirection.Output
//                        };
//                        cmd.Parameters.Add(outputMessage);

//                        // Execute the stored procedure
//                        await cmd.ExecuteNonQueryAsync();

//                        // Get the output message
//                        string message = outputMessage.Value.ToString();

//                        // Return the message as the response
//                        return Ok(new { Message = message });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//    }
//}




