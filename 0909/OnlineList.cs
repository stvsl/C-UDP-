using _0906;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _0909
{
    internal class OnlineList {
        // 在线成员链表
        List<Online> onlines;

        public OnlineList()
        {
            onlines = new List<Online>();
        }

        public void Add(string name, string ip,string port) {
            onlines.Add(new Online(name,ip,port));
        }

        public void Remove(string name, string ip,string port) {
            for (int i = 0;i < onlines.Count();i++) {
                if (onlines[i].Equals(new Online(name, ip,port))) {
                    onlines.RemoveAt(i);
                }
            }      
        }

        public static void Check(Config cfg) { 
            
        }

        public Online? FindByName(string name) {
            for (int i = 0; i < onlines.Count(); i++)
            {
                if (onlines[i].Name.Equals(name))
                {
                    return onlines[i];
                }
            }
            return null;
        }
        public string GetAll() {
            string str = "";
            for (int i = 0; i < onlines.Count; i++) {
                str += onlines[i].ToString() + ";";
            }
            return str;
        }
        public string Get(int i) {
            return onlines[i].ToString();
        }
    }


    internal class Online
    {
        private string name;
        private string ip;
        private int port;

        public Online(string name, string ip, int port)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.ip = ip ?? throw new ArgumentNullException(nameof(ip));
            this.port = port;
        }

        public Online(string name, string ip, string port)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.ip = ip ?? throw new ArgumentNullException(nameof(ip));
            this.port = int.Parse(port);
        }

        public string Name { get => name; set => name = value; }
        public string Ip { get => ip; set => ip = value; }
        public int Port { get => port; set => port = value; }

        public override bool Equals(object? obj)
        {
            return obj is Online online &&
                   name == online.name &&
                   ip == online.ip &&
                   port == online.port;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name + "@" + ip + ":" + port;
        }
    }
}
