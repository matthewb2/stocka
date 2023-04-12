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
    public partial class QuickOrd : Form
    {
        private string accno, accpw;
        private TextBox txtbox;
        public QuickOrd(TextBox txtbox, string accno, string accpw)
        {
            this.txtbox = txtbox;
            this.accno = accno;
            this.accpw = accpw;

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Order or = new Order(txtbox, accno, accpw);
            //or.request("011500", "22050");
            or.request(textBox3.Text, textBox1.Text);
        }
    }
}
