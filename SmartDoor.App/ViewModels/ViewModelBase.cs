using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public abstract class ViewModelBase : BaseViewModel

    {
     

        public static bool InternetConnectivity()
        {
            var connectivity = Connectivity.NetworkAccess;
            if (connectivity == NetworkAccess.Internet)
                return true;

            return false;
        }

        public async Task DisplayMessage(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }

    }
}
