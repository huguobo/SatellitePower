using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace SatellitePower
{
    public partial class Splash : Form
    {
        public int xPos;
        public int yPos;
        public bool MoveFlag;

        public double rate = 0.0;

        public Splash()
        {
            InitializeComponent();

            if (rate > 1.5)//16:9
            {
                //宽=533，高=300
                this.Width = 550;
                this.Height = 300;
                Point pLT1 = new Point(145, 25);
                pnlMain.Location = pLT1;
                Point pLT2 = new Point(520, 0);
                btnClose.Location = pLT2;
            }
            else//4:3
            {
                //宽=400，高=300
                this.Width = 400;
                this.Height = 300;
                Point pLT1 = new Point(70, 25);
                pnlMain.Location = pLT1;
                Point pLT2 = new Point(370, 0);
                btnClose.Location = pLT2;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 鼠标拖动
        private void Splash_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;
            this.Cursor = Cursors.Hand;
            xPos = e.X;
            yPos = e.Y;
        }

        private void Splash_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                this.Left += Convert.ToInt16(e.X - xPos);
                this.Top += Convert.ToInt16(e.Y - yPos);
            }
        }

        private void Splash_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
            this.Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;
            this.Cursor = Cursors.Hand;
            xPos = e.X;
            yPos = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                this.Left += Convert.ToInt16(e.X - xPos);
                this.Top += Convert.ToInt16(e.Y - yPos);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
            this.Cursor = Cursors.Default;
        }

        private void labTitle_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;
            this.Cursor = Cursors.Hand;
            xPos = e.X;
            yPos = e.Y;
        }

        private void labTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                this.Left += Convert.ToInt16(e.X - xPos);
                this.Top += Convert.ToInt16(e.Y - yPos);
            }
        }

        private void labTitle_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
            this.Cursor = Cursors.Default;
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;
            this.Cursor = Cursors.Hand;
            xPos = e.X;
            yPos = e.Y;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                this.Left += Convert.ToInt16(e.X - xPos);
                this.Top += Convert.ToInt16(e.Y - yPos);
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
            this.Cursor = Cursors.Default;
        }

        private void pnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            MoveFlag = true;
            this.Cursor = Cursors.Hand;
            xPos = e.X;
            yPos = e.Y;
        }

        private void pnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                this.Left += Convert.ToInt16(e.X - xPos);
                this.Top += Convert.ToInt16(e.Y - yPos);
            }
        }

        private void pnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void Splash_Load(object sender, EventArgs e)
        {
            
        }
    }
}