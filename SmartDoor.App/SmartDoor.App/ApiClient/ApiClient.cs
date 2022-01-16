using SmartDoor.App.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Threading;


namespace SmartDoor.App.Client
{
    public static class ApiClient
    {

        //URL to SmartDoor Server API
        private static string BaseUrl = "http://192.168.68.105:5281"; /*172.16.10.132:5281 http://192.168.68.105:5281*/

        private static string BearerToken;


        static HttpClient client = new HttpClient()
        {

            BaseAddress = new Uri(BaseUrl)

        };

        static bool _auth;
        public static bool IsAuthorized
        {

            get
            {
                return _auth;
            }

            set
            {
                _auth = value;
                
            }
        }



        public static void Logout()
        {
            client.DefaultRequestHeaders.Remove("Token");
            IsAuthorized = false;
        }


        public static async Task<ApiResponse> Login(string username, string password, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                var request = new LoginRequest() { Login = username, Password = password };
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/Auth/Login");
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest, ct).ConfigureAwait(false);
                Console.WriteLine($"Response:{response.StatusCode} {response.ReasonPhrase}");

                IsAuthorized = response.StatusCode == System.Net.HttpStatusCode.OK;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = JsonConvert.DeserializeObject<AuthenticationModel>(await response.Content.ReadAsStringAsync());
                    Console.WriteLine($"ResponseData:{responseData}");
                    BearerToken = responseData.Token;
                    client.DefaultRequestHeaders.Add("Token", BearerToken);

                    return new ApiResponse(response.StatusCode);
                }
                else
                {
                    return new ApiResponse(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed {ex}");
                return new ApiResponse(System.Net.HttpStatusCode.InternalServerError);
            }

        }


        public static async Task<ApiResponse<User>> GetUser(int id, CancellationToken ct = default)
        {
            var response = await Send<User>(HttpMethod.Get, $"/User/Get/{id}", null, ct).ConfigureAwait(false);

            return response;
        }

        public static async Task<ApiResponse> OpenDoor(CancellationToken ct = default)
        {
            var response = await Send<string>(HttpMethod.Get, $"/Door/Open", null, ct).ConfigureAwait(false);

            return response;
        }



        public static async Task<ApiResponse<List<User>>> GetUsers(CancellationToken ct = default)
        {
            var response = await Send<List<User>>(HttpMethod.Get, "/User/Get/AllUsers", null, ct).ConfigureAwait(false);

            if (!response.IsSuccess)
                return null;
            return response;
        }


        public static async Task<ApiResponse> AddUser(string login, int roleId, string password, CancellationToken ct = default)
        {

            var user = new NewUser
            {
                Login = login,
                RoleId = roleId,
                Password = password
            };

            var response = await Send<NewUser>(HttpMethod.Post, "/User/Create", user, ct).ConfigureAwait(false);

            if (!response.IsSuccess)
            {
                return null;
            }
            return response;
        }

        public static async Task<ApiResponse>RemoveUser(int id, CancellationToken ct = default)
        {

            var response = await Send<int>(HttpMethod.Post, $"/User/Delete/{id}", null, ct).ConfigureAwait(false);

            if (!response.IsSuccess)
                return null;
            return response;

        }


        private static async Task<ApiResponse<T>> Send<T>(HttpMethod httpMethod, string endpoint, object data, CancellationToken ct = default(CancellationToken)) 
        {

            var endpointUri = new Uri(BaseUrl + endpoint);

            try
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, endpointUri);


                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    httpRequest.Content = content;
                }

                var response = await client.SendAsync(httpRequest, ct).ConfigureAwait(false);
                Console.WriteLine($"Response:{response.StatusCode} {response.ReasonPhrase}");
                if (((int)response.StatusCode) < 400)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ResponseData:{responseData}");
                    
                    return new ApiResponse<T>(response.StatusCode, JsonConvert.DeserializeObject<T>(responseData));
                }
                return new ApiResponse<T>(response.StatusCode, default(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed {ex}");
                return new ApiResponse<T>(System.Net.HttpStatusCode.InternalServerError,default(T)); ;
            }

        }



    }
}
