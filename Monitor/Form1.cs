using awaken;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Monitor
{
    public partial class Form1 : Form
    {

        List<MonitorModel> monitorList = new List<MonitorModel>();
        public Form1()
        {
            InitializeComponent();
            richTextBox1.Text = "";
            toLog(" ------Welcome------ ");
            CheckForIllegalCrossThreadCalls = false;
            loadConfig();
            this.Cursor = Cursors.Arrow;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (monitorList.Count != 0)
            {
                toLog("开始监控");
                Thread t = new Thread(monitorHandel);
                t.IsBackground = true;
                //try
                //{
                t.Start();
                //}
                //catch (Exception)
                //{
                //  t = new Thread(monitorHandel);
                //t.IsBackground = true;
                //t.Start();
                //}

            }
        }

        //加载配置文件
        private void loadConfig()
        {
            toLog("加载配置文件");
            String filePath = ConfigModel.getFullFilePath();
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"));
            string txt = sr.ReadToEnd();
            if (txt == "" || txt == null)
            {
                toLog("配置文件加载失败");
                return;
            }
            try
            {
                monitorList = JsonConvert.DeserializeObject<List<MonitorModel>>(txt);
                if (monitorList == null)
                {
                    MessageBox.Show("找不到配置文件，初始化失败");
                }
                toLog("配置文件加载成功,监控数量:" + monitorList.Count);
            }
            catch (Exception ex)
            {

            }
        }

        private void monitorHandel()
        {
            while (true)
            {
                for (int i = 0; i < monitorList.Count; i++)
                {
                    MonitorModel model = monitorList[i];
                    try
                    {
                        //      Thread t = new Thread( new ParameterizedThreadStart(_monitorHandel));
                        //    t.Start(model);
                        _monitorHandel(model);
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                toLog("------------------------------------------");
                toLog("休眠" + ConfigModel.getSleep());
                Thread.Sleep(ConfigModel.getSleep());
            }
        }

        private void _monitorHandel(Object o)
        {
            MonitorModel model = (MonitorModel)o;
            try
            {
                DateTime dt = DateTime.Now;
                toLog("------------------------------------------");
                toLog(model.host);
                toLog("准备请求");
                Uri uri = new Uri(model.host);
                WebClient webClient = new WebClient();
                byte[] byt = webClient.DownloadData(uri);
                String content = System.Text.Encoding.UTF8.GetString(byt);
                TimeSpan tm = DateTime.Now - dt;
                toLog("请求完成,耗时:" + tm.Milliseconds);

                if (content.Contains(model.key))
                {
                    return;
                }
                showError(model.host + "服务器可能异常，未返回指定的内容:" + model.key, model);
            }
            catch (Exception ex)
            {
                showError(model.host + "服务器访问异常," + ex.Message, model);
            }

        }
        List<String> errorHost = new List<string>();

        MsgForm msgForm = null;
        private void showError(String msg, MonitorModel model)
        {
            toLog(msg);
            //if (errorHost.Contains(model.host)) {
            //  return;
            //}
            //            if (msgForm != null) return;
            //            errorHost.Add(model.host);
            try
            {
                Uri uri = new Uri(ConfigModel.getMsgHost() + msg);
                WebClient webClient = new WebClient();
                byte[] byt = webClient.DownloadData(uri);
                String content = System.Text.Encoding.UTF8.GetString(byt);
                LogHelper.Log("发送消息");
            }
            catch (Exception ex)
            {
                LogHelper.Log("消息发送异常:" + ex);
            }

            msgForm = new MsgForm(model.host + "异常", msg);
            msgForm.ShowDialog();
            //Thread t = new Thread(new ParameterizedThreadStart(showMsgForm));
            //t.Start(msgForm);
        }

        private void toLog(String str)
        {
            LogHelper.Log(str);
            richTextBox1.AppendText("\r\n" + str);
        }

        private void showMsgForm(Object obj)
        {
            MsgForm msgForm = (MsgForm)obj;
            msgForm.Show();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //this.richTextBox1.Select(this.richTextBox1.TextLength, 0);
            this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.contextMenuStrip1.Show();
        }

        private void 显示窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOrHidden();
        }

        private void 退出系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            showOrHidden();
        }

        void showOrHidden() {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("欢迎使用本软件，作者Nathan,396369854@qq.com");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogHelper.Log("------系统推出------");
        }

        private void Form1_MinimumSizeChanged(object sender, EventArgs e)
        {
            //this.Hide();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) {
                this.Hide();
            }
        }


        /*
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7 || m.Msg == 0x201 || m.Msg == 0x202
                || m.Msg == 0x203 || m.Msg == 0x204 || m.Msg == 0x205
                || m.Msg == 0x206 || m.Msg == 0x0100 || m.Msg == 0x0101) {
                return;
            }
            base.WndProc(ref m);
        }
        */
    }
}
