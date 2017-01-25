using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChatroClient.Views;
using Microsoft.AspNet.SignalR.Client;

namespace ChatroClient
{
    internal class SignalRController : IDisposable
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

            this._hubProxy.On("NewMessage",
                    (string content, string sender) => this.NewMessageHandler?.Invoke(content, sender));

            this._hubProxy.On("NewBroadcast",
                    (string content, string sender) => this.NewBroadcastHandler?.Invoke(content, sender));


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

        private void SendBroadcastInternal(string content)
        {
            this._hubProxy.Invoke("NewBroadcast", content);
        }

        private LoginResult LoginInternal(string username, string password)
        {
            LoginResult taskResult = this._hubProxy.Invoke<LoginResult>("Login", username, password).Result;
            return taskResult;
        }

        public LoginDelegate LoginPointer;
        public SendBroadcastDelegate SendBroadcastPointer;

        #region Callbacks

        internal delegate void NewMessage(string content, string sender);

        public event NewMessage NewMessageHandler;

        internal delegate void NewBroadcast(string content, string sender);

        public event NewBroadcast NewBroadcastHandler;

        #endregion

        public void Dispose() => this._hubConnection.Dispose();
    }

    internal delegate LoginResult LoginDelegate(string username, string password);

    internal delegate void SendBroadcastDelegate(string content);
}
