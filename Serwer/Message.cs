using System;
using System.Collections.Generic;
using System.Text;

namespace Serwer
{
    class Message
    {
        public string message;
        public DateTime date;
        public int contactID;
        public int messageID;
        public bool readed = false;

        string messageContent
        {
            get { return messageContent; }
            set { messageContent = value; }
        }

        DateTime Time
        {
            get { return date; }
        }

        public Message(int userID, int msgID, string messageContent)
        {
            messageID = msgID;
            contactID = userID;
            message = messageContent;
            date = DateTime.Now;
        }

        int ContactID
        {
            get { return contactID; }
            set { contactID = value; }
        }

        bool Readed
        {
            get { return readed; }
            set { readed = value; }
        }
    
    }
}
