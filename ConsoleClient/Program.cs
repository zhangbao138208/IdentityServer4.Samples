using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new HttpClient();
            //Task.Run(async ()=> {
            //    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            //}).GetAwaiter().GetResult();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (disco.IsError)
            {
               Console.WriteLine(disco.Error);
               return;
            }

            // request access token
            var responseToken = await client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest { 
                    Address=disco.TokenEndpoint,
                    ClientId= "console client",
                    ClientSecret= "511536EF-F270-4058-80CA-1C89C192F69A",
                    Scope="api1"
                });
            if (responseToken.IsError)
            {
                Console.WriteLine(responseToken.Error);
                return;
            }

            //call Identity Resource  api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(responseToken.AccessToken);
            var response = await apiClient.GetAsync("http://localhost:5002/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
            Console.ReadKey();
        }
    }
}