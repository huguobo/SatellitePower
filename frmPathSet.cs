using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SatellitePower
{
    public partial class frmPathSet : Form
    {
        public CSysConfig sysCfgMy;
        public CSysParams sysParams;

        public frmPathSet()
        {
            InitializeComponent();
        }

        private void InitThisForm()
        {
            //0-100，2015-1-22,wm add
            //nudRGBvis.Minimum = decimal.MinValue;
            //nudRGBvis.Maximum = decimal.MaxValue;
            if (null != sysCfgMy)
            {
                textBox1.Text = sysCfgMy.sActPath;
                //textBox2.Text = sysCfgMy.sRawPath;
                //sysCfgMy.sPalletPath;
                //sysCfgMy.sPluginsPath;
                //textBox1.Text = sysCfgMy.sWorkPath;
                //textBox1.Text = sysCfgMy.sMapPath;
                //textBox3.Text = sysCfgMy.sBackupRawPath;

                //textBox5.Text = sysCfgMy.sSecPath;
                //textBox6.Text = sysCfgMy.sIR3Path;
                //textBox7.Text = sysCfgMy.sIR4Path;
                //textBox8.Text = sysCfgMy.sVIS1Path;
            }

            if (null != sysParams)
            {
                textBox4.Text = sysParams.sMapPath;
                textBox3.Text = sysParams.AutoSaveImgPath;

                //sysParams.TitleFontName = labFont.Font.Name;
                //sysParams.TitleFontSize = labFont.Font.Size;
                //sysParams.TitleFontFS = labFont.Font.Style;
                labFont.Font = new Font(sysParams.TitleFontName, sysParams.TitleFontSize, sysParams.TitleFontFS);
                labFont.Text = sysParams.TitleFontName + "," + sysParams.TitleFontSize;

                //sysParams.TitleFontColorVal = pnlColor.BackColor.ToArgb();
                pnlColor.BackColor = Color.FromArgb(sysParams.TitleFontColorVal);

                nudHigh.Value = sysParams.YDG_HighMeter;
                nudLow.Value = sysParams.YDG_LowMeter;
                nudHVal.Value = sysParams.YDG_HighVal;
                nudLVal.Value = sysParams.YDG_LowVal;

                //地面
                nudFloor.Value = sysParams.YDG_FloorMeter;// = 0;  //米
                nudFVal.Value = sysParams.YDG_FloorVal;// = 20;   //度

                nudFLeft.Value = sysParams.TitleLeft;
                nudFTop.Value = sysParams.TitleTop;

                nudViewDay.Value = sysParams.NearMayDays;

                chkDelMap.Checked = sysParams.AutoDelMapData;
                chkTeatMT.Checked = sysParams.AutoTestMTSAT;
                chkAutoSaveImg.Checked = sysParams.AutoSaveImg;

                //sysParams.AutoSaveImgTime = dtpSaveImg.Value.TimeOfDay.ToString();
                dtpSaveImg.Text = sysParams.AutoSaveImgTime;


                labMFont.Font = new Font(sysParams.MouseFontName, sysParams.MouseFontSize, sysParams.MouseFontFS);
                labMFont.Text = sysParams.MouseFontName + "," + sysParams.MouseFontSize;
                pnlMCol.BackColor = Color.FromArgb(sysParams.MouseFontColorVal);
                cbxMouseTM.Checked = sysParams.MouseLablTM;
                cbxMouseGet.Checked = sysParams.MouseGetVal;

                decimal dtmp = sysParams.VIScRGBOffset * 1.0m / 100;
                if (dtmp < nudRGBvis.Minimum)
                {
                    dtmp = nudRGBvis.Minimum;
                }
                //新值：x=(y-14)*10
                //原值：y=x/10+14
                dtmp = (dtmp - 14) * 10;
                nudRGBvis.Value = dtmp;//VIS颜色增强 = 20

                //MTSAT，2015-1-22，wm add
                decimal dtmp_MT = sysParams.VIScRGBoffset_MT;
                if (dtmp_MT < nudRGBvis_MT.Minimum)
                {
                    dtmp_MT = nudRGBvis_MT.Minimum;
                }
                nudRGBvis_MT.Value = dtmp_MT;

                
                //****************************************
                //地理位置微调，20150827 wm add
                decimal tmp = sysParams.Map_FY_top;
                if (tmp < numUD_FY_top.Minimum)
                {
                    tmp = numUD_FY_top.Minimum;
                }
                numUD_FY_top.Value = tmp;

                tmp = sysParams.Map_MT_top;
                if (tmp < numUD_MT_top.Minimum)
                {
                    tmp = numUD_MT_top.Minimum;
                }
                numUD_MT_top.Value = tmp;

                tmp = sysParams.Map_FY_left;
                if (tmp < numUD_FY_left.Minimum)
                {
                    tmp = numUD_FY_left.Minimum;
                }
                numUD_FY_left.Value = tmp;

                tmp = sysParams.Map_MT_left;
                if (tmp < numUD_MT_left.Minimum)
                {
                    tmp = numUD_MT_left.Minimum;
                }
                numUD_MT_left.Value = tmp;
                //****************************************

            }

        }

        private void frmPathSet_Load(object sender, EventArgs e)
        {
            InitThisForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //动画文件路径
            folderBrowserDialog1.Description = "请选择目录";
            folderBrowserDialog1.SelectedPath = sysCfgMy.sActPath;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();
                textBox1.Text = sPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ////原始数据路径
            //folderBrowserDialog1.Description = "请选择目录";
            //folderBrowserDialog1.SelectedPath = sysCfgMy.sRawPath;
            //if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{

            //    string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();
            //    textBox2.Text = sPath;
            //}
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //用户地图
            folderBrowserDialog1.Description = "请选择目录";
            folderBrowserDialog1.SelectedPath = sysParams.sMapPath;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();
                textBox4.Text = sPath;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            sysCfgMy.ResetDef();
            //sActPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "Animations" + Path.DirectorySeparatorChar.ToString();
            sysParams.sMapPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapspace" + Path.DirectorySeparatorChar.ToString();
            sysParams.AutoSaveImgPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "Image" + Path.DirectorySeparatorChar.ToString();//自动存图输出路径
            InitThisForm();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = labFont.Font;
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                labFont.Font = fontDialog1.Font;
                labFont.Text = labFont.Font.Name + "," + labFont.Font.Size;
                //+ " | " + this.Font.ToString() + " | " + this.Font.Style.ToString();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnlColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pnlColor.BackColor = colorDialog1.Color;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = labMFont.Font;
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                labMFont.Font = fontDialog1.Font;
                labMFont.Text = labMFont.Font.Name + "," + labMFont.Font.Size;
                //+ " | " + this.Font.ToString() + " | " + this.Font.Style.ToString();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnlMCol.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pnlMCol.BackColor = colorDialog1.Color;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //智能保存路径            
            folderBrowserDialog1.Description = "请选择目录";
            folderBrowserDialog1.SelectedPath = sysParams.AutoSaveImgPath;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();
                textBox3.Text = sPath;
            }
        }

        //2014-12-4 wm add
        private void nudHVal_ValueChanged(object sender, EventArgs e)
        {
            //nudLVal.Minimum = nudHVal.Value;
            //nudFVal.Minimum = nudFVal.Value;
        }
        //2014-12-4 wm add
        private void nudLVal_ValueChanged(object sender, EventArgs e)
        {
            //nudFVal.Minimum = nudLVal.Value;
            //nudHVal.Maximum = nudLVal.Value; 
        }
        //2014-12-4 wm add
        private void nudFVal_ValueChanged(object sender, EventArgs e)
        {
            //nudLVal.Maximum = nudFVal.Value;
            //nudHVal.Maximum = nudFVal.Value;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //if (nudHVal.Value >= nudLVal.Value)
            //{
            //    MessageBox.Show("高空温度应该低于低空温度！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (nudLVal.Value >= nudFVal.Value)
            //{
            //    MessageBox.Show("低空温度应该低于地面温度！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (nudHigh.Value <= nudLow.Value)
            //{
            //    MessageBox.Show("高空距离应该高于低空距离！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (nudLow.Value <= nudFloor.Value)
            //{
            //    MessageBox.Show("低空距离应该高于地面距离！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            try
            {
                if (!Directory.Exists(textBox1.Text))
                {
                    Directory.CreateDirectory(textBox1.Text);
                }

            }
            catch (Exception e1)
            {
                MessageBox.Show(string.Format("动画文件目录设置错误，请重新设置该路径！\n{0}", textBox1.Text));
                return;
            }

            try
            {
                if (!Directory.Exists(textBox4.Text))
                {
                    Directory.CreateDirectory(textBox4.Text);
                }

            }
            catch (Exception e1)
            {
                MessageBox.Show(string.Format("地图文件路径设置错误，请重新设置该路径！\n{0}", textBox4.Text));
                return;
            }

            try
            {
                if (!Directory.Exists(textBox3.Text))
                {
                    Directory.CreateDirectory(textBox3.Text);
                }

            }
            catch (Exception e1)
            {
                MessageBox.Show(string.Format("智能保存路径设置错误，请重新设置该路径！\n{0}", textBox3.Text));
                return;
            }

            sysCfgMy.sActPath = textBox1.Text;
            //sysCfgMy.sRawPath = textBox2.Text;
            //sysCfgMy.sBackupRawPath = textBox3.Text;
            sysParams.sMapPath = textBox4.Text;
            //sysCfgMy.sSecPath = textBox5.Text;
            //sysCfgMy.sIR3Path = textBox6.Text;
            //sysCfgMy.sIR4Path = textBox7.Text;
            //sysCfgMy.sVIS1Path = textBox8.Text;
            sysParams.AutoSaveImgPath = textBox3.Text;

            sysParams.TitleFontName = labFont.Font.Name;
            sysParams.TitleFontSize = labFont.Font.Size;
            sysParams.TitleFontFS = labFont.Font.Style;
            sysParams.TitleFontColorVal = pnlColor.BackColor.ToArgb();

            sysParams.YDG_HighMeter = Convert.ToInt32(nudHigh.Value);//高空高度
            sysParams.YDG_LowMeter = Convert.ToInt32(nudLow.Value);//低空高度

            sysParams.YDG_HighVal = Convert.ToInt32(nudHVal.Value);//高空温度
            sysParams.YDG_LowVal = Convert.ToInt32(nudLVal.Value);//低空温度

            //地面
            sysParams.YDG_FloorVal = Convert.ToInt32(nudFVal.Value);//地面温度
            sysParams.YDG_FloorMeter = Convert.ToInt32(nudFloor.Value);//地面高度

            sysParams.TitleLeft = Convert.ToInt32(nudFLeft.Value);
            sysParams.TitleTop = Convert.ToInt32(nudFTop.Value);

            sysParams.NearMayDays = Convert.ToInt32(nudViewDay.Value);

            //sysParams.VIScRGBOffset = Convert.ToInt32(nudRGBvis.Value * 100);//VIS颜色增强 = 20

            //新值：x=(y-14)*10
            //原值：y=x/10+14
            decimal dTmp = nudRGBvis.Value;
            dTmp = dTmp / 10 + 14;
            sysParams.VIScRGBOffset = Convert.ToInt32(dTmp * 100);

            //MTSAT，2015-1-22，wm add
            sysParams.VIScRGBoffset_MT = nudRGBvis_MT.Value;


            //地理位置微调，20150827 wm add
            sysParams.Map_FY_top = numUD_FY_top.Value;
            sysParams.Map_MT_top = numUD_MT_top.Value;
            sysParams.Map_FY_left = numUD_FY_left.Value;
            sysParams.Map_MT_left = numUD_MT_left.Value;

            sysParams.AutoDelMapData = chkDelMap.Checked;
            sysParams.AutoTestMTSAT = chkTeatMT.Checked;
            sysParams.AutoSaveImg = chkAutoSaveImg.Checked;
            sysParams.AutoSaveImgTime = dtpSaveImg.Value.TimeOfDay.ToString();

            //鼠标取值信息字体设置
            sysParams.MouseFontName = labMFont.Font.Name;
            sysParams.MouseFontSize = labMFont.Font.Size;
            sysParams.MouseFontFS = labMFont.Font.Style;
            sysParams.MouseFontColorVal = pnlMCol.BackColor.ToArgb();
            sysParams.MouseLablTM = cbxMouseTM.Checked;
            sysParams.MouseGetVal = cbxMouseGet.Checked;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
