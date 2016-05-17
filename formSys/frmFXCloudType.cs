using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;
using System.Threading;

namespace SatellitePower.formSys
{
    public partial class frmFXCloudType : Form
    {
        //==================================
        //异步执行需要委托声明
        //声明一个delegate（委托）类型：setMaxDelegate，该类型可以搭载返回值为空，参数只有一个(long型)的方法。
        public delegate void setMaxDelegate(int iMax);
        //声明一个delegate（委托）类型：updateDelegate，该类型可以搭载返回值为空，参数只有一个(long型)的方法。
        public delegate void updateDelegate(int i);
        //声明一个delegate（委托）类型：completeDelegate，该类型可以搭载返回值为空，参数为空的方法。
        public delegate void completeDelegate();

        public setMaxDelegate setMaxThread;
        //声明一个updateDelegate类型的对象。该对象代表了返回值为空，参数只有一个(long型)的方法。它可以搭载N个方法。
        public updateDelegate mainThread;
        //声明一个completeDelegate类型的对象。该对象代表了返回值为空，参数只有一个(long型)的方法。它可以搭载N个方法。
        public completeDelegate completeThread;
        private Thread makeThread;
        //=======================================================
        Bitmap newpic;
        Bitmap newpic0;
        //2014-9-20 wm add
        public PointF pf1;//第一点
        public PointF pf2;//第二点
        public Point p1;//左上，矩形经纬
        public Point p2;//右下，矩形经纬
        public VISSObject visObj_IR1;
        public VISSObject visObj_VIS;
        public SVissrUnit svu_IR1;
        public SVissrUnit svu_VIS;
        public int w, h;//原图尺寸
        public int w1, h1;//矩形尺寸
        public int JingDu_left, JingDu_right, WeiDu_top, WeiDu_down;//原图经纬
        public string sMapPathName_IR1;
        public string sMapPathName_VIS;
        public int x1, x2, y1, y2;//矩形起止像素点

        public int[,] XX;//总样本
        public int yx1, yx2;//有效阈值1,2
        public int[] tag;//总样本有效标志，1有效，0无效
        public int[,] YY;//剔除后，有效样本
        public int yxsum;//有效样本总数
        public int c;//聚类中心数
        public int[,] VV;//聚类中心
        public int[] LeiBie;//记录样本所属类别，以此判断云型
        public int jqm = 2;//加权指数
        public double yuzhi = 1E-10;//阈值

        public Color[] color;//类别颜色表
        public Color[] scolor;//用到的颜色
        public string[] zhushi;//类别注释
        public string[] szhushi;//用到的注释

        public Bitmap pic;//最终显示图像
        public Bitmap pic0;//原始图像
        public frmFXCloudType()
        {
            InitializeComponent();
        }

