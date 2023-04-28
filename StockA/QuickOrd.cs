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
        private RichTextBox txtbox;
        Order or;
        public QuickOrd(RichTextBox txtbox, string accno, string accpw)
        {
            this.txtbox = txtbox;
            this.accno = accno;
            this.accpw = accpw;

            InitializeComponent();

            or = new Order(txtbox, accno, accpw);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //or = new Order(txtbox, accno, accpw);
            //or.request("011500", "20050");
            this.txtbox.Text += textBox4.Text + Environment.NewLine;
            this.txtbox.Text += textBox1.Text.Replace(",", "") + Environment.NewLine;
            if (textBox1.Text.Length >0)
            {
                string price = textBox1.Text.Replace(",", "");
                if (Convert.ToInt32(price) < 0)
                {
                    MessageBox.Show("가격란에 유효한 값이 아닙니다");
                }
                string code = textBox4.Text;
                or.request(code, price, "1", "15");
                or.end();
            }
            
        }
    }
}
