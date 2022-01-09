using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using SmartDoor.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SmartDoor.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public UserPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<UserViewModel>();
        }
    }
}