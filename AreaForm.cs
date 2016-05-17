using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

using Bexa.Guojf.FreeMicaps.Coordinates;
using SatellitePower.UControl;

namespace SatellitePower
{
    public partial class AreaForm : Form
    {
        public static bool editMode = false;
        public static string[] arealist;
        public static string filename = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "workspace\\areaList.cfg";
        
        private Regex obj = new Regex("^\\w+$");//正则表达式，表示文件名只能由数字、英文字母、汉字以及下划线组成
        private Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");//判断是否是正整数
        private bool nameValible = true;
        private bool dataValible = true;
        public PointF pt;
        public static string smode="old";//用于确定在保存模式选择里边识别具体按得哪个按钮，默认为“新增”模式下的saveMode；
        public int points;//表示原来的文件中输入的点的个数。
        public PointF getPosition(TextBox d1, TextBox f1, TextBox d2, TextBox f2)//added by ZJB 20150409
        {
            float d;
            float f;
            float.TryParse(d1.Text, out d);
            float.TryParse(f1.Text, out f);
            pt.X = d + f / 60;
            float.TryParse(d2.Text, out d);
            float.TryParse(f2.Text, out f);
            pt.Y = d + f / 60;

            return pt;
        }
        public AreaForm()
        {
            InitializeComponent();
            
        }
        string[] files = Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\");
        private void AreaForm_Load(object sender, EventArgs e)
        {
            this.AreaList.BeginUpdate();

            /*foreach (string areaName in MainForm.AreaList)
            {
                this.AreaList.Items.Add(areaName);
            }*/
            
            string s;
            foreach (string aname in files)
            {
                s = aname.ToLower().Replace(".asd", "");
                string name = Path.GetFileNameWithoutExtension(s);
                //MessageBox.Show(name);
                AreaList.Items.Add(name);
            }
            this.AreaList.EndUpdate();
            if (editMode == true)
            {
                SetEditMode();
            }
        }
        public bool existed = false;
        public void nameCheck(string name)
        {
            if(AreaList.Items.Contains(name))
                existed=true;
            
        }
        private int currentcount = 3;//保证在点击"下一节点“时可以直接进入case 3： added by ZJB 20150409
        // private int i;
        private void checkE_d(TextBox tb, ref bool b)
        {
            int a;
            //b = true;
            int.TryParse(tb.Text, out a);
            if (a < 70 || a > 165 || reg1.IsMatch(tb.Text) == false)
            {
                b = false;

            }
        }
        private void checkE_d(TextBox tb)
        {
            int a;
            int.TryParse(tb.Text, out a);
            if (a < 70 || a > 165 || reg1.IsMatch(tb.Text) == false)
            {
                dataValible = false;

            }
        }
        private void checkN_d(TextBox tb, ref bool b)
        {
            int c;
            //b = true;
            int.TryParse(tb.Text, out c);
            if (c < 0 || c > 60 || reg1.IsMatch(tb.Text) == false)
            {
                b = false;

            }

        }
        private void checkN_d(TextBox tb)
        {
            int c;
            int.TryParse(tb.Text, out c);
            if (c < 0 || c > 60 || reg1.IsMatch(tb.Text) == false)
            {
                dataValible = false;

            }
        }
        private void checkF(TextBox tb, ref bool b)
        {
            int d;
            //b = true;
            int.TryParse(tb.Text, out d);
            if (d < 0 || d > 59 || reg1.IsMatch(tb.Text) == false)
            {
                b = false;

            }
        }
        private void checkF(TextBox tb)
        {
            if (tb.Text == "")
                tb.Text = "0";
            int d;
            int.TryParse(tb.Text, out d);
            if (d < 0 || d > 59 || reg1.IsMatch(tb.Text) == false)
            {
                dataValible = false;

            }
        }
        //bool p1Valible = true;
        //bool p2Valible = true;
        int errorPoint;
        public void checkP1()
        {
            dataValible = true;
            checkE_d(p1_e_d);
            //MessageBox.Show(p1Valible.ToString());
            checkN_d(p1_n_d);
            //MessageBox.Show(p1Valible.ToString());
            checkF(p1_e_f);
            //MessageBox.Show(p1Valible.ToString());
            checkF(p1_n_f);
            //MessageBox.Show(p1Valible.ToString());
            if (dataValible == false)
                errorPoint = 1;
        }
        public void checkP2()
        {
            dataValible = true;
            checkE_d(p2_e_d);
            checkN_d(p2_n_d);
            checkF(p2_e_f);
            checkF(p2_n_f);
            if (dataValible == false)
                errorPoint = 2;

        }
        public void checkP3()
        {
            dataValible = true;
            checkE_d(p3_e_d);
            checkN_d(p3_n_d);
            checkF(p3_e_f);
            checkF(p3_n_f);
            if (dataValible == false)
                errorPoint = 3;
        }
        public void checkP4()
        {
            dataValible = true;
            checkE_d(p4_e_d);
            checkN_d(p4_n_d);
            checkF(p4_e_f);
            checkF(p4_n_f);
            if (dataValible == false)
                errorPoint = 4;
        }
        public void checkP5()
        {
            dataValible = true;
            checkE_d(p5_e_d);
            checkN_d(p5_n_d);
            checkF(p5_e_f);
            checkF(p5_n_f);
            if (dataValible == false)
                errorPoint = 5;
        }
        public void checkP6()
        {
            dataValible = true;
            checkE_d(p6_e_d);
            checkN_d(p6_n_d);
            checkF(p6_e_f);
            checkF(p6_n_f);
            if (dataValible == false)
                errorPoint = 6;
        }
        public void checkP7()
        {
            dataValible = true;
            checkE_d(p7_e_d);
            checkN_d(p7_n_d);
            checkF(p7_e_f);
            checkF(p7_n_f);
            if (dataValible == false)
                errorPoint = 7;
        }
        public void checkP8()
        {
            dataValible = true;
            checkE_d(p8_e_d);
            checkN_d(p8_n_d);
            checkF(p8_e_f);
            checkF(p8_n_f);
            if (dataValible == false)
                errorPoint = 8;
        }
        private void checkThePoint(int a)
        {
            switch (a)
            {
                case 2:
                    checkP2();
                    checkP1();
                    if (errorPoint == 2 || errorPoint == 1)
                        dataValible = false;
                    break;
                case 3:
                    checkP2();
                    checkP1();
                    if (errorPoint == 2 || errorPoint == 1)
                        dataValible = false;
                    break;
                case 4:
                    checkP3();
                    break;
                case 5:
                    checkP4();
                    break;
                case 6:
                    checkP5();
                    break;
                case 7:
                    checkP6();
                    break;
                case 8:
                    checkP7();
                    break;
                default:
                    checkP8();
                    break;

            }
        }
        int pointCount = 2;

