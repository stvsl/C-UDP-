using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _0906;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace _0909C
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread tserver;
        Socket socket;
        string sendaim;
        public MainWindow()
        {
            InitializeComponent();
            this.clientip.Text = GetLocalIP();
            this.serverip.Text = GetLocalIP();
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName();
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void ConfigOKbtn_Click(object sender, RoutedEventArgs e)
        {
            tserver = new(new ParameterizedThreadStart(ThredServer))
            {
                IsBackground = true
            };
            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(clientip.Text), int.Parse(clientport.Text)));
            tserver.Start(new Config(socket, "", "", ""));
            this.ConfigOKbtn.IsEnabled = false;
            this.clientip.IsEnabled = false;
            this.clientport.IsEnabled = false;
            this.serverip.IsEnabled = false;
            this.serverport.IsEnabled = false;
            NewMsg("已开启对本地" + clientport.Text + "端口的监听");
        }

        public void NewMsg(string text)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.MsgCenter.Text = this.MsgCenter.Text + "\r\n" + text;
            }));
        }

        public void ThredServer(object? obj)
        {
            Config? cfg = obj as Config;
            byte[] buffer = new byte[4096];
            EndPoint remoteendPoint = new IPEndPoint(IPAddress.None, 0);
            while (cfg.Socket != null)
            {
                int count = cfg.Socket.ReceiveFrom(buffer, ref remoteendPoint);
                String s = Encoding.UTF8.GetString(buffer, 0, count);
                DataParse(s);
            }
        }

        private void DataParse(string s)
        {
            string[] datas = s.Split("$");
            if (datas[0].Equals("list"))
            {
                FreshList(datas[1]);
            }
            else {
                NewMsg(datas[0]);
            }
        }

        private void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            UDPSender(new Config(socket,this.serverip.Text,this.serverport.Text,"login@" + this.clientname.Text));
            NewMsg("已上线,正在拉取列表");
            this.clientname.IsEnabled = false;
            this.loginbtn.IsEnabled = false;
            this.logoutbtn.IsEnabled = true;
            FreshList();
        }

        private void FreshList()
        {
            UDPSender(new Config(socket, this.serverip.Text, this.serverport.Text, "friend"));
        }

        private void FreshList(string liststr)
        {
            string[] onlinelist = liststr.Split(";");
            List<string> list = new List<string>(onlinelist);
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.aimlist.ItemsSource = list;
            }));
        }

        private void logoutbtn_Click(object sender, RoutedEventArgs e)
        {
            UDPSender(new Config(socket, this.serverip.Text, this.serverport.Text, "logout@" + this.clientname.Text));
            NewMsg("已下线");
            this.clientname.IsEnabled = true;
            this.loginbtn.IsEnabled = true;
            this.logoutbtn.IsEnabled = false;
        }

        private void sendbtn_Click(object sender, RoutedEventArgs e)
        {
            UDPSender(new Config(socket, serverip.Text, serverport.Text, (this.clientname+"@"+sendaim.Split("@")[0]+"$")+this.sendtext.Text));
        }

        private void sendbroadcasrbtn_Click(object sender, RoutedEventArgs e)
        {
            UDPSender(new Config(socket, serverip.Text, serverport.Text, ("say@" + this.clientname.Text + ":" + clientport.Text+"$" + sendbroadcasttext.Text)));
        }

        private void UDPSender(Config cfg)
        {
            if (cfg != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(cfg.Data);
                EndPoint remoteEP = new IPEndPoint(cfg.Address, cfg.Port);
                cfg.Socket.SendTo(buffer, remoteEP);
            }
        }

        private void aimlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sendaim = (aimlist.SelectedItem as string);
        }
    }
}
