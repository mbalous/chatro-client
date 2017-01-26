using System;

namespace ChatroClient.ChatCommands
{
    public interface IChatCommand
    {
        string[] CommandAliases { get; }

        uint ArgumentCount { get;  }

        /// <summary>
        /// Defines wheter the command action takes part on server.
        /// Currently unused.
        /// </summary>
        bool ServerInvoke { get;  }

        void Invoke(params string[] args);
    }
}
