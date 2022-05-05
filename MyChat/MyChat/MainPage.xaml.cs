using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SimpleChatApp.CommonTypes;
using SimpleChatApp.GrpcService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SimpleChatApp.GrpcService.ChatService;

namespace MyChat
{
    public partial class MainPage : ContentPage
    {
        private ChatServiceClient chatServiceClient;
        private SimpleChatApp.GrpcService.Guid guid;
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "account.txt");
        //C:\Users\Your User\AppData\Local\Packages\15738FE9-45B8-4B14-B1DD-AF08133F570B_pf1d5chqd35vc\LocalState\account.txt
        List<string> lst;
        private ObservableCollection<SimpleChatApp.CommonTypes.MessageData> chat = new ObservableCollection<SimpleChatApp.CommonTypes.MessageData> ();
        


        public MainPage() 
        {
            InitializeComponent();
            lst = File.ReadAllLines(fileName).ToList();
            chatServiceClient = new ChatServiceClient(new Channel("Localhost", 30051, ChannelCredentials.Insecure));
            MessagesList.ItemsSource = chat;
            Login_Pressed();
            Resources["LoginColor"] = Color.FromHex(lst[2]);
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
        private async Task<SimpleChatApp.GrpcService.Guid> Login(string login, string password)
        {
            
            var userData = new UserData()
            {
                Login = login,
                PasswordHash = SHA256.GetStringHash(password)
            };
            var authorizationData = new AuthorizationData()
            {
                ClearActiveConnection = true,
                UserData = userData
            };
            var ans = await chatServiceClient.LogInAsync(authorizationData);
            return ans.Sid;
        }
        private async Task<List<SimpleChatApp.GrpcService.MessageData>> GetMessages()
        {
            var now = DateTime.MaxValue;
            var then = DateTime.MinValue;
            var timeIntervalRequest = new TimeIntervalRequest()
            {
                StartTime = Timestamp.FromDateTime(then.ToUniversalTime()),
                EndTime = Timestamp.FromDateTime(now.ToUniversalTime()),
                Sid = guid
            };
            var messages = await chatServiceClient.GetLogsAsync(timeIntervalRequest);
            return messages.Logs.ToList();

        }
        private async Task Subscribe(Action<SimpleChatApp.GrpcService.MessageData> onMessage)
        {
            var streamingCall = chatServiceClient.Subscribe(guid);
            while (await streamingCall.ResponseStream.MoveNext())
            {
                var messages = streamingCall.ResponseStream.Current;
                foreach (var message in messages.Logs)
                {
                    onMessage(message);
                }
            }
        }
        private async Task SendMessage(string message)
        {
            var outgoingMessage = new OutgoingMessage()
            {
                Sid = guid,
                Text = message
            };
            var ans = await chatServiceClient.WriteAsync(outgoingMessage);
            Debug.WriteLine(ans);
        }
        public async void Login_Pressed()
        {
            guid = await Login(lst[0], lst[1]);
            var messages = await GetMessages();
            foreach(var message in messages)
            {
                AddMessage(message);
            }
            await Subscribe(AddMessage);
            
        }
        private async void Send_Pressed(object sender, EventArgs e)
        {
            await SendMessage(MessageEntry.Text);
        }

        private void AddMessage(SimpleChatApp.GrpcService.MessageData md)
        {
            
            chat.Add(md.Convert());
        }
        private async void Exit_Pressed(object sender, EventArgs e)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "account.txt");
            lst.RemoveAt(3);
            lst.Add("false");
            File.WriteAllLines(fileName, lst);
            await Navigation.PushModalAsync(new FirstPage());

        }
    }
}
