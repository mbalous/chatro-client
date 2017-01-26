using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChatroClient.Entity;
using ChatroClient.Views;
using Microsoft.AspNet.SignalR.Client;

namespace ChatroClient
{
    public delegate LoginResult LoginDelegate(string username, string password);

    public delegate void SendBroadcastDelegate(string content);

    public delegate void SendPrivateMessage(string content, string recipient);

    public delegate void ChangeName(string name);


    public class SignalRController : IDisposable
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const string HUB_URL = "https://localhost:44397/";

        private readonly HubConnection _hubConnection;
        private readonly IHubProxy _hubProxy;

        public event EventHandler ConnectionSuccessful;
        public event EventHandler ConnectionUnsuccessful;

        public SignalRController()
        {
            this._hubConnection = new HubConnection(HUB_URL);

            this._hubProxy = this._hubConnection.CreateHubProxy("ChatHub");
            Initialize();
        }

        private async void Initialize()
        {
            this.LoginPointer = LoginInternal;
            this.SendBroadcastPointer = SendBroadcastInternal;
            this.SendPrivateMessagePointer = SendPrivateMessageInternal;
            this.ChangeNamePointer = ChangeNameInternal;

            this._hubProxy.On("NewMessage",
                    (string content, User sender) => this.NewMessageHandler?.Invoke(content, sender));

            this._hubProxy.On("NewBroadcast",
                    (string content, User sender) => this.NewBroadcastHandler?.Invoke(content, sender));

            this._hubProxy.On("NewServerEvent", 
                    (string content) => this.NewServerEvent?.Invoke(content));


            Debug.Write($"Opening connection to url {this._hubConnection.Url}...");
            await this._hubConnection.Start().ContinueWith(task =>
                      {
                          if (task.IsFaulted)
                          {
                              Debug.WriteLine("Connection failed...", "error");
                              if (task.Exception != null)
                              {
                                  Debug.WriteLine(task.Exception.Message, "exception");
                              }
                              this.ConnectionUnsuccessful?.Invoke(this, EventArgs.Empty);
                              throw new Exception("Connection failed.");
                          }
                          Debug.WriteLine("Connection successful...", "information");
                          this.ConnectionSuccessful?.Invoke(this, EventArgs.Empty);
                      });
        }

        public LoginDelegate LoginPointer;

        private LoginResult LoginInternal(string username, string password)
        {
            LoginResult taskResult = this._hubProxy.Invoke<LoginResult>("Login", username, password).Result;
            return taskResult;
        }

        public SendBroadcastDelegate SendBroadcastPointer;

        private void SendBroadcastInternal(string content)
        {
            this._hubProxy.Invoke("SendBroadcast", content);
        }

        public SendPrivateMessage SendPrivateMessagePointer;

        private void SendPrivateMessageInternal(string content, string recipient)
        {
            this._hubProxy.Invoke("SendMessage", content, recipient);
        }
        
        public ChangeName ChangeNamePointer;

        private void ChangeNameInternal(string newName)
        {
            throw new NotImplementedException();
            this._hubProxy.Invoke("",newName);
        }

        #region Callbacks

        public delegate void NewMessage(string content, User sender);

        public event NewMessage NewMessageHandler;

        public delegate void NewBroadcast(string content, User sender);

        public event NewBroadcast NewBroadcastHandler;

        public delegate void ServerEvent(string content);

        public event ServerEvent NewServerEvent;

        #endregion

        public void Dispose() => this._hubConnection.Dispose();
    }
}
