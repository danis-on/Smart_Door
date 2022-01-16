using SmartDoor.App.Client;
using SmartDoor.App.Models;
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
    public class AddUserViewModel : BaseViewModel
    {
        private string _username, _password;
        private int _roleId;
        public string UserName { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public int RoleId { get => _roleId; set => SetProperty(ref _roleId, value); }

        public ObservableCollection<UserRole> data { get; set; } = new ObservableCollection<UserRole>();




        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public AsyncCommand AddCommand { get; }
        
        public AddUserViewModel()
        {
            Title = "Add User";
            AddCommand  = new AsyncCommand(AddUser);

        }

        async Task AddUser()
        {
            Message = null;

            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Password) ||
                (RoleId == 0)
               )
                {
                Message = "Zadej všechny údaje!";
                await DisplayMessage("Chyba!", Message);
                return;
                }
           
                await ApiClient.AddUser(UserName,(RoleId),Password);

                await Shell.Current.GoToAsync($"//{nameof(UserPage)}");

        }

    }
}
