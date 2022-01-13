using Microsoft.Maui.Controls;
using SmartDoor.App.Client;
using SmartDoor.App.Models;
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
        private string username;
        private string password;


        public string Username { get => username; set => SetProperty(ref username, value); }
        public string Password { get => password; set => SetProperty(ref password, value); }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await ExecuteLoginCommand());

        }

        public Command LoginCommand { get; }


        private async Task ExecuteLoginCommand()
        {
            Message = null;

            try
            {
                if ((string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) && !ApiClient.IsAuthorized)
                {
                    Message = "Zadej username a password!";
                    await DisplayMessage("Chyba!", Message);
                }
                else
                {
                    var result = await ApiClient.Login(Username, Password);

                    if (!result.IsSuccess && result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Message = "Nesprávné přihlašovací údaje";
                        await DisplayMessage("Chyba!", Message);

                        return;
                    }
                    else if (!result.IsSuccess)
                    {
                        Message = "Upss! Něco se pokazilo";
                        await DisplayMessage("Chyba!",Message);
                        return;
                    }
                    Message = $"Vitej v aplikaci uživateli {Username}!";
                    await DisplayMessage("Přihlášení úspěšné!",Message);
                    await Shell.Current.GoToAsync("//home");
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                await DisplayMessage("Chyba!",Message);
            }
        }

     

    }

}

