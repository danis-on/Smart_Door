using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartDoor.App.Models


{

    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("pasword")]
        public string Password { get; set; }
        [JsonProperty("role")]
        public int RoleId { get; set; }
       
    }

    public class Login
    {
 
        [JsonProperty("login")]
        public string Loginname { get; set; }
        [JsonProperty("password")]
        public string Password{ get; set; }
        [JsonProperty("validity")]
        public int Validity { get; set; }

    }




}