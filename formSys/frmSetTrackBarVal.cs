using System;
using System.Windows.Forms;

namespace SatellitePower
{
    public partial class frmSetTrackBarVal : Form
    {
        public int iFlag = 0;
        public int iVal1 = 0;//上限
        public int iVal2 = 0;//下限
        public double dVal1 = 0.00d;
        public double dVal2 = 0.00d;
        public int iContrastType = 0;//0=fy[double],1=sec[float]
        public int iNowType = 1;    //1=IR,2=VIS,用来区分显示'温度'还是'反照率'

        public double[] dData;//定标数据实际值,len = 64/1024
        //public float[] fDataSec;//sec 定标数据 1024

        public frmSetTrackBarVal()
        {
            InitializeComponent();
        }

        private void frmSetTrackBarVal_Load(object sender, EventArgs e)
        {
            //iNowType = 1;    //1=IR,2=VIS,用来区分显示'温度'还是'反照率'
            if (iNowType == 1)
            {
                this.Text += " ( 温度 )";
                dVal1 = dData[0];
                dVal2 = dData[dData.Length - 1];
                dVal1 = PubUnit.getTemperatureTkToC(dVal1);
                dVal2 = PubUnit.getTemperatureTkToC(dVal2);
                labSM1.Text = "(下限至 256)";
                labSM2.Text = "(0 至上限)";
                labSM3.Text = "(下限至 " + dVal1.ToString("f3") + " 摄氏度)";
                labSM4.Text = "( " + dVal2.ToString("f3") + " 摄氏度至上限)";
            }
            else//2=VIS
            {
                this.Text += " ( 反照率 )";
                dVal1 = dData[dData.Length - 1];
                dVal2 = dData[0];
                labSM1.Text = "(下限至 64)";
                labSM2.Text = "(0 至上限)";
                labSM3.Text = "(下限至 " + dVal1.ToString("f3") + ")";
                labSM4.Text = "(" + dVal2.ToString("f3") + " 至上限)";
            }
            //--------------------------------------
            nudJ1.Maximum = dData.Length;
            nudJ2.Maximum = dData.Length;

            nudJ1.Value = iVal1;
            nudJ2.Value = iVal2;
            //--------------------------------------
            //decimal.TryParse()
            decimal decV1 = Convert.ToDecimal(dVal1);
            decimal decV2 = Convert.ToDecimal(dVal2);
            nudS1.Maximum = decV1;
            nudS1.Minimum = decV2;
            nudS1.Value = decV1;

            nudS2.Maximum = decV1;
            nudS2.Minimum = decV2;
            nudS2.Value = decV2;
            //--------------------------------------
            setMainSet(0);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (rdbJB.Checked)
            {
                iVal1 = Convert.ToInt32(nudJ1.Value);
                iVal2 = Convert.ToInt32(nudJ2.Value);

                if (iVal1 <= iVal2)
                {
                    MessageBox.Show("上限应该大于下限！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                    return;
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                decimal decV1 = nudS1.Value;
                decimal decV2 = nudS2.Value;

                if (decV1 <= decV2)
                {
                    MessageBox.Show("上限应该大于下限！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                double dselV1 = Convert.ToDouble(decV1);
                double dselV2 = Convert.ToDouble(decV2);

                //public int iVal1 = 0;//上限
                //public int iVal2 = 0;//下限

                int k = 0;
                for (int i = 0; i < dData.Length - 1; i++)
                {
                    dVal1 = dData[i];
                    dVal2 = dData[i + 1];
                    
                    if (iNowType == 1)
                    {
                        //1=IR
                        dVal1 = PubUnit.getTemperatureTkToC(dVal1);
                        dVal2 = PubUnit.getTemperatureTkToC(dVal2);

                        if ((dselV1 > dVal2) && (dselV1 < dVal1))
                        {
                            iVal2 = i;
                            k++;
                        }

                        if ((dselV2 > dVal2) && (dselV2 < dVal1))
                        {
                            iVal1 = i;
                            k++;
                        }
                    }
                    else//2=VIS
                    {
                        if ((dselV1 > dVal1) && (dselV1 < dVal2))
                        {
                            iVal1 = i;
                            k++;
                        }

                        if ((dselV2 > dVal1) && (dselV2 < dVal2))
                        {
                            iVal2 = i;
                            k++;
                        }
                    }
                    
                    if (k >= 2)
                    {
                        break;
                    }
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void btnDef_Click(object sender, EventArgs e)
        {
            rdbJB.Checked = true;
            nudJ1.Value = nudJ1.Maximum;
            nudJ2.Value = 0;
        }

        private void setMainSet(int iFZ)
        {
            groupBox1.Enabled = (iFZ == 0);
            groupBox2.Enabled = (iFZ == 1);
        }

        private void rdbJB_CheckedChanged(object sender, EventArgs e)
        {
            setMainSet(0);
        }

        private void rdbSZ_CheckedChanged(object sender, EventArgs e)
        {
            setMainSet(1);
        }

        private void btnGood_Click(object sender, EventArgs e)
        {
            rdbJB.Checked = true;
            if (iNowType == 1)
            {
                //1=IR
                nudJ1.Value = nudJ1.Maximum;
                nudJ2.Value = 130;
            }
            else//2=VIS
            {
                nudJ1.Value = nudJ1.Maximum;
                nudJ2.Value = 20;
            }
        }

    }
}
