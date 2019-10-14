using awaken;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monitor
{
    public partial class MsgForm : Form
    {
        public MsgForm(String title, String content)
        {
            InitializeComponent();
            label1.Text = title;
            label1.Update();
            richTextBox1.Text = content;
            richTextBox1.Update();
        }

        private void MsgForm_Load(object sender, EventArgs e)
        {
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
            //这个区域包括任务栏，就是屏幕显示的物理范围
            //Rectangle ScreenArea = System.Windows.Forms.Screen.GetBounds(this);
            int width1 = ScreenArea.Width; //屏幕宽度 
            int height1 = ScreenArea.Height; //屏幕高度
            this.Width = 300;
            this.Height = 250;
            int x = width1 - this.Width;
            this.Location = new System.Drawing.Point(x, height1 - this.Height); //指定窗体显示在右下角
            //AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_ACTIVE | AW_VER_NEGATIVE);
        }

        private void MsgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogHelper.Log("msg close");
            //AnimateWindow(this.Handle, 1000, AW_BLEND | AW_HIDE);
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
