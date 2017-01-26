using System;

namespace ChatroClient.ChatCommands
{
    internal class SendPrivateMessage : ChatCommand<ChatroClient.SendPrivateMessage>
    {
        public override string[] CommandAliases { get; } = {"msg"};
        public override uint ArgumentCount { get; } = 2;
        public override bool ServerInvoke { get; } = true;

        public override void Invoke(params string[] args)
        {
            this.Func.Invoke(args[1], args[0]);
        }

        public SendPrivateMessage(ChatroClient.SendPrivateMessage func) : base(func)
        {
        }
    }
}