using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;

namespace StockA
{
    class HoldStock
    {
        private XAQueryClass t0424;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;



        public RichTextBox output;
        public HoldStock(RichTextBox output, string accno, string accpw)
        {
            this.output = output;
            //모의투자
            this.account_number = accno;
            this.account_pwd = accpw;


            this.is_data_received = false;

            t0424 = new XAQueryClass();
            t0424.ResFileName = @"C:\eBEST\xingAPI\Res\t0424.res"; //RES 파일 등록
            //
            t0424.ReceiveData += t0424OnReceiveData;

            this.keyVal = "";


        }


        private void t0424OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */

            string r1 = t0424.GetFieldData("t0424OutBlock", "sunamt", 0);  //추정순자산
            string r2 = t0424.GetFieldData("t0424OutBlock", "tappamt", 0); //평가금액
            string r3 = t0424.GetFieldData("t0424OutBlock", "tdtsunik", 0); //평가손익
            string r4 = t0424.GetFieldData("t0424OutBlock", "dtsunik", 0); // 실현손익
            string r5 = t0424.GetFieldData("t0424OutBlock", "mamt", 0); //매입금액


            int nCount = t0424.GetBlockCount("t0424OutBlock1");

            Console.WriteLine(nCount.ToString());
            
            string s1, s2, s3, s4, s5, p4;
            int p1, p2, p3;
            //
            
            for (int i = 0; i < nCount; i++)
            {
                s1 = t0424.GetFieldData("t0424OutBlock1", "expcode", i); //종목번호

                s2 = t0424.GetFieldData("t0424OutBlock1", "hname", i); //종목번호
                s3 = t0424.GetFieldData("t0424OutBlock1", "price", i); //종목번호

                s4 = t0424.GetFieldData("t0424OutBlock1", "pamt", i); //평균단가
                s5 = t0424.GetFieldData("t0424OutBlock1", "janqty", i); //잔고수량
                p1 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "mamt", i)); //매입금액
                p2 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "appamt", i)); //평가금액

                p3 = Int32.Parse(t0424.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                p4 = t0424.GetFieldData("t0424OutBlock1", "sunikrt", i); //수익률



            }

            if (nCount == 0)
            {
                this.output.Text += String.Format("{ 'error':{ 'message':'order failed'} }") + Environment.NewLine;
            }

            is_data_received = true;

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

        }
    }
}
