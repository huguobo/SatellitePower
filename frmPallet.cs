using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace SatellitePower
{
    public partial class frmPallet : Form
    {
        //public frmPallet(IStyle style)
        //{
        //    InitializeComponent();
        //    pgStyle.SelectedObject = style;
        //}

        private ColorPalette _Palette;
        private Font fntImgOut = new Font("黑体", 8);
        private Color cImgOut = Color.Yellow;
        private string searchPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + @"pallet\";
        private string sPalFileName = "";
        private List<string> listItem = new List<string>();

        private void FindFile(string path)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(path);

            //递归搜索文件
            //foreach (DirectoryInfo dirinfo in new DirectoryInfo(path).GetDirectories())
            //{
            //addLog(dirinfo.Name);

            foreach (FileInfo fi in dirinfo.GetFiles())
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                listItem.Add(sShowName);
            }
            //FindFile(dirinfo.FullName);
            //}
        }

        public frmPallet()
        {
            InitializeComponent();
            //sPalFileName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "pal" + Path.DirectorySeparatorChar.ToString() + "I-01.pal";
            FindFile(searchPath);
            listBox1.Items.Clear();
            foreach (string item in listItem)
            {
                listBox1.Items.Add(item);

                string spn = searchPath + item;
                if (spn.Equals(PubUnit.sPalletPathName))
                {
                    listBox1.SelectedItem = listBox1.Items[listBox1.Items.Count - 1];
                    ReadPal(spn);
                    drawPal();
                }
            }

            if (null == listBox1.SelectedItem)
            {
                if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedItem = listBox1.Items[0];
                    string spfn = searchPath + listBox1.SelectedItem.ToString();
                    ReadPal(spfn);
                    drawPal();
                }
            }
        }

        public event EventHandler OnApply;

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (OnApply != null)
                OnApply(sender, e);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            btnApply_Click(sender, e);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = cImgOut;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cImgOut = colorDialog1.Color;
                drawPal();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = fntImgOut;
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fntImgOut = fontDialog1.Font;
                drawPal();
            }
        }

        private Bitmap _Bmp;
        public void ReadPal(string FileName)
        {
            //载入调色板
            if (!File.Exists(FileName))
            {
                MessageBox.Show("文件不存在！" + Environment.NewLine + FileName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //addLog(sPalFileName);

            _Bmp = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            _Palette = _Bmp.Palette;

            //在C#中，ColorPalette 是没有构造函数的。想要得到一个调色板对象，我们可以这样做：
            //ColorPalette Palette = bmp.GetPalette(Palette, 256); //bmp是一个普通的Bitmap对象
            //这样我们就得到了一个调色板了。同样，经过处理后，我们只要在描画（DrawImage）之前，把调色板对象赋值回去就可以了：bmp.SetPalette(Palette);

            try
            {
                WordStream ws = new WordStream(FileName, Encoding.Default);
                string ss = ws.ReadWord();
                int nn = Convert.ToInt16(ws.ReadWord());
                //Trace.WriteLine(ss + nn.ToString());
                if (ss.ToLower() == "march" & nn == 102)
                {
                    ws.ReadWord();
                    int iLen = 0;
                    while (ws.Peek() != -1)
                    {
                        ss = ws.ReadWord().Trim();
                        if (ss == "") break;
                        int i = Convert.ToInt16(ss);

                        ss = ws.ReadWord().Trim();
                        if (ss == "") break;
                        int r = Convert.ToInt16(ss);

                        ss = ws.ReadWord().Trim();
                        if (ss == "") break;
                        int g = Convert.ToInt16(ss);

                        ss = ws.ReadWord().Trim();
                        if (ss == "") break;
                        int b = Convert.ToInt16(ss);

                        if ((iLen >= 0) && (iLen < 256))
                        {
                            _Palette.Entries[iLen] = Color.FromArgb(255, b, g, r);
                            //addLog(i + " = " + b + "," + g + "," + r + " | " + _Palette.Entries[i].ToString());
                            iLen++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("读入调色板错误!\n\n" + e.Message);
            }

        }

        public void drawPal()
        {
            //绘制调色板
            if (null == _Palette)
            {
                //设置调色板文件                
                ReadPal(sPalFileName);
            }

            if (null == _Palette)
                return;

            int iGridW = 25;
            int iGridH = 24;

            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            int k = -1;
            int l = 0;
            int iw = iGridW;
            int ih = iGridH;

            for (int i = 0; i < _Palette.Entries.Length; i++)
            {
                Color co = _Palette.Entries[i];
                SolidBrush sbrush = new SolidBrush(co);
                sbrush.Color = co;

                int m = i % 16;
                l = m;
                if (m == 0)
                {
                    k++;
                }

                g.FillRectangle(sbrush, l * (iw + 2), k * (ih + 2), iw, ih);

                string msg = string.Format("{0:X}", i);
                msg = i.ToString();
                msg = Convert.ToString(co.R)
                    + "," + Convert.ToString(co.G)
                    + "," + Convert.ToString(co.B);

                Rectangle rect = new Rectangle(l * (iw + 2), k * (ih + 2), iw, ih);

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                //g.DrawString(msg, fntImgOut, Brushes.Yellow, rect, sf);
                SolidBrush sb2 = new SolidBrush(cImgOut);
                msg = i.ToString();
                g.DrawString(msg, fntImgOut, sb2, rect, sf);

                sbrush.Dispose();
            }

            //再绘制色带

            g.Dispose();

            //pictureBox1.CreateGraphics().DrawImage(bmp, 0, 0);   
            pictureBox1.Image = bmp;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != listBox1.SelectedItem)
            {
                string sPalFileName = searchPath + listBox1.SelectedItem.ToString();

                PubUnit.sPalletPathName = sPalFileName;

                //this.Text = sPalFileName;
                ReadPal(sPalFileName);
                drawPal();
            }
        }
    }
}
