using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.Client
{

    public class ApiResponse
    {
        public HttpResponseMessage ResponseMessage { get; set; }
        public string ResponseContent { get; set; }
        public string ErrorCode { get; set; }
        public bool IsSuccess { get { return ResponseMessage?.StatusCode == System.Net.HttpStatusCode.OK; } }
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int ValidityInSeconds { get; set; }
    }

    public class AuthenticationModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

}
