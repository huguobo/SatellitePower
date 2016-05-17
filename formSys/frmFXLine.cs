using System.Drawing;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;
using System;
using System.Windows.Forms.DataVisualization.Charting;//2014-9-18 wm add
using System.Collections.Generic;//2014-9-18 wm add

namespace SatellitePower.formSys
{
    public partial class frmFXLine : Form
    {
        public Point pnt1;
        public Point pnt2;
        public PointF pfJW1;
        public PointF pfJW2;

        public Point pLT;
        public Point pRB;

        public PointF pfJWLT;
        public PointF pfJWRB;
        public int iNowType = 1;//1=IR,2=VIS

        public string sMapPathName;
        public Bitmap bmpRect;

        //public SVissrUnit svuObj;
        public VISSObject visObj;

        //2014-9-18 wm add
        public Series ZZTu, ZXTu;
        public bool zzflag = true;
        public bool zxflag = true;
        public float[] value;//纵轴数值
        public int iCount;//记录点数

        public frmFXLine()
        {
            InitializeComponent();
        }

        private void frmFXLine_Load(object sender, System.EventArgs e)
        {
            //pbxMain.Image = bmpRect;
            drawLinePoints();
            //2014-9-20 wm add
            panel1.Width = pbxMain.Width + 1;
            panel1.Height = pbxMain.Height + 1;
            //2014-9-19 wm add
            zzflag = true;
            zxflag = true;
            if (iNowType == 1)
            {
                ZZTu = new Series("亮温℃");
                ZXTu = new Series("亮温℃");
            }
            else
            {
                ZZTu = new Series("反照率%");
                ZXTu = new Series("反照率%");
            }
            CreateZhu(iCount + 1, value, ZZTu, iNowType);
            CreateZhe(iCount + 1, value, ZXTu, iNowType);
        }

        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pCen"></param>
        /// <param name="iCol"></param>
        private void DrawFlagPoint(Graphics g, PointF pCen, int iCol = 0)
        {
            //绘制调试用标志坐标点
            //Screen Point : Screen

            Color cRecCol = Color.DarkRed;
            if (iCol == 1)
            {
                cRecCol = Color.GreenYellow;
            }
            else if (iCol == 2)
            {
                cRecCol = Color.Cyan;
            }

            Pen p = new Pen(cRecCol, 5);
            //g.DrawEllipse(p, pCen.X - 5, pCen.Y - 5, 10, 10);

            p.Color = Color.GreenYellow; // Color.Blue;
            p.Width = 4;
            g.DrawEllipse(p, pCen.X - 2, pCen.Y - 2, 4, 4);

            p.Color = Color.Red;
            p.Width = 2;
            g.DrawEllipse(p, pCen.X - 1, pCen.Y - 1, 2, 2);
        }

        //经纬得温度
        public float getWDStrByJW(float fx, float fy)//2014-9-19 wm add
        {
            string sCap = "";
            //1=IR,2=VIS
            if (iNowType == 1)
            {
                //IR 亮温
                //sCap = "亮温";
                float d = visObj.getTemperatureByJW(fx, fy);
                while (d > 240.00f)
                {
                    d = d - 240.00f;
                }
                //return sCap + ": " + String.Format("{0:0.00}", d) + " ℃";//K
                return d;//2014-9-19 wm add
                //return String.Format("{0:0.00}", d);
            }
            else
            {
                //VIS 反照率
                //sCap = "反照率";
                float d = visObj.getTemperatureByJW(fx, fy);
                //return sCap + ": " + String.Format("{0:0.00}", d);
                return d;//2014-9-19 wm add
                //return String.Format("{0:0.00}", d);
            }
        }