        private void button_nextTwoPoint_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("errorPoint值为：" + errorPoint.ToString());
            //MessageBox.Show("currentcout值为：" + currentcount.ToString());
            currentcount = pointCount + 1;
            errorPoint = 100;
            checkThePoint(currentcount);
            if (errorPoint >= 1 && errorPoint <= 8)
            {
                MessageBox.Show(errorPoint.ToString() + "号节点的数据输入有误！\nE：70-165度，0-59分(整数）\nN：0-60度，0-59分(整数）", "数据错误");
            }
            if (errorPoint == 100)
            {
                switch (currentcount)
                {
                    case 3:
                        point3.Enabled = true;
                        pointCount = 3;
                        break;
                    case 4:
                        point4.Enabled = true;
                        pointCount = 4;
                        break;
                    case 5:
                        point5.Enabled = true;
                        pointCount = 5;
                        break;
                    case 6:
                        point6.Enabled = true;
                        pointCount = 6;
                        break;
                    case 7:
                        point7.Enabled = true;
                        pointCount = 7;
                        break;
                    case 8:
                        point8.Enabled = true;
                        pointCount = 8;
                        break;
                    default:
                        MessageBox.Show("以达到最大可输入点数！", "错误");
                        break;
                }
            }




            /* checkP1();
             checkP2();
            // MessageBox.Show(p2Valible.ToString()+"222222222");
             //MessageBox.Show(p1Valible.ToString()+"1111111111");
             if (p1Valible == false)
             {
                 MessageBox.Show("1号节点的数据输入有误！", "数据错误");
                 //dataValible = true;
                 return;
             }
             else if (p2Valible == false)
             {

                 MessageBox.Show("2号节点的数据输入有误！", "数据错误");
                 return;
             }
             else//p1、p2的格式正确；
             {
                 //MessageBox.Show(dataValible.ToString()+"&&&&"+currentcount.ToString()+"$$$$$");
                 if (dataValible == true)
                 {
                     currentcount++;
                 }
                    
               
                 //MessageBox.Show(currentcount.ToString());
                 dataValible = true;
                 switch (currentcount)
                 {
                     case 2:
                         break;
                     case 3:
                         //dataValible = true;
                         if (dataValible == true)
                             point3.Enabled = true;
                         break;

                     case 4:
                         //dataValible = true;
                        
                         checkE_d(p3_e_d);
                         checkN_d(p3_n_d);
                         checkF(p3_e_f);
                         checkF(p3_n_f);
                         if (dataValible == true)
                             point4.Enabled = true;
                         break;
                     case 5:
                         //dataValible = true;
                         //point5.Enabled = true;
                         checkE_d(p4_e_d);
                         checkN_d(p4_n_d);
                         checkF(p4_e_f);
                         checkF(p4_n_f);
                         if (dataValible == true)
                             point5.Enabled = true;
                         break;
                     case 7:
                         //dataValible = true;
                         //point6.Enabled = true;
                         checkE_d(p6_e_d);
                         checkN_d(p6_n_d);
                         checkF(p6_e_f);
                         checkF(p6_n_f);
                         if (dataValible == true)
                             point7.Enabled = true;
                         break;
                     case 8:
                         //dataValible = true;
                         //point7.Enabled = true;
                         checkE_d(p7_e_d);
                         checkN_d(p7_n_d);
                         checkF(p7_e_f);
                         checkF(p7_n_f);
                         if (dataValible == true)
                             point8.Enabled = true;
                         break;
                     case 6:
                         //dataValible = true;
                         //point8.Enabled = true;
                         checkE_d(p5_e_d);
                         checkN_d(p5_n_d);
                         checkF(p5_e_f);
                         checkF(p5_n_f);
                         if (dataValible == true)
                             point6.Enabled = true;
                         break;
                     default:
                         MessageBox.Show("已达到可输入最大点数!", "错误");
                         break;


                 }
                  if(dataValible==false)
                 {
                     MessageBox.Show((currentcount-1).ToString() + "号节点的数据输入有误", "数据错误");
                    
                     //currentcount--;
                     return;    
                 }
             }*/
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        string datafolder = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "workspace\\Airspaces\\";
        public delegate void deleteLayer(string s);
        public deleteLayer deleteOldLayer;

