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
    class SearchSt
    {
        private XAQueryClass t1857;

        public bool is_data_received;
        public string keyVal;
        public string account_number;
        public string account_pwd;

        ListView balance_sheet;

        public TextBox output;
        public SearchSt(TextBox output)
        {
            this.output = output;
            
            this.is_data_received = false;

            t1857 = new XAQueryClass();
            t1857.ResFileName = @"C:\eBEST\xingAPI\Res\t1857.res"; //RES 파일 등록
            t1857.ReceiveData += OnReceiveData;

            this.keyVal = "";
            // 모의투자
            this.account_number = "";
            this.account_pwd = "0000";


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


            string r1 = t1857.GetFieldData("t1857OutBlock", "result_count", 0);
            this.keyVal = t1857.GetFieldData("t1857OutBlock", "AlertNum", 0);
            
            this.output.Text += String.Format("검색된 종목수 => {0}" ,r1) + Environment.NewLine;
            this.output.Text += String.Format("API Key =>  {0}", this.keyVal) + Environment.NewLine;
            

        }
        public void end()
        {
            t1857.RemoveService("t1857", this.keyVal);
        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t1857.SetFieldData("t1857InBlock", "sRealFlag", 0, "1");
            t1857.SetFieldData("t1857InBlock", "sSearchFlag", 0, "S");
            t1857.SetFieldData("t1857InBlock", "query_index", 0, "");
            
            //tr요청
            int result = t1857.RequestService("t1857", "");

        }
    }
}
