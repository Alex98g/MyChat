using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

namespace MyChat
{
    public class MessageListEntry
    {
        public MessageListEntry(MessageChat message, Color color)
        {
            UserLogin = message.UserLogin;
            Text = message.Text;
            LoginColor = color;
        }
        public string UserLogin { get; private set; }
        public string Text { get; private set; }
        public Color LoginColor { get; private set; }
    }

    public partial class MainPage : ContentPage
    {
        private ObservableCollection<MessageListEntry> chat = new ObservableCollection<MessageListEntry>();
        private Dictionary<string, Color> ColorDictionary = new Dictionary<string, Color>();
        private Random random = new Random();

        public MainPage()
        {
            InitializeComponent();
            MessagesList.ItemsSource = chat;
            Client.Instance.Subscribe(OnMessageReceived);
    
        }
        protected override void OnDisappearing()
        {
            Client.Instance.Unsubscribe();
            base.OnDisappearing();
        }

        private async void ReqSend(string message)
        {
            ServerStatus result = await Client.Instance.SendMessage(message);

            switch (result)
            {
                case ServerStatus.ERROR_UNKNOWN:
                    await DisplayAlert("Ошибка", "Отправка не удалась!", "OK");
                    break;
                default:
                    break;
            }
        }

        private void OnMessageReceived(MessageChat message)
        {
            Color color = Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
            if (ColorDictionary.ContainsKey(message.UserLogin))
            {
                color = ColorDictionary[message.UserLogin];
            }
            else
            {
                ColorDictionary[message.UserLogin] = color;
            }
            chat.Insert(0, new MessageListEntry(message, color));
        }
        public void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (MessageEntry.Text == "")
            {
                BtnSendMessage.IsEnabled = false;
            }
            else
            {
                BtnSendMessage.IsEnabled = true;
            }

        }
        private void Send_Pressed(object sender, EventArgs e)
        {
            ReqSend(MessageEntry.Text);
            MessageEntry.Text = string.Empty;
        }

        
        private async void Exit_Pressed(object sender, EventArgs e)
        {

            await Navigation.PushModalAsync(new FirstPage());

        }
    }
}
