using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Serwer
{
    public class Server : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private ServerInfo Info = new ServerInfo();

        private List<User> users = new List<User>();

        public HttpListener listener;
        private int _port;
        public string url = "http://*:8000/";
        private int pageViews = 0;
        public int requestCount = 0;
        public string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>Komunikator Lan</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <p>Server Hostname: {1}</p>" +
            "    <p>Client IP: {2}</p>" +
            "    <p>Client Hostname: {3}</p>" +
            "    <p>Server Name: {4}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public Server(int port)
        {
            url = "http://*:" + port + "/";
        }

        public int Port
        {
            get
            {
                return _port;
            }
        }

        public async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                // Oczekiwanie na połączenie
                HttpListenerContext ctx = await listener.GetContextAsync();


                // obiekty request i response
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Info o połączeniu w konsoli
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Zarządano Wyłączenia");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    Views += 1;

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                //byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, Dns.GetHostName(), ctx.Request.RemoteEndPoint.Address.ToString(), req.UserHostName, getusername() ,disableSubmit));




                int thisUserIndex = -1;

                try
                {

                    User thisUser = users.First(i => i.UserIP.ToString() == ctx.Request.RemoteEndPoint.Address.ToString());
                    thisUserIndex = users.IndexOf(thisUser);
                    thisUser.LastOnline = DateTime.Now;
                    thisUser.UserStatus = User.Status.NotDisturb;
                    thisUser.Username = "test2";
                    users[thisUserIndex] = thisUser;
                }
                catch
                {
                    if (thisUserIndex == -1)
                    {
                        Random rand = new Random();
                        string newUsername = "User" + rand.Next(0, 1000);

                        while (users.Count(i => i.Username == newUsername) != 0)
                        {
                            newUsername = "User" + rand.Next(0, 1000);
                        }

                        users.Add(new User(ctx.Request.RemoteEndPoint.Address, newUsername, DateTime.Now, User.Status.Online));
                        User thisUser = users.First(i => i.UserIP.ToString() == ctx.Request.RemoteEndPoint.Address.ToString());
                        thisUserIndex = users.IndexOf(thisUser);
                    }
                }


                Info.MyUsername = users[thisUserIndex].Username;
                Info.ServerName = getusername();
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Info));
                resp.ContentType = "application/json";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                resp.KeepAlive = false;


                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);



                resp.Close();
            }
        }

        public int Views
        {
            get { return pageViews; }
            set
            {
                if (pageViews != value)
                {
                    SendPropertyChanging("Name");
                    pageViews = value;

                    //clientUpdate();

                    SendPropertyChanged("Name");
                }



            }
        }

        void SendPropertyChanging(string property)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(property));
            }
        }
        void SendPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public async Task clientUpdate()
        {

        }

        private string getusername()
        {
            System.Security.Principal.WindowsIdentity identity = WindowsIdentity.GetCurrent();

            string Name = identity.Name;

            return Name.Substring(Name.IndexOf("\\") + 1);
        }

    }
}
