using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;

namespace StockA
{

    class Balance
    {
        private XAQueryClass t0424;
        private XAQueryClass t0150;
        private XAQueryClass t0425;

        private XAQueryClass CSPAQ12300;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;

        ListView balance_sheet;

        ListView stocks;


        public RichTextBox output;
        public Balance(RichTextBox output, ListView balance_sheet, ListView stocks, string accno, string accpw)
        {
            this.output = output;
            this.balance_sheet = balance_sheet;
            this.stocks = stocks;
            //모의투자
            this.account_number = accno;
            this.account_pwd = accpw;

            
            this.is_data_received = false;

            t0424 = new XAQueryClass();
            t0424.ResFileName = @"C:\eBEST\xingAPI\Res\t0424.res"; //RES 파일 등록
            CSPAQ12300 = new XAQueryClass();
            CSPAQ12300.ResFileName = @"C:\eBEST\xingAPI\Res\CSPAQ12300.res"; //RES 파일 등록

            t0150 = new XAQueryClass();
            t0150.ResFileName = @"C:\eBEST\xingAPI\Res\t0150.res"; //RES 파일 등록

            t0425 = new XAQueryClass();
            t0425.ResFileName = @"C:\eBEST\xingAPI\Res\t0425.res"; //RES 파일 등록

            //
            t0424.ReceiveData += t0424OnReceiveData;
            CSPAQ12300.ReceiveData += CSPAQ12300OnReceiveData;
            t0150.ReceiveData += t0150OnReceiveData;
            t0425.ReceiveData += t0425OnReceiveData;

            this.keyVal = "";
            

        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }
        private void CSPAQ12300OnReceiveData(string tr_code)
        {


            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */

           

            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            int c1 = Convert.ToInt32(CSPAQ12300.GetFieldData("CSPAQ12300OutBlock2", "DpsastTotamt", 0));
            string c2 = CSPAQ12300.GetFieldData("CSPAQ12300OutBlock2", "D1Dps", 0);
            string c3 = CSPAQ12300.GetFieldData("CSPAQ12300OutBlock2", "D2Dps", 0);
            string c4 = CSPAQ12300.GetFieldData("CSPAQ12300OutBlock2", "PnlRat", 0);

            //this.output.Text += String.Format("t0424 => {0} {1} {2} {3}", c1, c2, c3, c4) + Environment.NewLine;

            //this.balance_sheet.Items[0].SubItems[7].Text = string.Format("{0:0,0}", c1);
            this.balance_sheet.Items[0].SubItems[7].Text = string.Format("{0:#,0}", Convert.ToInt32(c2));
            this.balance_sheet.Items[0].SubItems[8].Text = string.Format("{0:#,0}", Convert.ToInt32(c3));
            this.balance_sheet.Items[0].SubItems[4].Text = string.Format("{0:0.00}", float.Parse(c4)*100);
            this.balance_sheet.Items[0].UseItemStyleForSubItems = false;
            if (float.Parse(c4) < 0)
            {
                this.balance_sheet.Items[0].SubItems[4].ForeColor = Color.Blue;
            }
            else
            {
                this.balance_sheet.Items[0].SubItems[4].ForeColor = Color.Red;

            }


        }

        private void t0424OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            string r1 = t0424.GetFieldData("t0424OutBlock", "sunamt", 0);  //추정순자산
            string r2 = t0424.GetFieldData("t0424OutBlock", "tappamt", 0); //평가금액
            string r3 = t0424.GetFieldData("t0424OutBlock", "tdtsunik", 0); //평가손익
            string r4 = t0424.GetFieldData("t0424OutBlock", "dtsunik", 0); // 실현손익
            string r5 = t0424.GetFieldData("t0424OutBlock", "mamt", 0); //매입금액
            
            //initialization
            this.balance_sheet.Items.Clear();

            var row1 = new ListViewItem(new[] { "", "", "", "", "", "", "", "", "" });

            this.balance_sheet.Items.Add(row1);

