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
        public string id;
        public TextBox output;
        public ListView bucket;
        private string account_number, account_pwd;

        public SearchSt(TextBox output, ListView bucket, string id, string accno, string accpw)
        {
            this.output = output;
            this.bucket = bucket;
            this.id = id;

            this.is_data_received = false;

            t1857 = new XAQueryClass();
            t1857.ResFileName = @"C:\eBEST\xingAPI\Res\t1857.res"; //RES 파일 등록
            t1857.ReceiveData += OnReceiveData;

            this.keyVal = "";

            // 모의투자
            this.account_number = accno;
            this.account_pwd = accpw;

        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }

        public List<string> getBucketItem()
        {
            List<string> AuthorList = new List<string>();
            foreach (ListViewItem sl in this.bucket.Items)
            {
                //this.output.Text += sl.SubItems[0].Text + Environment.NewLine;
                AuthorList.Add(sl.SubItems[0].Text);
            }
            return AuthorList;
        }
        private void OnReceiveData(string tr_code)
        {
            //get a stock list
            
            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            string r1 = t1857.GetFieldData("t1857OutBlock", "result_count", 0);
            this.keyVal = t1857.GetFieldData("t1857OutBlock", "AlertNum", 0);

            this.output.Text += String.Format("검색된 종목수 => {0}", r1) + Environment.NewLine;
            this.output.Text += String.Format("API Key =>  {0}", this.keyVal) + Environment.NewLine;

            int nCount = Convert.ToInt32(r1);


            //this.output.Text += "보유종목 =>" + Environment.NewLine;

            getBucketItem();
            Order od = new Order(this.output, this.account_number, this.account_pwd);

            string shcode, hname, price;
            if (nCount > 0)
            {
                for (int i = 0; i < nCount; i++)
                {

                    shcode = t1857.GetFieldData("t1857OutBlock1", "shcode", i);
                    hname = t1857.GetFieldData("t1857OutBlock1", "hname", i);
                    price = t1857.GetFieldData("t1857OutBlock1", "price", i);

                    //check if the stock exist in a bucket
                    // 보유종목 리스트에 있으면 제외
                    List<string> bucketItem = getBucketItem();
                    bool isBucket = false;

                    foreach (string bl in bucketItem)
                    {   
                        
                        if (shcode == bl)
                        {
                            isBucket = true;
                            break;
                        }
                            
                    }
                    //미체결 주문 목록에 있으면 제외

                    //order it
                    if (!isBucket)
                    {
                        od.request(shcode, price, "2", "15");
                        this.output.Text += shcode + Environment.NewLine;

                        od.end();
                        
                    }
                }
            }

        }
        public void end()
        {
            t1857.RemoveService("t1857", this.keyVal);
        }

        public void request()
        {
            t1857.SetFieldData("t1857InBlock", "sRealFlag", 0, "0"); //실시간 조회는 실계좌일 때만 가능
            t1857.SetFieldData("t1857InBlock", "sSearchFlag", 0, "S");
            t1857.SetFieldData("t1857InBlock", "query_index", 0, this.id+"  0000");

            //tr요청
            int result = t1857.RequestService("t1857", "");
            if (result <0)
                MessageBox.Show("error");

        }
    }
}