        public void drawLinePoints()
        {
            iCount = Convert.ToInt32(nudDS.Value) + 1;
            value = new float[iCount + 1];//初始化纵轴数值数组，2014-9-19 wm add
            pbxMain.Image = PubUnit.copyBitmap(bmpRect);

            //public double getSigns2ValByJW(float fx, float fy)
            //public float getTemperatureByJW(float fxLon, float fyLat)
            //visObj.getSigns2StrByJW(float fx, float fy)

            int iW = Math.Abs(pRB.X - pLT.X);
            int iH = Math.Abs(pRB.Y - pLT.Y);

            float fJWw = Math.Abs(pfJWRB.X - pfJWLT.X);
            float fJWh = Math.Abs(pfJWRB.X - pfJWLT.X);//??????????????????????????
            //--------------------------------------------------------------------
            Pen pn = new Pen(Color.DarkGreen, 1);
            Font fnt = new Font("宋体", 10, FontStyle.Bold);
            Brush br = new SolidBrush(Color.Red);

            Image temp = PubUnit.copyBitmap(bmpRect);//2014-9-21 wm add
            Graphics gR = Graphics.FromImage(temp);

            Point pTmp1 = new Point(pnt1.X - pLT.X, pnt1.Y - pLT.Y);
            Point pTmp2 = new Point(pnt2.X - pLT.X, pnt2.Y - pLT.Y);
            gR.DrawLine(pn, pTmp1, pTmp2);
            //--------------------------------------------------------------------
            int iArea = 0;//象限区域

            //第一点
            value[0] = this.getWDStrByJW(pfJW1.X, pfJW1.Y);//2014-9-19 wm add
            string sVal = String.Format("{0:0.00}", this.getWDStrByJW(pfJW1.X, pfJW1.Y));//2014-9-19 wm add
            DrawFlagPoint(gR, pTmp1);
            if (pTmp1.X < pTmp2.X && pTmp1.Y < pTmp2.Y)
            {
                //左上
                iArea = 0;
                gR.DrawString(sVal, fnt, br, pTmp1);
            }
            else if (pTmp1.X > pTmp2.X && pTmp1.Y < pTmp2.Y)
            {
                //右上
                iArea = 1;
                gR.DrawString(sVal, fnt, br, new Point(pTmp1.X - 45, pTmp1.Y));
            }
            else if (pTmp1.X < pTmp2.X && pTmp1.Y > pTmp2.Y)
            {
                //左下
                iArea = 4;
                gR.DrawString(sVal, fnt, br, new Point(pTmp1.X, pTmp1.Y - 14));
            }
            else if (pTmp1.X > pTmp2.X && pTmp1.Y > pTmp2.Y)
            {
                //右下
                iArea = 3;
                gR.DrawString(sVal, fnt, br, new Point(pTmp1.X - 45, pTmp1.Y - 14));
            }
            //--------------------------------------------------------------------
            //第二点
            value[iCount] = this.getWDStrByJW(pfJW2.X, pfJW2.Y);//2014-9-19 wm add
            sVal = String.Format("{0:0.00}", this.getWDStrByJW(pfJW2.X, pfJW2.Y));//2014-9-19 wm add
            DrawFlagPoint(gR, pTmp2);
            //gR.DrawString(sVal, fnt, br, new Point(iW - 45, iH - 14));
            if (iArea == 0)
            {
                //左上                
                gR.DrawString(sVal, fnt, br, new Point(pTmp2.X - 45, pTmp2.Y - 14));
            }
            else if (iArea == 1)
            {
                //右上                
                gR.DrawString(sVal, fnt, br, new Point(pTmp2.X, pTmp2.Y - 14));
            }
            else if (iArea == 4)
            {
                //左下                
                gR.DrawString(sVal, fnt, br, new Point(pTmp2.X - 45, pTmp2.Y));
            }
            else if (iArea == 3)
            {
                //右下                
                gR.DrawString(sVal, fnt, br, pTmp2);
            }
            //--------------------------------------------------------------------
            for (int i = 1; i < iCount; i++)
            {
                if (pTmp1.X < pTmp2.X && pTmp1.Y < pTmp2.Y)
                {
                    //左上
                    Point pnt = new Point(pTmp1.X + i * iW / iCount, pTmp1.Y + i * iH / iCount);
                    DrawFlagPoint(gR, pnt);

                    PointF pf = new PointF(pfJW1.X + i * fJWw / iCount, pfJW1.Y - i * fJWh / iCount);
                    value[i] = this.getWDStrByJW(pf.X, pf.Y);//2014-9-19 wm add
                    sVal = String.Format("{0:0.00}", this.getWDStrByJW(pf.X, pf.Y));//2014-9-19 wm add
                    gR.DrawString(sVal, fnt, br, new Point(pnt.X + 5, pnt.Y - 5));
                }
                else if (pTmp1.X > pTmp2.X && pTmp1.Y < pTmp2.Y)
                {
                    //右上
                    Point pnt = new Point(pTmp1.X - i * iW / iCount, pTmp1.Y + i * iH / iCount);
                    DrawFlagPoint(gR, pnt);

                    PointF pf = new PointF(pfJW1.X - i * fJWw / iCount, pfJW1.Y - i * fJWh / iCount);
                    value[i] = this.getWDStrByJW(pf.X, pf.Y);//2014-9-19 wm add
                    sVal = String.Format("{0:0.00}", this.getWDStrByJW(pf.X, pf.Y));//2014-9-19 wm add
                    gR.DrawString(sVal, fnt, br, new Point(pnt.X + 5, pnt.Y - 5));
                }
                else if (pTmp1.X < pTmp2.X && pTmp1.Y > pTmp2.Y)
                {
                    //左下
                    Point pnt = new Point(pTmp1.X + i * iW / iCount, pTmp1.Y - i * iH / iCount);
                    DrawFlagPoint(gR, pnt);

                    PointF pf = new PointF(pfJW1.X + i * fJWw / iCount, pfJW1.Y + i * fJWh / iCount);
                    value[i] = this.getWDStrByJW(pf.X, pf.Y);//2014-9-19 wm add
                    sVal = String.Format("{0:0.00}", this.getWDStrByJW(pf.X, pf.Y));//2014-9-19 wm add
                    gR.DrawString(sVal, fnt, br, new Point(pnt.X + 5, pnt.Y - 5));
                }
                else if (pTmp1.X > pTmp2.X && pTmp1.Y > pTmp2.Y)
                {
                    //右下
                    Point pnt = new Point(pTmp1.X - i * iW / iCount, pTmp1.Y - i * iH / iCount);
                    DrawFlagPoint(gR, pnt);

                    PointF pf = new PointF(pfJW1.X - i * fJWw / iCount, pfJW1.Y + i * fJWh / iCount);
                    value[i] = this.getWDStrByJW(pf.X, pf.Y);//2014-9-19 wm add
                    sVal = String.Format("{0:0.00}", this.getWDStrByJW(pf.X, pf.Y));//2014-9-19 wm add
                    gR.DrawString(sVal, fnt, br, new Point(pnt.X + 5, pnt.Y - 5));
                }

            }
            gR.Dispose();

            //2014-9-21 wm add 图像按比例调整
            Bitmap newbmpRect = new Bitmap(temp, panel1.Width, panel1.Height);
            pbxMain.Image = PubUnit.copyBitmap(newbmpRect);
        }

