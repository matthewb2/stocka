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
    public partial class ListCurTr : Form
    {
        public ListCurTr()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.ShowItemToolTips = true;
            
            listView1.Columns.Add("매도금액");
            listView1.Columns.Add("수수료및제비용");
            listView1.Columns.Add("당일손익");
            listView1.Columns.Add("당일수익률");
            listView1.Columns.Add("매수금액");

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
            //this.Close();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
