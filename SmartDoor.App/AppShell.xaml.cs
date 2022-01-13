using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using SmartDoor.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SmartDoor.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage),
              typeof(LoginPage));

            Routing.RegisterRoute(nameof(UserPage),
                typeof(UserPage));

            Routing.RegisterRoute(nameof(MainPage),
                typeof(MainPage));

        }

    }

       
 }

