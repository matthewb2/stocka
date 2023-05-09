using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XA_DATASETLib;

namespace StockA
{
    class Order
    {
        XAQueryClass CSPAT00600;

        string keyVal;
        string account_number;
        string account_pwd;
        string method;

        public RichTextBox output;

        public Order(RichTextBox output, string accno, string accpw) {
            this.output = output;
            //this.method = method;
            CSPAT00600 = new XAQueryClass();
            CSPAT00600.ResFileName = @"C:\eBEST\xingAPI\Res\CSPAT00600.res"; //RES 파일 등록

            CSPAT00600.ReceiveData += OnReceiveData;

            this.keyVal = "";
            // 모의투자
            this.account_number = accno;
            this.account_pwd = accpw;
        }

        void OnReceiveData(string tr_code) {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            
            int nCount = CSPAT00600.GetBlockCount("CSPAT00600OutBlock1");

            string rec_cnt, ord_qty, ord_price, ord_side, ord_num, ord_time;

            for (int i = 0; i <= nCount; i++) {
                rec_cnt = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "RecCnt", i);
                ord_qty = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "OrdQty", i); //주문수량
                ord_price = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "OrdPrc", i); //주문가
                ord_side = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "BnsTpCode", i); //매매구분
                ord_num = CSPAT00600.GetFieldData("CSPAT00600OutBlock2", "OrdNo", i); //주문번호
                ord_time = CSPAT00600.GetFieldData("CSPAT00600OutBlock2", "OrdNo", i); //주문시각

                this.output.Text += String.Format("주문번호 => {0} {1}:{2}:{3} {4} 주문이 접수되었습니다",ord_num, ord_time.Substring(0,2), ord_time.Substring(2, 4), ord_time.Substring(4, 6), ord_side) + Environment.NewLine;

            }
            if (nCount == 0) 
                Console.WriteLine("error:{ 'message':'order failed'}");
            
         }
        public void end() {
            CSPAT00600.RemoveService("CSPAT00600", this.keyVal);
        }

        public void request(string code, string price, string sorb, string qty) {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "AcntNo", 0, this.account_number);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "InptPwd", 0, this.account_pwd);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "IsuNo", 0, code);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdQty", 0, qty); //주문수량
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdPrc", 0, price); //주문가
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "BnsTpCode", 0, sorb); //매매구분) side : 2 (매수)  1(매도)
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdprcPtnCode", 0, "00"); //호가유형코드
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "MgntrnCode", 0, "000"); //신용거래코드
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "LoanDt", 0, "0"); //대출일
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdCndiTpCode", 0, "0"); //주문조건구분

            
            //tr요청
            int result = CSPAT00600.Request(false);
            
            if (result < 0)
                MessageBox.Show("error");
        }
    }
}
