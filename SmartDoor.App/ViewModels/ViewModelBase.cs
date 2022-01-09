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
    public abstract class ViewModelBase : BaseViewModel, INotifyPropertyChanged



    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (value == null ? field != null : !value.Equals(field))
            {
                field = value;

                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }

        public static bool InternetConnectivity()
        {
            var connectivity = Connectivity.NetworkAccess;
            if (connectivity == NetworkAccess.Internet)
                return true;

            return false;
        }
    }
}
