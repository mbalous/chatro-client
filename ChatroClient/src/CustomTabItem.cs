using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatroClient.Annotations;

namespace ChatroClient
{
    public class CustomTabItem : INotifyPropertyChanged
    {
        private string _tabContent;

        public string TabHeader { get; set; }

        public string TabContent
        {
            get { return this._tabContent; }
            set
            {
                this._tabContent = value;
                OnPropertyChanged(nameof(this.TabContent));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}