        //初始化矩阵相关数据，2014-9-21 wm add
        public void init()
        {

            svu_IR1 = visObj_IR1.svuObj;
            svu_VIS = visObj_VIS.svuObj;
            w = svu_IR1.iMWidth == svu_VIS.iMWidth ? svu_IR1.iMWidth : 0;
            h = svu_IR1.iMHigh == svu_VIS.iMHigh ? svu_IR1.iMHigh : 0;
            if (w == 0 || h == 0)
            {
                MessageBox.Show("检测到红外一通道和可见光通道云图不匹配，无法云类分析！", "提示");
                return;
            }
            JingDu_left = svu_IR1.iMLeft == svu_VIS.iMLeft ? svu_VIS.iMLeft : 0;
            JingDu_right = svu_IR1.iMRight == svu_VIS.iMRight ? svu_VIS.iMRight : 0;
            WeiDu_top = svu_IR1.iMTop == svu_VIS.iMTop ? svu_VIS.iMTop : 0;
            WeiDu_down = svu_IR1.iMBotton == svu_VIS.iMBotton ? svu_VIS.iMBotton : 0;
            if (JingDu_left == 0 || JingDu_right == 0 || WeiDu_top == 0 || WeiDu_down == 0)
            {
                MessageBox.Show("检测到红外一通道和可见光通道云图不匹配，无法云类分析！", "提示");
                return;
            }

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
            XX = new int[w1 * h1, 2];
            tag = new int[w1 * h1];
            for (int i = 0; i < w1; i++)
            {
                for (int j = 0; j < h1; j++)
                {
                    int val_IR1 = svu_IR1.bVissData[(y1 + j) * w + (x1 + i)];
                    int val_VIS = svu_VIS.bVissData[(y1 + j) * w + (x1 + i)];

                    XX[i * h1 + j, 0] = val_IR1;
                    XX[i * h1 + j, 1] = val_VIS;
                    if (val_IR1 > yx1 && val_VIS > yx2)//有效样本点,IR1>100,VIS>20
                    {
                        tag[i * h1 + j] = 1;
                    }
                    else
                    {
                        tag[i * h1 + j] = 0;
                    }
                }
            }
            List<int> ir1 = new List<int>();
            List<int> vis = new List<int>();
            for (int i = 0; i < w1 * h1; i++)
            {
                if (tag[i] == 1)
                {
                    ir1.Add(XX[i, 0]);
                    vis.Add(XX[i, 0]);
                }
            }
            yxsum = ir1.Count;
            YY = new int[yxsum, 2];
            for (int i = 0; i < yxsum; i++)
            {
                YY[i, 0] = Convert.ToInt32(ir1[i]);
                YY[i, 1] = Convert.ToInt32(vis[i]);
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

        //初始化样本点和隶属度矩阵UU
        public void initVVUU(double[,] UU)
        {
            double d1 = (255 - 110) / (double)c;
            double d2 = (60 - 20) / (double)c;
            for (int i = 0; i < c; i++)
            {
                VV[i, 0] = (int)(110 + d1 * i);
                VV[i, 1] = (int)(20 + d2 * i);
            }
            for (int i = 0; i < yxsum; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    UU[j, i] = 1.0 / (double)c;
                }
            }
        }

        /// <summary>
        /// 欧氏距离
        /// </summary>
        /// <param name="x"></param>样本集x
        /// <param name="v"></param>中心点集v
        /// <param name="xk"></param>样本索引xk
        /// <param name="vk"></param>中心点索引vk
        /// <returns></returns>
        public double Dik2(int[,] x, int[,] v, int xk, int vk)
        {
            double r1, r2;
            r1 = Math.Pow(x[xk, 0] - v[vk, 0], 2.0);
            r2 = Math.Pow(x[xk, 1] - v[vk, 1], 2.0);
            return Math.Sqrt(r1 + r2);//欧氏距离
        }

        /// <summary>
        /// 求隶属度Uik
        /// </summary>
        /// <param name="c"></param>类别数c
        /// <param name="m"></param>加权指数m
        /// <param name="dik"></param>欧氏距离dik
        /// <param name="x"></param>样本集x
        /// <param name="v"></param>中心点集合v
        /// <param name="k"></param>第k个样本
        /// <returns></returns>
        public double Uik(int c, int m, double dik, int[,] x, int[,] v, int k)
        {
            double down = 0.0;
            for (int j = 0; j < c; j++)
            {
                down += Math.Pow(dik / Dik2(x, v, k, j), (double)2 * (m - 1));
            }
            return 1 / down;
        }

        /// <summary>
        /// 求矩阵u范数
        /// </summary>
        /// <param name="n"></param>样本总数
        /// <param name="c"></param>类总数
        /// <param name="m"></param>加权指数
        /// <param name="u"></param>矩阵u
        /// <returns></returns>
        public double FanShu(int n, int c, int m, double[,] u)
        {
            double fs = 0.0;
            for (int k = 0; k < yxsum; k++)
            {
                for (int i = 0; i < c; i++)
                {
                    double subfs = Math.Pow(u[i, k], m) * Math.Pow(Dik2(YY, VV, k, i), 2.0);
                    fs += subfs;
                }
            }
            return fs;
        }

        /// <summary>
        /// 求第i个聚类中心
        /// </summary>
        /// <param name="u"></param>隶属度矩阵
        /// <param name="i"></param>第i个
        /// <param name="sum"></param>样本总数
        /// <param name="j"></param>维度j,0或1
        /// <param name="m"></param>加权指数
        /// <returns></returns>
        public double FCM_V(double[,] u, int i, int sum, int j, int m)
        {
            double vv;//返回的样本
            double t0 = 0.0, d0 = 0.0;
            for (int kk = 0; kk < sum; kk++)
            {
                double subt0 = Math.Pow(u[i, kk], m) * YY[kk, j];
                double subd0 = Math.Pow(u[i, kk], m);
                t0 += subt0;
                d0 += subd0;
            }
            vv = t0 / d0;
            return vv;
        }

        //FCM,模糊C均值聚类
        public void FCM()
        {
            double[,] UU = new double[c, yxsum];//隶属度矩阵，UUik表示样本k对类别i的隶属度，>=0且<=1
            double[,] UU1 = new double[c, yxsum];//新隶属度矩阵
            VV = new int[c, 2];

            int count = 1;//记录迭代次数
            int b = 1000;//最大迭代步数
            #region 迭代过程
            for (int dd = 0; dd < b; dd++)
            {
                if (dd == 0)
                {
                    initVVUU(UU);
                }
                else
                {
                    for (int jlc = 0; jlc < c; jlc++)
                    {
                        //求8个聚类中心
                        VV[jlc, 0] = (int)FCM_V(UU, jlc, yxsum, 0, jqm);
                        VV[jlc, 1] = (int)FCM_V(UU, jlc, yxsum, 1, jqm);
                    }
                }
                #region 求YYk所属类别VVj
                for (int k = 0; k < yxsum; k++)
                {
                    double temp2 = 0.0;//用于归一化处理
                    int II = -1;//记录YYi属于的类j,无则为-1
                    int[] _II = new int[c];
                    for (int k0 = 0; k0 < c; k0++)
                    {
                        _II[k0] = -1;//属于哪个类别就为1，否则为-1
                    }
                    for (int j = 0; j < c; j++)
                    {
                        if (Dik2(YY, VV, k, j) == 0.0)
                        {
                            II = j;
                            _II[j] = 1;
                        }
                    }
                    if (II == -1)//不属于任何类别
                    {
                        for (int i = 0; i < c; i++)
                        {
                            double dik = Dik2(YY, VV, k, i);
                            UU1[i, k] = Uik(c, jqm, dik, YY, VV, k);
                            temp2 += UU1[i, k];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < c; i++)
                        {
                            if (_II[i] == -1)
                            {
                                UU1[i, k] = 0.0;
                                temp2 += UU1[i, k];
                            }
                            if (_II[i] == 1)
                            {
                                UU1[i, k] = 1.0;
                                temp2 += UU1[i, k];
                            }
                        }
                    }
                    //归一化处理，使和为1.0
                    for (int i = 0; i < c; i++)
                    {
                        UU1[i, k] = UU1[i, k] / temp2;
                    }
                }
                #endregion
                //范数为目标函数，两个目标函数之差越小表示目标函数收敛
                double UUf = FanShu(yxsum, c, jqm, UU);
                double UU1f = FanShu(yxsum, c, jqm, UU1);
                double cha = Math.Abs(UU1f - UUf);
                if (cha < yuzhi)
                {
                    break;
                }
                else
                {
                    for (int i = 0; i < yxsum; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            UU[j, i] = UU1[j, i];//更新隶属度矩阵
                        }
                    }
                }
                count++;
            }
            #endregion

            #region 通过隶属度矩阵判断样本所属类型
            LeiBie = new int[yxsum];
            double[] max = new double[yxsum];//最大隶属度
            for (int i = 0; i < yxsum; i++)
            {
                LeiBie[i] = -999;//初始化类别
                max[i] = UU1[0, i];
                int idx = 0;//记录最大值索引
                for (int j = 0; j < c; j++)
                {
                    if (UU1[j, i] > max[i])
                    {
                        max[i] = UU1[j, i];
                        idx = j;
                    }
                }
                if (max[i] > 0.50)//大于0.50则判定为idx云
                {
                    LeiBie[i] = idx;
                }
                else if (max[i] >= 0.30 && max[i] <= 0.50)//过渡云系
                {
                    LeiBie[i] = 99;
                }
                else if (max[i] < 0.30)//不可辨别云系
                {
                    LeiBie[i] = 999;
                }
            }
            #endregion
        }


