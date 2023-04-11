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
    public partial class ListCurSt : Form
    {
        public ListCurSt()
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

            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

            foreach (ColumnHeader column in listView1.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }

            SetHeight(listView1, 40);


            string accno = "55501502101";
            string accpw = "0000";

            
            OrderCur odc = new OrderCur(listView1, accno, accpw);

            var row1 = new ListViewItem(new[] { "", "", "", "", "" });

            listView1.Items.Add(row1);
            odc.request();




        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
