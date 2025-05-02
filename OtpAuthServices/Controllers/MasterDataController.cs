//using Azure.Storage.Blobs;
//using Microsoft.AspNetCore.Mvc;
//using System.IO;
//using System.Threading.Tasks;

//namespace OtpAuthServices.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class StatesController : ControllerBase
//    {
//        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=handymanfiles;AccountKey=5Rlk7migIG6xwuKgz56d7/r1mvh4sHNL50vygJzHmJOV2QukJSEl3W8etC0vI3RXEFFrSJ/X0Yqg+AStz6FBrQ==;EndpointSuffix=core.windows.net";
//        private readonly string _containerName = "states";
//        private readonly string _blobName = "states.json";

//        [HttpGet("getStates")]
//        public async Task<IActionResult> GetStates()
//        {
//            // Create a BlobServiceClient to connect to your storage account
//            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

//            // Get the container client
//            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

//            // Get the blob client for states.json
//            BlobClient blobClient = containerClient.GetBlobClient(_blobName);

//            // Download the blob's content as a stream
//            var response = await blobClient.DownloadAsync();
//            using (StreamReader reader = new StreamReader(response.Value.Content))
//            {
//                // Read JSON content from the blob
//                string jsonContent = await reader.ReadToEndAsync();

//                // Return JSON content as a response
//                return Ok(jsonContent);
//            }
//        }
//    }
//}


using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using OtpAuthServices.Model;
using Azure.Storage.Blobs.Models;
using System.Dynamic;
using System.Security.Cryptography.Xml;
using OtpAuthServices.AzureService;
using System.Net;

