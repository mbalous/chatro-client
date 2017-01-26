using System;

namespace ChatroClient.ChatCommands
{
    internal class SendBroadcast : ChatCommand<SendBroadcastDelegate>
    {
        public override string[] CommandAliases { get; } = null;
        public override uint ArgumentCount { get; } = 1;
        public override bool ServerInvoke { get; } = true;

        public override void Invoke(params string[] args)
        {
            this.Func.Invoke(args[0]);
        }

        public SendBroadcast(SendBroadcastDelegate func) : base(func)
        {
        }
    }
}