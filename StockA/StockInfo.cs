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
    class StockInfo
    {

        private XAQueryClass t1302;

        public bool is_data_received;
        public string keyVal;
        public int index;
        public RichTextBox output;
        public ListView lst;
        public StockInfo(RichTextBox output, ListView stocks)
        {
            this.output = output;
            this.lst = stocks;

            this.is_data_received = false;

            t1302 = new XAQueryClass();
            t1302.ResFileName = @"C:\eBEST\xingAPI\Res\t1302.res"; //RES 파일 등록
            //
            t1302.ReceiveData += t1302OnReceiveData;

            this.keyVal = "";
            this.index = 0;
        }


        private void t1302OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            
            string r1 = t1302.GetFieldData("t1302OutBlock1", "close", 0); 
            string r2 = t1302.GetFieldData("t1302OutBlock1", "sign", 0); 
            string r3 = t1302.GetFieldData("t1302OutBlock1", "change", 0);
            string r4 = t1302.GetFieldData("t1302OutBlock1", "diff", 0);


            //

            this.output.Text += String.Format("현재가:{0} 전일대비:{1} 등락률:", string.Format("{0:#,0}", Convert.ToInt32(r1)), r3);
            
            this.output.SelectionStart = this.output.Text.Length;
            this.output.SelectionLength = 0;
            

            if (r4.Contains("-"))
            {
                this.output.SelectionColor = Color.Blue;
            }
            else
                this.output.SelectionColor = Color.Red;

            this.output.AppendText(String.Format(r4) + Environment.NewLine);
            this.output.SelectionColor = Color.Black;

            //return r4;
        }
        public void end()
        {
            t1302.RemoveService("t1302", this.keyVal);

        }

        public void request(string stockcode)
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t1302.SetFieldData("t1302InBlock", "shcode", 0, stockcode);
            //t1302.SetFieldData("t1302InBlock", "gubun", 0, stockcode);
            //tr요청
            t1302.Request(false);

        }
    }
}
