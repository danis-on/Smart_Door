using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.Client
{

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse(HttpStatusCode statusCode, T data)
            :base(statusCode)
        {
            Data = data;
        }
    }

    public class ApiResponse
    {
        public string ErrorCode { get; set; }
        public bool IsSuccess => StatusCode == HttpStatusCode.OK;
        public HttpStatusCode StatusCode { get; }

        public ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
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
