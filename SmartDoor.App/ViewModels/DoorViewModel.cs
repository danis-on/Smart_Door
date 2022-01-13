using Microsoft.Maui.Controls;
using SmartDoor.App.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public class DoorViewModel : ViewModelBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public Command OpenDoorCommand { get; }


        public DoorViewModel()
        {
            OpenDoorCommand = new Command(async () => await ExecuteOpenDoorCommand());

        }



        private async Task ExecuteOpenDoorCommand()
        {
            Message = null;

            try
            {
                if (!ApiClient.IsAuthorized)
                {
                    Message = "Nejste přihlášen!";
                    await DisplayMessage("Chyba!", Message);
                    await Shell.Current.GoToAsync("//login");
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
