using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using MvvmHelpers;
using SmartDoor.App.ViewModels;
using System;
using Application = Microsoft.Maui.Controls.Application;

namespace SmartDoor.App
{
    public partial class App : Application
    {

        protected static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            SetupServices();

            MainPage = new AppShell();
           
        }

        public static ViewModelBase GetViewModel<TViewModel>() where TViewModel : ViewModelBase
           => ServiceProvider.GetRequiredService<TViewModel>();

        private void SetupServices()
        {
            var services = new ServiceCollection();

           

            //ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<UserViewModel>();


            ServiceProvider = services.BuildServiceProvider();


        }


    }
}
