using System.Windows.Forms;
using System.IO;
using System;

namespace SatellitePower.formSys
{
    public partial class frmLogicOperation : Form
    {
        public string sMapPathMain = string.Empty;    //被减数
        public string sMapPathSub = string.Empty;    //减数
        public int iParam = 0;  //阈值

        public int iNowType = 1;    //1=IR,2=VIS
        public int iNowWay = 0;     //0-3:1~4 通道
        public int iNowProj = 2;    //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星

        public ListBox lsbSrcList;

        public frmLogicOperation()
        {
            InitializeComponent();
        }

        public void setMapFileList(ListBox lsbSrc, string sType = "f")
        {
            lsbSrcList = lsbSrc;

            listBoxMap.Items.Clear();
            listBoxSub.Items.Clear();
            for (int i = 0; i < lsbSrc.Items.Count; i++)
            {
                CSivFileInfo sfi = lsbSrc.Items[i] as CSivFileInfo;
                if (sfi != null)
                {
                    string sName = sfi.sName;
                    //if (sName.Substring(0, 1).Equals("m"))//f
                    if (sName.Substring(0, 1).Equals(sType))//f,m
                    {
                        listBoxMap.Items.Add(sfi);
                        listBoxSub.Items.Add(sfi);
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButton1.Checked)
            {
                setMapFileList(lsbSrcList, "f");
            }
        }

        private void radioButton2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioButton2.Checked)
            {
                setMapFileList(lsbSrcList, "m");
            }
        }

        private void frmLogicOperation_Load(object sender, System.EventArgs e)
        {
            //1=IR,2=VIS
            if (iNowType == 1)
            {
                //0-3:1~4 通道
                if (iNowWay == 0)
                {
                    labTD.Text = "红外一";
                }
                else if (iNowWay == 1)
                {
                    labTD.Text = "红外二";
                }
                else if (iNowWay == 2)
                {
                    labTD.Text = "水汽";
                }
                else if (iNowWay == 3)
                {
                    labTD.Text = "红外四";
                }

            }
            else
            {
                //0-3:1~4 通道
                labTD.Text = "可见光";
            }

            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            switch (iNowProj)
            {
                case 1:
                    labTD.Text = labTD.Text + " [兰勃托]";
                    break;
                case 2:
                    labTD.Text = labTD.Text + " [麦卡托]";
                    break;
                case 3:
                    labTD.Text = labTD.Text + " [极射]";
                    break;
                case 4:
                    labTD.Text = labTD.Text + " [经纬度]";
                    break;
                default:
                    break;
            }
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (null == listBoxMap.SelectedItem)
            {
                MessageBox.Show("请选择主图！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (null == listBoxSub.SelectedItem)
            {
                MessageBox.Show("请选择辅图！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //------------------------------------------------------------
            CSivFileInfo sfi1 = listBoxMap.SelectedItem as CSivFileInfo;
            if (sfi1 != null)
            {
                sMapPathMain = sfi1.getVissPathName(iNowType, iNowWay, iNowProj);
            }

            CSivFileInfo sfi2 = listBoxSub.SelectedItem as CSivFileInfo;
            if (sfi2 != null)
            {
                sMapPathSub = sfi2.getVissPathName(iNowType, iNowWay, iNowProj);
            }

            //------------------------------------------------------------
            if (sMapPathMain.Equals(sMapPathSub))
            {
                MessageBox.Show("选定云图相同，无法计算！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }

            if (!File.Exists(sMapPathMain))
            {
                MessageBox.Show("文件不存在: " + sMapPathMain, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(sMapPathSub))
            {
                MessageBox.Show("文件不存在: " + sMapPathSub, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            iParam = Convert.ToInt32(nudVal.Value);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
