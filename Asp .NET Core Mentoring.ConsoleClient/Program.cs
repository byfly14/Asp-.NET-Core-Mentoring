using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Asp_.NET_Core_Mentoring_Module1.Common.Entities;

namespace Asp_.NET_Core_Mentoring.ConsoleClient
{
    internal class Program
    {
        private const string CategoriesUri = "api/categories";
        private const string ProductsUri = "api/products";
        private const string MediaType = "application/json";

        private static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var categoriesResponse = await GetValues(CategoriesUri).ConfigureAwait(false);
            var categories = JsonConvert.DeserializeObject<IEnumerable<Categories>>(categoriesResponse);

            var productsResponse = await GetValues(ProductsUri).ConfigureAwait(false);
            var products = JsonConvert.DeserializeObject<IEnumerable<Products>>(productsResponse);

            foreach (var category in categories)
            {
                Console.WriteLine(category);
            }

            Console.WriteLine();

            foreach (var product in products)
            {
                Console.WriteLine(product);
            }
            Console.ReadLine();
        }

        private static async Task<string> GetValues(string relativeUri)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8088/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MediaType));

            var uri = new Uri(relativeUri, UriKind.Relative);
            var response = await client.GetAsync(uri).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseString;
        }
    }
}
