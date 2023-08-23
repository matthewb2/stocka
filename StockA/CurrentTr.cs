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
    class CurrentTr
    {
        private XAQueryClass t0150;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;
        public string Val_return;


        public ListView listView1;

        public CurrentTr(ListView cur_sheet, string accno, string accpw, string return_value)
        {
            listView1 = cur_sheet;

            this.is_data_received = false;

            t0150 = new XAQueryClass();
            t0150.ResFileName = @"C:\eBEST\xingAPI\Res\t0150.res"; //RES 파일 등록
            //
            t0150.ReceiveData += t0150OnReceiveData;


            this.keyVal = "";
            // 모의투자
            this.account_number = accno;
            this.account_pwd = accpw;
            this.Val_return = return_value;

            listView1.Items.Clear();
            var row1 = new ListViewItem(new[] { "", "", "","","" });
            listView1.Items.Add(row1);


        }

        private void t0150OnReceiveData(string tr_code)
        {

            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            listView1.Items[0].SubItems[2].Text = string.Format("{0:N0}", Convert.ToInt32(this.Val_return));
            //

            string r1 = t0150.GetFieldData("t0150OutBlock", "mdamt", 0);
            string r2 = t0150.GetFieldData("t0150OutBlock", "mdtax", 0);
            string r3 = this.Val_return;
            string r4 = t0150.GetFieldData("t0150OutBlock", "msamt", 0);

            listView1.Items[0].SubItems[0].Text = string.Format("{0:N0}", Convert.ToInt32(r1));
            listView1.Items[0].SubItems[1].Text = string.Format("{0:N0}", Convert.ToInt32(r2));
            listView1.Items[0].SubItems[2].Text = string.Format("{0:N0}", Convert.ToInt32(r3));

            int ret = Convert.ToInt32(this.Val_return);
            int amount = Convert.ToInt32(r1);

            double Val_return = (double)ret / (double)amount;
            listView1.Items[0].SubItems[3].Text = string.Format("{0:00}%", (Val_return*100).ToString("0.00"));

            listView1.Items[0].SubItems[4].Text = string.Format("{0:N0}", Convert.ToInt32(r4));

            listView1.Items[0].UseItemStyleForSubItems = false;
            if (Convert.ToInt32(r3) < 0)
            {
                listView1.Items[0].SubItems[2].ForeColor = Color.Blue;
                listView1.Items[0].SubItems[3].ForeColor = Color.Blue;
            }
            else
            {
                listView1.Items[0].SubItems[2].ForeColor = Color.Red;
                listView1.Items[0].SubItems[3].ForeColor = Color.Red;

            }

        }
        public void end()
        {
            t0150.RemoveService("t0150", this.keyVal);

        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t0150.SetFieldData("t0150InBlock", "accno", 0, this.account_number);
            
            //tr요청
            t0150.Request(false);


        }
    }
}

