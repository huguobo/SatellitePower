namespace SatellitePower.formSys
{
    partial class frmFXSelWay
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
            this.rdbVis = new System.Windows.Forms.RadioButton();
            this.rdbIR4 = new System.Windows.Forms.RadioButton();
            this.rdbIR3 = new System.Windows.Forms.RadioButton();
            this.rdbIR2 = new System.Windows.Forms.RadioButton();
            this.rdbIR1 = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(232, 209);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(72, 209);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 40);
            this.btnOk.TabIndex = 8;
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
            this.panel1.Size = new System.Drawing.Size(394, 195);
            this.panel1.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbVis);
            this.groupBox1.Controls.Add(this.rdbIR4);
            this.groupBox1.Controls.Add(this.rdbIR3);
            this.groupBox1.Controls.Add(this.rdbIR2);
            this.groupBox1.Controls.Add(this.rdbIR1);
            this.groupBox1.Location = new System.Drawing.Point(31, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 162);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // rdbVis
            // 
            this.rdbVis.Location = new System.Drawing.Point(50, 107);
            this.rdbVis.Name = "rdbVis";
            this.rdbVis.Size = new System.Drawing.Size(99, 24);
            this.rdbVis.TabIndex = 10;
            this.rdbVis.Text = "可见光";
            this.rdbVis.UseVisualStyleBackColor = true;
            // 
            // rdbIR4
            // 
            this.rdbIR4.Location = new System.Drawing.Point(183, 69);
            this.rdbIR4.Name = "rdbIR4";
            this.rdbIR4.Size = new System.Drawing.Size(99, 24);
            this.rdbIR4.TabIndex = 9;
            this.rdbIR4.Text = "红外四";
            this.rdbIR4.UseVisualStyleBackColor = true;
            // 
            // rdbIR3
            // 
            this.rdbIR3.Location = new System.Drawing.Point(50, 69);
            this.rdbIR3.Name = "rdbIR3";
            this.rdbIR3.Size = new System.Drawing.Size(99, 24);
            this.rdbIR3.TabIndex = 8;
            this.rdbIR3.Text = "水汽";
            this.rdbIR3.UseVisualStyleBackColor = true;
            // 
            // rdbIR2
            // 
            this.rdbIR2.Location = new System.Drawing.Point(183, 31);
            this.rdbIR2.Name = "rdbIR2";
            this.rdbIR2.Size = new System.Drawing.Size(99, 24);
            this.rdbIR2.TabIndex = 7;
            this.rdbIR2.Text = "红外二";
            this.rdbIR2.UseVisualStyleBackColor = true;
            // 
            // rdbIR1
            // 
            this.rdbIR1.Checked = true;
            this.rdbIR1.Location = new System.Drawing.Point(50, 31);
            this.rdbIR1.Name = "rdbIR1";
            this.rdbIR1.Size = new System.Drawing.Size(99, 24);
            this.rdbIR1.TabIndex = 6;
            this.rdbIR1.TabStop = true;
            this.rdbIR1.Text = "红外一";
            this.rdbIR1.UseVisualStyleBackColor = true;
            // 
            // frmFXSelWay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 278);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmFXSelWay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择通道";
            this.Load += new System.EventHandler(this.frmFXSelWay_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbIR2;
        private System.Windows.Forms.RadioButton rdbIR1;
        private System.Windows.Forms.RadioButton rdbVis;
        private System.Windows.Forms.RadioButton rdbIR4;
        private System.Windows.Forms.RadioButton rdbIR3;
    }
}