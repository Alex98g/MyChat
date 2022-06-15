using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SimpleChatApp.CommonTypes;
using SimpleChatApp.GrpcService;
using System;
using System.Threading.Tasks;
using static SimpleChatApp.GrpcService.ChatService;

namespace MyChat
{
    public enum ServerStatus
    {
        SUCCESS,
        ERROR_LOGIN_BAD_LOGIN,
        ERROR_REG_LOGIN_EXISTS,
        ERROR_REG_BAD_LOGIN,
        ERROR_UNKNOWN,
    }
    public class MessageChat
    {
        public MessageChat(string userName, string message)
        {
            UserLogin = userName;
            Text = message;
        }

        public string UserLogin { get; private set; }
        public string Text { get; private set; }
    }

    public delegate void OnMessageReceived(MessageChat message);

    internal class Client
    {
        private ChatServiceClient chatServiceClient;
        private string guid = "";
        public static Client Instance { get { return instance; } }
        private static readonly Client instance = new Client();

        public void Initialize()
        {
            chatServiceClient = new ChatServiceClient(new Channel("Localhost", 30051, ChannelCredentials.Insecure));
        }

        public async Task<ServerStatus> Login(string login, string password)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;
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
            switch (ans.Status)
            {
                case SimpleChatApp.GrpcService.AuthorizationStatus.AuthorizationSuccessfull:
                    guid = ans.Sid.Guid_;
                    outStatus = ServerStatus.SUCCESS;
                    break;
                case SimpleChatApp.GrpcService.AuthorizationStatus.WrongLoginOrPassword:
                    outStatus = ServerStatus.ERROR_LOGIN_BAD_LOGIN;
                    break;
                default:
                    break;
            }

            return outStatus;
        }
        public async Task<ServerStatus> RegisterNewUser(string login, string password)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;
            var userData = new UserData()
            {
                Login = login,
                PasswordHash = SHA256.GetStringHash(password)
            };
            var ans = await chatServiceClient.RegisterNewUserAsync(userData);
            
            switch (ans.Status)
            {
                case SimpleChatApp.GrpcService.RegistrationStatus.RegistrationSuccessfull:
                    outStatus = ServerStatus.SUCCESS;
                    break;
                case SimpleChatApp.GrpcService.RegistrationStatus.LoginAlreadyExist:
                    outStatus = ServerStatus.ERROR_REG_LOGIN_EXISTS;
                    break;
                case SimpleChatApp.GrpcService.RegistrationStatus.BadInput:
                    outStatus = ServerStatus.ERROR_REG_BAD_LOGIN;
                    break;
                default:
                    break;
            }
            return outStatus;


        }
        public async Task Subscribe(OnMessageReceived OnMessageReceived)
        {
            var timeIntervalRequest = new TimeIntervalRequest()
            {
                StartTime = Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
                EndTime = Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime()),
                Sid = new SimpleChatApp.GrpcService.Guid() { Guid_ = guid }
            };
            var logsmessages = await chatServiceClient.GetLogsAsync(timeIntervalRequest);
            
            foreach (SimpleChatApp.GrpcService.MessageData message in logsmessages.Logs)
            {
                var messageData = message.Convert();
                OnMessageReceived(new MessageChat(messageData.UserLogin, messageData.Text));
            }

            var streamingCall = chatServiceClient.Subscribe(new SimpleChatApp.GrpcService.Guid() { Guid_ = guid }).ResponseStream;


            while (await streamingCall.MoveNext())
            {
                if (streamingCall.Current.ActionStatus == SimpleChatApp.GrpcService.ActionStatus.Allowed)
                {
                    foreach (SimpleChatApp.GrpcService.MessageData message in streamingCall.Current.Logs)
                    {
                        var messageData = message.Convert();
                        OnMessageReceived(new MessageChat(messageData.UserLogin, messageData.Text));
                    }
                }
            }
        }
        public async void Unsubscribe()
        {
            await chatServiceClient.UnsubscribeAsync(new SimpleChatApp.GrpcService.Guid() { Guid_ = guid });
        }
        
        public async Task<ServerStatus> SendMessage(string message)
        {
            ServerStatus outStatus = ServerStatus.ERROR_UNKNOWN;
            var outgoingMessage = new OutgoingMessage()
            {
                Sid = new SimpleChatApp.GrpcService.Guid() { Guid_ = guid },
                Text = message
            };
            var ans = await chatServiceClient.WriteAsync(outgoingMessage);
            switch (ans.ActionStatus)
            {
                case SimpleChatApp.GrpcService.ActionStatus.Allowed:
                    outStatus = ServerStatus.SUCCESS;
                    break;
                default:
                    break;
            }

            return outStatus;
        }




    }
}
