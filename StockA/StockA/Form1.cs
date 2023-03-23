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
using XA_DATASETLib;
using XA_SESSIONLib;

namespace StockA
{
    

    public partial class Form1 : Form
    {
        XASessionClass session;
        public static bool logged = false;
        public List<Item> items;

        public Form1()
        {
            InitializeComponent();
            textBox4.Text = "";
            textBox2.Text = "";
            textBox6.Text = "";
            textBox5.Text = "0000";
            checkBox1.Checked = true;

            
            listView1.View = View.Details;
            //
            listView1.Columns.Add("추정순자산");
            listView1.Columns.Add("평가금액");
            listView1.Columns.Add("평가손익");
            listView1.Columns.Add("실현손익");
            listView1.Columns.Add("매입금액");

            listView1.Columns.Add("예탁자산총액");
            listView1.Columns.Add("D+1 예수금");
            listView1.Columns.Add("D+2 예수금");
            listView1.Columns.Add("손익률(%)");
            listView1.Columns.Add("보유종목수");
            //
            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

            foreach (ColumnHeader column in listView1.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }
            
            SetHeight(listView1, 40);

            listView2.View = View.Details;


            listView2.Columns.Add("종목번호");
            listView2.Columns.Add("종목명");
            listView2.Columns.Add("현재가");
            listView2.Columns.Add("평균단가");
            listView2.Columns.Add("잔고수량");
            listView2.Columns.Add("매입금액");
            listView2.Columns.Add("평가금액");
            listView2.Columns.Add("평가손익");
            listView2.Columns.Add("수익률(%)");
            
            listView2.Columns[0].Width = 300;
            listView2.Columns[0].TextAlign = HorizontalAlignment.Center;

            foreach (ColumnHeader column in listView2.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }

            SetHeight(listView2, 40);
            
        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }


        private void 시스템설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preference pw = new Preference();
            pw.Show();
        }

        private void 도움말ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About ab = new About();
                ab.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            session = new XASessionClass();
            bool conn  = session.ConnectServer("demo.ebestsec.co.kr", 20001);
            //로그인 성공 메시지 수신 후에 보유계좌 수 호출 로그인 이벤트 핸들러를 이용
            session._IXASessionEvents_Event_Login += XASession_Login;

            if (conn)
                ((XA_SESSIONLib.IXASession)session).Login(textBox4.Text, textBox2.Text, "", 0, false);
            
        }

        private void XASession_Login(string szCode, string szMsg)
        {
            logtxtBox.Text += "로그인 okay" + Environment.NewLine;
            logged = true;
            //잔고
            int nCount = session.GetAccountListCount();
            //
            logtxtBox.Text += String.Format("보유계좌수: {0}", nCount) + Environment.NewLine;
            Balance bl = new Balance(logtxtBox, listView1, listView2);
            bl.request();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("자동매매가 실행중입니다. 그래도 종료하시겠습니까", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //MessageBox.Show("예");
                Application.Exit();
            }
            else
            {
                //MessageBox.Show("아니요");
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\pref.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            //환경변수 로드
            string[] st = items[0].light;
            System.Object[] ItemObject = new System.Object[st.Length];

            for (int i = 0; i < st.Length; i++)
            {
                ItemObject[i] = String.Format("{0}", st[i]);
                
            }
            foreach (var item in items)
            {
                logtxtBox.Text += item.millis + Environment.NewLine;
                logtxtBox.Text += item.light + Environment.NewLine;

            }
            //실시간 조건검색 로드
            SearchSt sst = new SearchSt(logtxtBox);
            sst.request();
            sst.end();

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }


}