        public void makecolor()
        {
            //类别所有颜色,赤橙黄绿青蓝 紫黑
            color = new Color[8] { Color.Yellow, Color.Green, Color.Orange, Color.Cyan, Color.Blue, Color.Red, Color.Purple, Color.Black };
            scolor = new Color[c];
            zhushi = new string[8] { "淡积云", "低云", "卷云", "中云", "浓积云", "积雨云", "过渡云系", "不可辨云系" };
            szhushi = new string[c];
            for (int i = 0; i < c; i++)
            {
                //获取类别颜色
                if (VV[i, 0] >= 217)//积雨云红色
                {
                    scolor[i] = color[5];
                    szhushi[i] = zhushi[5];
                }
                else if (VV[i, 0] >= 200)//浓积云蓝色
                {
                    scolor[i] = color[4];
                    szhushi[i] = zhushi[4];
                }
                else if (VV[i, 0] >= 173)//中云青色
                {
                    scolor[i] = color[3];
                    szhushi[i] = zhushi[3];
                }
                else if (VV[i, 0] >= 152)//卷云橙色
                {
                    scolor[i] = color[2];
                    szhushi[i] = zhushi[2];
                }
                else if (VV[i, 0] >= 131)//低云绿色
                {
                    scolor[i] = color[1];
                    szhushi[i] = zhushi[1];
                }
                else//淡积云黄色
                {
                    scolor[i] = color[0];
                    szhushi[i] = zhushi[0];
                }
            }
        }

        //根据类别lb设置(x,y)颜色
        public void TianChong(int lb, int x, int y)
        {
            makecolor();
            if (lb == 99)
            {
                pic.SetPixel(x, y, Color.Purple);
            }
            else if (lb == 999)
            {
                pic.SetPixel(x, y, Color.Black);
            }
            else
            {
                for (int i = 0; i < c; i++)
                {
                    if (lb == i)
                    {
                        pic.SetPixel(x, y, scolor[i]);
                    }
                }
            }
        }

