using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public string st_num;
        public int km;
        public RichTextBox output;
        public ListView bucket;
        private string acc_number, acc_pwd;
        PendStock pds;

        public SearchSt(RichTextBox output, ListView bucket, string id, string accno, string accpw, int km)
        {
            this.output = output;
            this.bucket = bucket;
            this.id = id;
            this.km = km;

            this.is_data_received = false;

            t1857 = new XAQueryClass();
            t1857.ResFileName = @"C:\eBEST\xingAPI\Res\t1857.res"; //RES 파일 등록
            t1857.ReceiveData += OnReceiveData;

            this.keyVal = "";

            // 모의투자
            this.acc_number = accno;
            this.acc_pwd = accpw;

        }

        private void SetHeight(ListView LV, int height)
        {
            // listView 높이 지정
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            LV.SmallImageList = imgList;
        }

        public Bucket getBucketItem()
        {
            Bucket AuthorList = new Bucket();
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\bucket.json"))
            {
                string json = r.ReadToEnd();
                AuthorList = JsonConvert.DeserializeObject<Bucket>(json);
            }
            
            return AuthorList;
        }

        public Pendst getPendItem()
        {
            Pendst AuthorList = new Pendst();
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\pendst.json"))
            {
                string json = r.ReadToEnd();
                AuthorList = JsonConvert.DeserializeObject<Pendst>(json);
            }

            return AuthorList;
        }
        public string convertSN(string code)
        {
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            StreamReader sr = new StreamReader(path+@"\stocklist.csv", Encoding.GetEncoding("ks_c_5601-1987"));
            //
            Dictionary<string, string> nameToCode = new Dictionary<string, string>();
            string sName = "";
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                nameToCode.Add(data[0], data[1]);

            }
            foreach (KeyValuePair<string, string> kvp in nameToCode)
            {
                if (kvp.Key == code)
                    sName = kvp.Value;
            }

            return sName;
        }

        private void OnReceiveData(string tr_code)
        {
            //get a stock list

            this.output.Text += String.Format("TR code => {0}", tr_code) + Environment.NewLine;

            string r1 = t1857.GetFieldData("t1857OutBlock", "result_count", 0);
            this.keyVal = t1857.GetFieldData("t1857OutBlock", "AlertNum", 0);

            this.output.Text += String.Format("검색된 종목수 => {0}", r1) + Environment.NewLine;

            int nCount = Convert.ToInt32(r1);
            getBucketItem();
            
            string shcode, hname, price;

            List<string> listScode = new List<string>();


            if (nCount > 0)
            {
                for (int i = 0; i < nCount; i++)
                {

                    shcode = t1857.GetFieldData("t1857OutBlock1", "shcode", i);
                    hname = t1857.GetFieldData("t1857OutBlock1", "hname", i);
                    price = t1857.GetFieldData("t1857OutBlock1", "price", i);

                    //check if the stock exist in a bucket
                    // 보유종목 리스트에 있으면 제외
                    Bucket bucketItem = getBucketItem();
                    bool isBucket = false;
                    for(int j=0; j<bucketItem.scode.Length; j++)
                    {
                        if (shcode == bucketItem.scode[j])
                        {
                            isBucket = true;
                            break;
                        }

                    }
                    //important
                    //미체결 주문 목록에 있으면 제외
                    
                    Pendst pendItem = getPendItem();
                    for (int j = 0; j < pendItem.scode.Length; j++)
                    {

                        if (shcode == pendItem.scode[j])
                        {
                            isBucket = true;
                            break;
                        }

                    }
                    
                    //order it
                    if (!isBucket)
                    {
                        
                        int qnt = this.km / Convert.ToInt32(price);
                        if (shcode != null && shcode != "")
                        {
                            this.output.Text += String.Format("[{0}] {1} {2}주 매수 주문", DateTime.Now.ToString("hh:mm:ss tt"), convertSN(shcode), qnt) + Environment.NewLine;
                            //
                            Order od = new Order(this.output, this.acc_number, this.acc_pwd);
                            od.request(shcode, price, "2", qnt.ToString());
                            Thread.Sleep(200);
                            od.end();
                        }

                    }
                    listScode.Add(shcode);
                }
                
            }


            //
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            JObject sonSpec = new JObject(
                   new JProperty("scode", listScode.ToArray())
                   );


            if (!File.Exists(path + @"\" + this.st_num + ".json"))
            {
                using (FileStream fs = File.Create(path + @"\0000.json"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(sonSpec.ToString());
                    fs.Write(info, 0, info.Length);
                }
            }
            else File.WriteAllText(path + @"\"+this.st_num+".json", sonSpec.ToString());

            //미체결 종목 리스트 갱신

            pds = new PendStock(this.acc_number, this.acc_pwd);
            pds.request();
            pds.end();

        }
        public void end()
        {
            t1857.RemoveService("t1857", this.keyVal);
        }

        public void request(string st_num)
        {
            this.st_num = st_num;

            t1857.SetFieldData("t1857InBlock", "sRealFlag", 0, "0"); //실시간 조회는 실계좌일 때만 가능
            t1857.SetFieldData("t1857InBlock", "sSearchFlag", 0, "S");
            t1857.SetFieldData("t1857InBlock", "query_index", 0, this.id + st_num);

            //tr요청
            int result = t1857.RequestService("t1857", "");
            if (result < 0)
                MessageBox.Show("error");

        }
    }
}