using System.Collections.Generic;
using Asp_.NET_Core_Mentoring_Module1.Entities;

namespace Asp_.NET_Core_Mentoring_Module1.Common.Entities
{
    public sealed class Region
    {
        public Region()
        {
            Territories = new HashSet<Territories>();
        }

        public int RegionId { get; set; }
        public string RegionDescription { get; set; }

        public ICollection<Territories> Territories { get; set; }
    }
}
