using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public int profit, loss, km;
        public QuickOrd qo;
        public ListCurSt lsc;
        StockInfo si;

        public string ordermethod;
        Balance bl;
        private System.Threading.Timer timer;

        
        public Form1()
        {
            InitializeComponent();
            


            //환경변수 로드
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\pref.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            
            //조건식 로드
            this.profit = items[0].profitrate;
            this.loss = items[0].lossrate;
            this.ordermethod = items[0].method;
            this.km = items[0].km;

            logtxtBox.Text += this.ordermethod + Environment.NewLine;
            logtxtBox.Text += this.km + Environment.NewLine;

            //간단주문 준비
            qo = new QuickOrd(logtxtBox, accno, accno);

            lsc = new ListCurSt();
            //
            si = new StockInfo(logtxtBox, listView2);


            textBox4.Text = "";
            textBox2.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            checkBox1.Checked = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;


            listView1.View = View.Details;
            
            //
            listView1.Columns.Add("추정순자산");
            listView1.Columns.Add("매입금액");
            listView1.Columns.Add("평가금액");
            listView1.Columns.Add("평가손익");
            listView1.Columns.Add("손익률(%)");
            listView1.Columns.Add("보유종목수");
            listView1.Columns.Add("당일실현손익");
            listView1.Columns.Add("D+1 예수금");
            listView1.Columns.Add("D+2 예수금");
            
            
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
            
            listView2.Columns.Add("잔고수량");
            listView2.Columns.Add("매입금액");
            listView2.Columns.Add("평가금액");
            listView2.Columns.Add("평가손익");
            listView2.Columns.Add("수익률(%)");
            listView2.Columns.Add("평균단가");

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

            listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;

            logtxtBox.Text += getTime() + Environment.NewLine;
            //

            

        }

        public string getQnt(string price, string notes)
        {
            int qn = Convert.ToInt32(notes) / Convert.ToInt32(price);

            return qn.ToString();
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


        private void listView2_SelectedIndexChanged(object sender, System.EventArgs e)
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

        public string getTime()
        {
            return DateTime.Now.ToString("hh:mm:ss tt");
        }
        private void XASession_Login(string szCode, string szMsg)
        {

            logtxtBox.Text += getTime()+" 로그인 okay" + Environment.NewLine;
            logged = true;
            if(!button2.Enabled)
                button2.Enabled = true;
            button4.Enabled = true;

            //잔고
            int nCount = session.GetAccountListCount();
            //
            logtxtBox.Text += String.Format("보유계좌수: {0}", nCount) + Environment.NewLine;

            bl = new Balance(logtxtBox, listView1, listView2, this.accno, this.accpw);

            //test codes here
            
            bl.request();
            bl.end();

            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (running)
            {
                running = false;
                session.Logout();
                logged = false;
                button2.Enabled = false;
                button1.Enabled = true;
                button3.Enabled = false;
                if (button1.Enabled)
                    button4.Enabled = false;

            }
            else
            {   
                session.Logout();
                button2.Enabled = false;
                button1.Enabled = true;
                button4.Enabled = false;
                button3.Enabled = false;
                logged = false;

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            //
            ListViewHitTestInfo HI = listView1.HitTest(e.Location);
            Point mousePosition = listView2.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hit = listView2.HitTest(mousePosition);
            int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
            int rowindex = hit.Item.Index;

            if (e.Button == MouseButtons.Right)
            {
                //마우스 우클릭 현재가 매도
                SellHelper sh = new SellHelper(logtxtBox, listView2, accno, accpw);
                sh.StartPosition = FormStartPosition.Manual;
                sh.Location = Cursor.Position;
                sh.Show();

            }


            // stocks manipulation when a mouse click event occurs
            if (qo.Visible)
            {
                qo.textBox3.Text = listView2.Items[rowindex].SubItems[1].Text;
                qo.textBox1.Text = listView2.Items[rowindex].SubItems[2].Text;
                qo.textBox2.Text = listView2.Items[rowindex].SubItems[4].Text;
                qo.textBox4.Text = listView2.Items[rowindex].SubItems[0].Text;

            }
            //현재가 종목 정보
            StockInfo si = new StockInfo(logtxtBox, listView2);
            si.request(listView2.Items[rowindex].SubItems[0].Text);
            


        }
        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        public List<string> getBucketItem()
        {

            //보유종목리스트
            List<string> AuthorList = new List<string>();
            Bucket items = new Bucket();

            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\bucket.json"))
            {
                string json = r.ReadToEnd();

                items = JsonConvert.DeserializeObject<Bucket>(json);
                foreach (string code in items.scode)
                {
                    AuthorList.Add(code);
                }
            }


            return AuthorList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //자동매매시작
            button3.Enabled = true;
            button2.Enabled = false;
            running = true;

            
            //실시간 조건검색 로드
            sst = new SearchSt(logtxtBox, listView2, lsc.listView1, id, accno, accpw, ordermethod, this.km);
            sst.request();
            sst.end();



            //익절 또는 손절 조건을 만족하는 보유주식 매도
            Order od = new Order(logtxtBox, accno, accpw);
            float yield = this.profit;
            float negative = this.loss;

            
            foreach (ListViewItem itemRow in this.listView2.Items)
            {
                
                
                float ret = float.Parse(itemRow.SubItems[8].Text, CultureInfo.InvariantCulture.NumberFormat);
                
                if (ret > yield || ret< negative)
                {
                    logtxtBox.Text += String.Format("{0} {1} {2}",itemRow.SubItems[0].Text, itemRow.SubItems[2].Text, itemRow.SubItems[4].Text) + Environment.NewLine;

                    od.request(itemRow.SubItems[0].Text, itemRow.SubItems[2].Text.Replace(",", ""), "1", itemRow.SubItems[4].Text);
                }
                
                
            }
            
            // 15:15:00 전량매도
            //SetUpTimer(new TimeSpan(14, 44, 00));
        }

        private void SomeMethodRunsAt1515()
        {
            //this runs at 15:15:00
            //전량 매도
            //MessageBox.Show("run");
            
            HoldStock hs = new HoldStock(logtxtBox, accno, accpw);
            hs.request();
            hs.end();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            button3.Enabled = false;
            button2.Enabled = true;
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

        private void 정보ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About ab = new About();
                ab.Show();
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

                OrderCur odc = new OrderCur(lsc.listView1, accno, accpw);
                odc.request();
            }


        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logged)
                bl.request();
            else MessageBox.Show("로그인이 필요합니다");
        }

        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.SomeMethodRunsAt1515();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

    }


}
