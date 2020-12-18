using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using xNet;
using System.Net;
using System.IO;

namespace Logs
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        static string login;
        static string password;
        static string l_chat;
        static string g_chat;
        static string rep_chat;
        static string com_chat;
        static string sessions;
        static string phpsession;
        static string server;
        static string lastLine;

        static string discrod_url;
        static string avatar_url;
        static string logs_url;
        static string private_url;
        public static bool work;
        static bool write;
        static bool discord;
        static bool privat;
        static int interval;

        static CookieDictionary cookie = new CookieDictionary(false);
        public static List<string> Com_list = new List<string> { };

        public void Authme()
        {
            using (HttpRequest httpRequest1 = new HttpRequest())
            {
                httpRequest1.UserAgent = Http.ChromeUserAgent();
                httpRequest1.KeepAlive = true;
                httpRequest1.Cookies = cookie;
                httpRequest1.AddField("username", (object)login).AddField("password", (object)password).AddField("submit", (object)null).AddField("login", (object)"submit");
                bool enter = false;
                while (!enter)
                {
                    string check = httpRequest1.Post("https://mcskill.ru/").ToString();
                    if (check.IndexOf("Подтвердите вход в Вконтакте!") > 0)
                    {
                        DialogResult result = MessageBox.Show(
                        "Подтвердите вход, затем нажмите на 'OK'",
                        "Подтвердите вход в Вконтакте!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                        if (result == DialogResult.OK)
                        {
                            httpRequest1.UserAgent = Http.ChromeUserAgent();
                            httpRequest1.KeepAlive = true;
                            httpRequest1.Cookies = cookie;
                            httpRequest1.AddField("username", (object)login).AddField("password", (object)password).AddField("submit", (object)null).AddField("login", (object)"submit");
                            httpRequest1.Post("https://mcskill.ru/").None();
                            enter = true;
                        }
                    }
                    else enter = true;
                }
                foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)cookie)
                {
                    if (keyValuePair.Key == "PHPSESSID")
                    {
                        phpsession = keyValuePair.Value;
                        break;
                    }
                }
                using (HttpRequest httpRequest2 = new HttpRequest())
                {
                    httpRequest2.UserAgent = Http.ChromeUserAgent();
                    httpRequest2.KeepAlive = true;
                    httpRequest2.Cookies = cookie;
                    sessions = Html.Substring(((object)httpRequest2.Get("https://mcskill.ru/techpanel/server_private_logs.php", (RequestParams)null)).ToString(), "<input type=\"hidden\" name=\"session\" value=\"", "\">", StringComparison.Ordinal);
                    if (sessions == "")
                    {
                        work = false;
                        MessageBox.Show("Нет доступа к приватным логам!");
                        return;
                    }
                    else
                    {
                        httpRequest2.UserAgent = Http.ChromeUserAgent();
                        httpRequest2.KeepAlive = true;
                        httpRequest2.Cookies = cookie;
                        httpRequest2.AddField("player", (object)login).AddField("session", (object)sessions);
                        httpRequest2.Post(private_url).None();
                    }
                }
            }
        }

        public delegate void toForm(string chat, string iText);

        public void messages(string chat, string iText)
        {
            if (write)
            {
                switch (chat)
                {
                    case "l_chat":
                        {
                            listBox1.Items.Insert(0, iText);
                            break;
                        }
                    case "g_chat":
                        {
                            listBox2.Items.Insert(0, iText);
                            break;
                        }
                    case "com_chat":
                        {
                            listBox3.Items.Insert(0, iText);
                            break;
                        }
                }
            }
            if (discord)
            {
                string active = "";
                switch (chat)
                {
                    case "l_chat":
                        {
                            active = l_chat;
                            break;
                        }
                    case "g_chat":
                        {
                            active = g_chat;
                            break;
                        }
                    case "com_chat":
                        {
                            active = com_chat;
                            break;
                        }
                }
                try
                {
                    using (HttpRequest http = new HttpRequest())
                    {
                        http.UserAgent = Http.ChromeUserAgent();
                        http.KeepAlive = true;
                        http.Cookies = cookie;
                        http.AddParam("username", comboBox1.Text).AddParam("avatar_url", avatar_url).AddParam("content", iText);
                        http.Post(discrod_url + active).None();
                    }
                }
                catch
                {

                }
            }
        }

        public bool black_com(string iText)
        {
            for (int i = 0; i < Com_list.Count(); i++)
            {
                if (lastLine.IndexOf(Com_list[i]) > 0) return false;
            }
            return true;
        }

        public string allreplace(string text)
        {
            //text = text.Replace(" issued server command", "");

            text = text.Replace("[0;34;22m", ""); //&1
            text = text.Replace("[0;32;22m", ""); //&2
            text = text.Replace("[0;36;22m", ""); //&3
            text = text.Replace("[0;31;22m", ""); //&4
            text = text.Replace("[0;35;22m", ""); //&5
            text = text.Replace("[0;33;22m", ""); //&6
            text = text.Replace("[0;37;22m", ""); //&7
            text = text.Replace("[0;30;1m", "");  //&8
            text = text.Replace("[0;34;1m", "");  //&9
            text = text.Replace("[0;31;1m", "");  //&c
            text = text.Replace("[0;33;1m", "");  //&e
            text = text.Replace("[0;32;1m", "");  //&a
            text = text.Replace("[0;36;1m", "");  //&b
            text = text.Replace("[0;35;1m", "");  //&d
            text = text.Replace("[0;37;1m", "");  //&f

            text = text.Replace("[Server thread/INFO]: ", "");
            text = text.Replace("[m:", ":");
            text = text.Replace("[m", "");
            text = text.Replace("[37m", "");
            text = text.Replace("\u001b", "");
            return text;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        server = "EasyTech";
                        private_url = "https://s8.mcskill.ru/mpsl/mod_logs.php";
                        logs_url = "http://logs.s8.mcskill.ru/";
                        avatar_url = "https://mcskill.ru/style/images/techcraftclassic.png";
                        break;          
                    }
                case 1:
                    {
                        server = "Hitechcraft";
                        private_url = "https://s7.mcskill.ru/mpsl/mod_logs.php";
                        logs_url = "http://logs.s7.mcskill.ru/";
                        avatar_url = "https://mcskill.ru/style/images/htc1.png";
                        break;
                    }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Запустить")
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Не указан сервер!");
                    return;
                }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                login = textBox1.Text;
                password = textBox2.Text;
                work = true;
                Logs.RunWorkerAsync();
            }
            else work = false;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<string> List = new List<string> { };
                if (privat)
                {
                    Authme();
                    lastLine = "";
                    workk: while (work)
                    {
                        List.Clear();
                        int count = 0;
                        WebClient webClient = new WebClient();
                        webClient.Headers.Add("User-Agent", Http.ChromeUserAgent());
                        webClient.Headers.Add(HttpRequestHeader.Cookie, "PHPSESSID=" + phpsession + ";");
                        Stream data = webClient.OpenRead(private_url + "?server=" + server + "&logfile=latest.log");
                        StreamReader reader = new StreamReader(data);
                        string html = reader.ReadToEnd();
                        var str = html;
                        var result = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in result) List.Insert(0, allreplace(item));
                        if (lastLine == "")
                        {
                            lastLine = List[0];
                            goto workk;
                        }
                        for (int i = 0; i < List.Count(); i++)
                        {
                            if (lastLine == List[i]) break;
                            else count++;
                        }
                        for (int i = count - 1; i >= 0; i--)
                        {
                            lastLine = List[i];
                            if (lastLine.IndexOf("[L]") > 0)
                            {
                                if (lastLine.IndexOf("rep") > 0) BeginInvoke(new toForm(messages), rep_chat, lastLine);
                                else BeginInvoke(new toForm(messages), l_chat, lastLine);
                            }
                            else if (lastLine.IndexOf("[G]") > 0) BeginInvoke(new toForm(messages), g_chat, lastLine);
                            else if (lastLine.IndexOf("issued server command") > 0 && lastLine.IndexOf("@") < 0 && black_com(lastLine)) BeginInvoke(new toForm(messages), com_chat, lastLine);
                        }
                        Thread.Sleep(interval);
                    }
                }
                else
                {
                    workk: while (work)
                    {
                        List.Clear();
                        int count = 0;
                        string html;
                        using (HttpRequest http = new HttpRequest())
                        {
                            var localTime = DateTimeOffset.Now;
                            var offset = TimeSpan.FromHours(2); 
                            var pstTime = localTime.ToOffset(offset);
                            string date = pstTime.ToString("dd-MM-yyyy");

                            http.UserAgent = Http.ChromeUserAgent();
                            http.KeepAlive = true;
                            http.Cookies = cookie;
                            html = http.Get(logs_url + server + "_public_logs/" + date + ".txt").ToString();
                        }
                        var result = html.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in result) List.Insert(0, allreplace(item));
                        if (lastLine == "")
                        {
                            lastLine = List[0];
                            goto workk;
                        }
                        for (int i = 0; i < List.Count(); i++)
                        {
                            if (lastLine == List[i]) break;
                            else count++;
                        }
                        for (int i = count - 1; i >= 0; i--)
                        {
                            lastLine = List[i];
                            if (lastLine.IndexOf("[L]") > 0) BeginInvoke(new toForm(messages), "l_chat", lastLine);
                            else if (lastLine.IndexOf("[G]") > 0) BeginInvoke(new toForm(messages), "g_chat", lastLine);
                            else BeginInvoke(new toForm(messages), "com_chat", lastLine);
                        }
                        Thread.Sleep(interval);
                    }
                }
            }
            catch
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*using (HttpRequest httpRequest1 = new HttpRequest())
            {
                httpRequest1.UserAgent = Http.ChromeUserAgent();
                httpRequest1.KeepAlive = true;
                httpRequest1.Cookies = cookie;
                HttpResponse response = httpRequest1.Get("http://165.22.193.168/Logs.txt");
                string content = response.ToString();
                if (content.IndexOf("kudrin") < 0) this.Close();
            }*/
            lastLine    = "";
            discrod_url = "https://discordapp.com/api/webhooks/";
            l_chat      = "730115001213386892/UwA4K2MSHhafjHUT60OUA2ddUQTe0rxRobiDUm44Q-14EuxstkaEMZb5icfAIjS5Mb_d";
            g_chat      = "731744003145465867/1lSpfNzqs0XFp9dG2O2M2_aJZpSLSqAYNo3f_6VVP-eAzNPfhHUO_AmeaQheVRmY4MNN";
            rep_chat    = "730182889404235907/xqWTHxuirTG5RLnmqA4iKPpokZLN6LPuIzbCn7SuhR15PDNJCFIlrJq9-Zh6dXVvt7Ua";
            com_chat    = "730176824424005775/bAziMbksNvdQvxdIj9cKTNi-LforyBHKUpISgq5gVTWZv_b0gSWnvCi2GnqG-ZDsG3Uz";
            work        = false;
            write       = false;
            discord     = false;
            interval = (int)numericUpDown1.Value;
            Com_list.Add("/rollcase");
            Com_list.Add("/mpcaseview");
            Com_list.Add("/mpcaseshop");
            Com_list.Add("/warp nether");
            Com_list.Add("/warp mining");
            Com_list.Add("/warp end");
            Com_list.Add("/home");
            Com_list.Add("/vmtpdeny");
            Com_list.Add("/rtp");
            Com_list.Add("/tpa");
            Com_list.Add("/tpaccept");
            Com_list.Add("/we cui");
            Com_list.Add("/spawn");
            comboBox1.Items.Add("HitechClassic");
            comboBox1.Items.Add("HiTech TITAN");
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            interval = (int)numericUpDown1.Value;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) discord = true;
            else discord = false;
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                write = true;
                tabControl1.Visible = true;
            }
            else
            {
                write = false;
                tabControl1.Visible = false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool ok = false;
            foreach (Form f in Application.OpenForms)
                if (f.Name == "Form2") ok = true;
            if (!ok)
            {
                Form Form2 = new Logs.Form2();
                Form2.Text = "McSkill Логи";
                Form2.Show();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            work = false;
            Thread.Sleep(1000);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (work)
            {
                button1.Enabled = false;
                comboBox1.Enabled = false;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                numericUpDown1.Enabled = false;
                button2.Text = "Остановить";
            }
            else
            {
                button1.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                numericUpDown1.Enabled = true;
                button2.Text = "Запустить";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                privat = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            else
            {
                privat = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }
    }
}