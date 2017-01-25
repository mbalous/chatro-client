using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            public ICommand SubmitInput
            {
                get
                {
                    return new ActionCommand(delegate(object o)
                    {
                        TextBox textBox = (TextBox) o;
                        SumbitInput((textBox.Text));
                        textBox.Clear();
                    });
                }
            }

            private readonly MainWindow _window;
            private readonly ChatController _chatController;

            public MainWindowViewModel(MainWindow window)
            {
                this._window = window;
                this._chatController = new ChatController();

                
                ObservableCollection<CustomTabItem> observableCollection = new ObservableCollection<CustomTabItem>
                {
                    new CustomTabItem
                    {
                        // Broadcast tab
                        TabHeader = "All"
                    }
                };
                this.Tabs = observableCollection;
                this._window.TabControlChatWindows.SelectedIndex = 0;
            }

            private void ReceiveMessage(string content, string sender)
            {
                // Do we have a tab with a sender open?
                CustomTabItem tabItem = this.Tabs.FirstOrDefault(item => item.TabHeader == sender);
                CustomTabItem targetTab = tabItem;
                if (tabItem == null)
                {
                    targetTab = new CustomTabItem() {TabHeader = sender};
                    InvokeOnUiThread(() => this.Tabs.Add(targetTab));
                }
                targetTab.TabContent +=
                    $"{DateTime.Now.ToShortTimeString()} - {sender}: {content} {Environment.NewLine}";
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

            private void SumbitInput(string text)
            {
                this._chatController.NewInput(text);
            }
        }
    }
}