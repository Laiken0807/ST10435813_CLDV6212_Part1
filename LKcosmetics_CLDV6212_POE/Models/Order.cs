using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace LKcosmetics_CLDV6212_POE.Models
{
   
       public class Order : ITableEntity

    {
      
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? FileName { get; set; }

        
        public string PartitionKey { get; set; } 
        public string RowKey { get; set; }   
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}