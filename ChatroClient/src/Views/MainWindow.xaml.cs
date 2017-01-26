using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatroClient.ChatCommands;
using ChatroClient.Entity;

namespace ChatroClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);

#if DEBUG
            if (Debugger.IsAttached)
            {
                this.Title += " DEBUGGER ATTACHED";
            }
#endif
        }

        public class MainWindowViewModel
        {
            public ObservableCollection<CustomTabItem> Tabs { get; set; }

            private readonly List<IChatCommand> _chatCommands = new List<IChatCommand>();

            public SignalRController SignalRController { get; set; }

            public ICommand SubmitInput
            {
                get
                {
                    return new ActionCommand(delegate (object o)
                    {
                        TextBox textBox = (TextBox)o;
                        NewInput(textBox.Text);
                        textBox.Clear();
                    });
                }
            }

            private readonly MainWindow _window;
            private readonly CustomTabItem _broadcastTab;

            public MainWindowViewModel(MainWindow window)
            {
                this._window = window;
                InitChat();

                this._broadcastTab = new CustomTabItem
                {
                    // Broadcast tab
                    TabHeader = "All"
                };
                ObservableCollection<CustomTabItem> observableCollection = new ObservableCollection<CustomTabItem>
                {
                    this._broadcastTab
                };
                this.Tabs = observableCollection;
                this._window.TabControlChatWindows.SelectedIndex = 0;
            }

            private void InitChat()
            {
                this.SignalRController = new SignalRController();
                this.SignalRController.ConnectionSuccessful += (sender, args) =>
                {
                    Login loginCommand = new Login(this.SignalRController.LoginPointer);
                    loginCommand.LoginCompleted += LoginCommandOnLoginCompleted;

                    SendBroadcast sendBroadcast = new SendBroadcast(this.SignalRController.SendBroadcastPointer);

                    ChatCommands.SendPrivateMessage sendPrivateMessage = new ChatCommands.SendPrivateMessage(this.SignalRController.SendPrivateMessagePointer);

                    RegisterCommands(loginCommand, sendBroadcast, sendPrivateMessage);
                };


                this.SignalRController.NewMessageHandler += SignalRControllerOnNewMessageHandler;
                this.SignalRController.NewBroadcastHandler += SignalRControllerOnNewBroadcastHandler;
            }

            private void InvokeOnUiThread(Action action)
            {
                this._window.Dispatcher.Invoke(action);
            }

            private void LoginCommandOnLoginCompleted(object sender, LoginResult loginResult)
            {
                // TODO: Complete login implementation
                if (loginResult == LoginResult.Success)
                {
                    Debug.WriteLine("Login completed succesfuly.", "information");
                }
                else
                {
                    Debug.WriteLine("Login failed.", "information");
                }
            }

            private void SignalRControllerOnNewBroadcastHandler(string content, User user)
            {
                // Do we have a tab with a sender open?
                string tabContent = $"{DateTime.Now.ToShortTimeString()} - {user.Username}: {content} {Environment.NewLine}";
                this._broadcastTab.TabContent += tabContent;
            }

            private void SignalRControllerOnNewMessageHandler(string content, User user)
            {
                // Do we have a tab with a sender open?
                string tabContent = $"{DateTime.Now.ToShortTimeString()} - {user.Username}: {content} {Environment.NewLine}";
                CustomTabItem currentTab = this.Tabs.FirstOrDefault(item => item.UserGuid == user.Guid);
                if (currentTab == null)
                {
                    currentTab = new CustomTabItem()
                    {
                        TabContent = tabContent,
                        TabHeader = user.Username,
                        UserGuid = user.Guid
                    };
                    InvokeOnUiThread(() => this.Tabs.Add(currentTab));
                }
                else
                {
                    currentTab.TabContent += tabContent;
                }
            }

            internal void NewInput(string input)
            {
                input = input.Trim();
                if (input.Length == 0)
                {
                    return;
                }
                if (input[0] == '/')
                {
                    if (input.Length < 2)
                    {
                        return;
                    }

                    IEnumerable<string> args = input.Split(' ').Skip(1);
                    string command = new string(input.Skip(1).TakeWhile(c => c != ' ').ToArray());
                    IChatCommand cmd = null;

                    foreach (IChatCommand chatCommand in this._chatCommands.Where(chatCommand => chatCommand.CommandAliases != null))
                    {
                        if (chatCommand.CommandAliases.Any(s => string.Equals(s, command, StringComparison.OrdinalIgnoreCase)))
                        {
                            cmd = chatCommand;
                            break;
                        }
                    }
                    if (cmd != null)
                    {
                        cmd.Invoke(args.ToArray());
                    }
                    else
                    {
                        // Invalid command...
                    }
                }
                else
                {
                    IChatCommand broadCast = this._chatCommands.FirstOrDefault(command => command.CommandAliases == null);
                    broadCast?.Invoke(input);
                }
            }

            private void RegisterCommands(params IChatCommand[] commands)
            {
                foreach (IChatCommand cmd in commands)
                {
                    this._chatCommands.Add(cmd);
                }
            }
        }
    }
}