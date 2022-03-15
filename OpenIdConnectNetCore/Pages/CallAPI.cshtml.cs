using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace OpenIdConnectNetCore.Pages
{
    [Authorize]
    public class CallAPIModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public string Result { get; set; }

        public CallAPIModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task OnGet()
        {
            //TokenResult? token = await BadWay();

            var cClient = ConfidentialClientApplicationBuilder.Create("26aa8499-8163-41df-9dc5-599c8359b24c")
                .WithAdfsAuthority("https://adfs.contoso.com/adfs", false)
                .WithClientSecret("eA6r6Y2uTnuViYOZhqdIgejWj-nLNXP1ImM-k1PC")
                .Build();

            var authenticationResult = await cClient.AcquireTokenForClient(new string[] { "https://localhost:7299/" })
                 .ExecuteAsync();

            _httpClient.BaseAddress = new Uri("https://localhost:7297");
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            Result = await _httpClient.GetStringAsync("/WeatherForecast");
        }

        private static async Task<TokenResult> BadWay()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, policy) => true;
            HttpClient tokenClient = new HttpClient(httpClientHandler);
            tokenClient.BaseAddress = new Uri("https://adfs.contoso.com");
            var content = new FormUrlEncodedContent(new[] {
               new KeyValuePair<string, string>("grant_type", "client_credentials"),
               new KeyValuePair<string, string>("resource", "https://localhost:7299"),
               new KeyValuePair<string, string>("client_id", "26aa8499-8163-41df-9dc5-599c8359b24c"),
               new KeyValuePair<string, string>("client_secret", "eA6r6Y2uTnuViYOZhqdIgejWj-nLNXP1ImM-k1PC"),
            });

            tokenClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var message = await tokenClient.PostAsync("/adfs/oauth2/token/", content);
            var token = await message.Content.ReadFromJsonAsync<TokenResult>();
            return token;
        }
    }

    public class TokenResult
    {
        public string Access_Token { get; set; }
    }
}
