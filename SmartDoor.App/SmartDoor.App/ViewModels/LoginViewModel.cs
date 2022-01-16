using SmartDoor.App.Client;
using SmartDoor.App.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartDoor.App.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private string username;
        private string password;
        private string test;

        public string Test { get => test;  set  => SetProperty(ref test, value); }
        public string Username { get => username; set => SetProperty(ref username, value); }
        public string Password { get => password; set => SetProperty(ref password, value); }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }
        public Command LoginCommand { get; }

        public LoginViewModel()
        {

            test = "test";
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(DoorPage)}");
        }

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
                        await DisplayMessage("Chyba!", Message);
                        return;
                    }
                    Message = $"Vitej v aplikaci uživateli {Username}!";
                    await DisplayMessage("Přihlášení úspěšné!", Message);
                    //await Shell.Current.GoToAsync("//door");
                    await Shell.Current.GoToAsync($"//{nameof(DoorPage)}");
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                await DisplayMessage("Chyba!", Message);
            }
        }
    }
}
