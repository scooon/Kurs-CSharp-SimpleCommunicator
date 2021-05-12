using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;

namespace Serwer
{
    public class Server : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private List<User> users = new List<User>();

        public HttpListener listener;
        public  string url = "http://*:8000/";
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
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, Dns.GetHostName(), ctx.Request.RemoteEndPoint.Address.ToString(), req.UserHostName, getusername() ,disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                resp.KeepAlive = false;

                List<User> foundUsers = (from User in users
                            where User.UserIP == ctx.Request.RemoteEndPoint.Address
                            select User).ToList();
                int thisUserID = foundUsers[0].ContactID;


                users.Add(new User(ctx.Request.RemoteEndPoint.Address, "test", DateTime.Now, User.Status.Online));

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

            return Name.Substring(Name.IndexOf("\\")+1);
        }

    }
}
