using System;

namespace ChatroClient.ChatCommands
{
    internal class SendBroadcast : ChatCommand<SendBroadcastDelegate>
    {
        public override string[] CommandAliases { get; set; } = {};
        public override uint ArgumentCount { get; set; } = uint.MaxValue;
        public override bool ServerInvoke { get; set; } = true;

        public override void Invoke(params string[] args)
        {
            this.Func.Invoke(args[0]);
        }

        public SendBroadcast(SendBroadcastDelegate func) : base(func)
        {
        }
    }
}