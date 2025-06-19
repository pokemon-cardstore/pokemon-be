using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class MessageDtoResponse
    {
        public int MessageId { get; set; }
        public int CustomerId { get; set; }
        public string Type { get; set; }
        public string? CustomerName { get; set; }
        public string? Content { get; set; }
        public DateTime? SendTime { get; set; }
        public int Status { get; set; }
    }
}
