using System;

namespace ChatroClient.Entity
{
    internal class Message
    {
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public User Sender { get; set; }
        
        public User Recipient { get; set; }

        public string Content { get; set; }
    }
}