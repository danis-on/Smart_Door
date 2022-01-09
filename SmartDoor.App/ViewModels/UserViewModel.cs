﻿using Microsoft.Maui.Controls;
using MvvmHelpers;
using SmartDoor.App.Client;
using SmartDoor.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDoor.App.ViewModels
{
    public class UserViewModel : ViewModelBase
    {

        public ObservableRangeCollection<User> Users { get; set; }

        ApiClient _apiClient = new ApiClient();

        public Command RefreshCommand { get; }
        public Command AddCommand { get; }
        public Command<User> RemoveCommand { get; }

        public UserViewModel()

        {



            RefreshCommand = new Command(async () => await Refresh());
            AddCommand = new Command(async () => await Add());
           //RemoveCommand = new Command(async () => await Remove());
            GetUsers =  new Command(async () => await ExecuteGetUsers());


        }


        async Task Add()
        {
            var login = await App.Current.MainPage.DisplayPromptAsync("Login", "Login");
            var roleid = await App.Current.MainPage.DisplayPromptAsync("RoleId", "Role ID");
            var password = await App.Current.MainPage.DisplayPromptAsync("Password", "Passsword");
            await ApiClient.AddUser(login, Int32.Parse(roleid), password);
            await Refresh();
        }


        public Command GetUsers { get; }


        private async Task ExecuteGetUsers()
        {


            await _apiClient.GetUsers();

            await Shell.Current.GoToAsync("//home");



        }


        async Task Remove(User user)
        {
            await ApiClient.RemoveUser(user.Id);
            await Refresh();
        }

        async Task Refresh()
        {


            await Task.Delay(2000);


        }


    }
}