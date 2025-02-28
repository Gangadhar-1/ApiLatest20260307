namespace OtpAuthServices.Model
{
    public class BookTechnicianAddress
    {
        public string id { get; set; }

        public string ProfileType { get; set; } = null;
        public string BookTechnicianAddressId { get; set; }
        public bool? IsPrimaryAddress { get; set; }

        public string Address { get; set; }

        public string State { get; set; }

        public string District { get; set; }


        public string ZipCode { get; set; }


        public string UserId { get; set; }
      
        public string TechnicianFullName { get; set; }  


    


    }
}
