using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.AspNet.SignalR.Client;

namespace ChatroClient.ViewModels
{
    public class MainWindowViewModel
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const string HUB_URL = "https://localhost:44397/";

        private readonly Views.MainWindow _window;
        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;

        public ObservableCollection<ChatroTabItem> Tabs { get; set; }

        public ICommand SubmitInput
        {
            get { return new ActionCommand(o => SumbitInput((TextBox) o)); }
        }


        public MainWindowViewModel(Views.MainWindow window)
        {
            this._window = window;
            this._window.Loaded +=
                delegate(object sender, RoutedEventArgs args)
                {
                    this.Tabs.First().TabContent += @"Use command /name ""nickname"" to change your nickname." +
                                                    Environment.NewLine;
                };
            ObservableCollection<ChatroTabItem> observableCollection = new ObservableCollection<ChatroTabItem>
            {
                new ChatroTabItem
                {
                    TabHeader = "All"
                }
            };
            this.Tabs = observableCollection;
            this._window.TabControlChatWindows.SelectedIndex = 0;
            InitSignalR();
        }


        private void InitSignalR()
        {
            this._hubConnection = new HubConnection(HUB_URL);
            this._hubProxy = this._hubConnection.CreateHubProxy("ChatHub");
#if DEBUG
            Debug.Write($"Opening connection to url {this._hubConnection.Url}...");
#endif
            this._hubConnection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
#if DEBUG
                        Debug.WriteLine("Connection failed...");
#endif
                        if (task.Exception != null)
                        {
                            Debug.WriteLine(task.Exception.Message);
                        }
                        InvokeOnUiThread(() => { throw new Exception("Connection failed."); });
                    }
                    else
                    {
#if DEBUG
                        Debug.WriteLine("Connection successful...");
#endif
                    }
                });
            this._hubProxy.On("NewBroadcast", delegate(dynamic message, dynamic userName)
                {
                    ReceiveBroadcast(message, userName);
                });

            this._hubProxy.On("NewMessage", delegate (dynamic message, dynamic userName)
            {
                ReceiveMessage(message, userName);
            });
        }

        private void ReceiveMessage(string message, string userName)
        {
            ChatroTabItem tabItem =  this.Tabs.FirstOrDefault(item => item.TabHeader == userName);
            ChatroTabItem targetTab = tabItem;
            if (tabItem == null)
            {
                targetTab = new ChatroTabItem() { TabHeader = userName };
                InvokeOnUiThread(() => this.Tabs.Add(targetTab));
            }
            targetTab.TabContent +=
               $"{DateTime.Now.ToShortTimeString()} - {userName}: {message} {Environment.NewLine}";
        }

        private void ReceiveBroadcast(string message, string userName)
        {
            this.Tabs.First().TabContent +=
               $"{DateTime.Now.ToShortTimeString()} - {userName}: {message} {Environment.NewLine}";
        }
        

        private void InvokeOnUiThread(Action action)
        {
            this._window.Dispatcher.Invoke(action);
        }

        private void SumbitInput(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                return;
            }
            string input = textBox.Text.Trim();
            if (input[0].Equals('/'))
            {
                // is command
                string[] inputSplit = input.Split(' ');
                string command = inputSplit[0].Substring(1);
                
                switch (command)
                {
                    case "name":
                        if (inputSplit.Length != 2)
                        {
                            // Incorent syntax, more than 1 argument entered...
                        }
                        else
                        {
                            this._hubProxy.Invoke("SetUsername", inputSplit[1]);
                        }

                        break;
                    case "msg":
                        if (inputSplit.Length != 3)
                        {
                            // Incorent syntax, more than 2 argument entered...
                        }
                        else
                        {
                            this._hubProxy.Invoke("SendMessage", inputSplit[2], inputSplit[1]);
                        }
                        break;
                    case "login":
                        if (inputSplit.Length != 3)
                        {
                            // Incorent syntax, more than 2 argument entered...
                        }
                        else
                        {
                            this._hubProxy.Invoke<LoginResult>("Login", inputSplit[1], inputSplit[2]).ContinueWith(
                                    task =>
                                    {
                                        if (task.Result == LoginResult.Success)
                                        {
                                            this.Tabs.First().TabContent +=
               $"{DateTime.Now.ToShortTimeString()}: Login successfull.";

                                        }
                                        else
                                        {
                                            this.Tabs.First().TabContent +=
               $"{DateTime.Now.ToShortTimeString()}: Login failed.";
                                        }
                                    });
                        }
                        break;

                    default:
                        // TODO: Command not recognized
                        this.Tabs.First().TabContent += "Command not recognized" + Environment.NewLine;
                        break;
                }
            }
            else
            {
                this._hubProxy.Invoke("SendBroadcast", textBox.Text);
            }
            textBox.Clear();
        }
    }
    public enum LoginResult
    {
        Success,
        Failure
    }
}