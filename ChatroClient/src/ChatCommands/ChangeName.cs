using System;

namespace ChatroClient.ChatCommands
{
    internal class ChangeName : ChatCommand<ChatroClient.ChangeName>
    {
        public override string[] CommandAliases { get; } = {"name", "changename"};
        public override uint ArgumentCount { get; } = 1;
        public override bool ServerInvoke { get; } = true;

        public override void Invoke(params string[] args)
        {
            this.Func.Invoke(args[0]);
        }

        public ChangeName(ChatroClient.ChangeName func) : base(func)
        {
        }
    }
}