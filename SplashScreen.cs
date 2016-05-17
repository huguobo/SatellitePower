using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SatellitePower
{
    /// <summary>
    /// 启动画面
    /// </summary>
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

        }

        public int count = 0;
        private void timerInfo_Tick(object sender, EventArgs e)
        {
            if (count % 3 == 0)
            {
                labInfo.Text = "正在加载，请稍后.";
            }
            else if (count % 3 == 1)
            {
                labInfo.Text = "正在加载，请稍后..";
            }
            else if (count % 3 == 2)
            {
                labInfo.Text = "正在加载，请稍后...";
            }
            count++;
        }
    }
}
