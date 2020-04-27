﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp_.NET_Core_Mentoring_Module1.Common.Entities
{
    public class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int ProductId { get; set; }

        [Required, StringLength(80)]
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }

        [Required, StringLength(80)]
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public virtual Categories Category { get; set; }
        public virtual Suppliers Supplier { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public override string ToString()
        {
            return $"ProductId: {ProductId}, ProductName: {ProductName}";
        }
    }
}
