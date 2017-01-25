using System;
using ChatroClient.Views;

namespace ChatroClient.ChatCommands
{
    internal class Login : ChatCommand<LoginDelegate>
    {
        public override string[] CommandAliases { get; set; } = {"login"};
        public override uint ArgumentCount { get; set; } = 2;
        public override bool ServerInvoke { get; set; } = true;

        public override void Invoke(params string[] args)
        {
            LoginResult x = this.Func.Invoke(args[0], args[1]);
            OnLoginCompleted(x);
        }

        public Login(LoginDelegate func) : base(func)
        {
        }

        public event EventHandler<LoginResult> LoginCompleted;

        protected virtual void OnLoginCompleted(LoginResult e)
        {
            this.LoginCompleted?.Invoke(this, e);
        }
    }
}
