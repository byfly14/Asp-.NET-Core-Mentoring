// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Swagger.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CustomerCustomerDemo
    {
        /// <summary>
        /// Initializes a new instance of the CustomerCustomerDemo class.
        /// </summary>
        public CustomerCustomerDemo()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CustomerCustomerDemo class.
        /// </summary>
        public CustomerCustomerDemo(string customerId = default(string), string customerTypeId = default(string), Customers customer = default(Customers), CustomerDemographics customerType = default(CustomerDemographics))
        {
            CustomerId = customerId;
            CustomerTypeId = customerTypeId;
            Customer = customer;
            CustomerType = customerType;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerTypeId")]
        public string CustomerTypeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customer")]
        public Customers Customer { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customerType")]
        public CustomerDemographics CustomerType { get; set; }

    }
}
