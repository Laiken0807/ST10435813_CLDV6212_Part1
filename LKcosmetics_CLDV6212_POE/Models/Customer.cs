using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace LKcosmetics_CLDV6212_POE.Models
{
  
        public class Customer : ITableEntity
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }

       

            public string PartitionKey { get; set; } 
            public string RowKey { get; set; }  
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }
    }