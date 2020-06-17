using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serwer
{
    class Message
    {
        //message content
        private string content
        {
            get;
            set;
        }

        public string GetContent()
        {
            return content;
        }

        //weho sent message
        private string sender
        {
            get;
            set;
        }

        //to whom it was sent
        private string recipient
        {
            get;
            set;
        }

        //size of message
        private int size
        {
            get;
            set;
        }

        public Message(string sender, string recipient, string content)
        {
            this.sender = sender;
            this.recipient = recipient;
            this.content = content;
            size = content.Length;
        }

        public Message()
        {

        }

        public override string ToString()
        {
            return "message = { sender:" + sender + " ; content:\"" + content + "\" ; size:" + size + "B }\n";
        }




    }
}
