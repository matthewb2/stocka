using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockA
{
    public partial class TermRet : Form
    {
        public TermRet()
        {
            InitializeComponent();


            listView1.View = View.Details;
            listView1.ShowItemToolTips = true;
            //listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(listView1_MouseUp);

            listView1.Columns.Add("날짜");
            //listView1.Columns.Add("매도금액");
            listView1.Columns.Add("매매손익");
            listView1.Columns.Add("수익률");
            listView1.Columns.Add("매도금액");

            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

            

            foreach (ColumnHeader column in listView1.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }
            //
            SetHeight(listView1, 50);

            RetRec AuthorList = new RetRec();
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            
            using (StreamReader r = new StreamReader(path + @"\resultr.json"))
            {
                string json = r.ReadToEnd();
                AuthorList = JsonConvert.DeserializeObject<RetRec>(json);
            }

            listView1.Items.Clear();

            for (int j = AuthorList.rdate.Length-1; j >=0; j--)
            {
                
                var row = new ListViewItem(new[] { AuthorList.rdate[j], string.Format("{0:N0}", Convert.ToInt32(AuthorList.rmat[j])), AuthorList.rret[j]+"%", string.Format("{0:N0}", Convert.ToInt32(AuthorList.mdmat[j])) });
                listView1.Items.Add(row);

            }


            for (int i = listView1.Items.Count-1; i>=0 ; i--)
            {
                //important
                listView1.Items[i].UseItemStyleForSubItems = false;

                if (Convert.ToInt32(AuthorList.rmat[i]) < 0)
                {
                    listView1.Items[listView1.Items.Count-i-1].SubItems[1].ForeColor = Color.Blue;
                    listView1.Items[listView1.Items.Count-i-1].SubItems[2].ForeColor = Color.Blue;
                }
                if (Convert.ToInt32(AuthorList.rmat[i]) > 0)
                {
                    listView1.Items[listView1.Items.Count - i - 1].SubItems[1].ForeColor = Color.Red;
                    listView1.Items[listView1.Items.Count - i - 1].SubItems[2].ForeColor = Color.Red;
                }
            }

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
        }

        private static void WriteCSVRow(StringBuilder result, int itemsCount, Func<int, bool> isColumnNeeded, Func<int, string> columnValue)
        {
            bool isFirstTime = true;
            for (int i = 0; i < itemsCount; i++)
            {
                if (!isColumnNeeded(i))
                    continue;

                if (!isFirstTime)
                    result.Append(",");
                isFirstTime = false;

                result.Append(String.Format("\"{0}\"", columnValue(i)));
            }
            result.AppendLine();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "기간손익_" + DateTime.Now.ToString().Substring(0,10)+".csv";

            saveFileDialog1.Filter = "csv file|*.csv|Excel Worksheets|*.xls"+
             "|text file|*.txt" +
             "|All Files|*.*";
            saveFileDialog1.Title = "Save an csv File";
            saveFileDialog1.ShowDialog();
            
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                //make header string
                StringBuilder result = new StringBuilder();
                //WriteCSVRow(result, listView1.Columns.Count, i => listView1.Columns[i].Width > 0, i => listView1.Columns[i].Text);

                //export data rows
                foreach (ListViewItem listItem in listView1.Items)
                    WriteCSVRow(result, listView1.Columns.Count, i => listView1.Columns[i].Width > 0, i => listItem.SubItems[i].Text);

                File.WriteAllText(saveFileDialog1.FileName, result.ToString());

                this.Hide();
            }
        }
    }
}
