using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace StockA
{
    class Strategy
    {
        private XAQueryClass t1866;

        public bool is_data_received;
        public string id, keyVal;
        
        //ListView balance_sheet;
        ComboBox cb_strategyA;

        //public TextBox output;
        public Strategy(ComboBox cb, string id)
        {
            this.id = id;
            cb_strategyA = cb;

            this.is_data_received = false;

            t1866 = new XAQueryClass();
            t1866.ResFileName = @"C:\eBEST\xingAPI\Res\t1866.res"; //RES 파일 등록
            t1866.ReceiveData += OnReceiveData;

            this.keyVal = "";
            
        }

        private void OnReceiveData(string tr_code)
        {
            /*
            이베스트 서버에서 ReceiveData 이벤트 받으면 실행되는 event handler
            */
            
            string r1 = t1866.GetFieldData("t1866OutBlock", "result_count", 0);
            int nCount = Convert.ToInt32(r1);

            System.Object[] ItemObject = new System.Object[nCount-1];
            if (nCount > 0)
            {
                for (int i = 0; i < nCount-1; i++)
                {

                    string r2 = t1866.GetFieldData("t1866OutBlock1", "query_index", 0);
                    string r3 = t1866.GetFieldData("t1866OutBlock1", "group_name", 0);
                    string r4 = t1866.GetFieldData("t1866OutBlock1", "query_name", 0);

                    ItemObject[i] = String.Format("그룹[{0}]:{1}", r3, r4);

                    Console.WriteLine(String.Format("{0} {1} {2}", r2, r3, r4));
                    cb_strategyA.Items.AddRange(ItemObject);
                }
                cb_strategyA.SelectedIndex = 0;
                
            }
        }
        public void end()
        {
            t1866.RemoveService("t1866", this.keyVal);
        }

        public void request()
        {
            /*
            이베스트 서버에 일회성 TR data 요청함.
            */
            t1866.SetFieldData("t1866InBlock", "user_id", 0, this.id);
            t1866.SetFieldData("t1866InBlock", "gb", 0, "0");
            t1866.SetFieldData("t1866InBlock", "group_name", 0, "0");
            t1866.SetFieldData("t1866InBlock", "cont", 0, "0");
            t1866.SetFieldData("t1866InBlock", "cont_key", 0, "");

            //tr요청
            int result = t1866.Request(false);
            if (result <0)
                MessageBox.Show("error");
        }
    }

}
