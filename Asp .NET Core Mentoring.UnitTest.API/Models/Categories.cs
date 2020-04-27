// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Swagger.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Categories
    {
        /// <summary>
        /// Initializes a new instance of the Categories class.
        /// </summary>
        public Categories()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Categories class.
        /// </summary>
        public Categories(int? categoryId = default(int?), string categoryName = default(string), string description = default(string), byte[] picture = default(byte[]), IList<Products> products = default(IList<Products>))
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            Description = description;
            Picture = picture;
            Products = products;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "categoryId")]
        public int? CategoryId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "categoryName")]
        public string CategoryName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "picture")]
        public byte[] Picture { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "products")]
        public IList<Products> Products { get; set; }

    }
}
