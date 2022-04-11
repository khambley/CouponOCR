using System;
using System.Collections.Generic;
using System.Text;

namespace CouponOCR
{
    public class Coupon
    {
        public int Id { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ManufacturerName { get; set; }
        public decimal? SavingsAmount { get; set; }
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string Photo { get; set; } = string.Empty;

    }
}