        public void RewriteToFile(string name)//应该同时完成对checked文件夹中保存的文件的更新
        {
            string origionFile = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces" + name + ".ASD";
            string newFile = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked" + name + ".ASD";
            try
            {
                File.Delete(origionFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            deleteOldLayer(name);
            
            SaveDataToFile(name);
            
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\");
            }
            if (File.Exists(origionFile))
            {
                File.Copy(origionFile, newFile);
            }
        }
        public void SaveDataToFile(string name)
        {

            string fname = datafolder + name + ".ASD";
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("diamond 9");
            sw.WriteLine("0 0 0 0 0 0 0 0 0 0");
            sw.WriteLine((areaType_circle.Checked == true ? pointCount + 1 : pointCount).ToString() + "  map  274  1  "+name+".ASD");
            PointF p1, p2, pa;
            p1 = getPosition(p1_e_d, p1_e_f, p1_n_d, p1_n_f);
            p2 = getPosition(p2_e_d, p2_e_f, p2_n_d, p2_n_f);
            sw.WriteLine(p1.X.ToString() + " " + p1.Y.ToString());
            sw.WriteLine(p2.X.ToString() + " " + p2.Y.ToString());
            switch (pointCount)
            {
                case 3:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;
                case 4:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p4_e_d, p4_e_f, p4_n_d, p4_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;
                case 5:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p4_e_d, p4_e_f, p4_n_d, p4_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p5_e_d, p5_e_f, p5_n_d, p5_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;
                case 6:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p4_e_d, p4_e_f, p4_n_d, p4_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p5_e_d, p5_e_f, p5_n_d, p5_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p6_e_d, p6_e_f, p6_n_d, p6_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;
                case 7:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p4_e_d, p4_e_f, p4_n_d, p4_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p5_e_d, p5_e_f, p5_n_d, p5_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p6_e_d, p6_e_f, p6_n_d, p6_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p7_e_d, p7_e_f, p7_n_d, p7_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;
                case 8:
                    pa = getPosition(p3_e_d, p3_e_f, p3_n_d, p3_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p4_e_d, p4_e_f, p4_n_d, p4_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p5_e_d, p5_e_f, p5_n_d, p5_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p6_e_d, p6_e_f, p6_n_d, p6_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p7_e_d, p7_e_f, p7_n_d, p7_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    pa = getPosition(p8_e_d, p8_e_f, p8_n_d, p8_n_f);
                    sw.WriteLine(pa.X.ToString() + " " + pa.Y.ToString());
                    break;

            }
            if (areaType_circle.Checked == true)
                sw.WriteLine(p1.X.ToString() + " " + p1.Y.ToString());

            sw.Close();
            fs.Close();


        }
        public delegate void render();
        public render aRender;
        private void button_save_Click(object sender, EventArgs e)
        {
            string areaName = NameOfArea.Text;
            if (!Directory.Exists(datafolder))
                Directory.CreateDirectory(datafolder);
            #region 直接增加空域
            if (editMode == false)
            {
                
                #region 判断数据错误；
                errorPoint = 100;
                //MessageBox.Show(currentcount.ToString());
                //MessageBox.Show(errorPoint.ToString());
                checkThePoint(pointCount + 1);
                //MessageBox.Show(currentcount.ToString());
                //MessageBox.Show(errorPoint.ToString());
                if (errorPoint >= 1 && errorPoint <= 8)
                {
                    MessageBox.Show(errorPoint.ToString() + "号节点的数据输入有误！\nE：70-165度，0-59分(整数）\nN：0-60度，0-59分(整数)", "数据错误");
                    return;
                }
                #endregion
                #region 判断文件名是否合法
               
                nameValible = true;
                existed = false;
                bool rewrite = false;

                // MessageBox.Show("点选类型之前的nameValid：" + nameValible.ToString());
                if ((areaType_circle.Checked || areaType_multiLine.Checked) == false)
                {
                    MessageBox.Show("必须先选择要生成的空域类型！", "错误提示");
                    //MessageBox.Show("点选类型之后的nameValid：" + nameValible.ToString());
                    return;
                    //nameValible = false;
                }

                else
                {
                    // MessageBox.Show("判断有效之前的nameValid：" + nameValible.ToString());
                    if (obj.IsMatch(areaName) == false)
                    {
                        MessageBox.Show("当前名称无效！\n空域名称只能由数字、字母、汉字以及下划线组成。", "错误");
                        nameValible = false;
                        //MessageBox.Show("判断有效之后的nameValid：" + nameValible.ToString());
                        return;
                    }
                    else
                    {
                        //MessageBox.Show("判断重复之前的nameValid：" + nameValible.ToString());
                        nameCheck(areaName);
                        if (existed == true)//当前名称已经存在的情况
                        {

                            if (MessageBox.Show("当前空域已存在，是否覆盖写入？\n提示：覆盖写入会删除该空域之前保存的所有数据。", "重复的空域名", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                string name = areaName + ".ASD";

                                string fname = datafolder + areaName + ".ASD";
                                File.Delete(fname);
                                RewriteToFile(areaName);
                                rewrite = true;
                                AddTolistAndSetLayerVisible();
                                this.Close();
                                return;
                            }
                            else
                            {
                                nameValible = false;
                                return;
                            }

                            //MessageBox.Show("判断重复之后的nameValid：" + nameValible.ToString());
                            //MessageBox.Show("是否重写重复的空域："+ rewrite.ToString(),rewrite==true?"已重写":"未重写");

                        }

                    }


                }
                #endregion
                //MessageBox.Show("结束所有判断之后的nameValid：" + nameValible.ToString());

                if (nameValible == true)
                {
                    #region 文件名正确，加入相关的文件名列表中，并且通过刷新使信保存的文件名出现在“已存在空域列表”中

                    //不用arealist.cfg来控制显示了，不需要对该文件进行写操作了；
                    ////MessageBox.Show("当前文件名合法，可以加入文件名列表中");

                    ////string filename=Application.StartupPath+"\workspace"+"AreaList.cfg";
                    //FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None);
                    //StreamWriter writer = new StreamWriter(fs);
                    //writer.Write(areaName + "\r\n");
                    //writer.Close();
                    //fs.Close();

                    SaveDataToFile(areaName);//讲数据保存贷文件中，名字为AREANAME.ASD added by ZJB 20150409


                    this.AreaList.BeginUpdate();


                    AreaList.Items.Add(areaName);

                    AreaList.EndUpdate();
                    #endregion
                    if (MessageBox.Show("新的空域已经生成，是否立即显示？", "空域显示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //DrawAirspce(areaName);
                    {
                        delegateDraw();
                        //MessageBox.Show("ok");
                    }
                    else
                        return;
                }
                this.Close();

            }
            #endregion
            #region 编辑状态下的“save”

            else
            {
                #region 判断数据错误；
                errorPoint = 100;
                //MessageBox.Show(currentcount.ToString());
                //MessageBox.Show(errorPoint.ToString());
                checkThePoint(pointCount + 1);
                //MessageBox.Show(currentcount.ToString());
                //MessageBox.Show(errorPoint.ToString());
                if (errorPoint >= 1 && errorPoint <= 8)
                {
                    MessageBox.Show(errorPoint.ToString() + "号节点的数据输入有误！\nE：70-165度，0-59分(整数）\nN：0-60度，0-59分(整数)", "数据错误");
                    return;
                }
                #endregion
                SaveMode newForm = new SaveMode();
                newForm.ShowDialog();
                if (smode == "all")
                {
                    string name = areaName + ".ASD";
                    string fname = datafolder + areaName + ".ASD";
                    
                    
                    File.Delete(fname);
                    if (pointCount < points)
                    {
                        pointCount = points;//当前状态下points表示原有点数，pointCount表示修改状态下的点数，rewriteToFile函数会调用saveDataToFile，而此函数会根据pointCount值写文件。

                    }
                    RewriteToFile(areaName);//仅完成文件内容的更新
                    AddTolistAndSetLayerVisible();
                }
                else if (smode == "justEdited")
                {
                    string name = areaName + ".ASD";
                    string fname = datafolder + areaName + ".ASD";
                    
                   
                    File.Delete(fname);
                    RewriteToFile(areaName);
                    AddTolistAndSetLayerVisible();
                }
                else if (smode == "cancel")
                {
                    return;
                }
                else
                {
                    return;
                }
                aRender();
                this.Close();
            }
            #endregion
        }
       
        public void AddTolistAndSetLayerVisible()
        {
            /*string areaName = NameOfArea.Text;
            //arealist = File.ReadAllLines(filename);
            
            this.AreaList.BeginUpdate();
            
            AreaList.Items.Add(areaName);
            //AreaList.EndUpdate();*/

            if (MessageBox.Show("新的空域已经生成，是否立即显示？", "空域显示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //DrawAirspce(areaName);
                
                delegateDraw();
            }
            else
                return;
        }
        public delegate void DrawAirSpace(string s);
        public DrawAirSpace newDraw;
        private void delegateDraw()
        {
            string areaName = NameOfArea.Text;
            newDraw(areaName);
        }

        
        public void SetEditMode()
        {
            DirectoryInfo filePath = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\");
            if (filePath.GetFiles().Length>0) //有可编辑的空域文件存在
            {
                string TheFirst = AreaList.Items[0].ToString();
                #region 设置打开“编辑”时出事的窗口显示
                
                NameOfArea.Enabled = false;
                readFileData(TheFirst);
                AreaList.SelectedIndex = 0;
                AreaList.SelectedIndexChanged += new System.EventHandler(selectTA);
                #endregion
            }
        }
        
        public void readFileData(string name)
        {
            
            NameOfArea.Text = name;
            string fileName = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + name + ".ASD";
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string[] file = File.ReadAllLines(fileName);

            int i = file.Length - 1;
            
            if (file[3] == file[i])
            {
                areaType_circle.Checked = true;
                points = i - 3;
                //MessageBox.Show("circle"+points.ToString());
            }
            else
            {
                areaType_multiLine.Checked = true;
                points = i - 2;
                //MessageBox.Show("multiline" + points.ToString());
            }
            clearAll();//删除所有textBox内的数据，避免在选择A的情况下出现B里面的数据（B中的数据比A多的情况下）
            string [] p1=file[3].Split(' ');
           // MessageBox.Show(getF(p1[0]).ToString());
            p1_e_d.Text = getD(p1[0]).ToString();
            p1_e_f.Text = getF(p1[0]).ToString();
            p1_n_d.Text = getD(p1[1]).ToString();
            p1_n_f.Text = getF(p1[1]).ToString();
            string[] p2 = file[4].Split(' ');
            p2_e_d.Text = getD(p2[0]).ToString();
            p2_e_f.Text = getF(p2[0]).ToString();
            p2_n_d.Text = getD(p2[1]).ToString();
            p2_n_f.Text = getF(p2[1]).ToString();
            if (points > 2)
            {
                fillTheLastTB(file);//填充剩余的Point;
            }

            sr.Close();
            fs.Close();

            //MessageBox.Show(name);
            
        }
        public void clearAll()
        {
            try
            {
                point3.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
                point4.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
                point5.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
                point6.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
                point7.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
                point8.Controls.OfType<TextBox>().ToList().ForEach(t => t.Clear());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
       }
        public void fillTheLastTB(string[] file)
        {
            string[] p3 = file[5].Split(' ');
            p3_e_d.Text = getD(p3[0]).ToString();
            p3_e_f.Text = getF(p3[0]).ToString();
            p3_n_d.Text = getD(p3[1]).ToString();
            p3_n_f.Text = getF(p3[1]).ToString();
            if (points == 3)
            {
                return;
            }
            string[] p4 = file[6].Split(' ');
            p4_e_d.Text = getD(p4[0]).ToString();
            p4_e_f.Text = getF(p4[0]).ToString();
            p4_n_d.Text = getD(p4[1]).ToString();
            p4_n_f.Text = getF(p4[1]).ToString();
            if (points == 4)
            {
                return;
            }
            string[] p5 = file[7].Split(' ');
            p5_e_d.Text = getD(p5[0]).ToString();
            p5_e_f.Text = getF(p5[0]).ToString();
            p5_n_d.Text = getD(p5[1]).ToString();
            p5_n_f.Text = getF(p5[1]).ToString();
            if (points == 5)
            {
                return;
            }
            string[] p6 = file[8].Split(' ');
            p6_e_d.Text = getD(p6[0]).ToString();
            p6_e_f.Text = getF(p6[0]).ToString();
            p6_n_d.Text = getD(p6[1]).ToString();
            p6_n_f.Text = getF(p6[1]).ToString();
            if (points == 6)
            {
                return;
            }
            string[] p7 = file[9].Split(' ');
            p7_e_d.Text = getD(p7[0]).ToString();
            p7_e_f.Text = getF(p7[0]).ToString();
            p7_n_d.Text = getD(p7[1]).ToString();
            p7_n_f.Text = getF(p7[1]).ToString();
            if (points == 7)
            {
                return;
            }
            string[] p8 = file[10].Split(' ');
            p8_e_d.Text = getD(p8[0]).ToString();
            p8_e_f.Text = getF(p8[0]).ToString();
            p8_n_d.Text = getD(p8[1]).ToString();
            p8_n_f.Text = getF(p8[1]).ToString();
            
        }
        public void selectTA(object sender,EventArgs e)//TA为TheAirspace 的缩写；
        {
            string name = AreaList.SelectedItem.ToString();
            //MessageBox.Show(name);
            readFileData(name);
        }
        public int getD(string value)
        {
            float f;
            float.TryParse(value, out f);
            int x;
            x = (int)f;
            return x;
        }
        public int getF(string value)
        {
            float va;
            float.TryParse(value, out va);

            double f = 60.0f * (va - getD(value));
            double f1 = Math.Round(f, 0);

            return (int)f1;
        }
        public static void deleteTheLineInLayerset(string aname)
        {
            #region 删除layerset.cfg中的对应行
            string layerset = Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg";
            string filename = aname + ".ASD";
            try
            {
                string[] files = File.ReadAllLines(layerset);
                List<string> lines = new List<string>();
                foreach (string line in files)
                {
                    string[] strParams = line.Split('|');
                    if (!strParams[0].Equals(filename))
                    {
                        lines.Add(line);
                    }


                }
                if (lines.Count > 0)
                {
                    //foreach (string aline in lines)
                    //    MessageBox.Show(aline);
                    string sFilePathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "layerSet.cfg";
                    //StreamReader sr = new StreamReader(aFile); sr.EndOfStream
                    FileStream aFile = new FileStream(sFilePathName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(aFile);
                    //sw.BaseStream.Seek(0, SeekOrigin.Begin);
                    foreach (string item in lines)
                    {
                        sw.WriteLine(item);
                    }

                    sw.Close();
                    aFile.Close();
                }



            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            #endregion
        }
    }
}
