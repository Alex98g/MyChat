﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class FirstPage : ContentPage
    {
        public String login;
        public String password;
        List<string> lst;

        public FirstPage()
        {
            InitializeComponent();
        }
        private async void Reg(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegistrPage());
        }
        private async void Log_on(object sender, EventArgs e)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "account.txt");
            lst = File.ReadAllLines(fileName).ToList();
            Debug.WriteLine(lst[0]);
            if (Login.Text != lst[0] || Password.Text != lst[1])
            {
                await DisplayAlert("Ошибка", "Неверный логин или пароль, попробуйте еще раз", "OK");
            }
            else
            {
                lst.RemoveAt(3);
                lst.Add("true");
                File.WriteAllLines(fileName, lst);

                await Navigation.PushModalAsync(new MainPage());
            }
        }
       

    }
}