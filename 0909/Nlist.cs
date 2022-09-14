using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _0909
{
    internal class Nlist
    {
        private List<string> list;

        public Nlist()
        {
            this.list = new List<string>();
        }

        public void Add(string str) {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(str))
                {
                    return;
                }
            }
            list.Add(str);
        }

        public void Remove(string str)
        {
            for (int i = 0; i < list.Count; i++)
             {
                if (list[i].Equals(str))
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }
        public bool Find(string str) {
            for (int i = 0; i < list.Count; i++)
            {

                if (list[i].Equals(str))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> Get() {
            return list;
        }
    }
}
