using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public string method;
        public int km;
        public RichTextBox output;
        public ListView bucket;
        public ListView notyet;
        private string account_number, account_pwd;
        public Order od;

        public SearchSt(RichTextBox output, ListView bucket, ListView notyet, string id, string accno, 
                            string accpw, string method, int km)
        {
            
            this.output = output;
            this.bucket = bucket;
            this.notyet = notyet;
            this.id = id;
            this.method = method;
            this.km = km;
            this.is_data_received = false;

            t1857 = new XAQueryClass();
            t1857.ResFileName = @"C:\eBEST\xingAPI\Res\t1857.res"; //RES 파일 등록
            t1857.ReceiveData += OnReceiveData;

            this.keyVal = "";

            // 모의투자
            this.account_number = accno;
            this.account_pwd = accpw;
            od = new Order(this.output, this.account_number, this.account_pwd);

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

            //보유종목리스트
            List<string> AuthorList = new List<string>();
            Bucket items = new Bucket();
            
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\bucket.json"))
            {
                string json = r.ReadToEnd();

                items = JsonConvert.DeserializeObject<Bucket>(json);
                //MessageBox.Show(items.scode.Length.ToString());
                foreach (string code in items.scode)
                {
                    //Console.WriteLine(code);
                    AuthorList.Add(code);
                }
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

            //getBucketItem();
            

            string shcode, hname, price;
            if (nCount > 0)
            {
                for (int i = 0; i < 2; i++)
                {

                    shcode = t1857.GetFieldData("t1857OutBlock1", "shcode", i);
                    hname = t1857.GetFieldData("t1857OutBlock1", "hname", i);
                    price = t1857.GetFieldData("t1857OutBlock1", "price", i);

                    //check if the stock exist in a bucket
                    // 보유종목 리스트에 있으면 제외
                    List<string> bucketItem = getBucketItem();
                    bool isDup = false;
                    
                    foreach (string bl in bucketItem)
                    {
                        //MessageBox.Show(bl);
                        if (shcode == bl)
                        {
                            //MessageBox.Show(shcode);
                            isDup = true;
                            break;
                        }
                            
                    }
                    //미체결 주문 목록에 있으면 제외
                    List<string> AuthorList = new List<string>();
                    foreach (ListViewItem sl in this.notyet.Items)
                    {
                        AuthorList.Add(sl.SubItems[1].Text);
                    }

                    foreach (string bl in AuthorList)
                    {

                        if (bl == shcode)
                        {
                            isDup = true;
                            break;
                        }

                    }
                    //order it
                    if (!isDup)
                    {
                        if (method == "km")
                        {

                            //금액으로 매수                            
                            string qnt = getQnt(price, this.km.ToString());
                            this.output.Text += String.Format("{0} {1}주", shcode, qnt) + "를 매수합니다" + Environment.NewLine;
                            string[] param = {shcode, price, qnt};
                            Thread th = new Thread(new ParameterizedThreadStart(requestact));
                            th.Start(param);
                            Thread.Sleep(100);
                            

                        }
                        else
                        {
                            //수량으로 매수
                            /*
                            od.request(shcode, price, "2", "15");
                            this.output.Text += shcode +"를 매수합니다" + Environment.NewLine;
                            */
                        }
                        
                    }
                }
            }

        }
        public void requestact(object param)
        {
            string[] data = param as string[];
            //od = new Order(this.output, this.account_number, this.account_pwd);
            od.request(data[0], data[1], "2", data[2]);
            od.end();
        }
        public string getQnt(string price, string notes)
        {
            int qn = Convert.ToInt32(notes) / Convert.ToInt32(price);

            return qn.ToString();
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