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
using System.Net;
using System.Timers;

namespace StockA
{
    

    public partial class Form1 : Form
    {
        XASessionClass session;
        public static bool logged = false;
        public static bool running = false;

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer myTimer2 = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer myTimer3 = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer myTimer4 = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer myTimer5 = new System.Windows.Forms.Timer();

        static System.Windows.Forms.Timer dayTimer = new System.Windows.Forms.Timer();

        public Item items;
        public static string id;
        string password, accno, accpw;
        
        SearchSt sst;
        public int profit, loss, km;
        public QuickOrd qo;
        public ListCurSt lsc;
        public ListCurTr lct = new ListCurTr();
        StockInfo si;

        public string ordermethod;
        Balance bl;



        private void TimerEventDay(Object myObject,
                                                EventArgs myEventArgs)
        {
            //Console.WriteLine(getTime().Substring(0, 5));
            logtxtBox.Text += getTime() + Environment.NewLine;
            if (getTime().Substring(0, 5) == "09:00" && getTime().Substring(9, 2) == "오전")
            {
                //자동매매 시작
                button1.PerformClick();
                Thread.Sleep(300);

                button2.PerformClick();
            }

            if (getTime().Substring(0, 5) == "03:12" && getTime().Substring(9, 2) == "오후")
            {
                //0000 stop
                myTimer.Stop();

                //검색조건 0000 보유중이면 전량 매도
                sellAll0000();

            }


        }

        public Form1()
        {
            InitializeComponent();

            
            //MessageBox.Show(getTime().Substring(9, 2));

            // 타이머 생성 및 시작
            dayTimer.Tick += new EventHandler(TimerEventDay);
            dayTimer.Interval = 1000 * 30;
            dayTimer.Start();

            //환경변수 로드
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            items = new Item();
            using (StreamReader r = new StreamReader(path + @"\pref.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<Item>(json);
                this.ordermethod = items.method;
                this.km = items.km;

            }

            //조건식 로드
            this.profit = items.profitrate;
            this.loss = items.lossrate;

            logtxtBox.Text += this.ordermethod + Environment.NewLine;
            logtxtBox.Text += this.km + Environment.NewLine;

            float yield = this.profit;
            float negative = this.loss;

            logtxtBox.Text += String.Format("{0} {1}", yield, negative) + Environment.NewLine;

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
            listView1.Columns.Add("당일매매손익");
            listView1.Columns.Add("D+1 예수금");
            listView1.Columns.Add("D+2 예수금");
            
            
            //
            listView1.Columns[0].Width = 300;
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

            foreach (ColumnHeader column in listView1.Columns)
            {
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

        private void 도움말ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }


        private void listView2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
                                    
        }


        public Bucket getBucketItem()
        {
            Bucket AuthorList = new Bucket();
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\bucket.json"))
                AuthorList = JsonConvert.DeserializeObject<Bucket>(r.ReadToEnd());
            
            return AuthorList;
        }


        public Extra getExtraItem()
        {
            Extra AuthorList = new Extra();
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\0000.json"))
                AuthorList = JsonConvert.DeserializeObject<Extra>(r.ReadToEnd());

            return AuthorList;
        }

        public string getTime()
        {
            return DateTime.Now.ToString("hh:mm:ss tt");
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
            //
            logtxtBox.Text = "";
            //

            logtxtBox.Text += getTime() + " 로그인 okay" + Environment.NewLine;
            logged = true;
            if (!button2.Enabled)
                button2.Enabled = true;
            button4.Enabled = true;

            //잔고
            int nCount = session.GetAccountListCount();
            //
            logtxtBox.Text += String.Format("보유계좌수: {0}", nCount) + Environment.NewLine;

            bl = new Balance(logtxtBox, listView1, listView2, this.accno, this.accpw);
            bl.request();
            bl.end();
            

        }
        private void sellAll0000()
        {
            Order od = new Order(logtxtBox, this.accno, this.accpw);

            Bucket bucketItem = getBucketItem();
            Extra extraItem = getExtraItem();
            //bool isBucket = false;

            for (int i = 0; i < extraItem.scode.Length; i++)
            {
                for (int j = 0; j < bucketItem.scode.Length; j++)
                {

                    if (extraItem.scode[i] == bucketItem.scode[j])
                    {
                        //매도실행
                        Console.WriteLine(extraItem.scode[i]);
                        od.request(bucketItem.scode[j], bucketItem.sprice[j], "1", bucketItem.sqnt[j]); //1 sell
                        Thread.Sleep(100);
                        od.end();
                        break;

                    }

                }
            }
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            myTimer.Stop();
            //
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
            if (qo.Visible)
            {
                qo.textBox3.Text = listView2.Items[rowindex].SubItems[1].Text;
                qo.textBox1.Text = listView2.Items[rowindex].SubItems[2].Text;
                qo.textBox2.Text = listView2.Items[rowindex].SubItems[3].Text;
                qo.textBox4.Text = listView2.Items[rowindex].SubItems[0].Text;

            }
            //현재가 종목 정보
            StockInfo si = new StockInfo(logtxtBox, listView2);
            si.request(listView2.Items[rowindex].SubItems[0].Text);
            
        }

        private void sellBucket()
        {
            
            float yield = this.profit;
            float negative = this.loss;
            Bucket bucketItem = getBucketItem();

            for (int j = 0; j < bucketItem.scode.Length - 1; j++)
            {

                float ret = float.Parse(bucketItem.sret[j]);
                if (ret > yield || ret < negative)
                {
                    Order od = new Order(logtxtBox, accno, accpw);
                    var t = new Thread(() => RealStart(od, bucketItem.scode[j], bucketItem.sprice[j], bucketItem.sqnt[j]));
                    t.Start();
                }

            }
        }
        /*
        private void sell_0000()
        {

            float yield = this.profit;
            float negative = this.loss;
            Bucket bucketItem = getBucketItem();

            for (int j = 0; j < bucketItem.scode.Length - 1; j++)
            {

                float ret = float.Parse(bucketItem.sret[j]);
                if (ret > yield || ret < negative)
                {
                    Order od = new Order(logtxtBox, accno, accpw);
                    var t = new Thread(() => RealStart(od, bucketItem.scode[j], bucketItem.sprice[j], bucketItem.sqnt[j]));
                    t.Start();
                }

            }
        }
        */
        private void button2_Click(object sender, EventArgs e)
        {
            //자동매매시작
            button3.Enabled = true;
            button2.Enabled = false;
            running = true;

            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();

            // 타이머 생성 및 시작            
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = 1000 * 60;
            myTimer.Start();

            Thread.Sleep(3000);
            // 타이머 생성 및 시작
            myTimer2.Tick += new EventHandler(Timer2EventProcessor);
            myTimer2.Interval = 1000 * 60;
            //myTimer2.Start();

            Thread.Sleep(3000);
            // 타이머 생성 및 시작
            myTimer3.Tick += new EventHandler(Timer3EventProcessor);
            myTimer3.Interval = 1000 * 60;
            //myTimer3.Start();

            Thread.Sleep(3000);
            // 타이머 생성 및 시작
            myTimer4.Tick += new EventHandler(Timer4EventProcessor);
            myTimer4.Interval = 1000 * 60;
            //myTimer4.Start();

            Thread.Sleep(3000);
            // 타이머 생성 및 시작
            myTimer5.Tick += new EventHandler(Timer5EventProcessor);
            myTimer5.Interval = 1000 * 60;
            //myTimer5.Start();

        }

        // This is the method to run when the timer is raised.
        private void TimerEventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {
            
            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();
            
            //load real time search list
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw, this.km);
            sst.request("  0000");
            sst.end();
            /*
            bl = new Balance(logtxtBox, listView1, listView2, this.accno, this.accpw);
            bl.request();
            bl.end();
            */
        }

