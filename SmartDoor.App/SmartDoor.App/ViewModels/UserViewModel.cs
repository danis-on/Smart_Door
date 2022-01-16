
using SmartDoor.App.Client;
using SmartDoor.App.Models;
using SmartDoor.App.Services;
using SmartDoor.App.ViewModels;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SmartDoor.App.Views;

namespace SmartDoor.App.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand AddCommand { get; }
        public AsyncCommand<User> RemoveCommand { get; }
        public AsyncCommand<User> SelectedCommand { get; }

        public AsyncCommand<List<User>> GetUserCommand { get; }
        private string test;

        public string Test { get => test; set => SetProperty(ref test, value); }

        public UserViewModel()

        {

            Users = new ObservableCollection<User>();


            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(Add);
            RemoveCommand = new AsyncCommand<User>(Remove);
            SelectedCommand = new AsyncCommand<User>(Selected);
   

        }


        async Task Add()
        {

            await Shell.Current.GoToAsync($"//{nameof(AddUserPage)}");

        }

        async Task Selected(User user)
        {
            if (user == null)
                return;

            //var route = $"{nameof(UserDetailPage)}?CoffeeId={user.Id}";
            //await Shell.Current.GoToAsync(route);
        }

        async Task Remove(User user)
        {
            await ApiClient.RemoveUser(user.Id);
            await Refresh();
        }

        async Task Refresh()
        {
            IsBusy = true;


            Users.Clear();

            try
            {

                var users = await ApiClient.GetUsers();


                foreach (var u in users.Data)
                {
                    var user = new User();
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

            IsBusy = false;


            DependencyService.Get<IToast>()?.MakeToast("Refreshed!");
        }
    }
           
}