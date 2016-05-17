using System;
using System.Windows.Forms;
using System.IO;

namespace SatellitePower
{
    public partial class frmATimeSel : Form
    {
        public DateTime dtStart = new DateTime(2000, 1, 1, 0, 0, 0);
        public DateTime dtEndat = new DateTime(2050, 1, 1, 0, 0, 0);
        public DateTime dtNowDT = DateTime.Now;
        public string sOutFolder = "";
        public string sActPath = "";

        public frmATimeSel()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string sPathDir = sActPath + tbxName.Text;
            if (Directory.Exists(sPathDir))
            {
                MessageBox.Show("同名动画已存在，请重新命名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            sOutFolder = tbxName.Text;

            double dHoursLen = Convert.ToDouble(nudSJ.Value);
            if (rdbJB.Checked)
            {
                //之前
                dtEndat = dtNowDT;
                dtStart = dtEndat.AddHours(-dHoursLen);

                dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
            }
            else
            {
                //之后
                dtStart = dtNowDT;
                dtEndat = dtStart.AddHours(dHoursLen);

                //dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
                dtEndat = new DateTime(dtEndat.Year, dtEndat.Month, dtEndat.Day, dtEndat.Hour, 59, 59);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void frmATimeSel_Load(object sender, EventArgs e)
        {
            labSM1.Text = "当前时间：" + dtNowDT.ToString("yyyy-MM-dd HH:mm");
            tbxName.Text = sOutFolder;
        }
    }
}
