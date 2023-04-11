using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockA
{
    public partial class Preference : Form
    {
        List<Item> items;
        //public static string id, accNo;
        public string id;

        public Preference()
        {
            InitializeComponent();
            this.id = Form1.id;
            //
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            using (StreamReader r = new StreamReader(path + @"\pref.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            //조건식 로드
            string[] st = items[0].light;
            System.Object[] ItemObject = new System.Object[st.Length];

            for (int i = 0; i < st.Length; i++)
            {
                ItemObject[i] = String.Format("{0}", st[i]);
            }
            comboBox1.Items.AddRange(ItemObject);

            comboBox1.SelectedIndex = 0;
            //계좌번호
            textBox4.Text = items[0].millis;
            
            textBox5.Text = items[0].stamp;

            textBox1.Text = items[0].temp.ToString();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            if (textBox4.Text == string.Empty)
            {
                MessageBox.Show("계좌번호를 입력해주세요");
                return;
            }
            try
            {   
                JObject sonSpec = new JObject(
                new JProperty("millis", textBox4.Text),
                new JProperty("stamp", textBox5.Text),
                new JProperty("light", items[0].light),
                new JProperty("vcc", "false")
                );

                if (!File.Exists(path + @"\pref2.json"))
                {
                    using (FileStream fs = File.Create(path + @"\pref2.json"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sonSpec.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }
                else  File.WriteAllText(path+ @"\pref2.json", sonSpec.ToString());
             
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Form1.logged)
                MessageBox.Show("로그인이 필요합니다");
            Strategy stg = new Strategy(comboBox1, this.id);
            stg.request();
                    
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("모의투자에서는 실시간 기능은 제공되지 않습니다");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Text = "";
            comboBox2.Items.Clear();
            comboBox2.Text = "";

        }

        private void button4_Click(object sender, EventArgs e)
        {

            comboBox3.Items.Clear();
            comboBox3.Text = "";
            comboBox4.Items.Clear();
            comboBox4.Text = "";
        }
    }
}