        //==========================================================
        //2014-9-18 wm add
        //绘制柱状图
        private void CreateZhu(int f, float[] lw, Series zz, int type)
        {
            //刷新chart1
            if (!zzflag)
            {
                zz.Points.Clear();
                chart1.Series.Remove(zz);
            }
            List<string> xx1 = new List<string>();//x轴名称
            for (int i = 0; i < f; i++)
            {
                xx1.Add("点" + (i + 1).ToString());
            }
            for (int j = 0; j < f; j++)
            {
                zz.Points.AddXY(xx1[j], Math.Round(lw[j], 2));
            }
            zz.MarkerBorderColor = Color.Black;//数据点边框的颜色
            zz.MarkerColor = Color.Red;//数据点的颜色
            zz.MarkerStyle = MarkerStyle.Circle;//数值点形状
            zz.MarkerSize = 5;//数值点大小
            zz.IsValueShownAsLabel = true;//是否显示数值点值
            zz.SmartLabelStyle.Enabled = false;//直接控制可用不可用，建议不可用
            zz.IsXValueIndexed = true;
            zz.XValueMember = "点";
            zz.YValueMembers = "数值";
            zz.ChartType = SeriesChartType.Column;

            chart1.ChartAreas[0].AxisX.Interval = 1;   //设置X轴坐标的间隔为1
            chart1.ChartAreas[0].AxisX.IntervalOffset = 1;  //设置X轴坐标偏移为1
            chart1.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;   //设置是否交错显示,比如数据多的时间分成两行来显示 

            chart1.Series.Add(zz);
            if (type == 1)
            {
                chart1.Series[0].ToolTip = "亮温：#VAL ℃\r\n点：#AXISLABEL";
            }
            else
            {
                chart1.Series[0].ToolTip = "反照率：#VAL %\r\n点：#AXISLABEL";
            }
            zzflag = false;
        }

        //绘制折线图
        private void CreateZhe(int f, float[] lw, Series zx, int type)
        {
            //刷新chart2
            if (!zxflag)
            {
                zx.Points.Clear();
                chart2.Series.Remove(zx);
            }
            List<string> xx2 = new List<string>();//x轴名称
            for (int i = 0; i < f; i++)
            {
                xx2.Add("点" + (i + 1).ToString());
            }
            for (int j = 0; j < f; j++)
            {
                zx.Points.AddXY(xx2[j], Math.Round(lw[j], 2));
            }
            zx.MarkerBorderColor = Color.Black;//数据点边框的颜色
            zx.MarkerColor = Color.Red;//数据点的颜色
            zx.MarkerStyle = MarkerStyle.Circle;//数值点形状
            zx.MarkerSize = 5;//数值点大小
            zx.IsValueShownAsLabel = true;//是否显示数值点值
            zx.SmartLabelStyle.Enabled = false;//直接控制可用不可用，建议不可用
            zx.IsXValueIndexed = true;
            zx.XValueMember = "点";
            zx.YValueMembers = "数值";
            zx.ChartType = SeriesChartType.Line;

            chart2.ChartAreas[0].AxisX.Interval = 1;   //设置X轴坐标的间隔为1
            chart2.ChartAreas[0].AxisX.IntervalOffset = 1;  //设置X轴坐标偏移为1
            chart2.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;   //设置是否交错显示,比如数据多的时间分成两行来显示
            chart2.Series.Add(zx);
            if (type == 1)
            {
                chart2.Series[0].ToolTip = "亮温：#VAL ℃\r\n点：#AXISLABEL";
            }
            else
            {
                chart2.Series[0].ToolTip = "反照率：#VAL %\r\n点：#AXISLABEL";
            }
            zxflag = false;
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Point m1 = new Point(e.X, e.Y);
            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(m1, true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(m1, true);
        }
        private void chart2_MouseMove(object sender, MouseEventArgs e)
        {
            Point m2 = new Point(e.X, e.Y);
            chart2.ChartAreas[0].CursorX.SetCursorPixelPosition(m2, true);
            chart2.ChartAreas[0].CursorY.SetCursorPixelPosition(m2, true);
        }
        //==========================================================
        //计算
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            drawLinePoints();
            CreateZhu(iCount + 1, value, ZZTu, iNowType);
            CreateZhe(iCount + 1, value, ZXTu, iNowType);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
