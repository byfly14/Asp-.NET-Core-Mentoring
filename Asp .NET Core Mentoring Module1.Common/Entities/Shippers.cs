using System;
using System.Collections.Generic;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;

namespace Asp_.NET_Core_Mentoring_Module1.Entities
{
    public partial class Shippers
    {
        public Shippers()
        {
            Orders = new HashSet<Orders>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
