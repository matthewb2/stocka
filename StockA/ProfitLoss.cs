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
    public partial class ProfitLoss : Form
    {
        public ProfitLoss()
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.ShowItemToolTips = true;
            //listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(listView1_MouseUp);

            listView1.Columns.Add("주문번호");
            listView1.Columns.Add("종목번호");
            listView1.Columns.Add("종목명");
            listView1.Columns.Add("구분");
            listView1.Columns.Add("체결수량");
            listView1.Columns.Add("주문수량");
            listView1.Columns.Add("주문시각");

            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
