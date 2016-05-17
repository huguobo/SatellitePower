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
    public partial class frmFYParaSet : Form
    {
        public CSysParams sysParams;

        public frmFYParaSet()
        {
            InitializeComponent();
        }

        private void frmFYParaSet_Load(object sender, EventArgs e)
        {
            if (null != sysParams)
            {
                //反演产品参数设置,20150320 wm add
                //积冰
                numDayT1.Value = sysParams.DayT1;//可见光反照率阈值
                numDayT2.Value = sysParams.DayT2;//白天晴空背景亮温
                numDayT3.Value = sysParams.DayT3;//白天亮温差值
                numNightT1.Value = sysParams.NightT1;//夜晚晴空背景亮温
                numNightT2.Value = sysParams.NightT2;//夜晚亮温差值
                //海雾,20150417 wm add
                num_HW_angelDN.Value = sysParams.AngelDayNight;
                num_HW_angelS.Value = sysParams.AngelSummer;
                num_HW_angelW.Value = sysParams.AngelWinter;
                num_HW_DcdNight.Value = sysParams.DcdNight;
                num_HW_DcdDawn.Value = sysParams.DcdDawn;
                num_HW_DcdDay.Value = sysParams.DcdDay;
                num_HW_VisMin.Value = sysParams.DayVisMin;
                num_HW_VisMax.Value = sysParams.DayVisMax;

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //反演产品参数设置,20150320 wm add
            //积冰
            sysParams.DayT1 = Convert.ToInt32(numDayT1.Value);//可见光反照率阈值
            sysParams.DayT2 = Convert.ToInt32(numDayT2.Value);//白天晴空背景亮温
            sysParams.DayT3 = Convert.ToInt32(numDayT3.Value);//白天亮温差值
            sysParams.NightT1 = Convert.ToInt32(numNightT1.Value);//夜晚晴空背景亮温
            sysParams.NightT2 = Convert.ToInt32(numNightT2.Value);//夜晚亮温差值

            //海雾,20150417 wm add
            sysParams.AngelDayNight = Convert.ToInt32(num_HW_angelDN.Value);
            sysParams.AngelWinter = Convert.ToInt32(num_HW_angelW.Value);
            sysParams.AngelSummer = Convert.ToInt32(num_HW_angelS.Value);
            sysParams.DcdNight = Convert.ToInt32(num_HW_DcdNight.Value);
            sysParams.DcdDawn = Convert.ToInt32(num_HW_DcdDawn.Value);
            sysParams.DcdDay = Convert.ToInt32(num_HW_DcdDay.Value);
            sysParams.DayVisMin = Convert.ToInt32(num_HW_VisMin.Value);
            sysParams.DayVisMax = Convert.ToInt32(num_HW_VisMax.Value);


            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
