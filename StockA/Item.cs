using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockA
{

    public class Item
    {
        public string accno;
        public string stamp;
        public string datetime;
        public string[] light;
        public int temp;
        public string method;
        public int km;
        public float profitrate;
        public float lossrate;
        public bool vcc;
        public bool vcc2;
    }

    public class Bucket
    {
        public string[] scode;
        public string[] sret;
        public string[] sqnt;
        public string[] sprice;
    }

    public class Pendst
    {
        public string[] scode;
        public string[] sqty;
        
    }

    public class Extra
    {
        public string[] scode;
        //public string[] sqty;

    }


    public class RetRec
    {
        public string[] rdate;
        public string[] rret;
        public string[] rmat;
        public string[] mdmat;

    }

}
