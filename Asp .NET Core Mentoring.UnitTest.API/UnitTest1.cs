using NUnit.Framework;
using Swagger;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Asp_.NET_Core_Mentoring.UnitTest.API
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            using var client = new HttpClient();
            using var myApi = new MyAPI(client, false)
            {
                BaseUri = new Uri("http://localhost:8088/")
            };
            var response = await myApi.Get1WithHttpMessagesAsync(1).ConfigureAwait(false);
        }
    }
}