using Microsoft.Maui.Controls;
using SmartDoor.App.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

          ApiClient _apiClient = new ApiClient();

        public string Username { get; set; }
        public string Password { get; set; }


        public LoginViewModel ()
        {
            LoginCommand = new Command(async () => await ExecuteLoginCommand());

        }

        public Command LoginCommand { get; }


        private async Task ExecuteLoginCommand()
        {
            if ((string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) && !_apiClient.IsAuthorized)
            {
                Console.WriteLine($"zadej uzivatele");
                
            }
            else {

                await _apiClient.Login(Username, Password);
                await _apiClient.GetUsers();

                await Shell.Current.GoToAsync("//home");
            }
          

        }



    }

}

