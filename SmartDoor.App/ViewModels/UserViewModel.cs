using MvvmHelpers.Commands;
using SmartDoor.App.Models;
using SmartDoor.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public class UserViewModel : ViewModelBase
    {

        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand AddCommand { get; }
        public AsyncCommand<User> RemoveCommand { get; }

        public UserViewModel()

        {
            Title = "Users of this home";


            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
            RemoveCommand = new AsyncCommand<User>(Remove);


        }


        async Task Add()
        {
            var login = await App.Current.MainPage.DisplayPromptAsync("Login", "Login");
            var roleid = await App.Current.MainPage.DisplayPromptAsync("RoleId", "Role ID");
            var password = await App.Current.MainPage.DisplayPromptAsync("Password", "Passsword");
            await UserService.AddUser(login,Int32.Parse(roleid),password);
            await Refresh();
        }

        async Task Remove(User user)
        {
            await UserService.RemoveUser(user.Id);
            await Refresh();
        }

        async Task Refresh()
        {
            IsBusy = true;

            await Task.Delay(2000);

            IsBusy = false;
        }


    }
}
