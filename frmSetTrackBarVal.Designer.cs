namespace SatellitePower
{
    partial class frmSetTrackBarVal
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
            this.rdbJB = new System.Windows.Forms.RadioButton();
            this.rdbSZ = new System.Windows.Forms.RadioButton();
            this.nudJ1 = new System.Windows.Forms.NumericUpDown();
            this.nudJ2 = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nudS1 = new System.Windows.Forms.NumericUpDown();
            this.nudS2 = new System.Windows.Forms.NumericUpDown();
            this.labSM4 = new System.Windows.Forms.Label();
            this.labSM3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labSM2 = new System.Windows.Forms.Label();
            this.labSM1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDef = new System.Windows.Forms.Button();
            this.btnGood = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudJ1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudJ2)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudS1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(438, 348);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(311, 348);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 40);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // rdbJB
            // 
            this.rdbJB.Checked = true;
            this.rdbJB.Location = new System.Drawing.Point(88, 37);
            this.rdbJB.Name = "rdbJB";
            this.rdbJB.Size = new System.Drawing.Size(150, 24);
            this.rdbJB.TabIndex = 6;
            this.rdbJB.TabStop = true;
            this.rdbJB.Text = "级别";
            this.rdbJB.UseVisualStyleBackColor = true;
            this.rdbJB.CheckedChanged += new System.EventHandler(this.rdbJB_CheckedChanged);
            // 
            // rdbSZ
            // 
            this.rdbSZ.Location = new System.Drawing.Point(347, 37);
            this.rdbSZ.Name = "rdbSZ";
            this.rdbSZ.Size = new System.Drawing.Size(150, 24);
            this.rdbSZ.TabIndex = 7;
            this.rdbSZ.Text = "数值";
            this.rdbSZ.UseVisualStyleBackColor = true;
            this.rdbSZ.CheckedChanged += new System.EventHandler(this.rdbSZ_CheckedChanged);
            // 
            // nudJ1
            // 
            this.nudJ1.Location = new System.Drawing.Point(85, 60);
            this.nudJ1.Name = "nudJ1";
            this.nudJ1.Size = new System.Drawing.Size(130, 29);
            this.nudJ1.TabIndex = 8;
            this.nudJ1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // nudJ2
            // 
            this.nudJ2.Location = new System.Drawing.Point(85, 150);
            this.nudJ2.Name = "nudJ2";
            this.nudJ2.Size = new System.Drawing.Size(130, 29);
            this.nudJ2.TabIndex = 9;
            this.nudJ2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.rdbSZ);
            this.panel1.Controls.Add(this.rdbJB);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 325);
            this.panel1.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nudS1);
            this.groupBox2.Controls.Add(this.nudS2);
            this.groupBox2.Controls.Add(this.labSM4);
            this.groupBox2.Controls.Add(this.labSM3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(297, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 239);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            // 
            // nudS1
            // 
            this.nudS1.DecimalPlaces = 3;
            this.nudS1.Location = new System.Drawing.Point(85, 60);
            this.nudS1.Name = "nudS1";
            this.nudS1.Size = new System.Drawing.Size(130, 29);
            this.nudS1.TabIndex = 16;
            this.nudS1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // nudS2
            // 
            this.nudS2.DecimalPlaces = 3;
            this.nudS2.Location = new System.Drawing.Point(85, 150);
            this.nudS2.Name = "nudS2";
            this.nudS2.Size = new System.Drawing.Size(130, 29);
            this.nudS2.TabIndex = 17;
            this.nudS2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labSM4
            // 
            this.labSM4.AutoSize = true;
            this.labSM4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSM4.ForeColor = System.Drawing.Color.Blue;
            this.labSM4.Location = new System.Drawing.Point(29, 191);
            this.labSM4.Name = "labSM4";
            this.labSM4.Size = new System.Drawing.Size(56, 16);
            this.labSM4.TabIndex = 15;
            this.labSM4.Text = "(说明)";
            // 
            // labSM3
            // 
            this.labSM3.AutoSize = true;
            this.labSM3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSM3.ForeColor = System.Drawing.Color.Blue;
            this.labSM3.Location = new System.Drawing.Point(29, 100);
            this.labSM3.Name = "labSM3";
            this.labSM3.Size = new System.Drawing.Size(192, 16);
            this.labSM3.TabIndex = 14;
            this.labSM3.Text = "(下限至 123.456 摄氏度)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 19);
            this.label4.TabIndex = 13;
            this.label4.Text = "下限";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 19);
            this.label2.TabIndex = 12;
            this.label2.Text = "上限";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labSM2);
            this.groupBox1.Controls.Add(this.labSM1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudJ2);
            this.groupBox1.Controls.Add(this.nudJ1);
            this.groupBox1.Location = new System.Drawing.Point(38, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 239);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // labSM2
            // 
            this.labSM2.AutoSize = true;
            this.labSM2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSM2.ForeColor = System.Drawing.Color.Blue;
            this.labSM2.Location = new System.Drawing.Point(81, 191);
            this.labSM2.Name = "labSM2";
            this.labSM2.Size = new System.Drawing.Size(56, 16);
            this.labSM2.TabIndex = 13;
            this.labSM2.Text = "(说明)";
            // 
            // labSM1
            // 
            this.labSM1.AutoSize = true;
            this.labSM1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSM1.ForeColor = System.Drawing.Color.Blue;
            this.labSM1.Location = new System.Drawing.Point(81, 100);
            this.labSM1.Name = "labSM1";
            this.labSM1.Size = new System.Drawing.Size(56, 16);
            this.labSM1.TabIndex = 12;
            this.labSM1.Text = "(说明)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 19);
            this.label3.TabIndex = 11;
            this.label3.Text = "下限";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "上限";
            // 
            // btnDef
            // 
            this.btnDef.Location = new System.Drawing.Point(57, 348);
            this.btnDef.Name = "btnDef";
            this.btnDef.Size = new System.Drawing.Size(90, 40);
            this.btnDef.TabIndex = 11;
            this.btnDef.Text = "默认";
            this.btnDef.UseVisualStyleBackColor = true;
            this.btnDef.Click += new System.EventHandler(this.btnDef_Click);
            // 
            // btnGood
            // 
            this.btnGood.Location = new System.Drawing.Point(184, 348);
            this.btnGood.Name = "btnGood";
            this.btnGood.Size = new System.Drawing.Size(90, 40);
            this.btnGood.TabIndex = 12;
            this.btnGood.Text = "推荐";
            this.btnGood.UseVisualStyleBackColor = true;
            this.btnGood.Click += new System.EventHandler(this.btnGood_Click);
            // 
            // frmSetTrackBarVal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 412);
            this.Controls.Add(this.btnGood);
            this.Controls.Add(this.btnDef);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSetTrackBarVal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "显示数值设置";
            this.Load += new System.EventHandler(this.frmSetTrackBarVal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudJ1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudJ2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudS1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudS2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RadioButton rdbJB;
        private System.Windows.Forms.RadioButton rdbSZ;
        private System.Windows.Forms.NumericUpDown nudJ1;
        private System.Windows.Forms.NumericUpDown nudJ2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDef;
        private System.Windows.Forms.Label labSM1;
        private System.Windows.Forms.Label labSM4;
        private System.Windows.Forms.Label labSM3;
        private System.Windows.Forms.Label labSM2;
        private System.Windows.Forms.NumericUpDown nudS1;
        private System.Windows.Forms.NumericUpDown nudS2;
        private System.Windows.Forms.Button btnGood;
    }
}