using SmartDoor.App.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Essentials;
using System.Diagnostics;

namespace SmartDoor.App.Services
{
    public static class UserService
    {

        //URL to SmartDoor Server API
        static string BaseUrl = "https://localhost:7281";


        static HttpClient client;

        static UserService()
        {
            try
            {
                client = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl)

                };


            }
            catch
            { 
            
            }
        
        }

        public static async Task<User> GetUser(int id)
        {

            User user = new User();
            HttpResponseMessage response = await client.GetAsync($"/User/Get/{id}");

            if (response.IsSuccessStatusCode)
            {    
                string responseString = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(responseString);
            
            };
            
            return  user ;
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




    }
}
