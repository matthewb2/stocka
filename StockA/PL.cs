using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;


namespace StockA
{
    class PL
    {
        private XAQueryClass CDPCQ04700;

        public bool is_data_received;
        public string keyVal;

        public string account_number;
        public string account_pwd;

        public RichTextBox output;
        public PL(RichTextBox output, ListView sheet, string acc_number, string acc_pwd)
        {
            this.output = output;
            this.account_number = acc_number;
            this.account_pwd = acc_pwd;

            this.is_data_received = false;

            CDPCQ04700 = new XAQueryClass();
            CDPCQ04700.ResFileName = @"C:\eBEST\xingAPI\Res\CDPCQ04700.res"; //RES 파일 등록
            //
            CDPCQ04700.ReceiveData += CDPCQ04700OnReceiveData;

            this.keyVal = "";

        }


        private void CDPCQ04700OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;
            String blockName1 = "CDPCQ04700OutBlock1";
            String blockName2 = "CDPCQ04700OutBlock2";

            string r1 = CDPCQ04700.GetFieldData("CDPCQ04700OutBlock1", "AcntNo", 0);

            int nCount = CDPCQ04700.GetBlockCount("CDPCQ04700OutBlock3");

            this.output.Text += String.Format("{0} {1} ", r1, nCount) + Environment.NewLine;
            
            for (int i = 0; i < nCount; i++)
            {
                //string r1 = CDPCQ04700.GetFieldData("CDPCQ04700OutBlock2", "InvstPlAmt", 0);
                //string r2 = CDPCQ04700.GetFieldData("CDPCQ04700OutBlock2", "InvstErnrat", 0);
                string r3 = CDPCQ04700.GetFieldData("CDPCQ04700OutBlock3", "EvalAmt", i);
                this.output.Text += String.Format("{0}", r3) + Environment.NewLine;
            }
            
        }
        public void end()
        {
            CDPCQ04700.RemoveService("CDPCQ04700", this.keyVal);

        }

        public void request(string start, string end)
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            String blockName = "CDPCQ04700InBlock1";

            CDPCQ04700.SetFieldData(blockName, "RecCnt", 0, "00001");
            CDPCQ04700.SetFieldData(blockName, "QryTp", 0, "0");
            CDPCQ04700.SetFieldData(blockName, "AcntNo", 0, this.account_number);
            CDPCQ04700.SetFieldData(blockName, "Pwd", 0, this.account_pwd);
            CDPCQ04700.SetFieldData(blockName, "QrySrtDt", 0, "20230510");
            CDPCQ04700.SetFieldData(blockName, "QryEndDt", 0, "20230527");
            
            //tr요청
            CDPCQ04700.Request(false);

        }
    }
}
