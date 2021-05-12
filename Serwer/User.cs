using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Serwer
{
    class User
    {

        public enum Status 
        {
            Offline = 0,
            Busy = 1,
            NotDisturb = 2,
            Away = 3,
            Online = 4,
        }
        
        public string username;
        public DateTime lastOnline;
        public Status status;
        public int lastVisible;
        public IPAddress ip;

        private List<Message> myMessages = new List<Message>();

        public User(IPAddress IP, string USERNAME, DateTime LASTONLINE, Status STATUS) 
        {
        
        }

        string Username
        {
            get { return username; }
            set { username = value; } 
        }

        DateTime LastOnline
        {
            get { return LastOnline; }
            set { LastOnline = value; }
        }

        Status UserStatus
        {
            get { return status; }
            set { status = value; }
        }

        IPAddress UserIp
        {
            get { return ip; }
            set { ip = value; }
        }

       public int whenLastVisible()
        {
            DateTime now = DateTime.Now;
            return (int) now.Subtract(lastOnline).TotalMinutes;
        }

        public void sendMessage(int contactID, int messageID, string message)
        {
            myMessages.Add(new Message(contactID, messageID, message)); 
        }

        public List<Message> getConversation(int contactID)
        {
            List<Message> thisConversation = new List<Message>();
            var items = from Message in myMessages
            where Message.contactID == contactID
            select Message;
            thisConversation = items.ToList();
            return thisConversation;
            //niedok
        }


    }
}
