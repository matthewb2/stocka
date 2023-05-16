using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockA
{

    public class Item
    {
        public string millis;
        public string stamp;
        public string datetime;
        public string[] light;
        public int temp;
        public string method;
        public int km;
        public int profitrate;
        public int lossrate;
        public bool vcc;
        public bool vcc2;
    }

    public class Bucket
    {
        public string[] scode;
        public string[] sret;
        public string[] sqnt;
    }

}