        //将图像画到显示图像p上
        public void HuiTu(Bitmap p, int k)
        {
            if (k == 1)
            {
                int bg = 0;
                for (int i = 0; i < w1; i++)
                {
                    for (int j = 0; j < h1; j++)
                    {
                        if (tag[i * h1 + j] == 1)//有效样本点
                        {
                            TianChong(LeiBie[bg], i, j);
                            bg++;
                        }
                        else if (tag[i * h1 + j] == 0)
                        {
                            Color wxc = Color.FromArgb(XX[i * h1 + j, 1], XX[i * h1 + j, 1], XX[i * h1 + j, 1]);//无效点设置为2通道原色
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
                        Color wxc = Color.FromArgb(XX[i * h1 + j, 1], XX[i * h1 + j, 1], XX[i * h1 + j, 1]);//设置为2通道原色
                        p.SetPixel(i, j, wxc);
                    }
                }
            }
        }

        //色标
        public void SeBiao(Graphics gra)
        {
            //去除重复元素
            Color[] qcolor = scolor.Distinct().ToArray();
            string[] qzhushi = szhushi.Distinct().ToArray();
            int kkk = qcolor.Length;

            Font fnt = new Font("宋体", 9);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;
            string sMsg;
            Color sCol;
            for (int i = 0; i < kkk; i++)
            {
                sMsg = qzhushi[i];
                sCol = qcolor[i];
                Point sbP = new Point(200 + 100 * i, 15);
                Size sz = new Size(40, 20);
                Rectangle rect = new Rectangle(sbP, sz);
                System.Drawing.Drawing2D.LinearGradientBrush lBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, sCol, sCol, 0.0);//线性渐变为0.0
                gra.FillRectangle(lBrush, rect);
                gra.DrawString(sMsg, fnt, Brushes.Black, 240 + 100 * i, 15);
            }
            Point p99 = new Point(200 + 100 * kkk, 15);
            Point p999 = new Point(200 + 100 * (kkk + 1), 15);
            Size sz99 = new Size(40, 20);
            Rectangle rect99 = new Rectangle(p99, sz99);
            Rectangle rect999 = new Rectangle(p999, sz99);
            gra.FillRectangle(Brushes.Purple, rect99);
            gra.FillRectangle(Brushes.Black, rect999);
            string s99 = "过渡云系";
            string s999 = "不可辨云系";
            gra.DrawString(s99, fnt, Brushes.Black, 240 + 100 * kkk, 15);
            gra.DrawString(s999, fnt, Brushes.Black, 240 + 100 * (kkk + 1), 15);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void frmFXCloudType_Load(object sender, EventArgs e)
        {
            fcm_pbar.Visible = true;
            fcm_label.Text = "正在生成...";

            //在ai对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            mainThread = new updateDelegate(updateProgressBar);
            completeThread = new completeDelegate(onMakeDataCompleted);
            setMaxThread = new setMaxDelegate(setMax);

            //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
            makeThread = new Thread(new ThreadStart(process));
            //启动线程，启动之后线程才开始执行
            makeThread.Start();
        }

        private void process()
        {
            setMax(100);
            updateProgressBar(5);

            init();
            updateProgressBar(20);
            FCM();
            updateProgressBar(80);
            HuiTu(pic, 1);//聚类分析图
            HuiTu(pic0, 0);//原图

            updateProgressBar(100);

            onMakeDataCompleted();
        }



        //=============================================================================================
        //海雾监听函数开始
        public void setMax(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.fcm_pbar.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(setMaxThread, new object[] { iMax });
            }
            else
            {
                fcm_pbar.Maximum = iMax;
            }
        }

        public void updateProgressBar(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.fcm_pbar.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(mainThread, new object[] { value });
            }
            else
            {
                fcm_pbar.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted()
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.fcm_pbar.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(completeThread, new object[] { });
            }
            else
            {
                newpic = new Bitmap(pic, pbxYuan.Width, pbxYuan.Height);
                newpic0 = new Bitmap(pic0, pbxMain.Width, pbxMain.Height);
                pbxYuan.Image = PubUnit.copyBitmap(newpic0);
                pbxMain.Image = PubUnit.copyBitmap(newpic);

                Bitmap sebiao = new Bitmap(900, 50);
                pbxSeBiao.Image = PubUnit.copyBitmap(sebiao);
                Graphics g = Graphics.FromImage(pbxSeBiao.Image);
                SeBiao(g);

                fcm_pbar.Visible = false;
                fcm_label.Text = "生成成功";
            }
        }

        private void frmFXCloudType_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (makeThread != null)
            {
                makeThread.Abort();
            }
        }
    }
}
