using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class ChatRequest
    {
        public int MessageId { get; set; }
        public int CustomerId { get; set; }
        public string Type { get; set; } = null!;
        public string? Content { get; set; }
        public DateTime? SendTime { get; set; }
        public int Status { get; set; }
    }
}
