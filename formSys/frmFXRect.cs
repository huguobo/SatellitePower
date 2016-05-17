using System;
using System.Drawing;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;

namespace SatellitePower.formSys
{
    public partial class frmFXRect : Form
    {
        public Bitmap bmpRect1;
        public Bitmap bmpRect2;

        public PointF pf1, pf2;
        public string mapPath1, mapPath2;
        public VISSObject visObj1, visObj2;
        public SVissrUnit svu1, svu2;
        public int w, h;//原始图像尺寸
        public int JingDu_left, JingDu_right, WeiDu_top, WeiDu_down;//原始图像经纬范围
        public PointF p1, p2;//矩形左上点、右下点
        public int x1, x2, y1, y2;//矩形在原图起止像素点
        public int w1, h1;//矩形尺寸
        public int[,] hd1, hd2;//矩形各像素点灰度

        public frmFXRect()
        {
            InitializeComponent();
        }

        private void frmFXRect_Load(object sender, EventArgs e)
        {
            init();
            HuiTu(bmpRect1, hd1);
            HuiTu(bmpRect2, hd2);
            Bitmap newpic = new Bitmap(bmpRect1, pbxLeft.Width, pbxLeft.Height);
            Bitmap newpic0 = new Bitmap(bmpRect2, pbxRight.Width, pbxRight.Height);
            bmpRect1.Dispose();
            bmpRect2.Dispose();
            pbxLeft.Image = PubUnit.copyBitmap(newpic);
            pbxRight.Image = PubUnit.copyBitmap(newpic0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void init()
        {

            svu1 = visObj1.svuObj;
            svu2 = visObj2.svuObj;
            w = svu1.iMWidth == svu2.iMWidth ? svu1.iMWidth : 0;
            h = svu1.iMHigh == svu2.iMHigh ? svu1.iMHigh : 0;
            if (w == 0 || h == 0)
            {
                MessageBox.Show("检测到两个通道云图不匹配，无法云类对比！", "提示");
                return;
            }
            JingDu_left = svu1.iMLeft == svu2.iMLeft ? svu2.iMLeft : 0;
            JingDu_right = svu1.iMRight == svu2.iMRight ? svu2.iMRight : 0;
            WeiDu_top = svu1.iMTop == svu2.iMTop ? svu2.iMTop : 0;
            WeiDu_down = svu1.iMBotton == svu2.iMBotton ? svu2.iMBotton : 0;
            if (JingDu_left == 0 || JingDu_right == 0 || WeiDu_top == 0 || WeiDu_down == 0)
            {
                MessageBox.Show("检测到两个通道云图不匹配，无法云类对比！", "提示");
                return;
            }

            float x11 = pf1.X < pf2.X ? pf1.X : pf2.X;//x小
            float x22 = pf1.X > pf2.X ? pf1.X : pf2.X;//x大
            float y11 = pf1.Y < pf2.Y ? pf1.Y : pf2.Y;//y小
            float y22 = pf1.Y > pf2.Y ? pf1.Y : pf2.Y;//y大
            //重定义矩形经纬范围,p1恒为左上，p2恒为右下
            p1.X = x11;
            p1.Y = y22;
            p2.X = x22;
            p2.Y = y11;

            //p1.X = (int)Math.Round(x11, 0) - 1;
            //p1.Y = (int)Math.Round(y22, 0) + 1;
            //p2.X = (int)Math.Round(x22, 0) + 1;
            //p2.Y = (int)Math.Round(y11, 0) - 1;

            JWtoXS();//经纬转像素点坐标

            //===========================取两个矩形灰度值===============================
            hd1 = new int[w1, h1];
            hd2 = new int[w1, h1];
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    int val1 = svu1.bVissData[(y1 + j) * w + (x1 + i)];
                    int val2 = svu2.bVissData[(y1 + j) * w + (x1 + i)];
                    hd1[i, j] = val1;
                    hd2[i, j] = val2;
                }
            }
            //==================================================================
            bmpRect1 = new Bitmap(w1, h1);
            bmpRect2 = new Bitmap(w1, h1);
        }

        //经纬转像素点坐标
        public void JWtoXS()
        {
            int x10 = (int)Math.Round((p1.X - JingDu_left) * (w / (double)Math.Abs(JingDu_right - JingDu_left)));
            int x20 = (int)Math.Round((p2.X - JingDu_left) * (w / (double)Math.Abs(JingDu_right - JingDu_left)));
            int y10 = (int)Math.Round((WeiDu_top - p1.Y) * (h / (double)Math.Abs(WeiDu_top - WeiDu_down)));
            int y20 = (int)Math.Round((WeiDu_top - p2.Y) * (h / (double)Math.Abs(WeiDu_top - WeiDu_down)));
            if (x10 < x20 && y10 < y20)
            {
                x1 = x10;
                x2 = x20;
                y1 = y10;
                y2 = y20;
                w1 = x20 - x10;
                h1 = y20 - y10;
            }
            else
            {
                MessageBox.Show("所选矩形区域无法进行云类对比，请重新选择！", "提示");
                return;
            }
        }

        //填充bmp
        public void HuiTu(Bitmap bmp, int[,] hd)
        {
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    Color wxc = Color.FromArgb(hd[i, j], hd[i, j], hd[i, j]);
                    bmp.SetPixel(i, j, wxc);
                }
            }
        }
    }
}
