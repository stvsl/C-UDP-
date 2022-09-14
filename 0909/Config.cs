using System.Net.Sockets;
using System.Net;

namespace _0906
{
    internal class Config
    {
        private Socket socket;
        private IPAddress address;
        private int port;
        private string data;

        public Config(Socket ss, string stradd, string strport, string data)
        {
            this.Socket = ss;
            this.Address = IPAddress.Parse((stradd == "" ? "127.0.0.1" : stradd));
            this.Port = int.Parse(strport == ""? "8081":strport);
            this.data = data;
        }

        public Socket Socket { get => socket; set => socket = value; }
        public IPAddress Address { get => address; set => address = value; }
        public int Port { get => port; set => port = value; }
        public string Data { get => data; set => data = value; }
    }
}
