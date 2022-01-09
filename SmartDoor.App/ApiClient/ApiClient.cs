using SmartDoor.App.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Essentials;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Threading;
using SmartDoor.App.Client;

namespace SmartDoor.App.Client
{
    public class ApiClient
    {

        public const int ValidityOfToken = 3600;

        //URL to SmartDoor Server API
        private string BaseUrl = "http://smartdoor.cz";
        public Uri Uri { get; }

        private string BearerToken;
        private DateTime BearerValidTo;

        //static string Login(string user, string password);
        static HttpClient client;


        public bool IsAuthorized
        {
            get
            {
                return !string.IsNullOrEmpty(BearerToken) && BearerValidTo > DateTime.Now;
            }
        }

        public ApiClient()
        {
            try
            {
                client = new HttpClient()
                {
                    BaseAddress = new Uri(BaseUrl)

                };
            }
            catch
            {

            }

        }


        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }


        public async Task<bool> Login(string username, string password, CancellationToken ct = default(CancellationToken))
        {
            //HttpClientHandler insecureHandler = GetInsecureHandler();

            using (var client = new HttpClient())
            {
                try
                {
                    var request = new LoginRequest() { Login = username, Password = password, ValidityInSeconds = ValidityOfToken };
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/Auth/Login");
                    httpRequest.Content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(httpRequest, ct).ConfigureAwait(false);
                    Console.WriteLine($"Response:{response.StatusCode} {response.ReasonPhrase}");

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseData = JsonConvert.DeserializeObject < AuthenticationModel >(await response.Content.ReadAsStringAsync());
                        Console.WriteLine($"ResponseData:{responseData}");
                        BearerToken = responseData.Token;
                        //string  BearerToken2 = JsonConvert.DeserializeAnonymousType<>(responseData);
                        BearerValidTo = DateTime.Now.AddSeconds(ValidityOfToken * 0.9);

                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed {ex}");
                    return false;
                }
            }
        }



        public  async Task<User> GetUser(int id)
        {

            User user = new User();

            HttpResponseMessage response = await client.GetAsync($"/User/Get/{id}");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(responseString);

            };

            return user;
        }

        public  async Task<IEnumerable<User>> GetUsers(CancellationToken ct = default(CancellationToken))
        {

            //if (!IsAuthorized) return null;

            var response = await Send(HttpMethod.Get, "/User/Get/AllUsers", null, ct).ConfigureAwait(false);
            if (!response.IsSuccess)
                return null;
            return JsonConvert.DeserializeObject<IEnumerable<User>>(response.ResponseContent);

        }

        public static async Task AddUser(string login, int roleId, string password)
        {
           
            var user = new User
            {
                Login = login,
                RoleId = roleId,
                Password = password
            };

            var json = JsonConvert.SerializeObject(user);
            var content =
                new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/User/Create", content);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Nepodařilo se přidat člena domáctnosti");
            }
        }

        public static async Task RemoveUser(int id)
        {
            var response = await client.DeleteAsync($"/User/Delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Nepodařilo se odebrat člena domáctnosti");
            }
        }


        private async Task<ApiResponse> Send(HttpMethod httpMethod, string endpoint, object data, CancellationToken ct = default(CancellationToken))
        {
            //if (!IsAuthorized)
              //  return new ApiResponse() { ErrorCode = "Client not authorized" };

            var endpointUri = new Uri(BaseUrl+endpoint);
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("", BearerToken); 
                try
                {
                    HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, endpointUri);
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

                    if (data != null)
                    {
                        var json = JsonConvert.SerializeObject(data);
                        var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
                        httpRequest.Content = content;
                    }

                    var response = await client.SendAsync(httpRequest, ct).ConfigureAwait(false);
                    Console.WriteLine($"Response:{response.StatusCode} {response.ReasonPhrase}");
                    if (((int)response.StatusCode) < 400)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"ResponseData:{responseData}");
                        return new ApiResponse() { ResponseContent = responseData, ResponseMessage = response };
                    }
                    return new ApiResponse() { ResponseContent = null, ResponseMessage = response };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed {ex}");
                    return new ApiResponse() { ErrorCode = ex.Message };
                }
            }
        }



    }
}
