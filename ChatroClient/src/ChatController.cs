using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ChatroClient.ChatCommands;
using ChatroClient.Views;

namespace ChatroClient
{
    internal class ChatController
    {
        private readonly List<IChatCommand> _chatCommands = new List<IChatCommand>();

        public SignalRController SignalRController { get; set; }

        internal ChatController()
        {
            this.SignalRController = new SignalRController();
            this.SignalRController.ConnectionSuccessful += (sender, args) =>
            {
                Login loginCommand = new Login(this.SignalRController.LoginPointer);
                loginCommand.LoginCompleted += LoginCommandOnLoginCompleted;
                RegisterCommands(loginCommand);
            };


            this.SignalRController.NewMessageHandler += SignalRControllerOnNewMessageHandler;
            this.SignalRController.NewBroadcastHandler += SignalRControllerOnNewBroadcastHandler;
        }

        private void LoginCommandOnLoginCompleted(object sender, LoginResult loginResult)
        {
            // TODO: Complete login implementation
            if (loginResult == LoginResult.Success)
            {
                Debug.WriteLine("Login completed succesfuly.", "information");
            }
            else
            {
                Debug.WriteLine("Login failed.", "information");
            }
        }

        private void SignalRControllerOnNewBroadcastHandler(string message, string sender)
        {
        }

        private void SignalRControllerOnNewMessageHandler(string message, string sender)
        {
        }

        internal void NewInput(string input)
        {
            input = input.Trim();
            if (input[0] == '/')
            {
                if (input.Length < 2)
                {
                    return;
                }

                var args = input.Split(' ').Skip(1);
                var command = new string(input.Skip(1).TakeWhile(c => c != ' ').ToArray());
                IChatCommand cmd = null;

                foreach (IChatCommand chatCommand in this._chatCommands)
                {
                    if (
                        chatCommand.CommandAliases.Any(
                                       s => string.Equals(s, command, StringComparison.OrdinalIgnoreCase)))
                    {
                        cmd = chatCommand;
                        break;
                    }
                }
                cmd.Invoke(args.ToArray());
            }
        }

        private void RegisterCommands(params IChatCommand[] commands)
        {
            foreach (IChatCommand cmd in commands)
            {
                this._chatCommands.Add(cmd);
            }
        }
    }
}