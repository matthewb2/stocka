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
        ListView listView1;
        Order or;
        public SellHelper(TextBox textBox, ListView lst, string accno, string accpw)
        {
            InitializeComponent();
            this.logtextBox = textBox;
            this.accno = accno;
            this.accpw = accpw;
            this.listView1 = lst;
            or = new Order(logtextBox, accno, accpw);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Point mousePosition = listView1.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hit = listView1.HitTest(mousePosition);
            int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
            int rowindex = hit.Item.Index;
            string code = listView1.Items[rowindex].SubItems[0].Text;
            string price = listView1.Items[rowindex].SubItems[2].Text;
            string qty = listView1.Items[rowindex].SubItems[4].Text;
            price = price.Replace(",", "");


            this.logtextBox.Text += code + Environment.NewLine;
            

            or.request(code, price, "1", qty);

            or.end();

            this.Close();

        }
    }
}