            this.balance_sheet.Items[0].SubItems[0].Text = string.Format("{0:N0}", Convert.ToInt32(r1));
            this.balance_sheet.Items[0].SubItems[2].Text = string.Format("{0:N0}", Convert.ToInt32(r2));
            this.balance_sheet.Items[0].SubItems[3].Text = string.Format("{0:N0}", Convert.ToInt32(r3));
            this.balance_sheet.Items[0].SubItems[6].Text = string.Format("{0:N0}", Convert.ToInt32(r4));
            this.balance_sheet.Items[0].SubItems[1].Text = string.Format("{0:N0}", Convert.ToInt32(r5));
            
            this.balance_sheet.Items[0].UseItemStyleForSubItems = false;
            if (Convert.ToInt32(r3) < 0)
            {
                this.balance_sheet.Items[0].SubItems[3].ForeColor = Color.Blue;
            }
            else
            {
                this.balance_sheet.Items[0].SubItems[3].ForeColor = Color.Red;

            }
            if (Convert.ToInt32(r4) < 0)
            {
                this.balance_sheet.Items[0].SubItems[6].ForeColor = Color.Blue;
            }
            else
            {
                this.balance_sheet.Items[0].SubItems[6].ForeColor = Color.Red;

            }
            //            

            for (int i = 0; i < this.balance_sheet.Columns.Count; i++)
            {
                this.balance_sheet.Columns[i].TextAlign = HorizontalAlignment.Center;
                this.balance_sheet.Columns[i].Width = 100;
            }

            //
            int nCount = t0424.GetBlockCount("t0424OutBlock1");
            this.balance_sheet.Items[0].SubItems[5].Text = nCount.ToString();

            string s1, s2, s3, s4, s5,p4;
            
            int p1, p2, p3;
            //
            this.stocks.Items.Clear();


            List<string> listScode= new List<string>();
            List<string> listRate = new List<string>();
            List<string> listQnt = new List<string>();
            List<string> listPrice = new List<string>();

            StockInfo si = new StockInfo(this.output, this.stocks);

