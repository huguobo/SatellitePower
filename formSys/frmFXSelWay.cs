using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;

namespace SatellitePower.formSys
{
    public partial class frmFXSelWay : Form
    {
        public int iNowType = 1;        //1=IR,2=VIS
        public int iNowWay = 0;         //0-3:1~4 通道
        public int iNowProj;

        public PointF pf1, pf2;
        public string mapPath1, mapPath2;
        public VISSObject visObj1, visObj2;
        public CSivFileInfo sfi;
        public int[] td2 = new int[2] { -1, -1 };//{1,0}=IR1,{1,1}=IR2,{1,2}=IR3,{1,3}=IR4,{2,0}=VIS

        public bool[] exist = new bool[5];
        public frmFXSelWay()
        {
            InitializeComponent();
        }

        private void frmFXSelWay_Load(object sender, EventArgs e)
        {
            int td;
            int first = -1;
            if (iNowType == 1)
            {
                td = iNowType + iNowWay;
            }
            else
            {
                td = 5;
            }

            //屏蔽不存在通道，2014-12-17,wm add
            if (!exist[0])
                rdbIR1.Enabled = false;
            if (!exist[1])
                rdbIR2.Enabled = false;
            if (!exist[2])
                rdbIR3.Enabled = false;
            if (!exist[3])
                rdbIR4.Enabled = false;
            if (!exist[4])
                rdbVis.Enabled = false;

            //选中状态
            for (int i = 0; i < 5; i++)
            {
                if (exist[i] && i != (td - 1))
                {
                    first = i + 1;
                    break;
                }
            }
            switch(first)
            {
                case -1:
                    btnOk.Enabled = false;//全不可用
                    break;
                case 1:
                    rdbIR1.Checked = true;
                    break;
                case 2:
                    rdbIR2.Checked = true;
                    break;
                case 3:
                    rdbIR3.Checked = true;
                    break;
                case 4:
                    rdbIR4.Checked = true;
                    break;
                case 5:
                    rdbVis.Checked = true;
                    break;
            }

            //屏蔽当前通道
            if (iNowType == 1)
            {
                //rdbIR1.Checked = true;
                //IR
                switch (iNowWay)
                {
                    case 0:
                        rdbIR1.Enabled = false;
                        //rdbIR2.Checked = true;
                        break;
                    case 1:
                        rdbIR2.Enabled = false;
                        break;
                    case 2:
                        rdbIR3.Enabled = false;
                        break;
                    case 3:
                        rdbIR4.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //VIS
                rdbVis.Enabled = false;
                //rdbIR1.Checked = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            #region 设定通道2
            if (rdbIR1.Checked)
            {
                td2[0] = 1; td2[1] = 0;
            }
            else if (rdbIR2.Checked)
            {
                td2[0] = 1; td2[1] = 1;
            }
            else if (rdbIR3.Checked)
            {
                td2[0] = 1; td2[1] = 2;
            }
            else if (rdbIR4.Checked)
            {
                td2[0] = 1; td2[1] = 3;
            }
            else if (rdbVis.Checked)
            {
                td2[0] = 2; td2[1] = 0;
            }
            else
            {
                return;
            }
            #endregion

            mapPath1 = sfi.getVissPathName(iNowType, iNowWay, iNowProj);
            mapPath2 = sfi.getVissPathName(td2[0], td2[1], iNowProj);
            LayerVISS lvis1 = new LayerVISS();
            LayerVISS lvis2 = new LayerVISS();
            lvis1.Load(mapPath1);
            lvis2.Load(mapPath2);
            visObj1 = lvis1.visObj;
            visObj2 = lvis2.visObj;

            frmFXRect fRect = new frmFXRect();
            fRect.pf1 = pf1;
            fRect.pf2 = pf2;
            fRect.mapPath1 = mapPath1;
            fRect.mapPath2 = mapPath2;
            fRect.visObj1 = visObj1;
            fRect.visObj2 = visObj2;
            fRect.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
