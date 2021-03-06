using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using NetFwTypeLib;
using System.Security.Principal;
using System.ComponentModel;
using System.Drawing;

namespace Serwer
{
    public partial class MainWindow : Form
    {

        Server server = new Server(8000);

        private bool mouseDown;
        private Point lastLocation;


        public MainWindow()
        {
            InitializeComponent();


            try
            {
                addFirewallRule(server.Port);
                addFirewallRule(5000);

            }
            catch 
            {
                MessageBox.Show("Uruchom jako Administrator !");
            }


            
            // API
            server.listener = new System.Net.HttpListener();
            server.listener.Prefixes.Add(server.url);
            server.listener.Start();

            Console.WriteLine("Nasłuchiwanie Adresu {0} - API", server.url);

            Task listenServer = server.HandleIncomingConnections();

            string appPath = Application.StartupPath + "public_html\\";
            App.StartHttpServerOnThread(appPath, 5000);

           



            //listen.GetAwaiter().GetResult();

            //server.listener.Close();

            server.PropertyChanged += new PropertyChangedEventHandler(ServerEvent);


            
        }

        private void addFirewallRule(int port)
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            // Let's create a new rule
            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;
            //Allow through firewall
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            //Using protocol TCP
            inboundRule.Protocol = 6; // TCP
                                      //Port 81
            inboundRule.LocalPorts = port.ToString();
            //Name of rule
            inboundRule.Name = "SimpleCommunicator " + port;
            // ...//
            inboundRule.Profiles = currentProfiles;

            // Now add the rule
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(inboundRule);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
    void ServerEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Views") 
            {
                Views.Text = server.Views.ToString();
            }
            Console.WriteLine(e.PropertyName + " is changing");
            Views.Text = server.Views.ToString();
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

    }
}
