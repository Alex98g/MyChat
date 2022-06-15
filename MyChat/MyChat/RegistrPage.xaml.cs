using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;


namespace MyChat
{

    public partial class RegistrPage : ContentPage
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public bool bool_login, bool_password, bool_replypassword;
        public RegistrPage()
        {
            InitializeComponent();
        }

        private async void ReqRegister(string username, string password)
        {
            ServerStatus result = await Client.Instance.RegisterNewUser(username, password);

            switch (result)
            {
                case ServerStatus.SUCCESS:
                    await DisplayAlert("Выполнено!", "Регистрация выполнена.", "OK");
                    await Navigation.PushModalAsync(new FirstPage());
                    break;
                case ServerStatus.ERROR_REG_LOGIN_EXISTS:
                    await DisplayAlert("Ошибка", "Логин уже существует", "OK");
                    break;
                case ServerStatus.ERROR_REG_BAD_LOGIN:
                    await DisplayAlert("Ошибка", "Неверный логин или пароль", "OK");
                    break;
                case ServerStatus.ERROR_UNKNOWN:
                    await DisplayAlert("Ошибка", "Неизвестная ошибка", "OK");
                    break;
                default:
                    break;
            }
        }
        private async void Exit(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new FirstPage());
        }

        private async void Register_Pressed(object sender, EventArgs e)
        {
            if (Password.Text != ReplyPassword.Text)
            {
                await DisplayAlert("Ошибка", "Не совпадают пароли, введите еще раз!", "OK");
            }
            else
            {
                ReqRegister(Login.Text, Password.Text);
            }
        }

        public void EntryLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Login.Text == "")
            {
                bool_login = false;
            }
            else
            {
                bool_login = true;
            }
            if ((bool_login == true) & (bool_password == true) & (bool_replypassword == true))
            {
                Reg.IsEnabled = true;
            }
            else
            {
                Reg.IsEnabled = false;
            }
        }
        public void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Password.Text == "")
            {
                bool_password = false;
            }
            else
            {
                bool_password = true;
            }
            if ((bool_login == true) & (bool_password == true) & (bool_replypassword == true))
            {
                Reg.IsEnabled = true;
            }
            else
            {
                Reg.IsEnabled = false;
            }
        }
        public void EntryReplyPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ReplyPassword.Text == "")
            {
                bool_replypassword = false;
            }
            else
            {
                bool_replypassword = true;
            }
            if ((bool_login == true) & (bool_password == true) & (bool_replypassword == true))
            {
                Reg.IsEnabled = true;
            }
            else
            {
                Reg.IsEnabled = false;
            }

        }


    }
}