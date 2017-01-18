using System.Windows.Markup;

namespace ChatroClient.ChatCommands
{
    interface IChatCommand
    {
        uint ArgumentCount { get; set; }

        bool ServerInvoke { get; set; }
    }
}
