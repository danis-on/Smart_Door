using Microsoft.Maui.Controls;
using MvvmHelpers;
using SmartDoor.App.Client;
using SmartDoor.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public class UserViewModel : ViewModelBase
    {

        public ObservableRangeCollection<User> Users {get ; set;} 
       

        public Command RefreshCommand { get; }
        public Command AddCommand { get; }
        public Command<User> RemoveCommand { get; }

        public UserViewModel()

        {

           Users = new ObservableRangeCollection<User>();

            RefreshCommand = new Command(async () => await Refresh());
            AddCommand = new Command(async () => await Add());
           //RemoveCommand = new Command(async () => await Remove());
           GetUsers =  new Command(async () => await ExecuteGetUsers());

            
        }


        async Task Add()
        {
           /* var login = await App.Current.MainPage.DisplayPromptAsync("Login", "Login");
            UserRole roleid = await App.Current.MainPage.DisplayPromptAsync("RoleId", "Role ID");
            var password = await App.Current.MainPage.DisplayPromptAsync("Password", "Passsword");
            await ApiClient.AddUser(login, , password);
            await Refresh();*/
        }


        public Command GetUsers { get; }


       private async Task ExecuteGetUsers()
        {
            try
            {

                var users  =  await ApiClient.GetUsers();


            foreach (var u in users.Data)
            {
                   var  user = new User();
                {
                        user.Id = u.Id;
                        user.Login = u.Login;
                        user.Password = u.Password;
                        user.RoleId = u.RoleId;

                };
                Users.Add(user);
                            
            }


            }
            catch (Exception ex)
            {
                await DisplayMessage("Chyba!", ex.Message);
            }
         

        }
       

        async Task Remove(User user)
        {
            await ApiClient.RemoveUser(user.Id);
            await Refresh();
        }

        async Task Refresh()
        {

            await Task.Delay(2000);
            //await Task.Run(ExecuteGetUsers);

        }


    }
}