namespace SatellitePower
{
    partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.pBarMain = new System.Windows.Forms.ProgressBar();
            this.labInfo = new System.Windows.Forms.Label();
            this.timerInfo = new System.Windows.Forms.Timer(this.components);
            this.labName = new System.Windows.Forms.Label();
            this.labV = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pBarMain
            // 
            this.pBarMain.BackColor = System.Drawing.Color.White;
            this.pBarMain.Location = new System.Drawing.Point(449, 543);
            this.pBarMain.MarqueeAnimationSpeed = 50;
            this.pBarMain.Name = "pBarMain";
            this.pBarMain.Size = new System.Drawing.Size(300, 23);
            this.pBarMain.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pBarMain.TabIndex = 1;
            this.pBarMain.UseWaitCursor = true;
            // 
            // labInfo
            // 
            this.labInfo.BackColor = System.Drawing.Color.Transparent;
            this.labInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labInfo.ForeColor = System.Drawing.Color.Red;
            this.labInfo.Location = new System.Drawing.Point(293, 543);
            this.labInfo.Name = "labInfo";
            this.labInfo.Size = new System.Drawing.Size(150, 23);
            this.labInfo.TabIndex = 2;
            this.labInfo.Text = "正在加载，请稍后...";
            this.labInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerInfo
            // 
            this.timerInfo.Interval = 500;
            this.timerInfo.Tick += new System.EventHandler(this.timerInfo_Tick);
            // 
            // labName
            // 
            this.labName.BackColor = System.Drawing.Color.Transparent;
            this.labName.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labName.ForeColor = System.Drawing.Color.Red;
            this.labName.Location = new System.Drawing.Point(354, 91);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(280, 25);
            this.labName.TabIndex = 3;
            this.labName.Text = "多星气象卫星云图处理程序 v2.0";
            this.labName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labV
            // 
            this.labV.BackColor = System.Drawing.Color.Transparent;
            this.labV.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labV.ForeColor = System.Drawing.Color.Red;
            this.labV.Location = new System.Drawing.Point(624, 95);
            this.labV.Name = "labV";
            this.labV.Size = new System.Drawing.Size(40, 23);
            this.labV.TabIndex = 4;
            this.labV.Text = "v2.0";
            this.labV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1008, 600);
            this.ControlBox = false;
            this.Controls.Add(this.labV);
            this.Controls.Add(this.labName);
            this.Controls.Add(this.labInfo);
            this.Controls.Add(this.pBarMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pBarMain;
        private System.Windows.Forms.Label labInfo;
        private System.Windows.Forms.Timer timerInfo;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.Label labV;
    }
}