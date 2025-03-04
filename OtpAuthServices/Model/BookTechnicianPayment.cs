namespace OtpAuthServices.Model
{
    public class BookTechnicianPayment 
    {
    
        public string id { get; set; }


        public string BookTechnicianId { get; set; }
        public string PaymentId { get; set; }

        public string PaymentMode { get; set; }

        public string ApprovedAmount { get; set; }


        public string PaidAmount { get; set; }


        public string BalancedAmount { get; set; }


        public string PaymentDataTime { get; set; }

        public string TechnicianAmount { get; set; }


     



        public string UTRTransactionNumber { get; set; }


        public string TechnicianConfirmationCode { get; set; }
    }
}