            for (int i = 0; i < nCount; i++)
            {
                s1 = t0424.GetFieldData("t0424OutBlock1", "expcode", i); //종목번호

                s2 = t0424.GetFieldData("t0424OutBlock1", "hname", i); //종목번호
                s3 = t0424.GetFieldData("t0424OutBlock1", "price", i); //현재가

                
                s5 = t0424.GetFieldData("t0424OutBlock1", "janqty", i); //잔고수량
                p1 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "mamt", i)); //매입금액
                p2 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "appamt", i)); //평가금액

                p3 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                p4 = t0424.GetFieldData("t0424OutBlock1", "sunikrt", i); //평가수익률

                s4 = t0424.GetFieldData("t0424OutBlock1", "pamt", i); //평균단가
                
                if (p4 == "0.")
                    p4 = "0";
                
                var row2 = new ListViewItem(new[] { "", "", "", "", "", "", "","","" });
                row2.ToolTipText = "클릭하여 주문";
                this.stocks.Items.Add(row2);
                
                this.stocks.Items[i].SubItems[0].Text = s1;
                this.stocks.Items[i].SubItems[1].Text = s2;
                this.stocks.Items[i].SubItems[2].Text = string.Format("{0:#,0}", Convert.ToInt32(s3));
                this.stocks.Items[i].SubItems[8].Text = string.Format("{0:#,0}", Convert.ToInt32(s4));
                this.stocks.Items[i].SubItems[3].Text = string.Format("{0:#,0}", Convert.ToInt32(s5));
                this.stocks.Items[i].SubItems[4].Text = string.Format("{0:#,0}", Convert.ToInt32(p1));
                this.stocks.Items[i].SubItems[5].Text = string.Format("{0:#,0}", Convert.ToInt32(p2));
                this.stocks.Items[i].SubItems[6].Text = string.Format("{0:#,0}", Convert.ToInt32(p3));
                this.stocks.Items[i].SubItems[7].Text = p4;
                this.stocks.Items[i].UseItemStyleForSubItems = false;
                if (Convert.ToInt32(p3) < 0)
                {
                    this.stocks.Items[i].SubItems[6].ForeColor = Color.Blue;
                }
                else
                {
                    this.stocks.Items[i].SubItems[6].ForeColor = Color.Red;

                }
                if (float.Parse(p4) < 0)
                { 
                    this.stocks.Items[i].SubItems[7].ForeColor = Color.Blue;
                }
                else
                {
                    this.stocks.Items[i].SubItems[7].ForeColor = Color.Red;

                }
                //
                listScode.Add(s1);
                listRate.Add(p4);
                listQnt.Add(s5);
                listPrice.Add(s3);

            }

            try
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                //
                JObject sonSpec = new JObject(
                    new JProperty("scode", listScode.ToArray()),
                    new JProperty("sret", listRate.ToArray()),
                    new JProperty("sqnt", listQnt.ToArray()),
                    new JProperty("sprice", listPrice.ToArray())
                    );



                if (!File.Exists(path + @"\bucket.json"))
                {
                    using (FileStream fs = File.Create(path + @"\bucket.json"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sonSpec.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else File.WriteAllText(path + @"\bucket.json", sonSpec.ToString());

            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
            foreach (ColumnHeader column in this.stocks.Columns)
            {
                //column.Width = -2;
                column.Width = 100;
                column.TextAlign = HorizontalAlignment.Center;

            }
            this.stocks.Columns[1].Width = -2;

            if (nCount == 0)
                this.output.Text += String.Format("{ 'error':{ 'message':'order failed'} }") + Environment.NewLine;
            

            is_data_received = true;

        }


        private void t0150OnReceiveData(string tr_code)
        {
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            //string r1 = t0150.GetFieldData("t0150OutBlock", "mdamt", 0);
            string r12 = t0150.GetFieldData("t0150OutBlock", "mdadjamt", 0);

            this.output.Text += r12 + Environment.NewLine;
            int nCount = t0150.GetBlockCount("t0150OutBlock1");
            //MessageBox.Show(nCount.ToString());


        }


        private void t0425OnReceiveData(string tr_code)
        {
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            //string r1 = t0150.GetFieldData("t0150OutBlock", "mdamt", 0);
            //string r12 = t0150.GetFieldData("t0150OutBlock", "mdadjamt", 0);

            //this.output.Text += r1 + Environment.NewLine;
            int nCount = t0425.GetBlockCount("t0425OutBlock1");
            MessageBox.Show(nCount.ToString());


        }

        public void end()
        {
            t0424.RemoveService("t0424", this.keyVal);
        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t0424.SetFieldData("t0424InBlock", "accno", 0, this.account_number);
            t0424.SetFieldData("t0424InBlock", "passwd", 0, this.account_pwd);
            t0424.SetFieldData("t0424InBlock", "prcgb", 0, "1");
            t0424.SetFieldData("t0424InBlock", "chegb", 0, "2");
            t0424.SetFieldData("t0424InBlock", "dangb", 0, "0");
            t0424.SetFieldData("t0424InBlock", "charge", 0, "0");
            t0424.SetFieldData("t0424InBlock", "cts_expcode", 0, "");
            //tr요청
            t0424.Request(false);

            CSPAQ12300.SetFieldData("CSPAQ12300InBlock1", "RecCnt", 0, "00001");
            CSPAQ12300.SetFieldData("CSPAQ12300InBlock1", "AcntNo", 0, this.account_number);
            CSPAQ12300.SetFieldData("CSPAQ12300InBlock1", "Pwd", 0, this.account_pwd);
            CSPAQ12300.SetFieldData("CSPAQ12300InBlock1", "BalCreTp", 0, "0");

            //tr요청
            CSPAQ12300.Request(false);
            //
            t0150.SetFieldData("t0150InBlock", "accno", 0, this.account_number);
            t0150.SetFieldData("t0150InBlock", "cts_medosu", 0, "1");
            //
            t0150.Request(false);

            t0425.SetFieldData("t0425InBlock", "accno", 0, this.account_number);
            t0425.SetFieldData("t0425InBlock", "passwd", 0, this.account_pwd);
            //t0424.SetFieldData("t0424InBlock", "prcgb", 0, "1");
            t0425.SetFieldData("t0425InBlock", "chegb", 0, "2");
            //t0424.SetFieldData("t0424InBlock", "dangb", 0, "0");
            //t0424.SetFieldData("t0424InBlock", "charge", 0, "0");
            //t0424.SetFieldData("t0424InBlock", "cts_expcode", 0, "");
            //t0425.Request(false);
        }
    }

}
