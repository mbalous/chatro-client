namespace ChatroClient.ChatCommands
{
    internal abstract class ChatCommand<T> : IChatCommand
    {
        public abstract string[] CommandAliases { get; set; }
        public abstract uint ArgumentCount { get; set; }
        public abstract bool ServerInvoke { get; set; }

        public abstract void Invoke(params string[] args);

        protected readonly T Func;

        protected ChatCommand(T func)
        {
            this.Func = func;
        }
    }
}