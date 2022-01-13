using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Essentials;
using SmartDoor.App.Client;
using SmartDoor.App.ViewModels;
using System;

namespace SmartDoor.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        int count = 0;



        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<DoorViewModel>();
            
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            CounterLabel.Text = $"Current count: {count}";

            SemanticScreenReader.Announce(CounterLabel.Text);
        }

        private async void Logout(object sender, EventArgs e)
        {
            ApiClient.Logout();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
