using System.Collections.Generic;

namespace Asp_.NET_Core_Mentoring_Module1.Common.Entities
{
    public class Categories
    {
        public Categories()
        {
            Products = new HashSet<Products>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<Products> Products { get; set; }

        public override string ToString() =>
            $"CategoryId: {CategoryId}, CategoryName: {CategoryName}, Description: {Description};";
    }
}
