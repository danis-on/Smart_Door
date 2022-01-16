using SmartDoor.App.ViewModels;
using SmartDoor.App.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SmartDoor.App
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddUserPage), typeof(AddUserPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
            Routing.RegisterRoute(nameof(DoorPage), typeof(DoorPage));
        }

    }
}
