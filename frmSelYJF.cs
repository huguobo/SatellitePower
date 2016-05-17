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
    public partial class frmSelYJF : Form
    {
        public int iNowType = 1;    //1=IR,2=VIS
        public int iNowWay = 0;     //0-3:1~4 通道

        public string sMapPath1 = string.Empty;    //时间靠后的，如：10点
        public string sMapPath2 = string.Empty;    //时间中间的，如： 9点
        public string sMapPath3 = string.Empty;    //时间靠前的，如： 8点

        public ListBox lsbSrcList;

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

        public frmSelYJF()
        {
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
                        if (listBoxYJF.Items.Count < 3)
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
            if (listBoxYJF.Items.Count < 3)
            {
                MessageBox.Show("选定云图数不足3，无法计算！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //------------------------------------------------------------
            CSivFileInfo sfi1 = listBoxYJF.Items[0] as CSivFileInfo;
            if (sfi1 != null)
            {
                sMapPath1 = sfi1.getVissPathName(iNowType, iNowWay, 4);
            }

            CSivFileInfo sfi2 = listBoxYJF.Items[1] as CSivFileInfo;
            if (sfi2 != null)
            {
                sMapPath2 = sfi2.getVissPathName(iNowType, iNowWay, 4);
            }

            CSivFileInfo sfi3 = listBoxYJF.Items[2] as CSivFileInfo;
            if (sfi3 != null)
            {
                sMapPath3 = sfi3.getVissPathName(iNowType, iNowWay, 4);
            }
            //------------------------------------------------------------
            if (!File.Exists(sMapPath1))
            {
                MessageBox.Show("云图文件不存在: " + sMapPath1, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(sMapPath2))
            {
                MessageBox.Show("云图文件不存在: " + sMapPath2, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(sMapPath3))
            {
                MessageBox.Show("云图文件不存在: " + sMapPath3, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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
