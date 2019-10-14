using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monitor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Application.Run(new MsgForm("http://www.wakeyoga.com/auth/2pland3452.html异常", "你站在桥上看风景看风景的人在楼上看你，明月装饰了你的窗子..."));
        }
    }
}
