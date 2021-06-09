using System;
using System.Collections.Generic;
using System.Text;

namespace Serwer
{
    class ServerInfo
    {
        private string _ServerName = "";
        private string _MyUsername = "";
        private User.Status _Status = User.Status.Online;
        public string ServerName
        {
            get
            {
                return _ServerName;
            }

            set
            {
                _ServerName = value;
            }
        }

        public string MyUsername
        {
            get
            {
                return _MyUsername;
            }

            set
            {
                _MyUsername = value;
            }
        }

        public User.Status Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }
    }
}
