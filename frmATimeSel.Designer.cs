namespace SatellitePower
{
    partial class frmATimeSel
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxName = new System.Windows.Forms.TextBox();
            this.labSM1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudSJ = new System.Windows.Forms.NumericUpDown();
            this.rdbSZ = new System.Windows.Forms.RadioButton();
            this.rdbJB = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSJ)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(231, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(71, 252);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 40);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 231);
            this.panel1.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbxName);
            this.groupBox1.Controls.Add(this.labSM1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudSJ);
            this.groupBox1.Controls.Add(this.rdbSZ);
            this.groupBox1.Controls.Add(this.rdbJB);
            this.groupBox1.Location = new System.Drawing.Point(30, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 200);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 19);
            this.label2.TabIndex = 14;
            this.label2.Text = "名称";
            // 
            // tbxName
            // 
            this.tbxName.Location = new System.Drawing.Point(85, 141);
            this.tbxName.Name = "tbxName";
            this.tbxName.Size = new System.Drawing.Size(200, 29);
            this.tbxName.TabIndex = 13;
            // 
            // labSM1
            // 
            this.labSM1.AutoSize = true;
            this.labSM1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSM1.ForeColor = System.Drawing.Color.Blue;
            this.labSM1.Location = new System.Drawing.Point(21, 34);
            this.labSM1.Name = "labSM1";
            this.labSM1.Size = new System.Drawing.Size(104, 19);
            this.labSM1.TabIndex = 12;
            this.labSM1.Text = "当前时间：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(238, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "小时";
            // 
            // nudSJ
            // 
            this.nudSJ.Location = new System.Drawing.Point(85, 103);
            this.nudSJ.Name = "nudSJ";
            this.nudSJ.Size = new System.Drawing.Size(140, 29);
            this.nudSJ.TabIndex = 8;
            this.nudSJ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudSJ.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // rdbSZ
            // 
            this.rdbSZ.Location = new System.Drawing.Point(186, 64);
            this.rdbSZ.Name = "rdbSZ";
            this.rdbSZ.Size = new System.Drawing.Size(99, 24);
            this.rdbSZ.TabIndex = 7;
            this.rdbSZ.Text = "之后";
            this.rdbSZ.UseVisualStyleBackColor = true;
            // 
            // rdbJB
            // 
            this.rdbJB.Checked = true;
            this.rdbJB.Location = new System.Drawing.Point(81, 64);
            this.rdbJB.Name = "rdbJB";
            this.rdbJB.Size = new System.Drawing.Size(99, 24);
            this.rdbJB.TabIndex = 6;
            this.rdbJB.TabStop = true;
            this.rdbJB.Text = "之前";
            this.rdbJB.UseVisualStyleBackColor = true;
            // 
            // frmATimeSel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 323);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmATimeSel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "时间设置";
            this.Load += new System.EventHandler(this.frmATimeSel_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSJ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labSM1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudSJ;
        private System.Windows.Forms.RadioButton rdbSZ;
        private System.Windows.Forms.RadioButton rdbJB;
        private System.Windows.Forms.TextBox tbxName;
        private System.Windows.Forms.Label label2;
    }
}