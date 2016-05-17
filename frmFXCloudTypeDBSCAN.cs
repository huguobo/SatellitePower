using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;

namespace SatellitePower.formSys
{
    public partial class frmFXCloudTypeDBSCAN : Form
    {
        public PointF pf1;//第一点
        public PointF pf2;//第二点
        public Point p1;//左上，矩形经纬
        public Point p2;//右下，矩形经纬
        public VISSObject visObj_IR1;
        public SVissrUnit svu_IR1;
        public int w, h;//原图尺寸
        public int w1, h1;//矩形尺寸
        public int JingDu_left, JingDu_right, WeiDu_top, WeiDu_down;//原图经纬
        public string sMapPathName_IR1;
        public int x1, x2, y1, y2;//矩形起止像素点
        public Bitmap pic;//最终显示图像
        public Bitmap pic0;//原始图像

        public int Eps = 4;//点的半径
        public int Minpts = 20;//核心点临界值
        public int pointnum;//样本总数
        public int clustered_points_count = 0;//已分类点的数量
        public int clusters_count;//总类数

        public int[,] data_id;//类别,0=未分类
        public int[,] data_type;//-999=无效点，888=核心点，999=噪点
        public int[,] hd;//灰度矩阵
        public int[] cluster_hd_sum;//簇灰度和
        public int[] cluster_hd_avg;//簇灰度平均值
        public int[] cluster_num;//簇内元素个数
        public Point[] cluster_center;//簇中心点
        public Color[] cluster_center_color;

        public frmFXCloudTypeDBSCAN()
        {
            InitializeComponent();
        }

        //初始化矩阵相关数据，2014-10-9 wm add
        public void init()
        {
            svu_IR1 = visObj_IR1.svuObj;
            w = svu_IR1.iMWidth;
            h = svu_IR1.iMHigh;

            JingDu_left = svu_IR1.iMLeft;
            JingDu_right = svu_IR1.iMRight;
            WeiDu_top = svu_IR1.iMTop;
            WeiDu_down = svu_IR1.iMBotton;

            float x11 = pf1.X < pf2.X ? pf1.X : pf2.X;//x小
            float x22 = pf1.X > pf2.X ? pf1.X : pf2.X;//x大
            float y11 = pf1.Y < pf2.Y ? pf1.Y : pf2.Y;//y小
            float y22 = pf1.Y > pf2.Y ? pf1.Y : pf2.Y;//y大
            //重定义矩形经纬范围,p1恒为左上，p2恒为右下
            p1.X = (int)Math.Round(x11, 0) - 1;
            p1.Y = (int)Math.Round(y22, 0) + 1;
            p2.X = (int)Math.Round(x22, 0) + 1;
            p2.Y = (int)Math.Round(y11, 0) - 1;

            JWtoXS();

            //===========================取样本值===============================
            #region 取样本值
            data_id = new int[w1, h1];
            data_type = new int[w1, h1];
            hd = new int[w1, h1];
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    int val_IR1 = svu_IR1.bVissData[(y1 + j) * w + (x1 + i)];
                    if (val_IR1 > 150)//有效样本点
                    {
                        data_id[i, j] = 0;
                        data_type[i, j] = 0;
                        pointnum++;
                    }
                    else
                    {
                        data_id[i, j] = 0;
                        data_type[i, j] = -999;
                    }
                    hd[i, j] = val_IR1;
                }
            }
            #endregion
            //==================================================================

