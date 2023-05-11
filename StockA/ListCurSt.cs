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
            listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(listView1_MouseUp);

            listView1.Columns.Add("주문번호");
            listView1.Columns.Add("종목번호");
            listView1.Columns.Add("종목명");
            listView1.Columns.Add("구분");
            listView1.Columns.Add("체결수량");
            listView1.Columns.Add("주문수량");
            listView1.Columns.Add("주문시각");

            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

            foreach (ColumnHeader column in listView1.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }

            SetHeight(listView1, 40);

                    
            
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
            this.Hide();
            this.Visible=false;
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo HI = listView1.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                //마우스 우클릭 현재가 매도
                Point mousePosition = listView1.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hit = listView1.HitTest(mousePosition);
                int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
                int rowindex = hit.Item.Index;
                //MessageBox.Show(rowindex.ToString());
                CancelOrder co = new CancelOrder();
                co.StartPosition = FormStartPosition.Manual;
                co.Location = Cursor.Position;
                co.Show();



            }

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
