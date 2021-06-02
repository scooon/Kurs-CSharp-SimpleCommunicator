using System;
using System.Collections.Generic;
using System.Text;

namespace Serwer
{
    public class ServerInfo
    {
        private string _ServerName = "";
        private string _MyUsername = "";
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
    }
}
