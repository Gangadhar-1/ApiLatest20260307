using System.ComponentModel.DataAnnotations;

namespace OtpAuthServices.Model
{

    public class RaiseTicket
    {
        public string RaiseTicketId { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }

        public string Details { get; set; }
        public string Category { get; set; }
        public string AssignedTo { get; set; }

        public string id     { get; set; }
        public string status { get; set; } 

        public string internalStatus { get; set; }  
        public string CustomerId { get; set; }

        public string State { get; set; }

            public string LowestBidderTechnicainId { get; set; }
        public string LowestBidderDealerId {  get; set; }


        public int IsMaterialType { get; set; }

        public string District { get; set; }
        public string ZipCode { get; set; }
        public string RequestType { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
        public List<Material> Materials { get; set; }

        public List <Comment> comments { get; set; } 
    }

   
    public class Material
    {
        public string material { get; set; }
        public string Quantity { get; set; }
    }

    public class Comment
    {
        public DateTime UpdatedDate { get; set; }
        public string CommentText
        {
            get; set;
        }

    }

}







    