        private void Timer2EventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {
            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();
            
            //load real time search list
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw, this.km);
            sst.request("  0001");
            sst.end();
            
        }


        private void Timer3EventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {

            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();

            //load real time search list
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw, this.km);
            sst.request("  0002");
            sst.end();

        }

        private void Timer4EventProcessor(Object myObject,
                                        EventArgs myEventArgs)
        {

            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();

            //load real time search list
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw, this.km);
            sst.request("  0006");
            sst.end();

        }
        private void Timer5EventProcessor(Object myObject,
                                        EventArgs myEventArgs)
        {

            //익절 또는 손절 조건을 만족하는 보유주식 매도
            sellBucket();

            //load real time search list
            sst = new SearchSt(logtxtBox, listView2, id, accno, accpw, this.km);
            sst.request("  0003");
            sst.end();

        }
        private static void RealStart(Order od, string scode, string price, string qnt)
        {
            od.request(scode, price, "1", qnt);
            Thread.Sleep(1000);
            Console.WriteLine(scode);
            od.end();
            
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //timer.Stop();
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

        private void 기간손익ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProfitLoss pl = new ProfitLoss();
            pl.Show();

        }

        private void 당일매매ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //당일매매수익률 표시
            if (!logged)
            {
                MessageBox.Show("로그인이 필요합니다");
                return;
            }
            if (!lct.Visible)
            {

                lct.Show();
                lct.Visible = true;

                //OrderCur odc = new OrderCur(lsc.listView1, accno, accpw);
                
                string return_value = listView1.Items[0].SubItems[6].Text.Replace(",","");
                logtxtBox.Text += return_value + Environment.NewLine;
                CurrentTr ct = new CurrentTr(lct.listView1, accno, accpw, return_value);
                ct.request();
            }


        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

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


    }


}
