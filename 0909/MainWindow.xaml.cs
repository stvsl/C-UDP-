using System.Net.Sockets;
using System.Net;
using System;
using System.Windows;
using System.Threading;
using _0906;
using System.Text;

namespace _0909
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread ? tserver;
        Socket? socket;
        OnlineList onlineList;
        Nlist wlist;
        Nlist blist;
        
        public MainWindow()
        {
            InitializeComponent();
            this.serverip.Content = GetLocalIP();
            onlineList = new OnlineList();
            wlist = new Nlist();
            blist = new Nlist();
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

        private void ServerOKbtn_Click(object sender, RoutedEventArgs e)
        {
            tserver = new(new ParameterizedThreadStart(ThredServer))
            {
                IsBackground = true
            };
            socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(serverip.Content.ToString()??""), int.Parse(serverport.Text)));
            tserver.Start(new Config(socket, "", "", ""));
            this.ServerOKbtn.IsEnabled = false;
            NewMsg("已开启对本地" + serverport.Text + "端口的监听");
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
                if (!blist.Find(remoteendPoint.ToString())) {
                    DataParse(s, remoteendPoint.ToString());
                }
                if (wlist.Find(remoteendPoint.ToString()))
                {
                    DataParse(s, remoteendPoint.ToString());
                }
            }
        }

        public void NewMsg(string text) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                this.GlobalMsg.Text = this.GlobalMsg.Text + "\r\n" + text;
            } ));
        }

        // 数据解析
        public void DataParse(string data,string endip)
        {
            // 判断登录login@name/下线logout@name/通讯name@aim$text/广播say@name:port$text/ping/friend
            // 解析data
            string[] datas = data.Split("@");
            if (datas[0].Equals("login"))
            {
                // 登录
                string[] cip = endip.Split(":");
                onlineList.Add(datas[1], cip[0], cip[1]);
                NewMsg(datas[1] + "@" + cip[0] + ":" + cip[1] + "上线");
            }
            else if (datas[0].Equals("logout"))
            {
                // 下线
                string[] cip = endip.Split(":");
                onlineList.Remove(datas[1], cip[0], cip[1]);
                NewMsg(datas[1] + "@" + cip[0] + ":" + cip[1] + "下线");
            }
            else if (datas[0].Equals("say"))
            {
                // 广播
                string[] cdata = datas[1].Split("$");
                string[] wp = cdata[0].Split(":");
                string[] aip = GetLocalIP().Split(".");
                NewMsg((wp[0] + "发送了广播:" + cdata[1]));
                UDPSender(new Config(socket, (aip[0] + "." + aip[1] + "." + aip[2] + "." + 255), wp[1], (wp[0] + "发送了广播:" + cdata[1])));

            }
            else if (datas[0].Equals("ping"))
            {
                //ping
                string[] cdata = endip.Split(":");
                UDPSender(new Config(socket, cdata[0], cdata[1], "pong"));
            }
            else if (datas[0].Equals("friend"))
            {
                // 获取在线列表
                string[] cdata = endip.Split(":");
                UDPSender(new Config(socket, cdata[0], cdata[1], "list$" + onlineList.GetAll()));
            }
            else
            {
                // 通讯
                try
                {
                    string[] cdata = datas[1].Split("$");
                    Online? online = onlineList.FindByName(cdata[0]);
                    UDPSender(new Config(socket, online.Ip, online.Port.ToString(), cdata[0] + "@" + cdata[1]));
                    NewMsg(datas[0] + "@" + cdata[0] + "$" + cdata[1]);
                }
                catch{ }
            }
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

        private void sendbroadcastbtn_Click(object sender, RoutedEventArgs e)
        {
            string[] aip = GetLocalIP().Split(".");
            UDPSender(new Config(socket, (aip[0] + "." + aip[1] + "." + aip[2] + "." + 255), sendbroadcastport.Text, ("服务器发送了广播:" + sendbroadcasttext.Text)));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            blist.Add(blackip.Text);
            blacklist.ItemsSource = null;
            blacklist.ItemsSource = blist.Get();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            blacklist.ItemsSource = null;
            blist.Remove(blackip.Text);
            blacklist.ItemsSource = blist.Get();
        }

        private void whiteaddbtn_Click(object sender, RoutedEventArgs e)
        {
            whitelist.ItemsSource = null;
            wlist.Add(whiteip.Text);
            whitelist.ItemsSource = wlist.Get();
        }

        private void whitedelbtn_Click(object sender, RoutedEventArgs e)
        {
            whitelist.ItemsSource = null;
            wlist.Remove(whiteip.Text);
            whitelist.ItemsSource = wlist.Get();
        }
    }
}