namespace OtpAuthServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterDataController : ControllerBase
    {
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=handymanfiles;AccountKey=5Rlk7migIG6xwuKgz56d7/r1mvh4sHNL50vygJzHmJOV2QukJSEl3W8etC0vI3RXEFFrSJ/X0Yqg+AStz6FBrQ==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "states";
        private readonly string _userscontainerName = "users";
        private readonly string _customerscontainerName = "customer";

        private readonly string _dealercontainerName = "dealer";

        private readonly string _blobName = "states.json";
        private readonly string _blobdisticts = "Disticts.json";
        private readonly string _blobcategories = "category.json";
        private readonly string _blobbuildercategorie = "BuilderCategories.json";

        private readonly ICosmosDbService<Customer> _cosmosDbService;

        // Modify the constructor to accept the interface ICosmosDbService<Customer>
        public MasterDataController(ICosmosDbService<Customer> cosmosDbService)
        {
          
            _cosmosDbService = cosmosDbService;
        }


        // Method to get the list of states
        [HttpGet("getStates")]
        public async Task<IActionResult> GetStates()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(_blobName);

            var response = await blobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(response.Value.Content))
            {
                string jsonContent = await reader.ReadToEndAsync();
                return Ok(jsonContent);
            }
        }


        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(_blobcategories);

            var response = await blobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(response.Value.Content))
            {
                string jsonContent = await reader.ReadToEndAsync();
                return Ok(jsonContent);
            }
        }

        [HttpGet("getdealerCategories")]
        public async Task<IActionResult> GetbuilderCategories()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(_blobbuildercategorie);

            var response = await blobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(response.Value.Content))
            {
                string jsonContent = await reader.ReadToEndAsync();
                return Ok(jsonContent);
            }
        }







        //public async Task<IActionResult> GetUsers()
        //{
        //    BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_userscontainerName);

        //}


        //[HttpGet("VerifyUserProfile")]
        //public async Task<IActionResult> GetUsers(string value)
        //{
        //    // Initialize BlobServiceClient and BlobContainerClient for users
        //    BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_userscontainerName);

        //    List<UserOnBoarding> users = new List<UserOnBoarding>();

        //    // Iterate over all blobs (user files) in the container
        //    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        //    {
        //        // Get the blob client for the current blob item
        //        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

        //        // Download the content of the blob
        //        var blobDownloadInfo = await blobClient.DownloadAsync();
        //        using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
        //        {
        //            string content = await streamReader.ReadToEndAsync();

        //            // Deserialize the content into a UserOnBoarding object
        //            UserOnBoarding onboarduser = Newtonsoft.Json.JsonConvert.DeserializeObject<UserOnBoarding>(content);
        //            users.Add(onboarduser);
        //        }
        //    }

        //    // Find the user by MobileNo, EmailId, or UserId
        //    var user = users.FirstOrDefault(x =>
        //        x.MobileNo == value ||
        //        x.EmailId == value ||
        //        (x.GetType().GetProperty("UserId") != null && x.GetType().GetProperty("UserId").GetValue(x).ToString() == value)
        //    );

        //    // If user is not found, return 404 with a message
        //    if (user == null)
        //    {
        //        return NotFound(new { message = "User not found" });
        //    }


        //    // Return the customer object if found, otherwise return 404
        //    if (user != null)
        //    {

        //        return Ok(user);
        //    }
        //    else
        //    {
        //        return NotFound(new { message = "user not found" });
        //    }
        //}

        private static Dictionary<string, UserOnBoarding> _userCache = new Dictionary<string, UserOnBoarding>();
        private static DateTime _lastCacheUpdateTime = DateTime.MinValue;

        // Method to refresh user cache
        private async Task RefreshUserCacheAsync()
        {
            // Only refresh the cache if it's been more than 15 minutes since the last update
            if ((DateTime.Now - _lastCacheUpdateTime).TotalMinutes < 15) return;

            // Clear the existing cache
            _userCache.Clear();

            // Initialize BlobServiceClient and BlobContainerClient for users
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_userscontainerName);

            // Load all users into the cache
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                var blobDownloadInfo = await blobClient.DownloadAsync();

                using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
                {
                    string content = await streamReader.ReadToEndAsync();
                    UserOnBoarding user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserOnBoarding>(content);

                    // Cache user by MobileNo, EmailId, and UserId
                    _userCache[user.MobileNo] = user;
                    _userCache[user.EmailId] = user;
                    _userCache[user.UserId.ToString()] = user; // Assuming UserId is of type Guid
                }
            }

            // Update the last cache update time
            _lastCacheUpdateTime = DateTime.Now;
        }

        //[HttpGet("VerifyUserProfile")]
        //public async Task<IActionResult> GetUsers(string value)
        //{
        //    // Refresh the user cache before checking
        //    await RefreshUserCacheAsync();

        //    // Check if the user is in the cache
        //    if (_userCache.TryGetValue(value, out UserOnBoarding user))
        //    {
        //        return Ok(user);
        //    }

        //    // If no user is found, return 404 with a message
        //    return NotFound(new { message = "User not found" });
        //}

        [HttpGet]
        [Route("VerifyUserProfile")]
        public async Task<IActionResult> GetUser(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("Either mobile number or email address must be provided.");
            }

            // Use the GetUserByEmailOrMobileAsync method to fetch the user
            var user = await _cosmosDbService.GetUserByEmailOrMobileAsync(value);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("User not found.");
        }


        [HttpGet("VerifyUserLogin")]
        public async Task<IActionResult> VerifyUserLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return BadRequest("Either mobile number or email address must be provided.");
            }

            // Use the GetUserByEmailOrMobileAsync method to fetch the user
            var user = await _cosmosDbService.GetUserByLogin(username,password);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("User not found.");
        }







        //[HttpGet("VerifyUserLogin")]
        //public async Task<IActionResult> VerifyUserLogin(string username,string password)
        //{
        //    // Initialize BlobServiceClient and BlobContainerClient for users
        //    BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_userscontainerName);

        //    List<UserOnBoarding> users = new List<UserOnBoarding>();

        //    // Iterate over all blobs (user files) in the container
        //    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        //    {
        //        // Get the blob client for the current blob item
        //        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

        //        // Download the content of the blob
        //        var blobDownloadInfo = await blobClient.DownloadAsync();
        //        using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
        //        {
        //            string content = await streamReader.ReadToEndAsync();

        //            // Deserialize the content into a UserOnBoarding object
        //            UserOnBoarding onboarduser = Newtonsoft.Json.JsonConvert.DeserializeObject<UserOnBoarding>(content);
        //            users.Add(onboarduser);
        //        }
        //    }

        //    // Find the user by MobileNo, EmailId, or UserId
        //    var user = users.FirstOrDefault(x =>
        //        x.UserName == username &&
        //        x.UserPassword == password);


        //    // If user is not found, return 404 with a message
        //    if (user == null)
        //    {
        //        return NotFound(new { message = "User not found" });
        //    }


        //    // Return the customer object if found, otherwise return 404
        //    if (user != null)
        //    {

        //        return Ok(user);
        //    }
        //    else
        //    {
        //        return NotFound(new { message = "user not found" });
        //    }
        //}

        //[HttpGet("GetProfileData")]
        //public async Task<IActionResult> GetProfileData(string profileType, string UserId)
        //{
        //    try
        //    {

        //        ProfileData profileData = new ProfileData();
        //            string sanitizedProfileType = profileType.ToLower();            



        //        if (string.IsNullOrEmpty(UserId))
        //        {
        //            return BadRequest("User Id cannot be emply");
        //        }

               
        //        var user = await _cosmosDbService.GetUserProflie(UserId,profileType);

        //                       if (user != null)
        //        {


                    
        //                if (profileType == "customer")
        //                {
        //                // Extract FirstName and LastName safely
        //                profileData.FullName = user.FirstName + " " + user.LastName;
        //                profileData.MobileNumber = user.MobileNumber;

        //                }
        //                else if (profileType == "dealer")
        //                {


        //                profileData.FullName = user.OwnershipName;

        //                }
        //                else if (profileType == "technician")
        //                {
        //                profileData.FullName = user.TechnicianFullName;

        //                }

        //                else if (profileType == "builder")

        //                {
        //                profileData.FullName = user.BuilderName;

        //                }
        //                else if (profileType == "estimator")
        //                {
        //                profileData.FullName = user.EstimatorName;


        //                }
        //            // Extract Email and MobileNumber safely
        //            profileData.Email = user.EmailAddress;
        //            if (profileType != "customer")
        //                profileData.MobileNumber = user.PhoneNumber;
        //                string capitalizedProfileType = char.ToUpper(profileType[0]) + profileType.Substring(1);

                    


        //            profileData.PhotoAttachmentId = user.capitalizedProfileType + "PhotoId";
                   




        //            profileData.Address = user.Address;
        //                profileData.UserId = UserId;
        //                profileData.UserProfileType = profileType;
                        


                   




        //            return Ok(profileData);
        //        }
        //        else
        //        {
        //            return NotFound(new { message = "Customer not found" });
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (for example, log the error and return an internal server error)
        //        return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
        //    }
        //}





        //[HttpGet("GetProfileData")]
        //public async Task<IActionResult> GetProfileData(string profileType, string UserId)
        //{
        //    try
        //    {

        //        ProfileData profileData = new ProfileData();
        //        // Initialize the BlobServiceClient
        //        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

        //        // Sanitize the ProfileType to adhere to Azure Blob Storage naming conventions
        //        string sanitizedProfileType = profileType.ToLower(); // Azure container names must be lowercase

        //        // Initialize BlobContainerClient for the user's profile content based on the sanitized ProfileType
        //        BlobContainerClient profileContentBlob = blobServiceClient.GetBlobContainerClient(sanitizedProfileType);
        //        List<dynamic> profiles = new List<dynamic>();

        //        // Iterate over all blobs (profile files) in the profile container
        //        await foreach (BlobItem blobItem in profileContentBlob.GetBlobsAsync())
        //        {
        //            // Get the blob client for the current blob item
        //            BlobClient blobClient = profileContentBlob.GetBlobClient(Uri.EscapeDataString(blobItem.Name));

        //            // Download the blob content
        //            var blobDownloadInfo = await blobClient.DownloadAsync();
        //            using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
        //            {
        //                string content = await streamReader.ReadToEndAsync();

        //                // Deserialize the content into a dynamic object
        //                dynamic profile = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(content);
        //                profiles.Add(profile);
        //            }
        //        }

        //        dynamic profileObj = null;

        //        // Find the specific customer profile based on UserId
        //        foreach (var userObj in profiles)
        //        {
        //            // Initialize expandoDict
        //            IDictionary<string, object>? expandoDict = userObj as IDictionary<string, object>;

        //            // Check if expandoDict is not null and contains the "UserId" property
        //            if (expandoDict != null && expandoDict.ContainsKey("UserId"))
        //            {
        //                // Compare the value of UserId to the target user's UserId
        //                if (expandoDict["UserId"].ToString() == UserId)
        //                {
        //                    profileObj = userObj;
        //                    break; // Exit the loop once found
        //                }
        //            }
        //        }

        //        // Return the customer object if found, otherwise return 404
        //        if (profileObj != null)
        //        {


        //                // Cast customerObj as an IDictionary to access its properties
        //                var customerDict = profileObj as IDictionary<string, object>;

        //                if (customerDict != null)
        //                {
        //                    //// Log all the keys in customerDict to verify available fields
        //                    //Console.WriteLine("Keys in customerDict:");
        //                    //foreach (var key in customerDict.Keys)
        //                    //{
        //                    //    Console.WriteLine($"{key}: {customerDict[key]}"); // Log each key-value pair
        //                    //}
        //                if (profileType == "customer")
        //                {
        //                    // Extract FirstName and LastName safely
        //                    profileData.FullName = (customerDict.ContainsKey("FirstName") ? customerDict["FirstName"].ToString() : "") + (customerDict.ContainsKey("LastName") ? customerDict["LastName"].ToString() : "");
        //                    profileData.MobileNumber = customerDict.ContainsKey("MobileNumber") ? customerDict["MobileNumber"].ToString() : "";

        //                }
        //                else if (profileType == "dealer")
        //                {


        //                    profileData.FullName = (customerDict.ContainsKey("OwnershipName") ? customerDict["OwnershipName"].ToString() : "");

        //                }
        //                else if (profileType =="technician")
        //                {
        //                    profileData.FullName = (customerDict.ContainsKey("TechnicianFullName") ? customerDict["TechnicianFullName"].ToString() : "");

        //                }

        //                  else if (profileType == "builder")

        //                {
        //                    profileData.FullName = (customerDict.ContainsKey("BuilderName") ? customerDict["BuilderName"].ToString() : "");

        //                }
        //                else if (profileType == "estimator")
        //                {
        //                    profileData.FullName = (customerDict.ContainsKey("EstimatorName") ? customerDict["EstimatorName"].ToString() : "");


        //                }
        //                // Extract Email and MobileNumber safely
        //                profileData.Email = customerDict.ContainsKey("EmailAddress") ? customerDict["EmailAddress"].ToString() : "";
        //                if(profileType!="customer")
        //                profileData.MobileNumber = customerDict.ContainsKey("PhoneNumber") ? customerDict["PhoneNumber"].ToString() : "";
        //                string capitalizedProfileType = char.ToUpper(profileType[0]) + profileType.Substring(1);

        //                //if (customerDict.ContainsKey(capitalizedProfileType + "PhotoId").ToString().Contains("."))
        //                //{


        //                  profileData.PhotoAttachmentId = customerDict.ContainsKey(capitalizedProfileType + "PhotoId") ? customerDict[capitalizedProfileType + "PhotoId"].ToString() : "download (1).jpg";
        //                //}
        //                //else
        //                //{
        //                //    profileData.PhotoAttachmentId = "download (1).jpg";
        //                //}




        //                profileData.Address = customerDict.ContainsKey("Address") ? customerDict["Address"].ToString() : "";
        //                    profileData.UserId = UserId;
        //                    profileData.UserProfileType = profileType;
        //                    // Bind them to profileData


        //                }
        //               else
        //                {
        //                    Console.WriteLine("customerDict is null");
        //                }




        //            return Ok(profileData);
        //        }
        //        else
        //        {
        //            return NotFound(new { message = "Customer not found" });
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (for example, log the error and return an internal server error)
        //        return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
        //    }
        //}





        // Method to get districts by StateId
        [HttpGet("getDistricts/{stateId}")]
        public async Task<IActionResult> GetDistricts(int stateId)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(_blobdisticts);

            var response = await blobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(response.Value.Content))
            {
                // Read JSON content
                string jsonContent = await reader.ReadToEndAsync();

                // Deserialize the JSON to a list of State objects
                var states = JsonSerializer.Deserialize<List<State>>(jsonContent);

                // Find the state by StateId and get its districts
                var state = states.FirstOrDefault(s => s.StateId == stateId);

                if (state == null)
                {
                    return NotFound(new { Message = "State not found." });
                }

                return Ok(state.Districts);
            }
        }
    }

    // Define classes matching your JSON structure
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public List<District> Districts { get; set; }
    }

    public class District
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
    }
}
