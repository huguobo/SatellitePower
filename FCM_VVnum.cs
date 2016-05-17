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
    public partial class FCM_VVnum : Form
    {
        public int VVnum = 0;
        public int yx1, yx2;
        public PointF pf1, pf2;
        public string sMapPathName_IR1, sMapPathName_VIS;
        public VISSObject visObj_IR1, visObj_VIS;

        public FCM_VVnum()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0 && comboBox1.SelectedItem != null)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        VVnum = 1;
                        break;
                    case 1:
                        VVnum = 2;
                        break;
                    case 2:
                        VVnum = 3;
                        break;
                    case 3:
                        VVnum = 4;
                        break;
                    case 4:
                        VVnum = 5;
                        break;
                    case 5:
                        VVnum = 6;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (VVnum == 0)
            {
                MessageBox.Show("未设置云类数目！", "提示");
                return;
            }
            else
            {
                frmFXCloudType fCloudType = new frmFXCloudType();
                //根据通道限定阈值
                fCloudType.yx1 = yx1;
                fCloudType.yx2 = yx2;

                fCloudType.pf1 = pf1;
                fCloudType.pf2 = pf2;
                fCloudType.sMapPathName_IR1 = sMapPathName_IR1;
                fCloudType.sMapPathName_VIS = sMapPathName_VIS;
                fCloudType.visObj_IR1 = visObj_IR1;
                fCloudType.visObj_VIS = visObj_VIS;
                fCloudType.c = VVnum;
                fCloudType.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
