using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string BankCode { get; set; } = null!;
        public string BankTranNo { get; set; } = null!;
        public string CardType { get; set; } = null!;
        public string? PaymentInfo { get; set; }
        public DateTime? PayDate { get; set; }
        public string TransactionNo { get; set; } = null!;
        public int TransactionStatus { get; set; }
        public decimal PaymentAmount { get; set; }
        public int? OrderId { get; set; }

        public virtual Order? Order { get; set; }
    }
}
