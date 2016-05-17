using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SatellitePower
{
    public partial class FrmSelectYT : Form
    {
        public int iNowType = 1;    //1=IR,2=VIS
        public int iNowWay = 0;     //0-3:1~4 通道

        public string sMapPathIR1 = string.Empty;
        public string sMapPathIR2 = string.Empty;
        public string sMapPathIR3 = string.Empty;
        public string sMapPathIR4 = string.Empty;
        public string sMapPathVis1 = string.Empty;
        int type;//标识是雷暴积冰和海雾

        public ListBox lsbSrcList;

        public CSivFileInfo first = new CSivFileInfo();//选中的云图,2014-10-27 wm add

        public void setMapFileList(ListBox lsbSrc, string sType = "f")
        {
            lsbSrcList = lsbSrc;
            listBoxMap.Items.Clear();
            listBoxYJF.Items.Clear();
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
                    }
                }
            }
        }
        /************************************************************************/
        /* type:
         *      1:雷暴、积冰
         *      2:海雾
        /************************************************************************/
        public FrmSelectYT(int type)
        {
            this.type = type;
            InitializeComponent();
        }

        private void listBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxYJF.Items.Clear();
            int iSel = listBoxMap.SelectedIndex;
            if (iSel >= 0 && iSel < listBoxMap.Items.Count)
            {
                for (int i = iSel; i < listBoxMap.Items.Count; i++)
                {
                    CSivFileInfo sfi = listBoxMap.Items[i] as CSivFileInfo;
                    if (sfi != null)
                    {
                        if (listBoxYJF.Items.Count < 1)
                        {
                            listBoxYJF.Items.Add(sfi);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (listBoxYJF.Items.Count <= 0)
            {
                MessageBox.Show("云图数不足，无法计算！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //------------------------------------------------------------
            CSivFileInfo sfi1 = listBoxYJF.Items[0] as CSivFileInfo;
            if (sfi1 != null)
            {
                if (type == 1)
                {//雷暴
                    sMapPathIR1 = sfi1.getVissPathName(1, 0, 4);
                    sMapPathIR2 = sfi1.getVissPathName(1, 1, 4);
                    sMapPathIR3 = sfi1.getVissPathName(1, 2, 4);
                    if (!File.Exists(sMapPathIR1))
                    {
                        MessageBox.Show("红外一云图文件不存在，无法进行反演！(" + sMapPathIR1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!File.Exists(sMapPathIR2))
                    {
                        MessageBox.Show("红外二云图文件不存在，无法进行反演！(" + sMapPathIR2 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!File.Exists(sMapPathIR3))
                    {
                        MessageBox.Show("水汽云图文件不存在，无法进行反演！(" + sMapPathIR3 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (type==2)
                {//积冰
                    sMapPathIR1 = sfi1.getVissPathName(1, 0, 4);
                    sMapPathVis1 = sfi1.getVissPathName(2, 0, 4);
                    if (!File.Exists(sMapPathIR1))
                    {
                        MessageBox.Show("红外一云图文件不存在，无法进行反演！(" + sMapPathIR1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!File.Exists(sMapPathVis1))
                    {
                        MessageBox.Show("可见光云图文件不存在，无法进行反演！(" + sMapPathVis1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (type==3)
                {//等值线
                    sMapPathIR1 = sfi1.getVissPathName(1, 0, 2);
                    if (!File.Exists(sMapPathIR1))
                    {
                        MessageBox.Show("红外一云图文件不存在，无法进行反演！(" + sMapPathIR1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {//海雾
                    sMapPathIR1 = sfi1.getVissPathName(1, 0, 4);//2=麦卡托
                    sMapPathIR4 = sfi1.getVissPathName(1, 3, 4);//2=麦卡托
                    sMapPathVis1 = sfi1.getVissPathName(2, 0, 4);//2=麦卡托
                    if (!File.Exists(sMapPathIR1))
                    {
                        MessageBox.Show("红外一云图文件不存在，无法进行反演！(" + sMapPathIR1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!File.Exists(sMapPathIR4))
                    {
                        MessageBox.Show("红外四云图文件不存在，无法进行反演！(" + sMapPathIR4 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (!File.Exists(sMapPathVis1))
                    {
                        MessageBox.Show("可见光云图文件不存在，无法进行反演！(" + sMapPathVis1 + ")", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            first = listBoxYJF.Items[0] as CSivFileInfo;//获取选中的云图,2014-10-27 wm add

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                setMapFileList(lsbSrcList, "f");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                setMapFileList(lsbSrcList, "m");
            }
        }

        private void frmSelYJF_Load(object sender, EventArgs e)
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
        }
    }
}
