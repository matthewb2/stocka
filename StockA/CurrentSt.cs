using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;


namespace StockA
{
    class CurrentSt
    {
        private XAQueryClass t1101;
        
        public bool is_data_received;
        public string keyVal;
        
        public TextBox output;
        public CurrentSt(TextBox output, ListView balance_sheet, ListView stocks)
        {
            this.output = output;

            this.is_data_received = false;

            t1101 = new XAQueryClass();
            t1101.ResFileName = @"C:\eBEST\xingAPI\Res\t1101.res"; //RES 파일 등록
            //
            t1101.ReceiveData += t1101OnReceiveData;

            this.keyVal = "";
            // 모의투자
            //this.account_number = "55501502101";
            //this.account_pwd = "0000";


        }


        private void t1101OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            string r1 = t1101.GetFieldData("t1101OutBlock", "sunamt", 0);  //추정순자산
            string r2 = t1101.GetFieldData("t1101OutBlock", "tappamt", 0); //평가금액
            string r3 = t1101.GetFieldData("t1101OutBlock", "tdtsunik", 0); //평가손익
            string r4 = t1101.GetFieldData("t1101OutBlock", "dtsunik", 0); // 실현손익
            string r5 = t1101.GetFieldData("t1101OutBlock", "mamt", 0); //매입금액


            //
            int nCount = t1101.GetBlockCount("t1101OutBlock1");
            
            this.output.Text += String.Format("ncount: {0}", nCount) + Environment.NewLine;
            


        }
        public void end()
        {
            t1101.RemoveService("t1101", this.keyVal);
            
        }

        public void request(string stockcode)
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t1101.SetFieldData("t1101InBlock", "shcode", 0, stockcode);
            //tr요청
            t1101.Request(false);

        }
    }
}
