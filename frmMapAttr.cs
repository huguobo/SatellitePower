using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Layers;

namespace SatellitePower
{
    public partial class frmMapAttr : Form
    {
        public IList<ICustmoLayer> myLayerList = new List<ICustmoLayer>();
        public List<Pen> listPen = new List<Pen>();

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

        private void button1_Click(object sender, EventArgs e)
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
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
