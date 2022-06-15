using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class FirstPage : ContentPage
    {
        public String login;
        public String password;
        public FirstPage()
        {
            InitializeComponent();
            Login1.Text = "";
            Password.Text = "";
        }

        private async void ReqLogin(string username, string password)
        {
            ServerStatus result = await Client.Instance.Login(username, password);

            switch (result)
            {
                case ServerStatus.SUCCESS:
                    await Navigation.PushModalAsync(new MainPage());
                    break;
                case ServerStatus.ERROR_LOGIN_BAD_LOGIN:
                    await DisplayAlert("Ошибка", "Неверный пароль или логин", "OK");
                    break;
                case ServerStatus.ERROR_UNKNOWN:
                    await DisplayAlert("Ошибка", "Неизвестная ошибка", "OK");
                    break;
                default:
                    break;
            }
        }

        public void EntryLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((Login1.Text != "") & (Password.Text != ""))
            {
              Login_ON.IsEnabled = true;
            }
            else
            {
               Login_ON.IsEnabled = false;
            }
        }
        public void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((Login1.Text != "") & (Password.Text != ""))
            {
                Login_ON.IsEnabled = true;
            }
            else
            {
                Login_ON.IsEnabled = false;
            }
        }
        private async void Log_on(object sender, EventArgs e)
        {
            ReqLogin(Login1.Text, Password.Text);
        }
        private async void Reg(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegistrPage());
        }











    }
}