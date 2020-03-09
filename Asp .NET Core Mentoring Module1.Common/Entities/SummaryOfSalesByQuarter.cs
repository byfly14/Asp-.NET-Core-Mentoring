using System;

namespace Asp_.NET_Core_Mentoring_Module1.Common.Entities
{
    public class SummaryOfSalesByQuarter
    {
        public DateTime? ShippedDate { get; set; }
        public int OrderId { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
