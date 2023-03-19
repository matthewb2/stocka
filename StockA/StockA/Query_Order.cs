using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;
using XA_SESSIONLib;

namespace StockA
{
    class Order
    {
        XAQueryClass CSPAT00600;

        bool is_data_received;
        string keyVal;
        string account_number;
        string account_pwd;
        string side_type;
        string side;
        private Order() {

            CSPAT00600 = new XAQueryClass();
            CSPAT00600.ReceiveData += OnReceiveData;

            this.is_data_received = false;
            this.keyVal = "";
            // 모의투자
            this.account_number = "55501502101";
            this.account_pwd = "0000";
        }

        void OnReceiveData(string tr_code) {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            this.is_data_received = true;
            //print("TR code => {0}".format(tr_code))

            int nCount = CSPAT00600.GetBlockCount("CSPAT00600OutBlock1");
                    string rec_cnt, ord_qty, ord_price, ord_side, ord_num;
            for (int i = 0; i <= nCount; i++) {
                rec_cnt = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "RecCnt", i);
                ord_qty = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "OrdQty", i); //주문수량
                ord_price = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "OrdPrc", i); //주문가
                ord_side = CSPAT00600.GetFieldData("CSPAT00600OutBlock1", "BnsTpCode", i); //매매구분
                ord_num = CSPAT00600.GetFieldData("CSPAT00600OutBlock2", "OrdNo", i); //주문번호
            }
            this.side_type = "ask";
                if (this.side == "2")  // 매수
                    this.side_type = "bid";

            //print({ 'uuid':ord_num, 'side': side_type, 'status': 1, 'org_qty' : str(qty), 'done_qty' : '0.0', 'price':str(price)})
            if (nCount == 0) {
                    //print({ 'error':{ 'message':'order failed'} })
                }
         }
        public void end() {
            CSPAT00600.RemoveService("CSPAT00600", this.keyVal);
        }

        public void request(string code, string qty, string price, string side) {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            CSPAT00600.ResFileName = @"C:\eBEST\xingAPI\Res\CSPAT00600.res"; //RES 파일 등록

            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "AcntNo", 0, this.account_number);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "InptPwd", 0, this.account_pwd);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "IsuNo", 0, code);
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdQty", 0, qty); //주문수량
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdPrc", 0, price); //주문가
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "BnsTpCode", 0, side); //매매구분) side : 2 (매수)  1(매도)
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdprcPtnCode", 0, "00"); //호가유형코드
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "MgntrnCode", 0, "000"); //신용거래코드
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "LoanDt", 0, "0"); //대출일
            CSPAT00600.SetFieldData("CSPAT00600InBlock1", "OrdCndiTpCode", 0, "0"); //주문조건구분
            CSPAT00600.Request(false);
        }
    }
}
