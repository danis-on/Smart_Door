using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Animations;
using SmartDoor.App.ViewModels;
using SmartDoor.App.Client;

namespace SmartDoor.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = App.GetViewModel<LoginViewModel>();

        }

        protected override void OnAppearing()
        {
            if (!ApiClient.IsAuthorized)
            {
                Shell.Current.GoToAsync("//login");
            }
            base.OnAppearing();
        }



    }

}