using System;

namespace ChatroClient.ChatCommands
{
    internal abstract class ChatCommand<T> : IChatCommand
    {
        public abstract string[] CommandAliases { get;  }
        public abstract uint ArgumentCount { get;  }
        public abstract bool ServerInvoke { get; }

        public abstract void Invoke(params string[] args);

        protected readonly T Func;

        protected ChatCommand(T func)
        {
            this.Func = func;
        }
    }
}