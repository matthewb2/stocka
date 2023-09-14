using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;
using System.Drawing;

namespace StockA
{
    class OrderCancel
    {
        private XAQueryClass t0425;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;


        public ListView listView1;

        public string convertSN(string code)
        {
            StreamReader sr = new StreamReader("stocklist.csv", Encoding.GetEncoding("ks_c_5601-1987"));
            //
            Dictionary<string, string> nameToCode = new Dictionary<string, string>();
            string sName = "";
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                nameToCode.Add(data[0], data[1]);

            }
            foreach (KeyValuePair<string, string> kvp in nameToCode)
            {
                if (kvp.Key == code)
                    sName = kvp.Value;
            }

            return sName;
        }
        public OrderCancel(ListView cur_sheet, string accno, string accpw)
        {
            listView1 = cur_sheet;

            this.is_data_received = false;

            t0425 = new XAQueryClass();
            t0425.ResFileName = @"C:\eBEST\xingAPI\Res\t0425.res"; //RES 파일 등록
            //
            t0425.ReceiveData += t0425OnReceiveData;

            this.keyVal = "";
            // 모의투자
            this.account_number = accno;
            this.account_pwd = accpw;

        }


        private void t0425OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */

            //
            int nCount = t0425.GetBlockCount("t0425OutBlock1");
            string ord_no, ord_code, ord_name, ord_side, org_qty, ord_price, done_price, done_qty, ord_time;

            listView1.Items.Clear();

            for (int i = 0; i < nCount; i++)
            {
                var row1 = new ListViewItem(new[] { "", "", "", "", "", "", "" });
                listView1.Items.Add(row1);


                ord_no = t0425.GetFieldData("t0425OutBlock1", "ordno", i); //주문번호        
                ord_code = t0425.GetFieldData("t0425OutBlock1", "expcode", i); //종목번호
                ord_name = t0425.GetFieldData("t0425OutBlock1", "hname", i); //종목명
                ord_side = t0425.GetFieldData("t0425OutBlock1", "medosu", i); //구분
                org_qty = t0425.GetFieldData("t0425OutBlock1", "qty", i); //주문수량
                ord_price = t0425.GetFieldData("t0425OutBlock1", "price", i); //주문가격
                done_price = t0425.GetFieldData("t0425OutBlock1", "cheprice", i); //주문가격
                done_qty = t0425.GetFieldData("t0425OutBlock1", "cheqty", i); //체결수량
                ord_time = t0425.GetFieldData("t0425OutBlock1", "ordtime", i); //주문시각

                ord_time = ord_time.Substring(0, 2) + ":" + ord_time.Substring(2, 2) + ":" + ord_time.Substring(4, 2);

                listView1.Items[i].SubItems[0].Text = string.Format(ord_no);
                listView1.Items[i].SubItems[1].Text = string.Format("{0:0,0}", ord_code);
                listView1.Items[i].SubItems[2].Text = string.Format(convertSN(ord_code));
                listView1.Items[i].SubItems[3].Text = string.Format(ord_side);
                listView1.Items[i].SubItems[4].Text = string.Format("{0:0,0}", done_qty);
                listView1.Items[i].SubItems[5].Text = string.Format("{0:0,0}", org_qty);
                listView1.Items[i].SubItems[6].Text = ord_time;

                listView1.Items[i].UseItemStyleForSubItems = false;
                if (ord_side == "매도")
                {
                    listView1.Items[i].SubItems[3].ForeColor = Color.Blue;
                }
                else
                {
                    listView1.Items[i].SubItems[3].ForeColor = Color.Red;

                }

            }





        }
        public void end()
        {
            t0425.RemoveService("t0425", this.keyVal);

        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t0425.SetFieldData("t0425InBlock", "accno", 0, this.account_number);
            t0425.SetFieldData("t0425InBlock", "passwd", 0, this.account_pwd);
            t0425.SetFieldData("t0425InBlock", "expcode", 0, ""); //종목번호 or blank(all)
            t0425.SetFieldData("t0425InBlock", "chegb", 0, "0"); //미체결 :'?' 체결: '1' 전체:'0'
            t0425.SetFieldData("t0425InBlock", "medosu", 0, "0"); //매매구분
            t0425.SetFieldData("t0425InBlock", "sortgb", 0, "2");
            //tr요청
            t0425.Request(false);

        }
    }
}

