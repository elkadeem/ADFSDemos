using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using Microsoft.Identity.Client;

namespace MSALConfiedential
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback
                += (sender, cert, chain, sslPolicyErrors) => true;

            var cClient = ConfidentialClientApplicationBuilder.Create("26aa8499-8163-41df-9dc5-599c8359b24c")
                .WithAdfsAuthority("https://adfs.contoso.com/adfs", false)
                .WithClientSecret("eA6r6Y2uTnuViYOZhqdIgejWj-nLNXP1ImM-k1PC")
                .Build();

           var authenticationResult = await cClient.AcquireTokenForClient(new string[] { "https://localhost:7299/" })
                .ExecuteAsync();

            ShowTokens(authenticationResult);
            await CallApi(authenticationResult);

            Console.ReadLine();
        }

        private static async Task CallApi(AuthenticationResult authenticationResult)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7297");
            httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            var result = await httpClient.GetStringAsync("/WeatherForecast");
            Console.WriteLine(result);
        }

        private static void ShowTokens(AuthenticationResult result)
        {
            try
            {
                foreach (var p in result.GetType().GetProperties())
                {
                    Console.WriteLine($"{p.Name}: {p.GetValue(result)}");
                }
            }
            catch (Exception)
            {
            }
            Console.WriteLine();
        }
    }
}