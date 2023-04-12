using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockA
{
    

    public partial class SellHelper : Form
    {
        string accno, accpw;
        TextBox logtextBox;
        public SellHelper(TextBox textBox, string accno, string accpw)
        {
            InitializeComponent();
            this.logtextBox = textBox;
            this.accno = accno;
            this.accpw = accpw;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OrderCur oc = new OrderCur();
            Order or = new Order(logtextBox, accno, accpw);
            or.request("011500", "22050");


        }
    }
}
