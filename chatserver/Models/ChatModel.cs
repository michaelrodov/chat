using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chatserver.Models
{
    public class ChatModel
    {
        private List<MESSAGE> messages = new List<MESSAGE>();
        public String groupName { get; set; }

        public void addMessages(List<MESSAGE> messageList)
        {
            messages.AddRange(messageList);
        }

        public List<MESSAGE> getMessages()
        {
            return this.messages;
        }

        public void addMessage(MESSAGE message)
        {
            messages.Add(message);
        }


    }
}