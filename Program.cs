using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SatellitePower
{
    static class Program
    {
        /// <summary>
        /// 主程序的入口点在此设置，包括一些初始化操作，启动窗体等
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new MainForm());
            Application.Run(new mycontext());//2015-1-9,wm add
        }
    }
}
