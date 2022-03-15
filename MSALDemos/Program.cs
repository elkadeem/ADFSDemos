using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using Microsoft.Identity.Client;

namespace MSALDemos
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback
                += (sender, cert, chain, sslPolicyErrors) => true;

            var pClient = PublicClientApplicationBuilder
                .Create("ad48e236-b593-4449-a5ab-a84f95295656")
                .WithAdfsAuthority("https://adfs.contoso.com/adfs", false)
                .WithRedirectUri("http://localhost:8656")
                .Build();

            var scopes = new string[] { "openid" };
            SecureString passwordString = new SecureString();
            foreach(var passChar in  "P@ssw0rd@321".ToArray())
                passwordString.AppendChar(passChar);

            var authenticationResult = await pClient.AcquireTokenWithDeviceCode(scopes, (result) => {
                Console.WriteLine($"Open your browser and use url {result.VerificationUrl} " +
                    $"and code {result.UserCode}");
                return Task.CompletedTask;
            }).ExecuteAsync();

            ShowTokens(authenticationResult);

            Console.ReadKey();

            authenticationResult = await pClient
                .AcquireTokenByUsernamePassword(scopes, "elkadim@contoso.com"
                , passwordString)
                .ExecuteAsync();

            ShowTokens(authenticationResult);

            authenticationResult = await pClient
                .AcquireTokenInteractive(scopes)
                .ExecuteAsync();
            ShowTokens(authenticationResult);


            var account = (await pClient.GetAccountsAsync()).First();
            authenticationResult = await pClient
                .AcquireTokenSilent(new string[] { "https://localhost:7299/openid" }
            , account).ExecuteAsync();

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