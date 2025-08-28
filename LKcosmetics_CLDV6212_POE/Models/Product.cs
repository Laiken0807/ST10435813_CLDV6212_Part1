using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace LKcosmetics_CLDV6212_POE.Models
{
    public class Product : ITableEntity
    {
       
        public string? Name { get; set; }
        public string? Category { get; set; }
        public double? Price { get; set; }
        public int? Stock { get; set; }
        public string? ImageUrl { get; set; }

       

        public string PartitionKey { get; set; } 
        public string RowKey { get; set; }   
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
