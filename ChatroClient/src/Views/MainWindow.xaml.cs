using System.Diagnostics;
using System.Windows;
using ChatroClient.ViewModels;

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
                this.Title += "DEBUGGER ATTACHED";
            }
#endif
        }
    }
}