            pic = new Bitmap(w1, h1);
            pic0 = new Bitmap(w1, h1);
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
                MessageBox.Show("所选矩形区域无法进行云类分析，请重新选择！", "提示");
                return;
            }
        }

        //根据点坐标返回邻域内点
        public List<Point> get_Neighbour2(int x, int y)
        {
            List<Point> nb = new List<Point>();
            for (int i = -Eps; i <= Eps; i++)
            {
                for (int j = -Eps; j <= Eps; j++)
                {
                    if ((x + i) >= 0 && (x + i) < w1 && (y + j) >= 0 && (y + j) < h1)
                    {
                        if (data_type[x + i, y + j] != -999)//有效点
                            nb.Add(new Point(x + i, y + j));
                    }
                }
            }
            /*
            if ((y - 5) >= 0)
            {
                if (data_type[x, y - 5] != -999)//有效点
                    nb.Add(new Point(x, y - 5));
            }
            if ((y + 5) < iHigh)
            {
                if (data_type[x, y + 5] != -999)//有效点
                    nb.Add(new Point(x, y + 5));
            }
            if ((x - 5) >= 0)
            {
                if (data_type[x - 5, y] != -999)//有效点
                    nb.Add(new Point(x - 5, y));
            }
            if ((x + 5) < iWidth)
            {
                if (data_type[x + 5, y] != -999)//有效点
                    nb.Add(new Point(x + 5, y));
            }
            */
            nb.Remove(new Point(x, y));
            //nb.Remove(new Point(x - Eps, y - Eps));
            //nb.Remove(new Point(x - Eps, y + Eps));
            //nb.Remove(new Point(x + Eps, y + Eps));
            //nb.Remove(new Point(x + Eps, y - Eps));

            return nb;
        }

        //确定核心点和噪点
        public void judge_core2()
        {
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    if (data_type[i, j] != -999)//对有效点处理
                    {
                        List<Point> nbs = get_Neighbour2(i, j);
                        if (nbs.Count >= Minpts)
                        {
                            data_type[i, j] = 888;
                        }
                        else if (nbs.Count < Minpts / 8)
                        {
                            data_type[i, j] = 999;
                        }
                    }
                }
            }
        }

        //以（x,y）为核心点扩展该类——id
        public void expand_Cluster2(int x, int y, int id)
        {
            List<Point> seeds = get_Neighbour2(x, y);
            data_id[x, y] = id;
            clustered_points_count++;
            Point p = new Point();
            while (seeds.Count > 0)
            {
                p = (Point)seeds.First();
                if (data_type[p.X, p.Y] == 888)
                {
                    List<Point> nbs = get_Neighbour2(p.X, p.Y);
                    for (int k = 0; k < nbs.Count; k++)
                    {
                        if (data_id[nbs[k].X, nbs[k].Y] == 0)
                        {
                            seeds.Add(nbs[k]);
                            data_id[nbs[k].X, nbs[k].Y] = id;
                            clustered_points_count++;
                        }
                    }
                }
                seeds.Remove(p);
            }
        }

        //DBSCAN算法
        public void DBSCAN_algorithm2()
        {
            clusters_count = 0;
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    if (data_type[i, j] == 888 && data_id[i, j] == 0)//选择一个尚未分类的核心点作为种子开始聚类
                    {
                        clusters_count++;
                        expand_Cluster2(i, j, clusters_count);
                    }
                }
            }
        }

        public Color CloudRecognize(int h)
        {
            if (h > 217)//积雨云
            {
                return Color.Red;
            }
            else if (h > 200)//浓积云
            {
                return Color.Blue;
            }
            else if (h > 173)//中云
            {
                return Color.Cyan;
            }
            else if (h > 152)//卷云
            {
                return Color.Orange;
            }
            else if (h > 131)//低云
            {
                return Color.Green;
            }
            else if (h == 0)
            {
                return Color.Black;
            }
            else
                return Color.Yellow;//淡积云
        }

        //按簇中心点灰度值获取颜色
        public void GetClusterCenter()
        {
            cluster_center = new Point[clusters_count];
            cluster_num = new int[clusters_count];
            cluster_hd_sum = new int[clusters_count];
            cluster_hd_avg = new int[clusters_count];
            for (int m = 0; m < clusters_count; m++)
            {
                cluster_center[m].X = 0;
                cluster_center[m].Y = 0;
                cluster_num[m] = 0;
                cluster_hd_sum[m] = 0;
                cluster_hd_avg[m] = 0;
            }

            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    if (data_id[i, j] != 0)//已分类的云
                    {
                        cluster_center[data_id[i, j] - 1].X += i;
                        cluster_center[data_id[i, j] - 1].Y += j;
                        cluster_num[data_id[i, j] - 1]++;
                        cluster_hd_sum[data_id[i, j] - 1] += hd[i, j];
                    }
                }
            }
            for (int n = 0; n < clusters_count; n++)
            {
                cluster_center[n].X = cluster_center[n].X / cluster_num[n];
                cluster_center[n].Y = cluster_center[n].Y / cluster_num[n];
                cluster_hd_avg[n] = cluster_hd_sum[n] / cluster_num[n];
            }

            cluster_center_color = new Color[clusters_count];
            for (int kk = 0; kk < clusters_count; kk++)
            {
                int HDmn = hd[cluster_center[kk].X, cluster_center[kk].Y];
                if (data_type[cluster_center[kk].X, cluster_center[kk].Y] == -999)//中心点为无效点则取簇灰度均值
                {
                    cluster_center_color[kk] = CloudRecognize(cluster_hd_avg[kk]);
                }
                else
                {
                    cluster_center_color[kk] = CloudRecognize(HDmn);
                }
            }
        }

        //将图像画到显示图像p上
        public void HuiTu(Bitmap p, int k)
        {
            if (k == 1)
            {
                for (int i = 0; i < w1; i++)
                {
                    for (int j = 0; j < h1; j++)
                    {
                        if (data_id[i, j] != 0)
                            p.SetPixel(i, j, cluster_center_color[data_id[i, j] - 1]);
                        else
                        {
                            Color wxc = Color.FromArgb(hd[i, j], hd[i, j], hd[i, j]);//设置为原色
                            p.SetPixel(i, j, wxc);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < w1; i++)
                {
                    for (int j = 0; j < h1; j++)
                    {
                        Color wxc = Color.FromArgb(hd[i, j], hd[i, j], hd[i, j]);//设置为原色
                        p.SetPixel(i, j, wxc);
                    }
                }
            }
        }

        //色标
        public void ColorTable(Graphics g3)
        {
            Color[] color = new Color[6] { Color.Yellow, Color.Green, Color.Orange, Color.Cyan, Color.Blue, Color.Red };
            string[] Msg = new string[6] { "淡积云", "低云", "卷云", "中云", "浓积云", "积雨云" };
            Font fnt = new Font("宋体", 9);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;

            string sMsg;
            Color sCol;
            for (int i = 0; i < 6; i++)
            {
                sMsg = Msg[i];
                sCol = color[i];
                Point sbP = new Point(400 + 100 * i, 15);
                Size sz = new Size(40, 20);
                Rectangle rect = new Rectangle(sbP, sz);
                System.Drawing.Drawing2D.LinearGradientBrush lBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, sCol, sCol, 0.0);//线性渐变为0.0
                g3.FillRectangle(lBrush, rect);
                g3.DrawString(sMsg, fnt, Brushes.Black, 440 + 100 * i, 15);
            }
        }

        private void frmFXCloudTypeDBSCAN_Load(object sender, EventArgs e)
        {
            init();
            judge_core2();
            DBSCAN_algorithm2();
            GetClusterCenter();
            HuiTu(pic, 1);
            HuiTu(pic0, 0);

            Bitmap sebiao = new Bitmap(1000, 50);
            pbxSeBiao.Image = PubUnit.copyBitmap(sebiao);
            Graphics g = Graphics.FromImage(pbxSeBiao.Image);
            ColorTable(g);

            Bitmap newpic = new Bitmap(pic, pbxYuan.Width, pbxYuan.Height);
            Bitmap newpic0 = new Bitmap(pic0, pbxMain.Width, pbxMain.Height);
            pbxYuan.Image = PubUnit.copyBitmap(newpic);
            pbxMain.Image = PubUnit.copyBitmap(newpic0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
