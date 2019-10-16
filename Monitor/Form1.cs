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
                t.Start();
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
                toLog(",监控数量:" + monitorList.Count,false);
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
                    if (showErrorThread!=null&&showErrorThread.IsAlive) {
                        //toLog("等待处理中,请稍后...");
                        i--;
                        continue;
                    } 

                    MonitorModel model = monitorList[i];
                    try
                    {
                        _monitorHandel(model);
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //toLog("------------------------------------------");
                LogHelper.Log("休眠" + ConfigModel.getSleep());
                Thread.Sleep(ConfigModel.getSleep());
            }
        }

        private void _monitorHandel(Object o)
        {
            MonitorModel model = (MonitorModel)o;
            try
            {
                DateTime dt = DateTime.Now;
                //toLog("------------------------------------------");
                //toLog("");
                toLog("请求:"+ model.host);
                Uri uri = new Uri(model.host);
                WebClient webClient = new WebClient();
                byte[] byt = webClient.DownloadData(uri);
                String content = System.Text.Encoding.UTF8.GetString(byt);
                TimeSpan tm = DateTime.Now - dt;
                toLog(" 耗时:" + tm.Milliseconds,false);

                if (content.Contains(model.key))
                {
                    return;
                }
                showError("未返回指定的内容:" + model.key, model);
            }
            catch (Exception ex)
            {
                showError("访问异常," + ex.Message, model);
            }

        }
        List<String> errorHost = new List<string>();

        MsgForm msgForm = null;
        private void showError(String msg, MonitorModel model)
        {
            toLog(msg);
            try
            {
                Uri uri = new Uri(ConfigModel.getMsgHost() + model.host+msg);
                WebClient webClient = new WebClient();
                byte[] byt = webClient.DownloadData(uri);
                String content = System.Text.Encoding.UTF8.GetString(byt);
                LogHelper.Log("发送消息");
            }
            catch (Exception ex)
            {
                LogHelper.Log("消息发送异常:" + ex);
            }

            /*
            msgForm = new MsgForm(model.host + "异常", msg);
            msgForm.ShowDialog();//正常
            */
            showErrorThread = new Thread(new ParameterizedThreadStart(showMsgForm));
            MsgModel msgModel = new MsgModel();
            msgModel.Name = model.host + "异常";
            msgModel.Remark = msg;
            showErrorThread.Start(msgModel);
        }

        private void toLog(String str) {
            toLog(str,true);
        }

        private void toLog(String str,Boolean isNewLine)
        {
            LogHelper.Log(str);
            if (isNewLine) {
                str = "\r\n" + str;
            }
            richTextBox1.AppendText(str);
            Thread.Sleep(200);
        }

        Thread showErrorThread = null;

        private void showMsgForm(Object obj)
        {
            //toLog("发现异常，请及时处理，休眠:"+ ConfigModel.getWarnTime());
            new Thread(new ThreadStart(() => {
                //休眠多少x秒后，关掉显示错误消息的线程，让线程继续走
                Thread.Sleep(ConfigModel.getWarnTime());
                msgForm.Close();
                showErrorThread.Abort();//结束线程，让它重新开始执行
            })).Start();

            MsgModel msgModel = (MsgModel)obj;
            msgForm = new MsgForm(msgModel.Name, msgModel.Remark);
            msgForm.ShowDialog();
            if (showErrorThread != null && showErrorThread.IsAlive) {
                showErrorThread.Abort();//结束线程，让它重新开始执行
            }
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
            LogHelper.Log("------系统退出------");
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
