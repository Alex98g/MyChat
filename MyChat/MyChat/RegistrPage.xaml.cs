using Grpc.Core;
using SimpleChatApp.CommonTypes;
using SimpleChatApp.GrpcService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SimpleChatApp.GrpcService.ChatService;


namespace MyChat
{

    public partial class RegistrPage : ContentPage
    {
        private ChatServiceClient chatServiceClient;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ObservableCollection<string> account = new ObservableCollection<string>();
        private List<string> lst;
        private String colortext;

        public RegistrPage()
        {
                        
            InitializeComponent();
            chatServiceClient = new ChatServiceClient(new Channel("Localhost", 30051, ChannelCredentials.Insecure));

        }
       
        public async Task RegisterNewUser(string login, string password)
        {
            var userData = new UserData()
            {
                Login = login,
                PasswordHash = SHA256.GetStringHash(password)
            };
            var ans = await chatServiceClient.RegisterNewUserAsync(userData);
            Debug.WriteLine(ans);

        }
        private async void Exit(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new FirstPage());
        }
        private async void Register_Pressed(object sender, EventArgs e)
        {
            if ((Login.Text == null & Password.Text == null & ReplyPassword.Text == null) || (Login.Text == "" & Password.Text == "" & ReplyPassword.Text == ""))
            {
                await DisplayAlert("Ошибка!", "Введите логин и пароль!", "OK");
            } else
            {
                if (Login.Text == null || Login.Text == "")
                {
                    await DisplayAlert("Ошибка!", "Введите логин!", "OK");
                }
                else
                {
                    if (Password.Text == null || Password.Text == "")
                    {
                        await DisplayAlert("Ошибка!", "Введите пароль!", "OK");
                    }
                    else
                    {
                        if (Password.Text != ReplyPassword.Text)
                        {
                            await DisplayAlert("Ошибка", "Не совпадают пароли, введите еще раз!", "OK");
                        }
                        else
                        {
                            await RegisterNewUser(Login.Text, Password.Text);
                            lst = new List<string>() { Login.Text, Password.Text, colortext, "false" };
                            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "account.txt");
                            File.WriteAllLines(fileName, lst);
                            Debug.WriteLine(fileName);
                            await DisplayAlert("Выполнено!", "Регистрация выполнена.", "ОK");
                        }
                    }
                }
            }


            

            

            


        }
        
        private async void Pressed_Red(object sender, EventArgs e)
        {
            Span_Color.TextColor = Color.Red;
            colortext = Color.Red.ToHex();
        }
        private async void Pressed_Black(object sender, EventArgs e)
        {
            Span_Color.TextColor = Color.Black;
            colortext = Color.Black.ToHex();
        }
        private async void Pressed_Yellow(object sender, EventArgs e)
        {
            Span_Color.TextColor = Color.Yellow;
            colortext = Color.Yellow.ToHex();
        }
        private async void Pressed_Blue(object sender, EventArgs e)
        {
            Span_Color.TextColor = Color.Blue;
            colortext = Color.Blue.ToHex();
        }
        private async void Pressed_Orange(object sender, EventArgs e)
        {
            Span_Color.TextColor = Color.Orange;
            colortext = Color.Orange.ToHex();
        }



    }
}