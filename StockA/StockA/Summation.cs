using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;

namespace StockA
{
    class Summation
    {
        private XAQueryClass CSPAQ12200;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;

        ListView balance_sheet;

        public TextBox output;
        public Summation(TextBox output, ListView balance_sheet)
        {
            this.output = output;
            this.balance_sheet = balance_sheet;

            this.is_data_received = false;

            CSPAQ12200 = new XAQueryClass();
            CSPAQ12200.ResFileName = @"C:\eBEST\xingAPI\Res\CSPAQ12200.res"; //RES 파일 등록
            CSPAQ12200.ReceiveData += OnReceiveData;

            this.keyVal = "";
            // 모의투자
            this.account_number = "";
            this.account_pwd = "";


        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }


        private void OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;


            string r1 = CSPAQ12200.GetFieldData("CSPAQ12200OutBlock2", "DpsastTotamt", 0);
            string r2 = CSPAQ12200.GetFieldData("CSPAQ12200OutBlock2", "D1Dps", 0);
            string r3 = CSPAQ12200.GetFieldData("CSPAQ12200OutBlock2", "D2Dps", 0);
            string r4 = CSPAQ12200.GetFieldData("CSPAQ12200OutBlock2", "PnlRat", 0);   //손익률
            string r5 = CSPAQ12200.GetFieldData("CSPAQ12200OutBlock2", "BalEvalAmt", 0);  //잔고평가금액

            this.output.Text += String.Format("[예탁자산총액 : {0} [D+1 예수금 : {1}] [D+2 예수금 {2}]", r1, r2, r3) + Environment.NewLine;

            string[] row = { r1, r2, r3, r4, r5 };
            var listViewItem = new ListViewItem(row);

            this.balance_sheet.Items.Add(listViewItem);

        }
        public void end()
        {
            CSPAQ12200.RemoveService("CSPAQ12200", this.keyVal);
        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            CSPAQ12200.SetFieldData("CSPAQ12200InBlock1", "RecCnt", 0, "00001");
            CSPAQ12200.SetFieldData("CSPAQ12200InBlock1", "MgmtBrnNo", 0, " ");
            CSPAQ12200.SetFieldData("CSPAQ12200InBlock1", "AcntNo", 0, "55501502101");
            CSPAQ12200.SetFieldData("CSPAQ12200InBlock1", "Pwd", 0, "0000");
            CSPAQ12200.SetFieldData("CSPAQ12200InBlock1", "BalCreTp", 0, "0");

            //tr요청
            int result = CSPAQ12200.Request(false);

        }
    }
}
