
using SmartDoor.App.Client;
using SmartDoor.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartDoor.App.ViewModels
{
    public class DoorViewModel : BaseViewModel
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string status;
        private Color color;
        public Color StatusColor
        {
            get { return color; }
            set { SetProperty(ref color, value); }

        }

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value);
            }
        }



        public Command OpenDoorCommand { get; }
        public Command LogoutCommand { get; }


        public DoorViewModel()
        {
            OpenDoorCommand = new Command(async () => await ExecuteOpenDoorCommand());
            LogoutCommand = new Command(async () => await ExecuteLogoutCommand());
            StatusColor = Color.Red;
            Status =  "ZAMČENO";

        }


        private async Task ExecuteLogoutCommand()
        {
            ApiClient.Logout();
            await DisplayMessage("Info!", "Byl jste úspěšně odhlášen");
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }


        private async Task ExecuteOpenDoorCommand()
        {
            Message = null;
            IsBusy = false;
            try
            {
                if (!ApiClient.IsAuthorized)
                {
                    Message = "Nejste přihlášen!";
                    await DisplayMessage("Chyba!", Message);
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    var result = await ApiClient.OpenDoor();

                    if (!result.IsSuccess)
                    {
                        Message = "Nelze otevřít dveře";
                        await DisplayMessage("Chyba!", Message);

                        return;
                    }
                    Status = "OTEVŘENO!";
                    StatusColor = Color.Green;


                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                await DisplayMessage("Chyba!", Message);
            }
            await Task.Delay(5000);
            IsBusy = true;
            Status = "ZAMČENO!";
            StatusColor = Color.Red;
        }




    }
}
