namespace ChatroClient.ChatCommands
{
    public interface IChatCommand
    {
        string[] CommandAliases { get; set; }

        uint ArgumentCount { get; set; }

        bool ServerInvoke { get; set; }

        void Invoke(params string[] args);
    }
}
