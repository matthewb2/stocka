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
        public static bool running = false;

        public List<Item> items;
        public static string id;
        string password, accno, accpw;
        
        SearchSt sst;
        public QuickOrd qo;
        public ListCurSt lsc;


        public Form1()
        {
            InitializeComponent();
            

            textBox4.Text = "";
            textBox2.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            checkBox1.Checked = true;
            button2.Enabled = false;
            button3.Enabled = false;


            listView1.View = View.Details;
            
            //
            listView1.Columns.Add("추정순자산");
            listView1.Columns.Add("평가금액");
            listView1.Columns.Add("평가손익");
            listView1.Columns.Add("당일실현손익");
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
            listView2.ShowItemToolTips = true;
            listView2.MouseUp += new System.Windows.Forms.MouseEventHandler(listView2_MouseUp);

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

            var row1 = new ListViewItem(new[] { "", "", "", "", "", "", "", "", "", "" });

            listView2.Items.Add(row1);
            
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

        private void 주문ToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }
        private void 도움말ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            id = textBox4.Text;
            this.password = textBox2.Text;
            this.accno = textBox6.Text;
            this.accpw = textBox5.Text;

            session = new XASessionClass();
            bool conn  = session.ConnectServer("demo.ebestsec.co.kr", 20001);
            button1.Enabled = false;
            //로그인 성공 메시지 수신 후에 보유계좌 수 호출 로그인 이벤트 핸들러를 이용
            session._IXASessionEvents_Event_Login += XASession_Login;

            if (conn)
                ((XA_SESSIONLib.IXASession)session).Login(textBox4.Text, textBox2.Text, "", 0, false);
            
        }

        private void XASession_Login(string szCode, string szMsg)
        {
            logtxtBox.Text += "로그인 okay" + Environment.NewLine;
            logged = true;
            button2.Enabled = true;

            //잔고
            int nCount = session.GetAccountListCount();
            //
            logtxtBox.Text += String.Format("보유계좌수: {0}", nCount) + Environment.NewLine;
            Balance bl = new Balance(logtxtBox, listView1, listView2, this.accno, this.accpw);
            bl.request();

            //간단주문 준비
            qo = new QuickOrd(logtxtBox, accno, accno);

            lsc = new ListCurSt();

            //test codes here
            Order or = new Order(logtxtBox, accno, accpw);
            //or.request("086520", "601000");


        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (running)
            {
                MessageBox.Show("자동매매가 실행중입니다. 그래도 종료하시겠습니까", "알림", MessageBoxButtons.YesNo);
                //MessageBox.Show("예");
                session.Logout();
                button2.Enabled = false;
                button1.Enabled = true;
                if (button1.Enabled)
                    button4.Enabled = false;


                //Application.Exit();
            }
            else
            {
                //MessageBox.Show("아니요");
                session.Logout();
                button2.Enabled = false;
                button1.Enabled = true;
                if (button1.Enabled)
                    button4.Enabled = false;

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            //QuickOrd qord = new QuickOrd();
            
            ListViewHitTestInfo HI = listView1.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                //마우스 우클릭 현재가 매도
                Point mousePosition = listView2.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hit = listView2.HitTest(mousePosition);
                int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
                int rowindex = hit.Item.Index;
                //MessageBox.Show(rowindex.ToString());
                SellHelper sh = new SellHelper(logtxtBox, accno, accpw);
                sh.StartPosition = FormStartPosition.Manual;
                sh.Location = Cursor.Position;
                sh.Show();



            }

            // stocks manipulation when a mouse click event occurs
            if (qo.Visible)
            {
                Point mousePosition = listView2.PointToClient(Control.MousePosition);
                ListViewHitTestInfo hit = listView2.HitTest(mousePosition);
                int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
                int rowindex = hit.Item.Index;
                qo.textBox3.Text = listView2.Items[rowindex].SubItems[1].Text;
                qo.textBox1.Text = listView2.Items[rowindex].SubItems[2].Text;
                qo.textBox2.Text = listView2.Items[rowindex].SubItems[4].Text;
                qo.textBox4.Text = listView2.Items[rowindex].SubItems[0].Text;

            }
            


        }
        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //자동매매시작
            button3.Enabled = true;
            button2.Enabled = false;
            running = true;
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            try
            {
                using (StreamReader r = new StreamReader(path + @"\pref.json"))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<Item>>(json);
                }
            }
            catch
            {
                logtxtBox.Text += "환경변수 불러오기를 실패했습니다" + Environment.NewLine;
                return;
            }
            //환경변수 로드
            string[] st = items[0].light;
            System.Object[] ItemObject = new System.Object[st.Length];

            for (int i = 0; i < st.Length; i++)
            {
                //ItemObject[i] = String.Format("{0}", st[i]);
                
            }
            foreach (var item in items)
            {
                //logtxtBox.Text += item.millis + Environment.NewLine;
                

            }
            //실시간 조건검색 로드
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw);
            sst.request();
            //
            sst.end();
            
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            sst.end();
        }

        private void 간단주문ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!logged)
            {
                MessageBox.Show("로그인이 필요합니다");
                return;
            }
            if (!qo.Visible)
            {

                qo.Show();
                qo.Visible = true;
            }
                
            
        }

        private void 주문내역확인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            

            if (!logged)
            {
                MessageBox.Show("로그인이 필요합니다");
                return;
            }
            if (!lsc.Visible)
            {

                lsc.Show();
                lsc.Visible = true;
            }


        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About ab = new About();
            ab.Show();

        }
    }


}
