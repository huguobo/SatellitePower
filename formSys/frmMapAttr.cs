using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;
using System.Text.RegularExpressions;
namespace SatellitePower
{
    public partial class frmMapAttr : Form
    {
        Regex asd = new Regex(@".(?i:ASD)$");//added by ZJB 判断是否后缀为.ASD
        public IList<ICustmoLayer> myLayerList = new List<ICustmoLayer>();
        public List<Pen> listPen = new List<Pen>();

        public IStyle AirLineStyle = new Style();

        public frmMapAttr()
        {
            InitializeComponent();
        }

        public void getMapLayersInfo(IList<ICustmoLayer> inLayerList)
        {
            listBoxMap.Items.Clear();
            myLayerList.Clear();
            listPen.Clear();
            foreach (ICustmoLayer lay in inLayerList)
            {
                if (lay.IsBaseMap && lay.Visible)
                {
                    //lay.Style.PenWidth = 5; //Pen _Pen.Width
                    //lay.Style.PenColor = Color.GreenYellow; //Pen _Pen.Color     

                    //Pen _Pen = new Pen(Color.Black);
                    Pen _Pen = (Pen)lay.Style.Pen.Clone();
                    string sLayName = lay.Name.ToLower().Replace(".dat", "");

                    myLayerList.Add(lay);
                    listBoxMap.Items.Add(sLayName);
                    listPen.Add(_Pen);
                }

                
                if (asd.IsMatch(lay.Name)&&lay.Visible)//将各个空域图层加入列表
                {
                    //lay.Style.PenWidth = 2;
                    //lay.Style.PenColor = Color.Purple;
                    //Pen _Pen = (Pen)lay.Style.Pen.Clone();
                    Pen _Pen = (Pen)lay.Style.Pen.Clone();//20150324 wm add
                    string sLayName = "空域："+lay.Name.ToLower().Replace(".asd", "");

                    myLayerList.Add(lay);
                    //MessageBox.Show("添加列表项");
                    listBoxMap.Items.Add(sLayName);//加入到属性设置列表
                    listPen.Add(_Pen);
                }
            }

            if (listBoxMap.Items.Count > 0)
            {
                listBoxMap.SelectedIndex = 0;
            }
        }

        private void listBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSel = listBoxMap.SelectedIndex;
            if (iSel >= 0 && iSel < listPen.Count)
            {
                labLNM.Text = listBoxMap.Items[iSel].ToString();
                Pen tpen = listPen[iSel];
                pnlColor.BackColor = tpen.Color;
                nudFWidth.Value = Convert.ToDecimal(tpen.Width);
            }
        }

        private void pnlColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnlColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pnlColor.BackColor = colorDialog1.Color;
            }
        }

        private void button1_Click(object sender, EventArgs e)//应用按钮；
        {
            int iSel = listBoxMap.SelectedIndex;
            if (iSel >= 0 && iSel < listPen.Count)
            {
                Pen tpen = listPen[iSel];
                tpen.Color = pnlColor.BackColor;
                tpen.Width = Convert.ToSingle(nudFWidth.Value);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int iSel = listBoxMap.SelectedIndex;
            if (iSel >= 0 && iSel < listPen.Count)
            {
                Pen tpen = listPen[iSel];
                tpen.Color = pnlColor.BackColor;
                tpen.Width = Convert.ToSingle(nudFWidth.Value);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
