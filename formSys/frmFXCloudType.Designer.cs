namespace SatellitePower.formSys
{
    partial class frmFXCloudType
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pbxYuan = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pbxSeBiao = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pbxMain = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.fcm_label = new System.Windows.Forms.ToolStripStatusLabel();
            this.fcm_pbar = new System.Windows.Forms.ToolStripProgressBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxYuan)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSeBiao)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMain)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pbxYuan);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 530);
            this.panel1.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(195, 505);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "原图（可见光）";
            // 
            // pbxYuan
            // 
            this.pbxYuan.BackColor = System.Drawing.SystemColors.Control;
            this.pbxYuan.Location = new System.Drawing.Point(0, 0);
            this.pbxYuan.Name = "pbxYuan";
            this.pbxYuan.Size = new System.Drawing.Size(500, 500);
            this.pbxYuan.TabIndex = 3;
            this.pbxYuan.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pbxSeBiao);
            this.panel2.Location = new System.Drawing.Point(0, 536);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1006, 50);
            this.panel2.TabIndex = 11;
            // 
            // pbxSeBiao
            // 
            this.pbxSeBiao.BackColor = System.Drawing.Color.White;
            this.pbxSeBiao.Location = new System.Drawing.Point(0, 0);
            this.pbxSeBiao.Name = "pbxSeBiao";
            this.pbxSeBiao.Size = new System.Drawing.Size(1006, 50);
            this.pbxSeBiao.TabIndex = 0;
            this.pbxSeBiao.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.pbxMain);
            this.panel3.Location = new System.Drawing.Point(506, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(500, 530);
            this.panel3.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(180, 505);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "属性分类结果（可见光）";
            // 
            // pbxMain
            // 
            this.pbxMain.BackColor = System.Drawing.SystemColors.Control;
            this.pbxMain.Location = new System.Drawing.Point(0, 0);
            this.pbxMain.Name = "pbxMain";
            this.pbxMain.Size = new System.Drawing.Size(500, 500);
            this.pbxMain.TabIndex = 0;
            this.pbxMain.TabStop = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(431, 592);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 37);
            this.button1.TabIndex = 14;
            this.button1.Text = "关  闭";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fcm_label,
            this.fcm_pbar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 635);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1014, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // fcm_label
            // 
            this.fcm_label.AutoSize = false;
            this.fcm_label.Name = "fcm_label";
            this.fcm_label.Size = new System.Drawing.Size(200, 17);
            // 
            // fcm_pbar
            // 
            this.fcm_pbar.Name = "fcm_pbar";
            this.fcm_pbar.Size = new System.Drawing.Size(200, 16);
            // 
            // frmFXCloudType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1014, 657);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFXCloudType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "属性云分类";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFXCloudType_FormClosed);
            this.Load += new System.EventHandler(this.frmFXCloudType_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxYuan)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxSeBiao)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxMain)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbxYuan;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pbxSeBiao;
        private System.Windows.Forms.PictureBox pbxMain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel fcm_label;
        private System.Windows.Forms.ToolStripProgressBar fcm_pbar;
    }
}