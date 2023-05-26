using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace StockA
{
    class PendStock
    {
        private XAQueryClass t0425;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;



        public PendStock(string accno, string accpw)
        {

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

            List<string> listCode = new List<string>();
            List<string> listQty = new List<string>();

            for (int i = 0; i < nCount; i++)
            {


                ord_no = t0425.GetFieldData("t0425OutBlock1", "ordno", i); //주문번호        
                ord_code = t0425.GetFieldData("t0425OutBlock1", "expcode", i); //종목번호
                ord_name = t0425.GetFieldData("t0425OutBlock1", "hname", i); //종목명
                ord_side = t0425.GetFieldData("t0425OutBlock1", "medosu", i); //구분
                org_qty = t0425.GetFieldData("t0425OutBlock1", "qty", i); //주문수량
                ord_price = t0425.GetFieldData("t0425OutBlock1", "price", i); //주문가격
                done_price = t0425.GetFieldData("t0425OutBlock1", "cheprice", i); //주문가격
                done_qty = t0425.GetFieldData("t0425OutBlock1", "cheqty", i); //체결수량
                ord_time = t0425.GetFieldData("t0425OutBlock1", "ordtime", i); //주문시각

                listCode.Add(ord_code);
                listQty.Add(done_qty);
                
            }


            try
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                //
                JObject sonSpec = new JObject(
                    new JProperty("scode", listCode.ToArray()),
                    new JProperty("sqry", listQty.ToArray())
                    
                    );



                if (!File.Exists(path + @"\pendst.json"))
                {
                    using (FileStream fs = File.Create(path + @"\pendst.json"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sonSpec.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else File.WriteAllText(path + @"\pendst.json", sonSpec.ToString());

            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
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
