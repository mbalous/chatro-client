using System;
using System.Collections.Generic;
using ChatroClient.ChatCommands;

namespace ChatroClient
{
    internal class ChatController
    {
        private readonly List<IChatCommand> _chatCommands = new List<IChatCommand>();

        public ChatController()
        {
            RegisterCommand(new ChangeName());
        }

        private void RegisterCommand(IChatCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }
            if (this._chatCommands.Contains(cmd))
            {
                throw new ArgumentException($"Command {cmd.GetType().Name} already registered.");
            }
            this._chatCommands.Add(cmd);
        }
    }
}
