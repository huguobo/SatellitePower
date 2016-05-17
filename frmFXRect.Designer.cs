namespace SatellitePower.formSys
{
    partial class frmFXRect
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
            this.pbxRight = new System.Windows.Forms.PictureBox();
            this.pbxLeft = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLeft)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(381, 509);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(254, 40);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "关  闭";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbxRight
            // 
            this.pbxRight.BackColor = System.Drawing.Color.Black;
            this.pbxRight.Location = new System.Drawing.Point(3, 0);
            this.pbxRight.Name = "pbxRight";
            this.pbxRight.Size = new System.Drawing.Size(500, 500);
            this.pbxRight.TabIndex = 4;
            this.pbxRight.TabStop = false;
            // 
            // pbxLeft
            // 
            this.pbxLeft.BackColor = System.Drawing.Color.Black;
            this.pbxLeft.Location = new System.Drawing.Point(3, 3);
            this.pbxLeft.Name = "pbxLeft";
            this.pbxLeft.Size = new System.Drawing.Size(500, 500);
            this.pbxLeft.TabIndex = 3;
            this.pbxLeft.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pbxLeft);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 500);
            this.panel1.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pbxRight);
            this.panel3.Location = new System.Drawing.Point(506, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(500, 500);
            this.panel3.TabIndex = 11;
            // 
            // frmFXRect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 558);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmFXRect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "云图对比";
            this.Load += new System.EventHandler(this.frmFXRect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLeft)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pbxRight;
        private System.Windows.Forms.PictureBox pbxLeft;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
    }
}