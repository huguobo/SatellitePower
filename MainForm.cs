using System;
using System.Drawing;
using System.Windows.Forms;
using Bexa.Guojf.FreeMicaps.Plugins;
using Bexa.Guojf.FreeMicaps.Ctrls;
using Bexa.Guojf.FreeMicaps.Coordinates;
using Bexa.Guojf.FreeMicaps.Layers;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SatellitePower.formSys;
using System.Collections;
using System.Threading;
//using RefreshSystemTray;//2014-11-19 wm add
using System.Text.RegularExpressions;
using SatellitePower.UControl;

namespace SatellitePower
{
    public partial class MainForm : Form
    {
        #region 空域功能之外的内容
        //Regex asd = new Regex(@".(?i:ASD)$");//added by ZJB 判断是否后缀为.ASD
        public IStyle AirLineStyle = new Style();//用于记录飞行航线的样式，20150324 wm add

        public List<CSivFileInfo> listFYYT;//反演云图列表，2014-10-27 wm add
        //public string listFYYT_path;//反演云图列表缓存文件路径
        //public frmSelYJF dlgSelYJF, dlgSelLB;//2014-10-27 wm add
        //public CSivFileInfo sfiJB, sfiHW;//2014-10-27 wm add
        public bool init_flag;//是否初次加载，2014-10-28 wm add

        //当前界面选中产品（可能的值有：云际风（wind），雷暴（thunder）,积冰（ice），海雾（fog），空（不是前面几种情况就是空）） 2014-11-4 wcs add
        private string selectedType = string.Empty;

        public string LogicOut;//逻辑减文件存放路径，2014-10-31 wm add
        //===================================================
        //反演产品异步执行所需参数声明结束
        //云际风相关代码 王昌帅
        AnalyCloudWind cloudWind;
        string sSalelliteName = "";
        int iType = 1;//1=IR,2=VIS
        int iWay = 0;//0-3:1~4 通道
        DateTime dateTime_yjf_begin = DateTime.Now;//时间靠后的，如：10点
        Thread makeYJFThread;
        //
        //雷暴
        AnalyThunderStorm thunder;
        Thread makeLBThread;
        //积冰
        AnalyIce ice;
        Thread makeJBThread;
        //海雾
        AnalyFog2 seaFog;
        Thread makeHWThread;
        //等值线
        AnalyContour contour;
        Thread makeDZXThread;
        //亮温或反射率字符串
        string strT;
        //鼠标跟随显示的字符串
        string strT2;
        Thread updateStatusBarThread, updateFollowingThread;

        //反演产品异步执行所需参数声明结束
        //===================================================

        //=======================================================
        //用于定位新生成的反演产品条目
        string newItemNameYJF = string.Empty;
        string newItemNameLB = string.Empty;
        string newItemNameJB = string.Empty;
        string newItemNameHW = string.Empty;
        string newItemNameDZX = string.Empty;
        bool isShowYJF = true;
        bool isShowLB = true;
        bool isShowJB = true;
        bool isShowHW = true;
        bool isShowDZX = true;
        //=======================================================


        public int WenDuJG = 5;//等值线温度间隔,默认为5

        //public static ILog mylog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);//???
        public PluginManager pm;
        public MapCtrl baseMap;
        public CSysConfig sysCfg = new CSysConfig();
        public EWorkType nowWorkType = EWorkType.FYMap;
        public Style userStyle = new Style();//统一各层样式
        public CSysParams userSysParams;
        public InitMap userIMap;//###

        public string sNowSelLayName = "";//当前选定图层文件名
        public string sNowSelYJFLayName = "";

        //public string sNowPalFileName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "pallet" + Path.DirectorySeparatorChar.ToString() + "I-01.pal";
        public string sNowPalFileName = Application.StartupPath + Path.DirectorySeparatorChar.ToString()
                                        + "workspace\\syspallet\\pal00.pal";

        public int iWorkingState = 0;   //0=空闲，1=解析生数据中，2=动画生成中
        public int iNowType = 1;        //1=IR,2=VIS
        public int iNowWay = 0;         //0-3:1~4 通道
        public int iNowProj = 2;        //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
        public int iToolBarTopType = 0; //工具栏图标状态:0=小,1=大

        private bool bMouseGetVal = true;   //鼠标取值
        private bool bMouseLablTM = true;   //取值框透明标志
        private int iGetLineDist = 0;       //直线测距标志，0=空状态，1=测距状态

        //消息同步计数器，超过n个周期后杀死进程
        private int iTBMessageFY = 0;
        private int iTBMessageMTSAT = 0;

        //新地图生成后通知刷新，云图列表就不能用 Index 记录定位了(新图载入、原图下串)，应该用图层名定位
        private int iMsgReLoadMapFlag = 0;

        List<CSivFileInfo> listMapData = new List<CSivFileInfo>();
        List<CYJFFileInfo> listYJFData = new List<CYJFFileInfo>();
        List<CLBFileInfo> listLBData = new List<CLBFileInfo>();

        //2014-10-14 wm add
        List<CJBFileInfo> listJBData = new List<CJBFileInfo>();
        List<CHWFileInfo> listHWData = new List<CHWFileInfo>();
        //2014-10-31 wcs add
        List<CDZXFileInfo> listDZXData = new List<CDZXFileInfo>();
        //==========================================================================================
        Splash splash = new Splash();
        frmPathSet frmPSet = new frmPathSet();

        //反演产品参数设置，20150320，wm add
        frmFYParaSet frmFYPSet = new frmFYParaSet();

        //==========================================================================================
        #region 移除的功能菜单

        //叠加(&O)
        //  地面天气图
        //  高空天气图
        //  T213数值天气预报

        //云图产品Y
        //  云雾识别
        //  云分类
        //  降水估算
        //  航线计算
        //  云迹风

        #endregion
        //==========================================================================================
        #region 传递系统消息

        //WM_USER = 0x0400 = 1024 //程序员定义的窗口消息 WM_USER~0x7FFF = 32767
        //PostMessage 是异步的，SendMessage 是同步的:
        //PostMessage 只把消息放入队列，不管消息是否被处理就返回，消息可能不被处理；
        //而 SendMessage 等待消息被处理完了之后才返回，如果消息不被处理，发送消息的线程将一直被阻塞。

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        //static extern bool PostMessage(int hWnd, int Msg, uint wParam, uint lParam);
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //if found  return the handle , otherwise return IntPtr.Zero
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter
            , string lpszClass, string lpszWindow);

        #endregion
        //==========================================================================================
        #region 释放内存

        /// <summary>
        /// 释放内存API
        /// </summary>
        /// <param name="hProcess">进程的句柄</param>
        /// <param name="dwMinimumWorkingSetSize">进程最小工作空间</param>
        /// <param name="dwMaximumWorkingSetSize">进程最大工作空间</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        /// <summary>
        /// 释放内存
        /// </summary>
        public void ClearMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch
            {
            }
        }

        #endregion
        //==========================================================================================
        //==========================================================================================
        void PlugMg_OnLoad(object plugin)
        {
            if (splash != null)
            {
                PluginInfoAttribute attrib = Plugin.GetAttrib(plugin.GetType());
                //splash.lbMessage.Text = "正在载入:" + attrib.DllName;// DisplayName;
                Application.DoEvents();
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            tmShowLeft.Checked = !scClient.Panel2Collapsed;
            tbFillScreen.Checked = !pm.MainForm.MainMenuStrip.Visible;
            tmFillScreen.Checked = !pm.MainForm.MainMenuStrip.Visible;
            setIRVisBtnState();

            if (nowWorkType == EWorkType.FYMap)
            {
                //tsbJ3.Visible = true;
                //muJ3.Visible = tsbJ3.Visible;                
                //tsbVIS2.Visible = tsbJ3.Visible;
                //tsbVIS3.Visible = tsbJ3.Visible;
                //tsbVIS4.Visible = tsbJ3.Visible;

                if (tabControl1.SelectedIndex == 1)
                {
                    //动画界面
                    //setToolBarBntState(1);
                    tsbIR1.Visible = false;
                    tsbIR2.Visible = tsbIR1.Visible;
                    tsbIR3.Visible = tsbIR1.Visible;
                    tsbIR4.Visible = tsbIR1.Visible;
                    tsbVIS1.Visible = tsbIR1.Visible;
                    //tsbVIS2.Visible = tsbIR1.Visible;
                    //tsbVIS3.Visible = tsbIR1.Visible;
                    //tsbVIS4.Visible = tsbIR1.Visible;

                    toolStripSeparator3.Visible = tsbIR1.Visible;
                    tsbL1.Visible = tsbIR1.Visible;
                    tsbM2.Visible = tsbIR1.Visible;
                    tsbJ3.Visible = tsbIR1.Visible;
                    tsbD4.Visible = tsbIR1.Visible;
                    toolStripSeparator4.Visible = tsbIR1.Visible;
                    //tsbAreaGrid.Visible = tsbIR1.Visible;

                    tsbSavePic.Visible = tsbIR1.Visible;
                    toolStripButton1.Visible = tsbIR1.Visible;
                    tsbPrint.Visible = tsbIR1.Visible;
                }
                else
                {
                    //setToolBarBntState(0);
                    tsbJ3.Visible = true;
                    muJ3.Visible = tsbJ3.Visible;
                    //tsbVIS2.Visible = tsbJ3.Visible;
                    //tsbVIS3.Visible = tsbJ3.Visible;
                    //tsbVIS4.Visible = tsbJ3.Visible;
                }

            }
            else if (nowWorkType == EWorkType.MTSATSec)
            {
                tsbJ3.Visible = false;
                muJ3.Visible = tsbJ3.Visible;
                //tsbVIS2.Visible = tsbJ3.Visible;
                //tsbVIS3.Visible = tsbJ3.Visible;
                //tsbVIS4.Visible = tsbJ3.Visible;
            }

        }

        private void listMapLays()
        {
            if (!listBox2.Visible)
            {
                return;
            }

            //列出当前载入的图层
            listBox2.Items.Clear();
            string ss = "";
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                if (!lay.IsBaseMap)
                {
                    ss = lay.Name;
                    listBox2.Items.Add(ss);
                }
            }
        }
        //==========================================================================================
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //const int WM_DEVICECHANGE = 0x219;
            //const int WM_DEVICEARRVIAL = 0x8000;//如果m.Msg的值为0x8000那么表示有U盘插入
            //const int WM_DEVICEMOVECOMPLETE = 0x8004;
            switch (m.Msg)
            {
                case 4455:
                    //Console.Write("收到内部通知。" + m.WParam + " | " + m.LParam + " | " + m.HWnd + " | " + m.Msg);
                    m.Result = (IntPtr)0;
                    break;
                case 4456:
                    //Console.Write("收到外部通知。" + m.WParam + " | " + m.LParam + " | " + m.HWnd + " | " + m.Msg);
                    m.Result = (IntPtr)0;

                    int iW = (int)m.WParam;
                    int iL = (int)m.LParam;
                    if (iW == 1)
                    {
                        //刷新类消息
                        if (iL == 1)
                        {
                            //刷新文件列表
                            //新地图生成后通知刷新，云图列表就不能用 Index 记录定位了(新图载入、原图下串)，应该用图层名定位
                            iMsgReLoadMapFlag = 1;
                            FindLoadFYMapFile();
                            iMsgReLoadMapFlag = 0;
                        }
                    }
                    break;
                //case WM_DEVICECHANGE:   //磁盘变动消息
                //    {
                //        ShowDeviceChanged("WM_DEVICECHANGE");//
                //        if (m.WParam.ToInt32() == WM_DEVICEARRVIAL)
                //            ShowDeviceChanged("WM_DEVICEARRVIAL");
                //        else if (m.WParam.ToInt32() == WM_DEVICEMOVECOMPLETE)
                //            ShowDeviceChanged("WM_DEVICEMOVECOMPLETE");
                //    }
                //    break;
                case 4458:
                    //收到FY解析程序响应
                    iTBMessageFY = 0;
                    break;
                case 4459:
                    //收到MT解析程序响应
                    iTBMessageMTSAT = 0;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

            //if (m.Msg == 1025)
            //{
            //    Console.Write("zz收到通知。" + m.WParam + " | " + m.LParam + " | " + m.HWnd + " | " + m.Msg);
            //}
        }

        public void setStyleFont()
        {
            //userStyle.TitleFont.Name = userSysParams.TitleFontName;
            //userStyle.TitleFont.Size = userSysParams.TitleFontSize;
            //userStyle.TitleFont.Style = userSysParams.TitleFontFS;//FontStyle.Regular;//普通文本
            try
            {
                Font fnt = new Font(userSysParams.TitleFontName, userSysParams.TitleFontSize, userSysParams.TitleFontFS);
                userStyle.TitleFont = fnt;
            }
            catch (Exception)
            {
                Font fnt = new Font("宋体", 20f);
                userStyle.TitleFont = fnt;
            }
            userStyle.TitleColor = Color.FromArgb(userSysParams.TitleFontColorVal);//Color.Red.ToArgb();
            //userStyle.TitleLeft = userSysParams.TitleLeft;//30;//100;
            //userStyle.TitleTop = userSysParams.TitleTop;//20;//60;

            userStyle.TitleLeft = userSysParams.TitleLeft;
            userStyle.TitleTop = userSysParams.TitleTop;

            userStyle.VIScRGBOffset = userSysParams.VIScRGBOffset;

            //MTSAT，2015-1-22，wm add
            userStyle.VIScRGBOffset_MT = userSysParams.VIScRGBoffset_MT;

            //****************************************
            //地理位置微调，20150827 wm add
            userStyle.Map_FY_top = userSysParams.Map_FY_top;
            userStyle.Map_MT_top = userSysParams.Map_MT_top;
            userStyle.Map_FY_left = userSysParams.Map_FY_left;
            userStyle.Map_MT_left = userSysParams.Map_MT_left;
            //****************************************
        }

        /// <summary>
        ///  安全设置进度条值
        /// </summary>
        /// <param name="iVal"></param>
        public void SetProgressBarVal(int iVal)
        {
            if (iVal >= 0 && iVal < tspBar1.Maximum)
            {
                tspBar1.Value = iVal;
            }
        }

        /// <summary>
        /// 显示状态栏提示，最左侧
        /// </summary>
        /// <param name="str"></param>
        public void ShowStatusInfo(string str)
        {
            stlbMessage.Text = str;
        }
        //==========================================================================================
        public void InitUCMapFileList()
        {
            //创建数据列表窗体
            MapFileList DataList = new MapFileList();
            DataList.Name = "MapFileList1";

            //数据列表加入客户区
            TabControl tabLeftTop = (TabControl)pm.FindControl("tabLeftTop");
            if (tabLeftTop != null)
            {
                //tabLeftTop.TabPages.Clear();

                TabPage tpDataList = new TabPage();
                tpDataList.Text = "数据层";
                DataList.Dock = DockStyle.Fill;

                tpDataList.Controls.Add(DataList);

                tabLeftTop.TabPages.Add(tpDataList);
                tabLeftTop.SelectedTab = tpDataList;//yym
            }

            //加入菜单,yym 2013-7-30 del 自动载入、无需打开
            ToolStripMenuItem muFile = (ToolStripMenuItem)pm.MainForm.MainMenuStrip.Items["muFile"];
            if (muFile != null)
            {
                ToolStripMenuItem tmOpenFile = new ToolStripMenuItem("打开文件...");
                tmOpenFile.Image = Resource1.openN;//.fileicon;
                tmOpenFile.ImageTransparentColor = Resource1.openN.GetPixel(0, 0);
                tmOpenFile.ShortcutKeys = Keys.Control | Keys.O;
                tmOpenFile.Click += DataList.btnAddData_Click;
                muFile.DropDownItems.Insert(0, tmOpenFile);
            }

            //加入工具条
            //ToolStrip tsToolBar = (ToolStrip)pm.FindControl("tsToolBar");
            //if (tsToolBar != null)
            //{
            //    ToolStripButton tbOpenFile = new ToolStripButton("打开");
            //    tbOpenFile.Image = Resource1.openN;
            //    tbOpenFile.ToolTipText = "打开文件...";
            //    tbOpenFile.Font = new Font("宋体", 9);
            //    tbOpenFile.ForeColor = Color.Yellow;
            //    tbOpenFile.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            //    tbOpenFile.TextImageRelation = TextImageRelation.ImageAboveText;
            //    tbOpenFile.ImageTransparentColor = Resource1.openN.GetPixel(0, 0);//open
            //    tbOpenFile.Click += DataList.btnAddData_Click;
            //    tsToolBar.Items.Insert(0, tbOpenFile);
            //    tsToolBar.Items.Insert(1, new ToolStripSeparator());
            //}
            //ToolStripButton tbOpenFile = (ToolStripButton)pm.FindControl("tsbOpen");
            ToolStrip tsToolBar = (ToolStrip)pm.FindControl("tsToolBar");
            if (tsToolBar != null)
            {
                for (int i = 0; i < tsToolBar.Items.Count; i++)
                {
                    ToolStripItem tsi = tsToolBar.Items[i];
                    if (tsi.Name.Equals("tsbOpen"))
                    {
                        tsi.Click += DataList.btnAddData_Click;
                        break;
                    }
                }
            }
            //tbOpen
        }

        #region OldSec

        //private void FindFileAll(string path)
        //{
        //    //递归搜索文件
        //    int k = 0;
        //    foreach (DirectoryInfo dirinfo in new DirectoryInfo(path).GetDirectories())
        //    {
        //        //addLog(dirinfo.Name);//文件夹名
        //        //addLog(dirinfo.FullName);//完整路径名，不含末尾\
        //        //addLog(dirinfo.Attributes.ToString());//='Directory'
        //        foreach (FileInfo fi in dirinfo.GetFiles())
        //        {
        //            string sShowName = fi.Name;//解析文件名
        //            string sPathName = fi.FullName;

        //            //加入缓存列表
        //            //MapFileInfo mfi = new MapFileInfo(sPathName);
        //            //sShowName = mfi.getShowName();
        //            //listMFI.Add(mfi);

        //            CSecFileInfo sfi = new CSecFileInfo();
        //            sfi.sPath = sPathName;
        //            sfi.sDefRootPath = sysCfg.sSecPath;
        //            sfi.loadHead();

        //            if (sfi.bIsMapFile)
        //            {
        //                listBoxMap.Items.Add(sfi);
        //            }
        //            k++;
        //        }
        //        FindFileAll(dirinfo.FullName);
        //    }

        //    //没有子目录
        //    if (k <= 0)
        //    {
        //        foreach (FileInfo fi in new DirectoryInfo(path).GetFiles())
        //        {
        //            string sShowName = fi.Name;//解析文件名
        //            string sPathName = fi.FullName;

        //            //加入缓存列表
        //            //MapFileInfo mfi = new MapFileInfo(sPathName);
        //            //sShowName = mfi.getShowName();
        //            //listMFI.Add(mfi);

        //            CSecFileInfo sfi = new CSecFileInfo();
        //            sfi.sPath = sPathName;
        //            sfi.sDefRootPath = sysCfg.sSecPath;
        //            sfi.loadHead();

        //            if (sfi.bIsMapFile)
        //            {

        //                string sListName = sfi.ToString();//sShortName
        //                bool bExist = false;
        //                //if (listBoxMap.Items.IndexOf(sListName) < 0)

        //                //if (nowWorkType == EWorkType.MTSATSec)
        //                //{
        //                foreach (CSecFileInfo item in listBoxMap.Items)
        //                {
        //                    if (item != null)
        //                    {
        //                        if (item.ToString().Equals(sListName))
        //                        {
        //                            bExist = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //                //}

        //                if (!bExist)
        //                {
        //                    listBoxMap.Items.Add(sfi);
        //                    k++;
        //                }
        //            }

        //        }
        //    }
        //}

        //private void FindLoadSECMapFile()
        //{
        //    listBoxMap.Items.Clear();
        //    if (!Directory.Exists(sysCfg.sSecPath))
        //    {
        //        return;
        //    }

        //    int iflag = 1;
        //    if (iflag == 0)
        //    {
        //        foreach (FileInfo fi in new DirectoryInfo(sysCfg.sSecPath).GetFiles("*.*"))
        //        {
        //            string sShowName = fi.Name;//解析文件名
        //            string sPathName = fi.FullName;

        //            CSecFileInfo sfi = new CSecFileInfo();
        //            sfi.sPath = sPathName;
        //            sfi.sDefRootPath = sysCfg.sSecPath;
        //            sfi.loadHead();

        //            //mi.bmpImage = new Bitmap(sPathName);
        //            if (sfi.bIsMapFile)
        //            {
        //                listBoxMap.Items.Add(sfi);
        //            }
        //        }
        //    }
        //    else if (iflag == 1)
        //    {
        //        string ss = sysCfg.sSecPath + "a\\";
        //        //FindFileAll(sysCfg.sSecPath);
        //        if (Directory.Exists(ss))
        //        {
        //            FindFileAll(ss);
        //        }
        //        else if (Directory.Exists(sysCfg.sSecPath + "b\\"))
        //        {
        //            FindFileAll(sysCfg.sSecPath + "b\\");
        //        }
        //        else if (Directory.Exists(sysCfg.sSecPath + "c\\"))
        //        {
        //            FindFileAll(sysCfg.sSecPath + "c\\");
        //        }
        //    }
        //}
        #endregion

        #region 地图文件载入

        private static int SortFileDesc(CSivFileInfo a1, CSivFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }

        private void AddYT(string path, string suffix, int flag)
        {
            foreach (FileInfo fi in new DirectoryInfo(path).GetFiles(suffix, SearchOption.AllDirectories))//20140513 add 改成搜索子目录//IR1,IR2,IR3,IR4,VIS1为子目录，2015-01-21,wm add
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CSivFileInfo sfi = new CSivFileInfo();
                sfi.sPath = sPathName;
                sfi.sDefRootPath = userSysParams.sMapPath;
                sfi.loadHead();

                //mi.bmpImage = new Bitmap(sPathName);
                if (sfi.bIsMapFile)
                {
                    if (flag != 2)
                    {//加载全部云图（包括设置的显示天数以外的云图）
                        if (sfi.dtBegin > (DateTime.Now.AddDays(-10000)))
                        {
                            ////listBoxMap.Items.Add(sfi);                        
                            //listMapData.Add(sfi);

                            #region 判断列表中是否已存在

                            bool bListExist = false;
                            foreach (CSivFileInfo item in listMapData)
                            {
                                if (item.sName.Equals(sfi.sName))
                                {
                                    bListExist = true;
                                    break;
                                }
                            }
                            if (!bListExist)
                            {
                                listMapData.Add(sfi);
                            }

                            #endregion
                        }
                        ADD_listFYYT(sfi);//2014-10-28 wm add
                    }
                    else
                    {//只加载设置的显示天数以内的云图
                        if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                        {
                            ////listBoxMap.Items.Add(sfi);                        
                            //listMapData.Add(sfi);

                            #region 判断列表中是否已存在

                            bool bListExist = false;
                            foreach (CSivFileInfo item in listMapData)
                            {
                                if (item.sName.Equals(sfi.sName))
                                {
                                    bListExist = true;
                                    break;
                                }
                            }
                            if (!bListExist)
                            {
                                listMapData.Add(sfi);
                            }

                            #endregion
                        }
                        ADD_listFYYT(sfi);//2014-10-28 wm add
                    }
                }
            }
        }

        /// <summary>
        /// 载入FY地图文件
        /// </summary>
        private void FindLoadFYMapFile(int flag = 2)//2014-12-16 wm add
        {
            ShowStatusInfo("正在载入数据文件列表...");

            int k = listBoxMap.SelectedIndex;//为了再次定位            
            listBoxMap.Items.Clear();
            listMapData.Clear();

            listFYYT.Clear();//2014-10-27 wm add

            //string sIR1P = sysCfg.getIR1Path(userSysParams);//20140513 del
            string sIR1P = userSysParams.sMapPath;//20140513 add 改成搜索子目录

            if (!Directory.Exists(sIR1P))
            {
                ShowStatusInfo("数据文件路径无效！");
                return;
            }

            if (flag != 2)
            {//加载全部云图的全部通道投影（包括设置的显示天数以外的云图和无效的云图，以确保删除的时候能彻底删除这些无效的云图文件）
                AddYT(sIR1P, "*.spl", flag);
                AddYT(sIR1P, "*.spd", flag);
                AddYT(sIR1P, "*.spj", flag);
                AddYT(sIR1P, "*.spm", flag);
                AddYT(sIR1P, "*.sib", flag);
                AddYT(sIR1P, "*.sil", flag);
                AddYT(sIR1P, "*.siv", flag);
            }
            else
            {

                AddYT(sIR1P, "*.spm", flag);//麦卡托是主要投影，2015-1-21，wm add

                //为避免遗漏，4个投影都算上，2015-1-21，wm add
                AddYT(sIR1P, "*.spl", flag);
                AddYT(sIR1P, "*.spd", flag);
                AddYT(sIR1P, "*.spj", flag);
            }

            //列表排序
            listMapData.Sort(SortFileDesc);

            listFYYT.Sort(SortFileDesc);//2014-10-27 wm add
            //展现地图列表
            int iLayNameIndex = -1;
            for (int i = 0; i < listMapData.Count; i++)
            {
                CSivFileInfo sfi = listMapData[i];
                listBoxMap.Items.Add(sfi);

                ////2014-8-11 del 暂时取消刷新地图列表定位到原图的功能，统一定位到新图
                string sWayName = sfi.getVissLayerName(iNowType, iNowWay, iNowProj);
                if (sWayName.Equals(sNowSelLayName))
                {
                    //得到原来在看的云图索引
                    iLayNameIndex = listBoxMap.Items.Count - 1;
                }
            }

            if (tabControl1.SelectedIndex == 0)//云图列表页
            {
                //新地图生成后通知刷新，云图列表就不能用 Index 记录定位了(新图载入、原图下串)，应该用图层名定位
                if (iMsgReLoadMapFlag == 1)
                {
                    //判断是否有匹配的新图
                    bool bHaveNewSameMap = false;

                    #region 2014-08-11 定位到新图

                    if (listBoxMap.Items.Count > 0)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[0] as CSivFileInfo;
                        if (sfi != null)
                        {
                            string sMapFile = "";
                            //0-3:1~4 通道
                            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度

                            //预留详细控制
                            if (iNowType == 1)//1=IR,2=VIS
                            {
                                //先检测"当前通道当前投影"
                                sMapFile = sfi.getVissPathName(iNowType, iNowWay, iNowProj);
                                bHaveNewSameMap = File.Exists(sMapFile);
                                if (!bHaveNewSameMap)
                                {
                                    //新图没有"当前通道当前投影"

                                    //0-3:1~4 通道
                                    if (iNowWay == 0)
                                    {
                                        //IR1-IR2 间可以互相跳转
                                        sMapFile = sfi.getVissPathName(iNowType, 1, iNowProj);
                                        bHaveNewSameMap = File.Exists(sMapFile);
                                    }
                                    else if (iNowWay == 1)
                                    {
                                        //IR1-IR2 间可以互相跳转
                                        sMapFile = sfi.getVissPathName(iNowType, 0, iNowProj);
                                        bHaveNewSameMap = File.Exists(sMapFile);
                                    }
                                    else if (iNowWay == 2)
                                    {
                                        //存在就定位到新图，没有则保持原来
                                    }
                                    else if (iNowWay == 3)
                                    {
                                        //存在就定位到新图，没有则保持原来
                                    }
                                }
                            }
                            else
                            {
                                //2=VIS
                                //先检测"当前通道当前投影"
                                sMapFile = sfi.getVissPathName(iNowType, iNowWay, iNowProj);
                                bHaveNewSameMap = File.Exists(sMapFile);//存在就定位到新图，没有则保持原来
                                //if (!bHaveNewSameMap)
                                //{
                                //    //新图没有"当前通道当前投影"
                                //}
                            }
                        }
                    }

                    #endregion

                    if (bHaveNewSameMap)
                    {
                        //找到对应新图
                        if (listBoxMap.Items.Count > 0)
                        {
                            listBoxMap.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        //停留在当前数据上

                        //图层名定位
                        if ((iLayNameIndex >= 0) && (iLayNameIndex < listBoxMap.Items.Count))
                        {
                            listBoxMap.SelectedIndex = iLayNameIndex;
                        }
                        else
                        {
                            if (listBoxMap.Items.Count > 0)
                            {
                                listBoxMap.SelectedIndex = 0;
                            }
                        }
                    }
                }
                else
                {
                    //索引定位
                    if ((k >= 0) && (k < listBoxMap.Items.Count))
                    {
                        listBoxMap.SelectedIndex = k;
                    }
                }
            }
            ShowStatusInfo("数据文件列表载入完毕");
        }
        #endregion

        //==========================================================================================
        #region 地图控件鼠标事件

        private Label labMapXYMsg;// = new Label();
        private PointF pfMousePxy = new PointF();
        private PointF pfMousePjw = new PointF();

        private void OnMapMouseMove(object sender, MouseEventArgs e)
        {
            //PointF pt = new PointF(e.X, e.Y);
            pfMousePxy.X = e.X;
            pfMousePxy.Y = e.Y;

            if (null != labMapXYMsg)
            {
                if (!bMouseGetVal)
                {
                    labMapXYMsg.Visible = false;
                    return;
                }
                labMapXYMsg.Top = e.Y + 20;
                labMapXYMsg.Left = e.X + 10;
            }
            MyMap_MouseMove(sender, e);
        }

        private void OnMapMouseEnter(object sender, EventArgs e)
        {
            if (null != labMapXYMsg)
            {
                labMapXYMsg.Visible = bMouseGetVal;// true;
            }
        }

        private void OnMapMouseLeave(object sender, EventArgs e)
        {
            if (null != labMapXYMsg)
            {
                labMapXYMsg.Visible = false;
            }
        }

        #endregion
        //==========================================================================================
        /// <summary>
        /// 切换工具栏按钮状态
        /// </summary>
        /// <param name="iFlag">0=小,1=大</param>
        private void setToolButtonViewType(int iFlag)
        {
            iToolBarTopType = iFlag;
            tsmiTBig.Checked = (iToolBarTopType == 1);
            tsmiTSml.Checked = (iToolBarTopType == 0);

            //###
            this.SuspendLayout();
            foreach (ToolStripItem item in tsToolBar.Items)
            {
                if (item is ToolStripButton)
                {
                    ToolStripButton tsb = item as ToolStripButton;
                    if (tsb.Visible)
                    {
                        if (iFlag == 0)//小无字
                        {
                            tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            tsb.AutoSize = false;
                            tsb.Width = 35;
                            tsb.Height = 35;
                            tsb.Font = this.Font;
                        }
                        else if (iFlag == 1)//大有字
                        {
                            tsb.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            tsb.TextImageRelation = TextImageRelation.ImageAboveText;
                            tsb.AutoSize = false;
                            tsb.Width = 50;
                            tsb.Height = 50;
                            tsb.Font = this.Font;
                        }
                        else if (iFlag == 2)//大无字
                        {
                            tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            //tsb.TextImageRelation = TextImageRelation.ImageAboveText;
                            tsb.AutoSize = false;
                            tsb.Width = 50;
                            tsb.Height = 50;
                            tsb.Font = this.Font;
                        }
                    }
                }
            }
            this.ResumeLayout();
            this.PerformLayout();
        }
        //==========================================================================================
        public MainForm()
        {
            //splash.Show();//yym
            InitializeComponent();

            //mylog.Info("系统启动");

            PubUnit.sPalletPathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "pallet" + Path.DirectorySeparatorChar.ToString() + "I-01.pal";

            PluginManager PlugMg = PluginManager.getInstance(this);

            PlugMg.OnLoadAPlugin += new PluginManager.LoadPlugEventHandler(PlugMg_OnLoad);
            PlugMg.LoadAll();
            PlugMg.FuncPlugTypeList.Sort(new PluginCompare());

            //###
            //PluginManager pm
            pm = PluginManager.getInstance();
            userIMap = new InitMap(pm);
            userIMap.OnLoad();

            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            //iniBaseMapList();
            //PluginManager pm = PluginManager.getInstance();
            //baseMap = (Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)pm.FindControl("Map");
            //if (null != baseMap)
            //{                 
            //    baseMap.Render();                
            //}

            Application.Idle += new EventHandler(Application_Idle);

            //mylog.Info("启动完成");

            //InitUCMapFileList();

            WRFileUnit.testSysFolderPath();
            userSysParams = WRFileUnit.loadSysParams();
            WRFileUnit.saveSysParams(userSysParams);
            setStyleFont();

            bMouseLablTM = userSysParams.MouseLablTM;
            bMouseGetVal = userSysParams.MouseGetVal;

            //20140118 右下的隐藏图层列表
            tabLeftBottom.Visible = false;
            scLeft.SplitterDistance = tabControl1.Height;

        }

        /// <summary>
        /// 自动删除过期数据
        /// </summary>
        private int autoDelMapData()
        {
            int iRet = 0;
            //userSysParams.NearMayDays = 3;
            if (userSysParams.AutoDelMapData)
            {
                #region 原代码,不用

                //string path = userSysParams.sMapPath;
                //foreach (DirectoryInfo dirinfo in new DirectoryInfo(path).GetDirectories())
                //{
                //    //addLog(dirinfo.Attributes.ToString());//='Directory'
                //    FileInfo[] fiMaps = dirinfo.GetFiles();
                //    //foreach (FileInfo fi in dirinfo.GetFiles())
                //    foreach (FileInfo fi in fiMaps)
                //    {
                //        //string sShowName = fi.Name;//解析文件名
                //        string sPathName = fi.FullName;
                //        //DateTime dtCreat = File.GetCreationTime(sPathName);
                //        DateTime dtCreat = File.GetLastWriteTime(sPathName);
                //        if (dtCreat.AddDays(userSysParams.NearMayDays) < DateTime.Now)
                //        {
                //            //删除
                //            //Console.WriteLine(sPathName);
                //            try
                //            {
                //                File.Delete(sPathName);
                //                iRet++;
                //                WRFileUnit.writeLogStr("删除成功: [" + dtCreat.ToString() + "] -> " + sPathName);
                //            }
                //            catch
                //            {
                //                WRFileUnit.writeLogStr("x删除失败: [" + dtCreat.ToString() + "] -> " + sPathName);
                //            }
                //        }
                //    }
                //}

                #endregion

                //2014-10-26,wm add
                #region 删除
                for (int k = listBoxMap.Items.Count - 1; k >= 0; k--)
                {
                    CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                    DateTime dt = sfi.dtBegin;
                    if (dt.AddDays(userSysParams.NearMayDays) >= DateTime.Now)//期限之内
                    {
                        break;
                    }
                    else//过期地图文件
                    {
                        bool h_flag = HaveFYProduct(k);//判断是否有反演产品存在
                        bool l_flag = HaveLogicFile(k);//判断是否有逻辑减产品存在
                        if (h_flag || l_flag)
                        {
                            continue;
                        }
                        else
                        {
                            #region 删除云图
                            try
                            {
                                string sLayName = sfi.ToString();
                                List<string> lstMaps = new List<string>();

                                #region 保存文件列表
                                if (sfi.iStructType == 5)//MTSAT
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 11));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 12));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 13));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 14));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 10));
                                    }
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 11));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 12));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 13));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 14));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 10));
                                }
                                else//FY
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 5));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 6));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 7));
                                    }
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 5));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 6));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 7));
                                }
                                #endregion

                                for (int i = 0; i < lstMaps.Count; i++)
                                {
                                    string sPathName = lstMaps[i];
                                    if (File.Exists(sPathName))
                                    {
                                        File.Delete(sPathName);
                                        iRet++;
                                    }
                                }

                                ////移除图层列表
                                //bool bLayerChanged = false;
                                //for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                                //{
                                //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                //    if (!lay.IsBaseMap)
                                //    {
                                //        baseMap.WMap.LayerList.RemoveAt(i);
                                //        bLayerChanged = true;
                                //    }
                                //}

                                ////刷新文件列表
                                //FindLoadFYMapFile();
                                //listMapLays();

                                //if (bLayerChanged)
                                //{
                                //    baseMap.Render();
                                //}
                            }
                            catch (Exception ex)
                            {
                                string ss = ex.ToString();
                            }
                            #endregion

                            listFYYT.Remove(sfi);//2014-12-27 wm add
                        }
                    }
                }

                //刷新文件列表
                //FindLoadFYMapFile();
                //listMapLays();

                #endregion
            }
            return iRet;
        }

        /// <summary>
        /// 检测数据处理服务程序
        /// </summary>
        private void autoTestRunData()
        {
            IntPtr iHandle = FindWindow(null, "SatelliteDataMaker");
            if (iHandle == IntPtr.Zero)
            {
                string sDataMakerExe = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteDataMaker.exe";
                //sDataMakerExe = @"D:\0SPapp\0 MicapsApp\0 201310\1025 app\SatelliteDataMaker\SatelliteDataMaker\bin\Debug\SatelliteDataMaker.exe";
                if (File.Exists(sDataMakerExe))
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(sDataMakerExe);
                }
            }
            else
            {
                //发送同步消息
                PostMessage(iHandle, 4457, 1, 1);

                iTBMessageFY++;

                //超时杀死进程
                if (iTBMessageFY >= 3)
                {
                    //MessageBox.Show("Kill SatelliteDataMaker");
                    KillDealProcess("SatelliteDataMaker");
                    iTBMessageFY = 0;
                }

                //iTBMessageFY++;
            }
            //=================================================================================
            iHandle = FindWindow(null, "SatelliteDataMakerMT");
            if (iHandle == IntPtr.Zero)
            {
                string sDataMakerExe = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteDataMakerMT.exe";
                if (File.Exists(sDataMakerExe))
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(sDataMakerExe);
                }
            }
            else
            {
                //发送同步消息
                PostMessage(iHandle, 4457, 1, 1);

                iTBMessageMTSAT++;

                //超时杀死进程
                if (iTBMessageMTSAT >= 3)
                {
                    //MessageBox.Show("Kill SatelliteDataMakerMT");
                    KillDealProcess("SatelliteDataMakerMT");
                    iTBMessageMTSAT = 0;
                }

                //iTBMessageMTSAT++;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            loadAirspaceSet();
            setToolButtonViewType(1);

            //splash.lbMessage.Text = "程序启动完成...欢迎使用";
            //splash.DelayClose(1000);
            //splash.DelayClose(3000);

            baseMap = (Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)pm.FindControl("Map");
            //loadAllExistedAirspace();
            if (baseMap != null)
            {
                //baseMap.Render();//yym 2013-8-9 del 原来为解决初次启动不显示地图问题，现在注掉

                //yym 2013-7-23 add
                baseMap.WMap.OnMapRendred += new Bexa.Guojf.FreeMicaps.WeatherMap.MapRenderHandler(WMap_OnMapRendre);

                if (null == labMapXYMsg)
                {
                    labMapXYMsg = new Label();
                    labMapXYMsg.Parent = baseMap;
                    labMapXYMsg.AutoSize = true;
                    labMapXYMsg.Text = "";// "经纬=\n温度=\n高度=";

                    //labMapXYMsg.Font = new Font("宋体", 10);
                    try
                    {
                        Font fnt = new Font(userSysParams.MouseFontName, userSysParams.MouseFontSize, userSysParams.MouseFontFS);
                        labMapXYMsg.Font = fnt;
                    }
                    catch (Exception)
                    {
                        labMapXYMsg.Font = new Font("宋体", 10);
                    }
                    labMapXYMsg.ForeColor = Color.FromArgb(userSysParams.MouseFontColorVal);

                    //labMapXYMsg.BackColor = Color.Transparent;

                    //20140515 提取鼠标设置到系统配置                    
                    //bMouseLablTM = userSysParams.MouseLablTM;
                    if (bMouseLablTM)
                    {
                        labMapXYMsg.BackColor = Color.Transparent;
                    }
                    else
                    {
                        labMapXYMsg.BackColor = SystemColors.Control;
                    }

                    baseMap.Controls.Add(labMapXYMsg);
                }

                baseMap.MouseEnter += new System.EventHandler(OnMapMouseEnter);
                baseMap.MouseLeave += new System.EventHandler(OnMapMouseLeave);

                baseMap.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMapMouseMove);
                baseMap.MouseMove += new System.Windows.Forms.MouseEventHandler(MyMap_MouseMove);
            }

            string scfg = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "maplist.cfg";
            listBox2.Visible = File.Exists(scfg);

            //打印功能
            _PrintDoc.DefaultPageSettings.Landscape = true;
            _PrintDoc.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
            _PrintDoc.DocumentName = "SatellitePower";
            _PrintDoc.PrintPage += new PrintPageEventHandler(ptDoc_PrintPage);

            comboBox1.SelectedIndex = 13;// 12;

            goUserViewCenter();

            #region 检测数据处理服务程序

            //IntPtr iHandle = FindWindow(null, "SatelliteDataMaker");
            //if (iHandle == IntPtr.Zero)
            //{
            //    string sDataMakerExe = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteDataMaker.exe";
            //    //sDataMakerExe = @"D:\0SPapp\0 MicapsApp\0 201310\1025 app\SatelliteDataMaker\SatelliteDataMaker\bin\Debug\SatelliteDataMaker.exe";
            //    if (File.Exists(sDataMakerExe))
            //    {
            //        System.Diagnostics.Process p = System.Diagnostics.Process.Start(sDataMakerExe);
            //    }
            //}

            //iHandle = FindWindow(null, "SatelliteDataMakerMT");
            //if (iHandle == IntPtr.Zero)
            //{
            //    string sDataMakerExe = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteDataMakerMT.exe";
            //    if (File.Exists(sDataMakerExe))
            //    {
            //        System.Diagnostics.Process p = System.Diagnostics.Process.Start(sDataMakerExe);
            //    }
            //}

            #endregion

            autoTestRunData();
            if (userSysParams.AutoTestMTSAT)
            {
                timer2.Interval = 5 * 60 * 1000;//5分钟改1分钟，2014-11-7 wm add
                //timer2.Interval = 10 * 1000;//10秒钟
                timer2.Enabled = true;
            }

            //autoDelMapData();

            //userStyle.TitleColor = Color.Green;
            if (nowWorkType == EWorkType.FYMap)
            {
                //tabPage1.Text = "FY 数据文件";
                init_flag = true;//2014-10-28 wm add
                listFYYT = new List<CSivFileInfo>();//2014-10-28 wm add

                FindLoadFYMapFile(1);
                if (listBoxMap.Items.Count > 0)
                {
                    listBoxMap.SelectedIndex = 0;//自动引发：SelectedIndexChanged(sender, e)                    
                }

            }
            else if (nowWorkType == EWorkType.MTSATSec)
            {
                //tabPage1.Text = "MTSAT 数据文件";
                //FindLoadSECMapFile();
            }

            FindLoadYJFFileList();
            FindLoadLeiBaoFileList();   //2014-8-17 add
            FindLoadJBFileList();   //2014-10-20 add
            FindLoadHaiWuFileList();    //2014-10-20 add
            FindLoadDZXFileList();//2014-10-31
            FindLoadLogicFileList(); //2014-7-22 add
            //WRFileUnit.readListBoxFromFile(lBoxDZX, "mapDZX.dat");
            //WRFileUnit.readListBoxFromFile(lBoxJB, "mapJB.dat");
            //WRFileUnit.readListBoxFromFile(lBoxHW, "mapHW.dat");

            autoDelMapData();//2014-10-27,wm add
            FindLoadFYMapFile();

            listMapLays();

            //自动检测删除地图数据
            timer3.Interval = 60 * 60 * 1000;//1小时
            timer3.Enabled = userSysParams.AutoDelMapData;

            loadLayerCWSet();

            //2014-08
            timer4.Interval = 5 * 60 * 1000;//5分钟
            //timer4.Interval = 5 * 1000;//debug
            timer4.Enabled = userSysParams.AutoSaveImg;

            init_flag = false;//2014-10-28 wm add
            if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg"))
                File.Create(Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg");

        }
        public void loadAirspaceSet()//added by ZJB 20150425
        {
            if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg"))
                File.Create(Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg");

            //20150503,wm add
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked"))
                Directory.CreateDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked");

            #region 删除所有airspace意外丢失，但是仍然存在与checked文件夹的文件
            try
            {
                string[] files = Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked");
                string name;
                foreach (string aname in files)
                {
                    name = aname.ToLower().Replace(".asd", "");
                    name = Path.GetFileNameWithoutExtension(name);
                    string path = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + name + ".ASD";
                    //MessageBox.Show(path);
                    if (!File.Exists(path))
                    {
                        //MessageBox.Show("正在删除文件：" + path);
                        File.Delete(aname);
                    }

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            #endregion
        }

        /// <summary>
        /// 分段求高度
        /// </summary>
        /// <param name="sWD"></param>
        /// <returns></returns>
        private string getYDGMeter3Par(string sWD)
        {
            if (sWD.Equals("0.00"))
            {
                return "";
            }

            string sRet = "";
            //float 0.00
            float fVal = 0.0f;
            float.TryParse(sWD, out fVal);

            if (fVal <= userSysParams.YDG_LowVal)//小于低空温度
            {
                // <=0
                float fValH = userSysParams.YDG_HighVal - userSysParams.YDG_LowVal;
                if (fValH == 0)
                {
                    return "-";
                }

                try
                {
                    float fNowVal = (fVal - userSysParams.YDG_LowVal) * (userSysParams.YDG_HighMeter - userSysParams.YDG_LowMeter) / fValH
                                    + userSysParams.YDG_LowMeter;

                    fVal = fNowVal;
                    if (fVal <= 0)
                    {
                        sRet = "地面";
                    }
                    else
                    {
                        sRet = String.Format("{0:0}", fVal) + " 米";
                    }
                }
                catch
                {
                    sRet = "-";
                }
            }
            else//大于低空温度
            {
                // > 0
                float fValH = userSysParams.YDG_LowVal - userSysParams.YDG_FloorVal;
                if (fValH == 0)
                {
                    return "-";
                }

                try
                {
                    float fNowVal = (fVal - userSysParams.YDG_FloorVal) * (userSysParams.YDG_LowMeter - userSysParams.YDG_FloorMeter) / fValH
                                    + userSysParams.YDG_FloorMeter;

                    fVal = fNowVal;
                    if (fVal <= 0)
                    {
                        sRet = "地面";
                    }
                    else
                    {
                        sRet = String.Format("{0:0}", fVal) + " 米";
                    }
                }
                catch
                {
                    sRet = "-";
                }
            }

            return sRet;
        }

        private string getYDGMeter(string sWD)
        {
            if (sWD.Equals("0.00"))
            {
                return "";
            }

            string sRet = "";
            //float 0.00
            float fVal = 0.0f;
            float.TryParse(sWD, out fVal);

            float fValH = userSysParams.YDG_HighVal - userSysParams.YDG_LowVal;//温度差
            if (fValH == 0)
            {
                return "-";
            }

            try
            {
                //float fNowVal = fVal * (userSysParams.YDG_HighMeter - userSysParams.YDG_LowMeter) / (-40) + userSysParams.YDG_LowMeter;

                //float fNowVal = (fVal - userSysParams.YDG_LowVal) * (userSysParams.YDG_HighMeter - userSysParams.YDG_LowMeter) / (userSysParams.YDG_HighVal - userSysParams.YDG_LowVal)
                float fNowVal = (fVal - userSysParams.YDG_LowVal) * (userSysParams.YDG_HighMeter - userSysParams.YDG_LowMeter) / fValH
                                + userSysParams.YDG_LowMeter;

                fVal = fNowVal;
                if (fVal <= 0)
                {
                    sRet = "地面";
                }
                else
                {
                    //sRet = String.Format("{0:0.00}", fVal);
                    sRet = String.Format("{0:0}", fVal) + " 米";
                }
            }
            catch
            {
                sRet = "-";
            }

            return sRet;
        }

        private void MyMap_MouseMove(object sender, MouseEventArgs e)
        {
            pfMousePxy.X = e.X;
            pfMousePxy.Y = e.Y;

            //if (null != labMapXYMsg)
            //{
            //    if (!bMouseGetVal)
            //    {
            //        labMapXYMsg.Visible = false;
            //        return;
            //    }
            //    labMapXYMsg.Top = e.Y + 20;
            //    labMapXYMsg.Left = e.X + 10;
            //}
            //-----------------------------------------------------
            //StatusStrip ssMainStatus = (StatusStrip)pm.FindControl("ssMainStatus");
            //if (ssMainStatus != null)
            //{
            PointF pt = new PointF(e.X, e.Y);
            pt = baseMap.WMap.Coord.ScreenToWorld(pt);

            //ToolStripItem tsi = ssMainStatus.Items["stlbRightMessage"];
            //tsi.Text = string.Format("经度: {0:f2}, 纬度: {1:f2}  缩放系数: {2:f1}", pt.X, pt.Y, Map.WMap.Coord.ZoomScale);

            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    if (lay.Visible == true)
                    {
                        //获取定标值
                        string ss = lay.GetType().ToString();
                        if (ss.Equals("Bexa.Guojf.FreeMicaps.Layers.LayerVISS"))
                        {
                            string sName = lay.Name;

                            //if (sName.Equals(sNowSelLayName))//2014-11-5 wm del
                            {
                                string sJW = "";
                                if (bMouseGetVal)
                                {
                                    if (null != baseMap)
                                    {
                                        pfMousePjw = baseMap.WMap.Coord.ScreenToWorld(pfMousePxy);

                                        //2015-1-22，wm add
                                        //经纬整数部分
                                        int jd_int0, wd_int0;
                                        //经纬小数部分
                                        int jd_f0, wd_f0;
                                        jd_int0 = (int)Math.Floor((double)pfMousePjw.X);
                                        wd_int0 = (int)Math.Floor((double)pfMousePjw.Y);
                                        jd_f0 = (int)Math.Floor((pfMousePjw.X - jd_int0) * 60);
                                        wd_f0 = (int)Math.Floor((pfMousePjw.Y - wd_int0) * 60);
                                        sJW = string.Format("经度: {0}度{1}分\n纬度: {2}度{3}分\n", jd_int0, jd_f0, wd_int0, wd_f0);
                                    }
                                }

                                string sVal = lay.GetStrValByXY(pt.X, pt.Y);
                                #region 亮温或反照率
                                //1=IR,2=VIS
                                if (iNowType == 1)
                                {
                                    //IR
                                    strT = "  亮温: " + sVal + " ℃";
                                    //tslabMsg.Invalidate();
                                    //tslabMsg.Text = "  亮温: " + sVal + " ℃";
                                    //tslabMsg.Invalidate();

                                    if (sVal.Equals("0.00"))
                                    {
                                        sVal = "";
                                    }
                                    else
                                    {
                                        string sGD = getYDGMeter3Par(sVal);
                                        sVal = "亮温: " + sVal + " ℃\n高度: " + sGD;
                                    }

                                }
                                else
                                {
                                    //VIS
                                    float fVal = 0.0f;
                                    float.TryParse(sVal, out fVal);
                                    strT = "  反照率: " + sVal;
                                    //tslabMsg.Invalidate();
                                    //tslabMsg.Text = "  反照率: " + sVal;
                                    //tslabMsg.Invalidate();

                                    if (fVal <= 0)
                                    {
                                        sVal = "";
                                    }
                                    else
                                    {
                                        sVal = "反照率: " + sVal;
                                    }
                                }

                                mainThread = new updateDelegate(updateBar);
                                //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
                                if (updateStatusBarThread != null)
                                {
                                    updateStatusBarThread.Abort();
                                }
                                updateStatusBarThread = new Thread(new ThreadStart(update));
                                //启动线程，启动之后线程才开始执行
                                updateStatusBarThread.Start();

                                #endregion
                                if (bMouseGetVal)
                                {
                                    if (null != labMapXYMsg)
                                    {
                                        //labMapXYMsg.Text = "经纬=\n温度=\n高度=";
                                        strT2 = sJW + sVal;
                                        mainThread2 = new updateDelegate2(updateBar2);
                                        //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
                                        if (updateFollowingThread != null)
                                        {
                                            updateFollowingThread.Abort();
                                        }
                                        updateFollowingThread = new Thread(new ThreadStart(update2));
                                        //启动线程，启动之后线程才开始执行
                                        updateFollowingThread.Start();
                                    }
                                }
                                break;
                            }
                        }
                        else if (ss.Equals("Bexa.Guojf.FreeMicaps.Layers.LayerSEC"))
                        {
                            string sName = lay.Name;
                            //if (sName.Equals(sNowSelLayName))//2014-11-5 wm del
                            {
                                //iNowType = 1;    //1=IR,2=VIS
                                if (iNowType == 1)
                                {
                                    //IR 亮温
                                    strT = " 亮温 = " + lay.GetStrValByXY(pt.X, pt.Y);
                                }
                                else
                                {
                                    //VIS 反照率
                                    strT = " 反照率 = " + lay.GetStrValByXY(pt.X, pt.Y);
                                }
                                mainThread = new updateDelegate(updateBar);
                                //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
                                if (updateStatusBarThread != null)
                                {
                                    updateStatusBarThread.Abort();
                                }
                                updateStatusBarThread = new Thread(new ThreadStart(update));
                                //启动线程，启动之后线程才开始执行
                                updateStatusBarThread.Start();
                                break;
                            }
                        }
                        Application.DoEvents();
                    }
                }
            }

        }

        //======================================================================================================================
        //下面部分用于设置底部状态条中的亮温显示部分
        public void update()
        {
            updateBar(strT);
        }
        //声明一个delegate（委托）类型：updateDelegate，该类型可以搭载返回值为空，参数只有一个(long型)的方法。
        public delegate void updateDelegate(string str);
        //声明一个updateDelegate类型的对象。该对象代表了返回值为空，参数只有一个(long型)的方法。它可以搭载N个方法。
        public updateDelegate mainThread;

        public void updateBar(string str)
        {
            try
            {
                //底部状态栏
                tslabMsg.Invalidate();
                tslabMsg.Text = str;
                tslabMsg.Invalidate();
            }
            catch (System.Exception ex)
            { }
        }
        //======================================================================================================================

        //======================================================================================================================
        //下面部分用于设置鼠标跟随显示部分
        public void update2()
        {
            updateBar2(strT2);
        }
        //声明一个delegate（委托）类型：updateDelegate，该类型可以搭载返回值为空，参数只有一个(long型)的方法。
        public delegate void updateDelegate2(string str);
        //声明一个updateDelegate类型的对象。该对象代表了返回值为空，参数只有一个(long型)的方法。它可以搭载N个方法。
        public updateDelegate2 mainThread2;

        public void updateBar2(string str)
        {
            try
            {
                //鼠标跟随部分显示
                labMapXYMsg.Refresh();
                labMapXYMsg.Text = str;
                labMapXYMsg.Refresh();
            }
            catch (System.Exception ex)
            { }
        }
        //======================================================================================================================

        private void ucTrackBar_VisibleChanged(object sender, EventArgs e)
        {
            //闸值工具条拖动结束事件            
            int k = 0;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                if (!lay.IsBaseMap)
                {
                    string ss = lay.GetType().ToString();
                    if (ss.Equals("Bexa.Guojf.FreeMicaps.Layers.LayerVISS"))
                    {
                        //if (lay is Bexa.Guojf.FreeMicaps.Layers.LayerVISS)
                        //{
                        //    WeatherFileLayer wfl = lay as WeatherFileLayer;
                        //    //LayerVISS lvis = wfl as LayerVISS;

                        //}

                        //ss = lay.Style.GetType().ToString();
                        //if (ss.Equals("Bexa.Guojf.FreeMicaps.Layers.VISSStyle"))
                        //{
                        //    Style sty = lay.Style as Style;
                        //    IVISSStyle vs = lay.Style as IVISSStyle;
                        //    //vs.ShowMaxVal = ucTrackBar1.iValue1;
                        //    //vs.ShowMinVal = ucTrackBar1.iValue2;
                        //    k++;
                        //}
                        lay.Style.ShowMaxVal = ucTrackBar1.iValue1;
                        lay.Style.ShowMinVal = ucTrackBar1.iValue2;
                        k++;
                    }
                    else if (ss.Equals("Bexa.Guojf.FreeMicaps.Layers.LayerSEC"))
                    {
                        lay.Style.ShowMaxVal = ucTrackBar1.iValue1;
                        lay.Style.ShowMinVal = ucTrackBar1.iValue2;
                        k++;
                    }
                }
            }

            if (k > 0)
            {
                baseMap.Render();
            }
        }

        private void WMap_OnMapRendre(object sender, EventArgs e)
        {
            //渲染完毕事件
            //if (e is Bexa.Guojf.FreeMicaps.WeatherMap.MapRenderArgs)
            //{
            //    Bexa.Guojf.FreeMicaps.WeatherMap.MapRenderArgs mra = e as Bexa.Guojf.FreeMicaps.WeatherMap.MapRenderArgs;
            //}

            //用于批量生成图片:1=生成动画，2=自动批量保存图像
            if (iExportImgFlag == 1)//1=生成动画
            {
                #region 生成动画

                System.Drawing.Imaging.ImageFormat imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg;
                //baseMap.WMap.SaveToImage(sExportImgPaht + iExportImgCount + "_" + sExportImgFileName + ".act", imgFmt);
                baseMap.WMap.SaveToImage(sExportImgPaht + iExportImgCount + "_" + sExportImgFileName + ".jpg", imgFmt);

                if (iExportImgCount >= listExportLayNames.Count)
                {
                    iExportImgFlag = 0;
                    tspBar1.Value = 0;
                    tspBar1.Visible = false;
                    ShowStatusInfo("生成完毕");

                    //调出动画查看程序
                    //tsmViewAct_Click(sender, e);
                    if (tabControl1.SelectedIndex != 1)
                    {
                        tabControl1.SelectedIndex = 1;
                    }
                }
                else
                {
                    //----------------------------------------------------------
                    for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            lay.Visible = false;
                        }
                    }
                    //----------------------------------------------------------
                    string sNowFrame = listExportLayNames[iExportImgIndex];//当前帧
                    //----------------------------------------------------------                    
                    for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            //string fn = ((IWeatherLayer)lay).Name;
                            string fn = lay.Name;
                            if (fn.Equals(sNowFrame))
                            {
                                sExportImgFileName = fn;
                                iExportImgCount++;
                                iExportImgIndex++;
                                SetProgressBarVal(iExportImgCount);
                                ShowStatusInfo("正在生成第 " + iExportImgCount + " 帧");

                                Application.DoEvents();

                                lay.Visible = true;
                                baseMap.Render();
                                break;
                            }
                        }
                    }
                }

                #endregion
            }//if (iExportImgFlag == 1)
            else if (iExportImgFlag == 2)//2=自动批量保存图像
            {
                #region 自动批量保存图像

                //userSysParams.AutoSaveImgPath + "图层名";
                string sOutPathName = sExportImgPaht + sExportImgFileName + ".jpg";
                System.Drawing.Imaging.ImageFormat imgFmt = System.Drawing.Imaging.ImageFormat.Jpeg;

                baseMap.WMap.SaveToImage(sOutPathName, imgFmt);//保存图片

                if (iExportImgCount >= listExportLayNames.Count)
                {//全部生成完毕
                    iExportImgFlag = 0;
                    tspBar1.Value = 0;
                    tspBar1.Visible = false;
                    ShowStatusInfo("生成完毕");
                }
                else
                {//生成下一幅，此时索引已指向下一条，count已加一
                    //----------------------------------------------------------
                    string sNowFrame = listExportLayNames[iExportImgIndex];//当前帧
                    //----------------------------------------------------------                    
                    for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            lay.Visible = false;//隐藏所有非底图图层，20150711 wm add

                            //string fn = ((IWeatherLayer)lay).Name;
                            string fn = lay.Name;
                            if (fn.Equals(sNowFrame))
                            {
                                sExportImgFileName = fn;
                                iExportImgCount++;
                                iExportImgIndex++;
                                SetProgressBarVal(iExportImgCount);
                                ShowStatusInfo("正在生成第 " + iExportImgCount + " 幅");

                                Application.DoEvents();

                                lay.Visible = true;//仅显示云图图层，20150711 wm add

                                baseMap.Render();
                                break;
                            }
                        }
                    }
                }

                #endregion
            }
            else if (iExportImgFlag == 3)//3=云图区域对比生成第二云图
            {
                iExportImgFlag = 0;

                if (null != baseMap.WMap.BufferBmp)
                {
                    this.bmpRect2 = PubUnit.copyBitmapRect(baseMap.WMap.BufferBmp, pRectLT, pRectRB);

                    frmFXRect fFXRect = new frmFXRect();
                    fFXRect.bmpRect1 = this.bmpRect1;
                    fFXRect.bmpRect2 = this.bmpRect2;
                    fFXRect.ShowDialog();
                }
            }

            if (tspBar1.Visible)
            {
                tspBar1.Value = 0;
                tspBar1.Visible = false;
                ShowStatusInfo("绘图完毕");
            }
            iWorkingState = 0;//0=空闲，1=解析生数据中，2=动画生成中
        }

        //------------------------------------------------------------------------------------------------------
        private void muExit_Click(object sender, EventArgs e)
        {
            #region 关闭时保存反演云图文件列表，2014-10-27 wm add
            //try
            //{
            //    ShowStatusInfo("正在保存缓存文件，请稍候...");
            //    ListBox temp = new ListBox();
            //    temp.Items.Clear();
            //    for (int i = 0; i < listFYYT.Count; i++)
            //    {
            //        if (listFYYT[i] != null)
            //        {
            //            temp.Items.Add(listFYYT[i]);
            //        }
            //    }
            //    int count = temp.Items.Count;
            //    if (!File.Exists(listFYYT_path))
            //    {
            //        File.Create(listFYYT_path);
            //    }
            //    StreamWriter sw = new StreamWriter(listFYYT_path);
            //    for (int i = 0; i < count; i++)
            //    {
            //        string data = temp.Items[i].ToString();
            //        sw.Write(data);
            //        sw.WriteLine();
            //    }
            //    sw.Flush();
            //    sw.Close();
            //    //WRFileUnit.saveListBoxToFile(temp, "listFYYT.dat");
            //    temp.Dispose();
            //    ShowStatusInfo("缓存文件已保存");
            //}
            //catch (IOException ex)
            //{
            //    string ss = ex.ToString();
            //    MessageBox.Show(ss);
            //}
            #endregion
            this.Close();
        }

        private void tbPluginManage_Click(object sender, EventArgs e)
        {
            //PluginManager.getInstance().ShowListDlg();
        }

        private void tsmiJSB_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("NOTEPAD.EXE");
        }

        private void tsmiHB_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("mspaint.exe");
        }

        private void tsmiJSQ_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("calc.exe");
        }

        private void tsmiBaidu_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("http://www.baidu.com");
        }

        private void tsmViewAct_Click(object sender, EventArgs e)
        {
            //string sPathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteImage.exe";
            //if (File.Exists(sPathName))
            //{
            //    System.Diagnostics.Process p = System.Diagnostics.Process.Start(sPathName);
            //}
        }

        private void tsmViewTool_Click(object sender, EventArgs e)
        {
            string sPathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "SatelliteVissr.exe";
            if (File.Exists(sPathName))
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(sPathName);
            }
        }

        //------------------------------------------------------------------------------------------------------

        #region 导出动画文件

        public int iExportImgFlag = 0;  //导出进行标志，1=生成动画，2=自动批量保存图像，3=云图区域对比生成第二云图
        public int iExportImgCount = 0; //当前导出个数        
        public int iExportImgIndex = 0; //当前导出的图层
        public string sExportImgPaht = "";      //导出路径
        public string sExportImgFileName = "";  //导出文件名
        public List<string> listExportLayNames = new List<string>();//记录要导出的图层文件名

        //public int outSaveAllLayerImage(string sPath, string sDirFolder)
        public int outSaveAllLayerImage()
        {
            if (listExportLayNames.Count <= 0)
            {
                return 0;
            }

            int iRet = 0;
            //-------------------------------------------------
            //if (!string.IsNullOrEmpty(sDirFolder))
            //{
            //    PubUnit.safetyCreateDir(sPath + sDirFolder);
            //}
            //safetyCreateDir(sPath + "IR1");
            //-------------------------------------------------
            //string sFileName = Path.GetFileNameWithoutExtension(sRAWFile);
            //-------------------------------------------------------------------
            iExportImgFlag = 1;//开始标志，1=生成动画，2=自动批量保存图像
            iExportImgCount = 0;

            //限制单一通道            
            //----------------------------------------------------------
            ////iNowType = 1;//1=IR,2=VIS
            ////iNowWay = 0;//0-3:1~4 通道
            //string sTypeWay = "";
            //if (iNowType == 1)
            //{
            //    //iWay;//0-3:1~4 通道
            //    sTypeWay = "IR" + (iNowWay + 1).ToString();
            //}
            //else if (iNowType == 2)
            //{
            //    sTypeWay = "VIS" + (iNowWay + 1).ToString();
            //}
            ////----------------------------------------------------------
            //tspBar1.Maximum = listExportLayNames.Count;
            tspBar1.Visible = true;

            iExportImgIndex = 0;
            string sFirst = listExportLayNames[iExportImgIndex];

            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    //string fn = ((IWeatherLayer)lay).Name;
                    string fn = lay.Name;
                    if (fn.Equals(sFirst))
                    {
                        sExportImgFileName = fn;
                        iExportImgCount++;
                        iExportImgIndex++;
                        SetProgressBarVal(iExportImgCount);
                        ShowStatusInfo("正在生成第 " + iExportImgCount + " 帧");

                        Application.DoEvents();

                        lay.Visible = true;
                        baseMap.Render();
                        break;
                    }

                }
            }
            //-------------------------------------------------------------------
            iRet = iExportImgIndex;
            return iRet;
        }

        private void tsbAnimation_Click(object sender, EventArgs e)
        {
            //生成动画
            //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            //{
            //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
            //    if (!lay.IsBaseMap)
            //    {
            //        //lay.Visible = false;
            //        iExportImgAll++;
            //    }
            //}

            //if (iExportImgAll <= 0)
            //{
            //    MessageBox.Show("没有地图文件可供生成动画！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //folderBrowserDialog1.Description = "请选择输出目录";
            //if (Convert.ToInt32(folderBrowserDialog1.Tag) == 0)
            //{
            //    folderBrowserDialog1.SelectedPath = sysCfg.sActPath;// Application.StartupPath;
            //}

            //if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string sTime = DateTime.Now.ToString("yyMMdd_HHmmss");
            //    folderBrowserDialog1.Tag = 1;
            //    string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();
            //    sExportImgPaht = sPath + "Act" + sTime + Path.DirectorySeparatorChar.ToString();

            //    if (MessageBox.Show("即将生成动画文件，请耐心等待，不要进行其他操作。", "生成动画", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
            //        == System.Windows.Forms.DialogResult.OK)
            //    {
            //        outSaveAllLayerImage(sPath, "Act" + sTime + Path.DirectorySeparatorChar.ToString());
            //    }

            //}
        }

        #endregion

        private void muAbout_Click(object sender, EventArgs e)
        {
            (new Splash()).ShowDialog();

            //Splash splash = new Splash();
            //splash.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region 关闭时保存反演云图文件列表，2014-10-27 wm add
            //try
            //{
            //    ShowStatusInfo("正在保存缓存文件，请稍候...");
            //    ListBox temp = new ListBox();
            //    temp.Items.Clear();
            //    for (int i = 0; i < listFYYT.Count; i++)
            //    {
            //        if (listFYYT[i] != null)
            //        {
            //            temp.Items.Add(listFYYT[i]);
            //        }
            //    }
            //    int count = temp.Items.Count;
            //    if (!File.Exists(listFYYT_path))
            //    {
            //        File.Create(listFYYT_path);
            //    }
            //    StreamWriter sw = new StreamWriter(listFYYT_path);
            //    for (int i = 0; i < count; i++)
            //    {
            //        string data = temp.Items[i].ToString();
            //        sw.Write(data);
            //        sw.WriteLine();
            //    }
            //    sw.Flush();
            //    sw.Close();
            //    //WRFileUnit.saveListBoxToFile(temp, "listFYYT.dat");
            //    temp.Dispose();
            //    ShowStatusInfo("缓存文件已保存");
            //}
            //catch (IOException ex)
            //{
            //    string ss = ex.ToString();
            //    MessageBox.Show(ss);
            //}
            #endregion
            e.Cancel = MessageBox.Show("确定退出系统吗?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
        }

        private void tbShowLeft_Click(object sender, EventArgs e)
        {
            scClient.Panel2Collapsed = !scClient.Panel2Collapsed;
        }

        private void tbFillScreen_Click(object sender, EventArgs e)
        {
            bool bLayerChanged = false;
            pm.MainForm.SuspendLayout();
            if (pm.MainForm.MainMenuStrip.Visible)
            {
                //全屏
                scClient.Panel2Collapsed = true;
                pm.MainForm.MainMenuStrip.Visible = false;
                //pm.MainForm.FormBorderStyle = FormBorderStyle.None;
                pm.MainForm.WindowState = FormWindowState.Maximized;
                tsToolBar.Visible = false;
                tsTRight.Visible = false;
                ssMainStatus.Visible = false;

                ////全屏不显示色条
                //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                //{
                //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                //    if (!lay.IsBaseMap)
                //    {
                //        Style tmpS = lay.Style as Style;//备份各层现有属性设置
                //        tmpS.ShowPaletteVitta = false;
                //        lay.Style = tmpS;
                //        bLayerChanged = true;
                //    }
                //}
                baseMap.ContextMenuStrip = contextMenuStrip3;
            }
            else
            {
                //退出全屏
                baseMap.ContextMenuStrip = null;

                scClient.Panel2Collapsed = false;
                pm.MainForm.MainMenuStrip.Visible = true;
                //pm.MainForm.FormBorderStyle = FormBorderStyle.Sizable;
                pm.MainForm.WindowState = FormWindowState.Maximized;//Normal
                //tsToolBar.Visible = true;
                tsToolBar.Visible = !tsmiHiden.Checked;
                //tsTRight.Visible = true;
                ssMainStatus.Visible = true;

                setToolBarBntState(tabControl1.SelectedIndex);//0=地图界面,1=动画界面

                //退出全屏,显示色条
                //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                //{
                //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                //    if (!lay.IsBaseMap)
                //    {
                //        Style tmpS = lay.Style as Style;//备份各层现有属性设置
                //        tmpS.ShowPaletteVitta = true;
                //        lay.Style = tmpS;
                //        bLayerChanged = true;
                //    }
                //}
            }
            pm.MainForm.ResumeLayout();
            pm.MainForm.PerformLayout();

            if (bLayerChanged)
            {
                baseMap.Render();
            }
        }

        private void tbAbout_Click(object sender, EventArgs e)
        {
            Rectangle rect = Screen.GetWorkingArea(this);
            double rate = rect.Width / (double)rect.Height;

            Splash splash = new Splash();
            splash.rate = rate;
            splash.Show(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            baseMap.WMap.CopyToClipboard();

            //if (_BufferBmp != null)
            //{
            //    Clipboard.SetImage(_BufferBmp);
            //}
            //Clipboard.GetImage();
        }

        #region 图表 Chart

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            frmChart1 frmChartmy = new frmChart1();
            //splash.ShowDialog(this);
            frmChartmy.Show(this);

            this.BringToFront();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            FrmChart2 frmChartmy = new FrmChart2();
            frmChartmy.Show(this);

            this.BringToFront();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            FrmChart3 frmChartmy = new FrmChart3();
            frmChartmy.Show(this);

            this.BringToFront();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            frmChart1 frmChartmy = new frmChart1();
            frmChartmy.iflag = 1;
            frmChartmy.sType = "云顶高分析";
            frmChartmy.Show(this);

            this.BringToFront();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            frmChart1 frmChartmy = new frmChart1();
            frmChartmy.iflag = 2;
            frmChartmy.sType = "红外分析";
            frmChartmy.Show(this);

            this.BringToFront();
        }

        #endregion

        private void dlgPallet_OnApply(object sender, EventArgs e)
        {
            //重新载入地图

            //string ss = PubUnit.sPalletPathName;
            //CSysConfig csc = new CSysConfig(ss);
            //csc.PalletPathName = PubUnit.sPalletPathName;
            //string scfg = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "sysconfig.xml";
            //scfg = "f:\\adfafadf.xml";
            //PubUnit.SaveToXml(scfg, csc);

            int k = 0;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                if (!lay.IsBaseMap)
                {
                    lay.Style.PalletPathName = PubUnit.sPalletPathName;
                    k++;
                }
            }

            if (k > 0)
            {
                baseMap.Render();
            }


        }

        private void tsbPallet_Click(object sender, EventArgs e)
        {
            //调色板
            frmPallet dlgPal = new frmPallet();
            dlgPal.OnApply += new EventHandler(dlgPallet_OnApply);
            dlgPal.ShowDialog();
        }

        private int setLayerVisible(string layName, bool bVisible)
        {
            int iRet = 0;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                //if ((lay is IWeatherLayer) && (lay.IsBaseMap))
                if (lay.IsBaseMap || lay.Name == "飞行航线.DAT")//20150324 wm add
                {
                    //string fn = ((IWeatherLayer)lay).Name;
                    string fn = lay.Name;
                    //if (fn.Equals("国界省界.dat"))
                    if (fn.Equals(layName))
                    {
                        if (lay.Visible != bVisible)
                        {
                            lay.Visible = bVisible;
                            //baseMap.Render();
                            iRet++;
                        }
                    }

                }
            }

            //### 刷新地图列表控件的复选框
            if (iRet > 0)
            {
                if (null != userIMap)
                {
                    userIMap.myBaseMapList.FillDataList();
                }
            }
            return iRet;
        }

        private void tsmShowArea_Click(object sender, EventArgs e)
        {
            //### 国界省界
            bool bShowShengJie = !tsmShowArea.Checked;//!(Convert.ToString(tsbAreaGrid.Tag).Equals("1"));//
            if (setLayerVisible("国界省界.dat", bShowShengJie) > 0)
            {
                baseMap.Render();
            }
            //tsbAreaGrid.Tag = (bShowShengJie ? "1" : "0");
            tsmShowArea.Checked = bShowShengJie;
            return;

            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                //if ((lay is IWeatherLayer) && (lay.IsBaseMap))
                if (lay.IsBaseMap || lay.Name == "飞行航线.DAT")//20150324 wm add
                {
                    //string fn = ((IWeatherLayer)lay).Name;
                    string fn = lay.Name;
                    if (fn.Equals("国界省界.dat"))
                    {
                        lay.Visible = !lay.Visible;
                        tsmShowArea.Checked = lay.Visible;
                        //tsbAreaGrid.Checked = lay.Visible;
                        baseMap.Render();
                    }

                }
            }
            //===============================================================================
            //查找地图控件、控制刷新图层复选状态
            //方法1
            //for (int i = 0; i < scLeft.Panel2.Controls.Count; i++)
            //{
            //    Control con = scLeft.Panel2.Controls[i];
            //    if (con is Bexa.Guojf.FreeMicaps.Plugins.BaseMapList)
            //    {
            //        (con as Bexa.Guojf.FreeMicaps.Plugins.BaseMapList).FillDataList();
            //        break;
            //    }
            //}

            //方法2,good
            //Control[] ctrls = this.Controls.Find("myBaseMapList1", true);
            //if (ctrls.Length > 0)
            //{
            //    Control con2 = ctrls[0];
            //    BaseMapList bml = con2 as BaseMapList;
            //    if (bml != null)
            //    {
            //        bml.FillDataList();
            //    }
            //}

        }

        private void tsmShowRiver_Click(object sender, EventArgs e)
        {
            bool bShowShengJie = !tsmShowRiver.Checked;//
            if (setLayerVisible("河流.DAT", bShowShengJie) > 0)
            {
                baseMap.Render();
            }
            tsmShowRiver.Checked = bShowShengJie;
        }
        //===============================================================================
        #region 界面缩放

        /// <summary>
        /// 动画界面缩放
        /// </summary>
        /// <param name="iFlag"></param>
        public void ZoomImage(int iFlag = 1)
        {
            pbxMap.SizeMode = PictureBoxSizeMode.Zoom;
            pbxMap.Dock = DockStyle.None;
            pbxMap.Left = 0;
            pbxMap.Top = 0;

            if (iFlag > 0)
            {
                if (null != pbxMap.Image)
                {
                    fZoomNow = fZoomNow * fZoomScale;
                    int iw = pbxMap.Image.Width;
                    int ih = pbxMap.Image.Height;
                    pbxMap.Width = Convert.ToInt32(iw * fZoomNow);
                    pbxMap.Height = Convert.ToInt32(ih * fZoomNow);
                    //RePaintMap();
                }


                //pictureBox1.Width += iZoomUnit;
                //pictureBox1.Height += iZoomUnit;                
                //pbxMap.Width = Convert.ToInt32(pbxMap.Width * fZoomScale);
                //pbxMap.Height = Convert.ToInt32(pbxMap.Height * fZoomScale);
            }
            else if (iFlag < 0)
            {
                //pictureBox1.Width -= iZoomUnit;
                //pictureBox1.Height -= iZoomUnit;

                //pbxMap.Width = Convert.ToInt32(pbxMap.Width * (1.0f / fZoomScale));
                //pbxMap.Height = Convert.ToInt32(pbxMap.Height * (1.0f / fZoomScale));

                if (null != pbxMap.Image)
                {
                    fZoomNow = fZoomNow * (1.0f / fZoomScale);
                    int iw = pbxMap.Image.Width;
                    int ih = pbxMap.Image.Height;
                    pbxMap.Width = Convert.ToInt32(iw * fZoomNow);
                    pbxMap.Height = Convert.ToInt32(ih * fZoomNow);
                    //RePaintMap();
                }
            }
            else if (iFlag == 0)
            {
                if (null != pbxMap.Image)
                {
                    fZoomNow = 1.0f;
                    int iw = pbxMap.Image.Width;
                    int ih = pbxMap.Image.Height;
                    pbxMap.Width = iw;
                    pbxMap.Height = ih;
                    //RePaintMap();
                }
            }
        }

        private void tsbVBig_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                //动画界面
                ZoomImage(1);
            }
            else
            {
                if (iNowProj == 5)
                {
                    ZoomImage(1);
                }
                else
                {
                    //地图界面
                    float fx = baseMap.Width / 2;
                    float fy = baseMap.Height / 2;
                    //PointF cpf = baseMap.WMap.Coord.Center;
                    PointF cpf = new PointF(fx, fy);

                    baseMap.ZoomPanTool.ZoomAt(cpf, 1.2f);
                    baseMap.Render();

                    //方法2
                    // baseMap.ZoomPanTool.Zoom(1.2f);
                    // baseMap.Render();
                }
            }
        }

        private void tsbVSmall_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                //动画界面
                ZoomImage(-1);
            }
            else
            {
                if (iNowProj == 5)
                {
                    ZoomImage(-1);
                }
                else
                {
                    //地图界面
                    float fx = baseMap.Width / 2;
                    float fy = baseMap.Height / 2;
                    //PointF cpf = baseMap.WMap.Coord.Center;
                    PointF cpf = new PointF(fx, fy);

                    baseMap.ZoomPanTool.ZoomAt(cpf, 1.0f / 1.2f);
                    baseMap.Render();

                    //方法2
                    // baseMap.ZoomPanTool.Zoom(1.2f);
                    // baseMap.Render();
                }
            }
        }

        private void tsmBig_Click(object sender, EventArgs e)
        {
            tsbVBig_Click(sender, e);
        }

        private void tsmSmall_Click(object sender, EventArgs e)
        {
            tsbVSmall_Click(sender, e);
        }

        #endregion
        //===============================================================================
        #region 动画播放

        private void listActFolder()
        {
            listBox1.Items.Clear();
            if (!Directory.Exists(sysCfg.sActPath))
            {
                return;
            }

            foreach (DirectoryInfo dirinfo in new DirectoryInfo(sysCfg.sActPath).GetDirectories())
            {
                string ss = dirinfo.Name;

                if (dirinfo.CreationTime > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                {
                    listBox1.Items.Add(ss);
                }
            }
        }

        /// <summary>
        /// 切换工具栏按钮隐现
        /// </summary>
        /// <param name="iFlag">0=地图,1=动画</param>
        public void setToolBarBntState(int iFlag)
        {
            tsbIR1.Visible = (iFlag == 0);
            tsbIR2.Visible = tsbIR1.Visible;
            tsbIR3.Visible = tsbIR1.Visible;
            tsbIR4.Visible = tsbIR1.Visible;
            tsbVIS1.Visible = tsbIR1.Visible;
            //tsbVIS2.Visible = tsbIR1.Visible;
            //tsbVIS3.Visible = tsbIR1.Visible;
            //tsbVIS4.Visible = tsbIR1.Visible;

            toolStripSeparator3.Visible = tsbIR1.Visible;
            tsbL1.Visible = tsbIR1.Visible;
            tsbM2.Visible = tsbIR1.Visible;
            tsbJ3.Visible = tsbIR1.Visible;
            tsbD4.Visible = tsbIR1.Visible;
            toolStripSeparator4.Visible = tsbIR1.Visible;
            //tsbAreaGrid.Visible = tsbIR1.Visible;

            tsbSavePic.Visible = tsbIR1.Visible;
            toolStripButton1.Visible = tsbIR1.Visible;
            tsbPrint.Visible = tsbIR1.Visible;
            //tsTRight.Visible = tsbIR1.Visible;

            //主显示区
            pnlMap.Visible = !tsbIR1.Visible;
            for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            {
                Control con = scClient.Panel1.Controls[i];
                if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
                {
                    (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = tsbIR1.Visible;
                    break;
                }
            }
        }


        public List<CMapInfo> listMap = new List<CMapInfo>();//动画文件缓冲列表
        public int iViewIndex = 0;
        public int iPlayFlag = 0;//0=stop,1=play
        public float fZoomScale = 1.2f;//缩放步进
        public float fZoomNow = 1.0f;//当前比例

        private void setButtonState()
        {
            //bool bPlaying = (iPlayFlag == 1);

            if (iPlayFlag == 1)
            {
                listBox1.Enabled = false;
                nudTime.Enabled = false;

                btnPlay.Enabled = false;
                btnPlay.BackColor = Color.SteelBlue;

                btnStop.Enabled = true;
                btnStop.BackColor = btnQuick.BackColor;

                btnGoNext.Enabled = false;
                btnGoNext.BackColor = Color.SteelBlue;

                btnGoFront.Enabled = false;
                btnGoFront.BackColor = Color.SteelBlue;
            }
            else//stop
            {
                listBox1.Enabled = true;
                nudTime.Enabled = true;

                btnPlay.Enabled = true;
                btnPlay.BackColor = btnQuick.BackColor;

                btnStop.Enabled = false;
                btnStop.BackColor = Color.SteelBlue;

                btnGoNext.Enabled = true;
                btnGoNext.BackColor = btnQuick.BackColor;

                btnGoFront.Enabled = true;
                btnGoFront.BackColor = btnQuick.BackColor;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (listMap.Count <= 0)
            {
                return;
            }

            //listBox1.Enabled = false;
            //nudTime.Enabled = false;

            iPlayFlag = 1;
            int iT = 1000 / Convert.ToInt32(nudTime.Value);
            timer1.Interval = iT;
            timer1.Enabled = true;

            //btnPlay.Enabled = false;
            //btnPlay.BackColor = Color.SteelBlue;

            //btnStop.Enabled = true;
            //btnStop.BackColor = btnGoNext.BackColor;
            setButtonState();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //listBox1.Enabled = true;
            //nudTime.Enabled = true;

            timer1.Enabled = false;

            //btnPlay.Enabled = true;
            //btnPlay.BackColor = btnGoNext.BackColor;

            //btnStop.Enabled = false;
            //btnStop.BackColor = Color.SteelBlue;

            iPlayFlag = 0;
            setButtonState();
        }

        private void btnQuick_Click(object sender, EventArgs e)
        {
            nudTime.Value = nudTime.Value + 1;
            int iT = 1000 / Convert.ToInt32(nudTime.Value);
            timer1.Interval = iT;
        }

        private void btnSlow_Click(object sender, EventArgs e)
        {
            if (nudTime.Value > 1)
            {
                nudTime.Value = nudTime.Value - 1;
                int iT = 1000 / Convert.ToInt32(nudTime.Value);
                timer1.Interval = iT;
            }
        }

        private void FindFile(string path)
        {
            //递归搜索文件
            listMap.Clear();
            int iCount = 0;
            foreach (FileInfo fi in new DirectoryInfo(path).GetFiles())
            {
                //addLog(dirinfo.Name);
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CMapInfo mi = new CMapInfo();
                mi.sName = Path.GetFileNameWithoutExtension(sShowName);
                mi.sPath = sPathName;
                mi.bmpImage = new Bitmap(sPathName);
                listMap.Add(mi);

                if (iCount == 0)
                {
                    pbxMap.Dock = DockStyle.Fill;//DockStyle.None;
                    pbxMap.Image = mi.bmpImage;
                }
                iCount++;

                //if (iCount > 0)
                //{
                //    listBox1.SelectedIndex = 0;
                //}
                //FindFile(dirinfo.FullName);
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (iPlayFlag == 0)
            {
                int iSel = listBox1.SelectedIndex;
                if ((iSel >= 0) && (iSel < listBox1.Items.Count))
                {
                    string sFolder = Convert.ToString(listBox1.Items[iSel]);
                    //pbxMap.Image = mi.bmpImage;
                    FindFile(sysCfg.sActPath + sFolder);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //播放动画
            if (listMap.Count <= 0)
            {
                return;
            }

            int iSel = iViewIndex;//listBox1.SelectedIndex;
            iSel++;
            //if (iSel < listBox1.Items.Count)
            if (iSel < listMap.Count)
            {

            }
            else
            {
                iSel = 0;
            }

            //listBox1.SelectedIndex = iSel;
            iViewIndex = iSel;
            //CMapInfo mi = listBox1.Items[iSel] as CMapInfo;
            CMapInfo mi = listMap[iSel] as CMapInfo;
            if (null != mi)
            {
                pbxMap.Image = mi.bmpImage;
            }
        }

        private void btnGoNext_Click(object sender, EventArgs e)
        {
            if (listMap.Count <= 0)
            {
                return;
            }

            int iSel = iViewIndex;//listBox1.SelectedIndex;
            iSel++;
            //if (iSel < listBox1.Items.Count)
            if (iSel < listMap.Count)
            {

            }
            else
            {
                iSel = 0;
            }

            iViewIndex = iSel;
            CMapInfo mi = listMap[iSel] as CMapInfo;
            if (null != mi)
            {
                pbxMap.Image = mi.bmpImage;
            }
        }

        private void btnGoFront_Click(object sender, EventArgs e)
        {
            if (listMap.Count <= 0)
            {
                return;
            }

            int iSel = iViewIndex;//listBox1.SelectedIndex;
            iSel--;
            //if (iSel < listBox1.Items.Count)
            if (iSel >= 0)
            {

            }
            else
            {
                iSel = listMap.Count - 1;
            }

            iViewIndex = iSel;
            CMapInfo mi = listMap[iSel] as CMapInfo;
            if (null != mi)
            {
                pbxMap.Image = mi.bmpImage;
            }
        }

        #endregion
        //===============================================================================
        #region 借鉴功能

        //反演产品参数设置,20150320,wm add
        private void tsmFYset_Click(object sender, EventArgs e)
        {
            frmFYPSet.sysParams = userSysParams;
            if (frmFYPSet.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                WRFileUnit.saveSysParams(userSysParams);
            }
        }

        private void tsmPathSet_Click(object sender, EventArgs e)
        {
            //系统参数设置
            int iTemp = userSysParams.NearMayDays;//记录“显示几天内的地图”

            frmPSet.sysCfgMy = sysCfg;
            frmPSet.sysParams = userSysParams;
            if (frmPSet.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                sysCfg.SaveParameters();
                WRFileUnit.saveSysParams(userSysParams);

                //20140515 提取鼠标设置到系统配置
                bMouseGetVal = userSysParams.MouseGetVal;
                bMouseLablTM = userSysParams.MouseLablTM;
                if (null != labMapXYMsg)
                {
                    if (bMouseLablTM)
                    {
                        labMapXYMsg.BackColor = Color.Transparent;
                    }
                    else
                    {
                        labMapXYMsg.BackColor = SystemColors.Control;
                    }
                }

                //更新地图列表的 Style
                bool bLayerChanged = false;
                if (iTemp != userSysParams.NearMayDays)
                {
                    //显示天数发生变换
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.RemoveAt(i);
                        }
                    }
                    //FindLoadFYMapFile();
                    ////if (listBoxMap.Items.Count > 0)
                    ////{
                    ////    listBoxMap.SelectedIndex = 0;//自动引发：SelectedIndexChanged(sender, e)                    
                    ////}                    
                    //bLayerChanged = true;
                }

                //一定要刷新地图列表，重置地图文件路径
                FindLoadFYMapFile();
                listMapLays();

                bLayerChanged = true;
                for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        userStyle = lay.Style as Style;//备份各层现有属性设置
                        setStyleFont();         //设置字体相关参数
                        lay.Style = userStyle;  //赋值给图层
                        bLayerChanged = true;
                    }
                }

                if (bLayerChanged)
                {
                    baseMap.Render();
                }

                setStyleFont();

                //重设置鼠标取值字体
                if (null != labMapXYMsg)
                {
                    //labMapXYMsg.Font = new Font("宋体", 10);
                    try
                    {
                        Font fnt = new Font(userSysParams.MouseFontName, userSysParams.MouseFontSize, userSysParams.MouseFontFS);
                        labMapXYMsg.Font = fnt;
                    }
                    catch (Exception)
                    {
                        labMapXYMsg.Font = new Font("宋体", 10);
                    }
                    labMapXYMsg.ForeColor = Color.FromArgb(userSysParams.MouseFontColorVal);
                    //labMapXYMsg.BackColor = Color.Transparent;                    
                }

                //通知处理程序刷新列表
            }
        }

        private void tsmCloseUser_Click(object sender, EventArgs e)
        {
            //关闭产品
            tsmCloseUser.Checked = !tsmCloseUser.Checked;
            int k = 0;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                //lay.Style.LimitRectangle = !tsmCloseUser.Checked;    //显示区域控制
                if (!lay.IsBaseMap)
                {
                    lay.Visible = !tsmCloseUser.Checked;
                    k++;
                }
            }

            if (k > 0)
            {
                //baseMap.ZoomPanTool.bLimitPan = !tsmCloseUser.Checked;//不显示用户图层时可随意拖动，显示用户图层时判断放大和区域、控制拖动
                if (tsmCloseUser.Checked)//关闭产品图层
                {
                    baseMap.ZoomPanTool.bLimitZoom = false;
                    baseMap.ZoomPanTool.bEnableBigZoom = true;
                    baseMap.ZoomPanTool.bEnableSmallZoom = true;
                }
                baseMap.Render();
            }
        }

        private void tsmCloseBase_Click(object sender, EventArgs e)
        {
            //关闭地图
            tsmCloseBase.Checked = !tsmCloseBase.Checked;
            int k = 0;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                if (lay.IsBaseMap || lay.Name == "飞行航线.DAT")//20150324 wm add
                {
                    lay.Visible = !tsmCloseBase.Checked;
                    k++;
                }
            }

            if (k > 0)
            {
                baseMap.Render();
            }
        }

        private void tsmShowGridJW_Click(object sender, EventArgs e)
        {
            //经纬度网格
            tsmShowGridJW.Checked = !tsmShowGridJW.Checked;
            foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            {
                if (lay.IsBaseMap)
                {
                    string fn = lay.Name;
                    if (fn.Equals("经纬度网格"))
                    {
                        lay.Visible = tsmShowGridJW.Checked;
                        baseMap.Render();
                    }

                }
            }
        }
        Regex asd = new Regex(@".(?i:ASD)$");//added by ZJB 判断是否后缀为.ASD
        private void tsmLayerProperty_Click(object sender, EventArgs e)
        {
            //图层属性
            //foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
            //{
            //    if (lay.Name.Equals(sNowSelLayName))
            //    {
            //        lay.ShowProperty();
            //        break;
            //    }
            //}
            frmMapAttr frmMapA = new frmMapAttr();
            //frmMapA.AirLineStyle = AirLineStyle;//传递飞行航线画笔样式，20150324 wm add

            frmMapA.getMapLayersInfo(baseMap.WMap.LayerList);
            if (frmMapA.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                AirLineStyle = frmMapA.AirLineStyle;//参数修改后传回，20150330,wm add

                List<string> listLaySet = new List<string>();

                int k = 0;
                foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                {
                    if (lay.IsBaseMap || asd.IsMatch(lay.Name))//20150416 ZJB edited
                    {
                        //lay.Style.PenWidth = 5; //Pen _Pen.Width
                        //lay.Style.PenColor = Color.GreenYellow; //Pen _Pen.Color
                        string sLayName = lay.Name;
                        for (int i = 0; i < frmMapA.myLayerList.Count; i++)
                        {
                            ICustmoLayer item = frmMapA.myLayerList[i];
                            if (item.Name.Equals(sLayName))
                            {
                                if (i >= 0 && i < frmMapA.listPen.Count)
                                {
                                    Pen tpen = frmMapA.listPen[i];
                                    lay.Style.PenColor = tpen.Color;
                                    lay.Style.PenWidth = tpen.Width;
                                    listLaySet.Add(sLayName + "|" + Convert.ToString(tpen.Width) + "|" + Convert.ToString(tpen.Color.ToArgb()));
                                    k++;
                                }
                                break;
                            }
                        }
                    }
                }

                if (k > 0)
                {
                    try
                    {
                        if (listLaySet.Count > 0)
                        {
                            string sFilePathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "layerSet.cfg";
                            //StreamReader sr = new StreamReader(aFile); sr.EndOfStream
                            FileStream aFile = new FileStream(sFilePathName, FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter sw = new StreamWriter(aFile);
                            sw.BaseStream.Seek(0, SeekOrigin.Begin);
                            foreach (string item in listLaySet)
                            {
                                sw.WriteLine(item);
                            }
                            sw.Flush();
                            sw.Close();
                            aFile.Close();
                        }
                    }
                    catch (IOException ex)
                    {
                    }
                    baseMap.Render();//先写完文件在画图，20150324 wm add
                }
            }
        }

        /// <summary>
        /// 载入底图颜色线宽参数
        /// </summary>
        public void loadLayerCWSet()
        {
            string sFilePathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "layerSet.cfg";
            if (!File.Exists(sFilePathName))
            {
                AirLineStyle.PenWidth = 1.0f;//设置默认参数，20150330，wm add
                AirLineStyle.PenColor = Color.Black;
                return;
            }

            try
            {
                List<string> listLaySet = new List<string>();

                //
                FileStream aFile = new FileStream(sFilePathName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(aFile);
                while (!sr.EndOfStream)
                {
                    // 这里处理每一行
                    string strLine = sr.ReadLine();
                    listLaySet.Add(strLine);
                }
                sr.Close();
                aFile.Close();

                //
                if (listLaySet.Count > 0)
                {
                    int k = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (lay.IsBaseMap || asd.IsMatch(lay.Name))
                        {
                            string sLayName = lay.Name;
                            for (int i = 0; i < listLaySet.Count; i++)
                            {
                                string item = listLaySet[i];
                                string[] strParams = item.Split('|');
                                if (strParams.Length == 3)
                                {
                                    if (strParams[0].Equals(sLayName))
                                    {

                                        float fWidth = 1.0f;
                                        if (float.TryParse(strParams[1], out fWidth))
                                        {
                                            lay.Style.PenWidth = fWidth;

                                        }

                                        int iCol = 0;
                                        if (int.TryParse(strParams[2], out iCol))
                                        {
                                            lay.Style.PenColor = Color.FromArgb(iCol);

                                        }
                                        k++;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (k > 0)
                    {
                        baseMap.Render();
                    }

                }
            }
            catch (IOException ex)
            {
            }
        }

        #endregion
        //===============================================================================        
        //===============================================================================
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //string scfg = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "sysconfig.xml";
            //Style st = new Style();
            //scfg = "f:\\adfafadf.xml";
            //string ss = PubUnit.sPalletPathName;

            //XMLTest ssss = new XMLTest();

            //PubUnit.SaveToXml(scfg, ss);

            //string ss = PubUnit.sPalletPathName;
            //CSysConfig csc = new CSysConfig(ss);
            //csc.PalletPathName = PubUnit.sPalletPathName;
            //string scfg = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "sysconfig.xml";
            //scfg = "f:\\adfafadf.xml";
            //PubUnit.SaveToXml(scfg, csc);

            string scfg = "f:\\a11052.xml";
            //Style st = new Style();
            PubUnit.SaveToXml(scfg, sysCfg);//[Serializable],[NonSerializable]
        }

        private void muSavePic_Click(object sender, EventArgs e)
        {
            //System.Drawing.Imaging.ImageFormat
            string sFileName = "";
            saveFileDialog1.DefaultExt = "bmp";
            saveFileDialog1.Filter = "图像文件|*.bmp";

            saveFileDialog1.InitialDirectory = userSysParams.AutoSaveImgPath;//保存路径与默认一致，20150430 wm add

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sFileName = saveFileDialog1.FileName;

                System.Drawing.Imaging.ImageFormat imgFmt = System.Drawing.Imaging.ImageFormat.Bmp;

                baseMap.WMap.SaveToImage(sFileName, imgFmt);

            }
        }

        private void tsbSavePic_Click(object sender, EventArgs e)
        {
            muSavePic_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //Map.WMap.CurrentLayer
            Control[] ctrls = this.Controls.Find("MapFileList1", true);
            //Control[] ctrls = ((Form)PluginManager.getInstance().MainForm).Controls.Find(CtrlName, true);
            if (ctrls.Length > 0)
            {
                Control con2 = ctrls[0];
                MapFileList mfl = con2 as MapFileList;// as DataListCtrl;

                //if (dlc.selILayer != null)
                //{
                //    string ss = dlc.GetType().ToString();
                //    //if (dlc.selILayer is LayerVISS)
                //    {
                //        LayerVISS lvis = dlc.selILayer as LayerVISS;                        
                //    }
                //}

                if (mfl != null)
                {
                    string sIR1P = sysCfg.getIR1Path(userSysParams);
                    //mfl.changeSelIROrVISS(1, 0, sysCfg.sIR1Path);
                    mfl.changeSelIROrVISS(1, 0, sIR1P);
                }

            }
        }

        private void setIRVisBtnState()
        {
            //通道控制
            tsbIR1.Checked = (iNowType == 1) && (iNowWay == 0);
            tsbIR2.Checked = (iNowType == 1) && (iNowWay == 1);
            tsbIR3.Checked = (iNowType == 1) && (iNowWay == 2);
            tsbIR4.Checked = (iNowType == 1) && (iNowWay == 3);
            tsbVIS1.Checked = (iNowType == 2) && (iNowWay == 0);
            //tsbVIS2.Checked = (iNowType == 2) && (iNowWay == 1);
            //tsbVIS3.Checked = (iNowType == 2) && (iNowWay == 2);
            //tsbVIS4.Checked = (iNowType == 2) && (iNowWay == 3);

            tsmIR1.Checked = tsbIR1.Checked;
            tsmIR2.Checked = tsbIR2.Checked;
            tsmIR3.Checked = tsbIR3.Checked;
            tsmIR4.Checked = tsbIR4.Checked;
            tsmVIS1.Checked = tsbVIS1.Checked;

            tsmIR1f.Checked = tsbIR1.Checked;
            tsmIR2f.Checked = tsbIR2.Checked;
            tsmIR3f.Checked = tsbIR3.Checked;
            tsmIR4f.Checked = tsbIR4.Checked;
            tsmVIS1f.Checked = tsbVIS1.Checked;
            //--------------------------------------
            //投影
            muL1.Checked = tsbL1.Checked;
            muM2.Checked = tsbM2.Checked;
            muJ3.Checked = tsbJ3.Checked;
            muD4.Checked = tsbD4.Checked;

            tsmi1lbt.Checked = tsbL1.Checked;
            tsmi2mkt.Checked = tsbM2.Checked;
            tsmi3js.Checked = tsbJ3.Checked;
            tsmi4jwd.Checked = tsbD4.Checked;
            //--------------------------------------
            //调色板
            tsmiFPal00.Checked = tsmPal00.Checked;
            tsmiFPal01.Checked = tsmPal01.Checked;
            tsmiFPal02.Checked = tsmPal02.Checked;
            tsmiFPal03.Checked = tsmPal05.Checked;

        }

        /// <summary>
        /// 在地图切换 IR/VIS 前执行初始化操作
        /// </summary>
        private void doBeforeMapSwitchIRorVIS()
        {
            ucTrackBar1.iNowType = iNowType;
            ucTrackBar1.rePaint();
        }

        #region 各通道按钮点击事件

        //--------------------IR1--------------------//
        private void tsbIR1_ClickDealing()
        {
            if (tsbIR1.Checked)
            {
                return;
            }
            iNowType = 1;//1=IR,2=VIS
            iNowWay = 0;//0-3:1~4 通道    
            ucTrackBar1.iValue1 = 256;

            //2015-6-4,wm add
            FindLoadLogicFileList();//载入逻辑运算云图

            doBeforeMapSwitchIRorVIS();
        }

        private void tsbIR1_Click(object sender, EventArgs e)
        {
            if (tsbIR1.Checked)
            {
                return;
            }
            tsbIR1_ClickDealing();
            ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
        }

        //--------------------IR2--------------------//
        private void tsbIR2_ClickDealing()
        {
            if (tsbIR2.Checked)
            {
                return;
            }
            iNowType = 1;//1=IR,2=VIS
            iNowWay = 1;//0-3:1~4 通道    
            ucTrackBar1.iValue1 = 256;

            doBeforeMapSwitchIRorVIS();
        }
        private void tsbIR2_Click(object sender, EventArgs e)
        {
            if (tsbIR2.Checked)
            {
                return;
            }
            tsbIR2_ClickDealing();
            lBoxLogicYT.Items.Clear();//2014-11-5 wm add
            ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
        }

        //--------------------IR3--------------------//
        private void tsbIR3_ClickDealing()
        {
            if (tsbIR3.Checked)
            {
                return;
            }
            iNowType = 1;//1=IR,2=VIS
            iNowWay = 2;//0-3:1~4 通道    
            ucTrackBar1.iValue1 = 256;

            doBeforeMapSwitchIRorVIS();
        }
        private void tsbIR3_Click(object sender, EventArgs e)
        {
            if (tsbIR3.Checked)
            {
                return;
            }
            tsbIR3_ClickDealing();
            lBoxLogicYT.Items.Clear();//2014-11-5 wm add
            ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
        }

        //--------------------IR4--------------------//
        private void tsbIR4_ClickDealing()
        {
            if (tsbIR4.Checked)
            {
                return;
            }
            iNowType = 1;//1=IR,2=VIS
            iNowWay = 3;//0-3:1~4 通道    
            ucTrackBar1.iValue1 = 256;

            doBeforeMapSwitchIRorVIS();
        }
        private void tsbIR4_Click(object sender, EventArgs e)
        {
            if (tsbIR4.Checked)
            {
                return;
            }
            tsbIR4_ClickDealing();
            lBoxLogicYT.Items.Clear();//2014-11-5 wm add
            ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
        }

        //--------------------VIS--------------------//
        private void tsbVIS_ClickDealing()
        {
            if (tsbVIS1.Checked)
            {
                return;
            }
            iNowType = 2;//1=IR,2=VIS
            iNowWay = 0;//0-3:1~4 通道

            doBeforeMapSwitchIRorVIS();
        }
        private void tsbVIS1_Click(object sender, EventArgs e)
        {
            if (tsbVIS1.Checked)
            {
                return;
            }
            tsbVIS_ClickDealing();
            lBoxLogicYT.Items.Clear();//2014-11-5 wm add
            ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
        }

        private void tsbVIS2_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 1;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbVIS3_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 2;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbVIS4_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 3;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbdVIS1_ButtonClick(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 0;//0-3:1~4 通道    
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbdVIS2_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 1;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbdVIS3_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 2;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }

        private void tsbdVIS4_Click(object sender, EventArgs e)
        {
            //iNowType = 2;//1=IR,2=VIS
            //iNowWay = 3;//0-3:1~4 通道
            //doBeforeMapSwitchIRorVIS();
            //listBoxMap_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region 各投影按钮点击事件

        public Projection pLbt1 = new ProjLbt();
        public Projection pMct2 = new ProjMct();
        public Projection pBbq3 = new ProjBbq();
        public Projection pLine4 = new ProjLine();

        private void tsbL1_Click(object sender, EventArgs e)
        {
            if (tsbL1.Checked)
            {
                return;
            }

            if (FYtype == "dzx")
            {
                MessageBox.Show("等值线只有在麦卡托投影下才可见！");
                return;
            }
            tsbL1.Checked = true;
            tsbM2.Checked = false;
            tsbJ3.Checked = false;
            tsbD4.Checked = false;
            tsbS5.Checked = false;
            if (baseMap.Projection != pLbt1)
            {
                if (nowWorkType == EWorkType.FYMap)
                {
                    tspBar1.Maximum = 100;
                    SetProgressBarVal(20);
                    tspBar1.Visible = true;
                    Application.DoEvents();

                    SetProgressBarVal(70);
                    Application.DoEvents();
                }

                baseMap.Projection = pLbt1;
                iNowProj = 1;
                //FindLoadLogicMapFile();//载入逻辑运算云图
                if (nowWorkType == EWorkType.MTSATSec)
                {
                    //切换投影文件
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
                else if (nowWorkType == EWorkType.FYMap)
                {
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
            }
            //-----------------------------------------------------
            //pnlMap.Visible = false;
            //for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            //{
            //    Control con = scClient.Panel1.Controls[i];
            //    if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
            //    {
            //        (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = true;
            //        break;
            //    }
            //}
            //-----------------------------------------------------
            //tabControl1_SelectedIndexChanged(null, null);
        }

        private void tsbM2_Click(object sender, EventArgs e)
        {//麦卡托投影

            if (tsbM2.Checked)
            {
                return;
            }

            tsbL1.Checked = false;
            tsbM2.Checked = true;
            tsbJ3.Checked = false;
            tsbD4.Checked = false;
            tsbS5.Checked = false;
            if (baseMap.Projection != pMct2)
            {
                if (nowWorkType == EWorkType.FYMap)
                {
                    tspBar1.Maximum = 100;
                    SetProgressBarVal(20);
                    tspBar1.Visible = true;
                    Application.DoEvents();

                    SetProgressBarVal(70);
                    Application.DoEvents();
                }
                baseMap.Projection = pMct2;
                iNowProj = 2;
                //FindLoadLogicMapFile();//载入逻辑运算云图
                if (nowWorkType == EWorkType.MTSATSec)
                {
                    //切换投影文件
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
                else if (nowWorkType == EWorkType.FYMap)
                {
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
                //tabControl1_SelectedIndexChanged(null, null);
            }
            //-----------------------------------------------------
            //pnlMap.Visible = false;
            //for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            //{
            //    Control con = scClient.Panel1.Controls[i];
            //    if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
            //    {
            //        (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = true;
            //        break;
            //    }
            //}
            //-----------------------------------------------------
            //if (setLayerVisible("中国-地形图--兰布托投影                 ", false) > 0)
            //{
            //    baseMap.Render();
            //}
        }

        private void tsbJ3_Click(object sender, EventArgs e)
        {
            if (tsbJ3.Checked)
            {
                return;
            }
            if (FYtype == "dzx")
            {
                MessageBox.Show("等值线只有在麦卡托投影下才可见！");
                return;
            }
            tsbL1.Checked = false;
            tsbM2.Checked = false;
            tsbJ3.Checked = true;
            tsbD4.Checked = false;
            tsbS5.Checked = false;
            if (baseMap.Projection != pBbq3)
            {
                if (nowWorkType == EWorkType.FYMap)
                {
                    tspBar1.Maximum = 100;
                    SetProgressBarVal(20);
                    tspBar1.Visible = true;
                    Application.DoEvents();

                    SetProgressBarVal(70);
                    Application.DoEvents();
                }
                baseMap.Projection = pBbq3;
                iNowProj = 3;
                //FindLoadLogicMapFile();//载入逻辑运算云图
                if (nowWorkType == EWorkType.MTSATSec)
                {
                    //切换投影文件
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
                else if (nowWorkType == EWorkType.FYMap)
                {
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
            }
            //-----------------------------------------------------
            //pnlMap.Visible = false;
            //for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            //{
            //    Control con = scClient.Panel1.Controls[i];
            //    if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
            //    {
            //        (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = true;
            //        break;
            //    }
            //}
            //-----------------------------------------------------
            //if (setLayerVisible("中国-地形图--兰布托投影                 ", false) > 0)
            //{
            //    baseMap.Render();
            //}
            //tabControl1_SelectedIndexChanged(null, null);
        }

        private void tsbD4_Click(object sender, EventArgs e)
        {
            if (tsbD4.Checked)
            {
                return;
            }
            if (FYtype == "dzx")
            {
                MessageBox.Show("等值线只有在麦卡托投影下才可见！");
                return;
            }
            tsbL1.Checked = false;
            tsbM2.Checked = false;
            tsbJ3.Checked = false;
            tsbD4.Checked = true;
            tsbS5.Checked = false;
            if (baseMap.Projection != pLine4)
            {
                if (nowWorkType == EWorkType.FYMap)
                {
                    tspBar1.Maximum = 100;
                    SetProgressBarVal(20);
                    tspBar1.Visible = true;
                    Application.DoEvents();

                    SetProgressBarVal(70);
                    Application.DoEvents();
                }
                baseMap.Projection = pLine4;
                iNowProj = 4;
                //FindLoadLogicMapFile();//载入逻辑运算云图
                if (nowWorkType == EWorkType.MTSATSec)
                {
                    //切换投影文件
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
                else if (nowWorkType == EWorkType.FYMap)
                {
                    //listBoxMap_SelectedIndexChanged(sender, e);
                    ChangeFYTD(sender, e, FYtype);//2014-11-2,wm add
                }
            }
            //-----------------------------------------------------
            //pnlMap.Visible = false;
            //for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            //{
            //    Control con = scClient.Panel1.Controls[i];
            //    if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
            //    {
            //        (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = true;
            //        break;
            //    }
            //}
            //-----------------------------------------------------
            //if (setLayerVisible("中国-地形图--兰布托投影                 ", false) > 0)
            //{
            //    baseMap.Render();
            //}
            //tabControl1_SelectedIndexChanged(null, null);
        }

        private void tsbS5_Click(object sender, EventArgs e)
        {
            return;

            tsbL1.Checked = false;
            tsbM2.Checked = false;
            tsbJ3.Checked = false;
            tsbD4.Checked = false;
            tsbS5.Checked = true;

            iNowProj = 5;
            //-----------------------------------------------------
            pnlMap.Visible = true;
            for (int i = 0; i < scClient.Panel1.Controls.Count; i++)
            {
                Control con = scClient.Panel1.Controls[i];
                if (con is Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl)
                {
                    (con as Bexa.Guojf.FreeMicaps.Ctrls.MapCtrl).Visible = false;
                    break;
                }
            }
            //-----------------------------------------------------
            //载入卫星图
            listBoxMap_SelectedIndexChanged(sender, e);

        }

        private void muM2_Click(object sender, EventArgs e)
        {
            tsbM2_Click(sender, e);
        }

        private void muL1_Click(object sender, EventArgs e)
        {
            tsbL1_Click(sender, e);
        }

        private void muJ3_Click(object sender, EventArgs e)
        {
            tsbJ3_Click(sender, e);
        }

        private void muD4_Click(object sender, EventArgs e)
        {
            tsbD4_Click(sender, e);
        }

        #endregion

        public void autoLoadPallet(int iIndec)
        {
            string sid = iIndec.ToString();
            while (sid.Length < 2)
            {
                sid = "0" + sid;
            }

            sNowPalFileName = sysCfg.sWorkPath + "syspallet\\pal" + sid + ".pal";

            if (File.Exists(sNowPalFileName))
            {
                int k = 0;
                foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                {
                    if (!lay.IsBaseMap)
                    {
                        lay.Style.PalletPathName = sNowPalFileName;
                        k++;
                    }
                }

                if (k > 0)
                {
                    baseMap.Render();
                }
            }

        }

        private void tsmPal00_Click(object sender, EventArgs e)
        {
            tsmPal00.Checked = false;
            tsmPal01.Checked = false;
            tsmPal02.Checked = false;
            tsmPal03.Checked = false;
            tsmPal04.Checked = false;
            tsmPal05.Checked = false;
            tsmPal06.Checked = false;
            tsmPal07.Checked = false;
            tsmPal08.Checked = false;
            tsmPal09.Checked = false;
            tsmPal10.Checked = false;
            tsmPal11.Checked = false;
            //--------------------------
            tsmiFPal00.Checked = false;
            tsmiFPal01.Checked = false;
            tsmiFPal02.Checked = false;
            tsmiFPal03.Checked = false;
            tsmiFPal04.Checked = false;
            tsmiFPal05.Checked = false;
            tsmiFPal06.Checked = false;
            tsmiFPal07.Checked = false;
            tsmiFPal08.Checked = false;
            tsmiFPal09.Checked = false;
            tsmiFPal10.Checked = false;
            tsmiFPal11.Checked = false;

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
                if (tsmi != null)
                {
                    tsmi.Checked = true;
                    string ss = Convert.ToString(tsmi.Tag);
                    int k = Convert.ToInt32(ss);
                    autoLoadPallet(k);
                }

            }
        }

        PrintDocument _PrintDoc = new PrintDocument();

        private void muPrint_Click(object sender, EventArgs e)
        {
            PrintDialog ptDialog = new PrintDialog();
            ptDialog.Document = _PrintDoc;
            if (ptDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _PrintDoc.Print();
                }
                catch
                {
                    ptDialog.Document.PrintController.OnEndPrint(ptDialog.Document, new System.Drawing.Printing.PrintEventArgs());
                }
            }
        }

        private void muPntPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog PntPreViewDlg = new PrintPreviewDialog();
            PntPreViewDlg.Document = _PrintDoc;
            try
            {
                PntPreViewDlg.ShowDialog();
            }
            catch
            {
                PntPreViewDlg.Document.PrintController.OnEndPrint(PntPreViewDlg.Document, new System.Drawing.Printing.PrintEventArgs());
            }
        }

        void ptDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (null != baseMap)
            {
                Graphics g = e.Graphics;
                g.DrawImage(baseMap.WMap.BufferBmp, e.MarginBounds);
                g.DrawRectangle(Pens.Black, e.MarginBounds);
            }

        }

        private void muCopy_Click(object sender, EventArgs e)
        {
            if (null != baseMap)
                baseMap.WMap.CopyToClipboard();
        }

        private void muBSTrackBar_Click(object sender, EventArgs e)
        {
            //消隐工具条
            muBSTrackBar.Checked = !muBSTrackBar.Checked;
            ucTrackBar1.Height = pnlRight.Height;
            pnlRight.Visible = muBSTrackBar.Checked;
        }

        private void pnlRight_Resize(object sender, EventArgs e)
        {
            //ucTrackBar1 = new UCTrackBar();//2014-9-20 wm add 
            ucTrackBar1.setBarSize(tabLeftTop.Height - 20);
        }

        private void listBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {

            FYtype = "";
            //显示地图
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = listBoxMap.SelectedIndex;
            if ((k >= 0) && (k < listBoxMap.Items.Count))
            {
                #region 显示地图

                if (nowWorkType == EWorkType.FYMap)
                {
                    #region VISS

                    CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                    if (sfi != null)
                    {
                        #region 控制通道投影按钮

                        //1=IR,2=VIS
                        //0-3:1~4 通道
                        //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                        string sMapFile;

                        //投影
                        sMapFile = sfi.getVissPathName(iNowType, iNowWay, 1);
                        tsbL1.Enabled = File.Exists(sMapFile);
                        muL1.Enabled = tsbL1.Enabled;

                        sMapFile = sfi.getVissPathName(iNowType, iNowWay, 2);
                        tsbM2.Enabled = File.Exists(sMapFile);
                        muM2.Enabled = tsbM2.Enabled;

                        sMapFile = sfi.getVissPathName(iNowType, iNowWay, 3);
                        tsbJ3.Enabled = File.Exists(sMapFile);
                        muJ3.Enabled = tsbJ3.Enabled;

                        sMapFile = sfi.getVissPathName(iNowType, iNowWay, 4);
                        tsbD4.Enabled = File.Exists(sMapFile);
                        muD4.Enabled = tsbD4.Enabled;

                        //通道                        
                        tsbIR1.Enabled = File.Exists(sfi.getVissPathName(1, 0, 1))
                                        || File.Exists(sfi.getVissPathName(1, 0, 2))
                                        || File.Exists(sfi.getVissPathName(1, 0, 3))
                                        || File.Exists(sfi.getVissPathName(1, 0, 4));
                        tsmIR1.Enabled = tsbIR1.Enabled;
                        tsmIR1f.Enabled = tsbIR1.Enabled;

                        tsbIR2.Enabled = File.Exists(sfi.getVissPathName(1, 1, 1))
                                        || File.Exists(sfi.getVissPathName(1, 1, 2))
                                        || File.Exists(sfi.getVissPathName(1, 1, 3))
                                        || File.Exists(sfi.getVissPathName(1, 1, 4));
                        tsmIR2.Enabled = tsbIR2.Enabled;
                        tsmIR2f.Enabled = tsbIR2.Enabled;

                        tsbIR3.Enabled = File.Exists(sfi.getVissPathName(1, 2, 1))
                                        || File.Exists(sfi.getVissPathName(1, 2, 2))
                                        || File.Exists(sfi.getVissPathName(1, 2, 3))
                                        || File.Exists(sfi.getVissPathName(1, 2, 4));
                        tsmIR3.Enabled = tsbIR3.Enabled;
                        tsmIR3f.Enabled = tsbIR3.Enabled;

                        tsbIR4.Enabled = File.Exists(sfi.getVissPathName(1, 3, 1))
                                        || File.Exists(sfi.getVissPathName(1, 3, 2))
                                        || File.Exists(sfi.getVissPathName(1, 3, 3))
                                        || File.Exists(sfi.getVissPathName(1, 3, 4));
                        tsmIR4.Enabled = tsbIR4.Enabled;
                        tsmIR4f.Enabled = tsbIR4.Enabled;

                        tsbVIS1.Enabled = File.Exists(sfi.getVissPathName(2, 0, 1))
                                        || File.Exists(sfi.getVissPathName(2, 0, 2))
                                        || File.Exists(sfi.getVissPathName(2, 0, 3))
                                        || File.Exists(sfi.getVissPathName(2, 0, 4));
                        tsmVIS1.Enabled = tsbVIS1.Enabled;
                        tsmVIS1f.Enabled = tsbVIS1.Enabled;

                        #endregion
                        //=================================================================================                        
                        #region 切换云图后，如果没有当前通道的则顺延显示

                        bool bAutoSelWayOK = false;

                        if (iNowType == 1)
                        {
                            //1=IR
                            if (iNowWay == 0)//0-3:1~4 通道
                            {
                                #region 切换处理

                                if (tsbIR1.Enabled == false)
                                {
                                    //2,3,4,5
                                    if (!bAutoSelWayOK && tsbIR2.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR2_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR3.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR3_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR4.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR4_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbVIS1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbVIS1_Click(sender, e);
                                        return;
                                    }
                                }

                                #endregion
                            }
                            else if (iNowWay == 1)
                            {
                                #region 切换处理

                                if (tsbIR2.Enabled == false)
                                {
                                    //1,3,4,5
                                    if (!bAutoSelWayOK && tsbIR1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR1_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR3.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR3_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR4.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR4_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbVIS1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbVIS1_Click(sender, e);
                                        return;
                                    }

                                }

                                #endregion
                            }
                            else if (iNowWay == 2)
                            {
                                #region 切换处理

                                if (tsbIR3.Enabled == false)
                                {
                                    //4,5,1,2
                                    if (!bAutoSelWayOK && tsbIR4.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR4_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbVIS1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbVIS1_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR1_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR2.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR2_Click(sender, e);
                                        return;
                                    }
                                }

                                #endregion
                            }
                            else if (iNowWay == 3)
                            {
                                #region 切换处理

                                if (tsbIR4.Enabled == false)
                                {
                                    //5,1,2,3
                                    if (!bAutoSelWayOK && tsbVIS1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbVIS1_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR1.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR1_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR2.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR2_Click(sender, e);
                                        return;
                                    }

                                    if (!bAutoSelWayOK && tsbIR3.Enabled)
                                    {
                                        bAutoSelWayOK = true;
                                        tsbIR3_Click(sender, e);
                                        return;
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            //2=VIS
                            if (tsbVIS1.Enabled == false)
                            {
                                //1,2,3,4
                                if (!bAutoSelWayOK && tsbIR1.Enabled)
                                {
                                    bAutoSelWayOK = true;
                                    tsbIR1_Click(sender, e);
                                    return;
                                }

                                if (!bAutoSelWayOK && tsbIR2.Enabled)
                                {
                                    bAutoSelWayOK = true;
                                    tsbIR2_Click(sender, e);
                                    return;
                                }

                                if (!bAutoSelWayOK && tsbIR3.Enabled)
                                {
                                    bAutoSelWayOK = true;
                                    tsbIR3_Click(sender, e);
                                    return;
                                }

                                if (!bAutoSelWayOK && tsbIR4.Enabled)
                                {
                                    bAutoSelWayOK = true;
                                    tsbIR4_Click(sender, e);
                                    return;
                                }
                            }
                        }
                        #endregion
                        //=================================================================================
                        //IR1 兰布托投影,默认文件
                        string sWayName = sfi.sVissLayName;//默认通道地图文件名

                        //根据当前投影换算
                        switch (iNowProj)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                //直接载入位图
                                #region 直接载入全球卫星位图

                                pbxMap.Image = null;

                                string sBMPPathName = sfi.getVissPathName(iNowType, iNowWay, iNowProj).ToLower();
                                sBMPPathName = sBMPPathName.Replace("sil", "sij");
                                if (File.Exists(sBMPPathName))
                                {
                                    pbxMap.SizeMode = PictureBoxSizeMode.AutoSize;
                                    pbxMap.Dock = DockStyle.None;
                                    Bitmap bmpImage = new Bitmap(sBMPPathName);
                                    pbxMap.Image = bmpImage;

                                    //------------------------------------------
                                    pbxMap.SizeMode = PictureBoxSizeMode.Zoom;
                                    pbxMap.Dock = DockStyle.Fill;
                                    pbxMap.Left = 0;
                                    pbxMap.Top = 0;

                                    fZoomNow = 0.5f;
                                    int iw = pbxMap.Image.Width;
                                    int ih = pbxMap.Image.Height;
                                    pbxMap.Width = Convert.ToInt32(iw * fZoomNow);
                                    pbxMap.Height = Convert.ToInt32(ih * fZoomNow);
                                }
                                tspBar1.Value = 0;
                                tspBar1.Visible = false;
                                return;

                                #endregion
                            //break;
                            default:
                                tspBar1.Value = 0;
                                tspBar1.Visible = false;
                                return;
                            //break;
                        }

                        //iNowType = 1; //1=IR,2=VIS
                        //iNowWay = 0;  //0-3:1~4 通道
                        sWayName = sfi.getVissLayerName(iNowType, iNowWay, iNowProj);

                        //校验是否已经存在
                        bool bIsExit = false;
                        bool bLayerChanged = false;

                        baseMap.ZoomPanTool.bLimitPan = false;//不显示用户图层时可随意拖动，显示用户图层时判断放大和区域、控制拖动
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if ((!lay.IsBaseMap))
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);//add by wcs 1014/11/14
                                i--;//add by wcs 1014/11/14

                                #region
                                /*
                                string ss = lay.Name;
                                if (ss.IndexOf("云迹风") < 0)//20140320 add 仅隐藏基础云图
                                {
                                    lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                    lay.Visible = false;    //隐藏图层

                                    if (ss.Equals(sWayName))
                                    {
                                        //已经存在                                        
                                        lay.Visible = true;
                                        //lay.Style.LimitRectangle = true;   //打开显示区域控制
                                        //baseMap.ZoomPanTool.bLimitPan = true;//不显示用户图层时可随意拖动，显示用户图层时判断放大和区域、控制拖动

                                        bIsExit = true;
                                        sNowSelLayName = lay.Name;
                                        //-------------------------------------------
                                        lay.Style = userStyle;
                                        //lay.Style.PalletPathName = sNowPalFileName;//???
                                        lay.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                        lay.Style.ShowMinVal = ucTrackBar1.iValue2;
                                        //-------------------------------------------
                                        object obj = lay.GetParams(null);
                                        if (obj is VISParams)
                                        {
                                            VISParams visP = obj as VISParams;
                                            if (visP != null)
                                            {
                                                ucTrackBar1.iContrastType = 0;
                                                ucTrackBar1.iNowType = iNowType;
                                                ucTrackBar1.dData = visP.dSigns2Data;
                                                ucTrackBar1.rePaint();
                                            }
                                        }
                                        //-------------------------------------------
                                        //2014-08 用于开关“直方图增强”
                                        if (tsmiZFTzq.Checked)
                                        {
                                            lay.SetParams(1);
                                        }
                                        else
                                        {
                                            lay.SetParams(0);
                                        }
                                        //-------------------------------------------
                                        bLayerChanged = true;
                                        //-------------------------------------------
                                    }
                                }
                                 * */
                                #endregion
                            }
                        }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)

                        if (!bIsExit)
                        {
                            string sVissPathName = sfi.sPath;
                            //换算文件名
                            sVissPathName = sfi.getVissPathName(iNowType, iNowWay, iNowProj);

                            if (!File.Exists(sVissPathName))
                            {
                                //隐藏所有云迹风
                                CloseAllYJFLayer();

                                baseMap.Render();
                                ShowStatusInfo("云图数据文件不存在！");// +sVissPathName;
                                //MessageBox.Show("文件不存在！" + Environment.NewLine + sVissPathName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                            if (dp != null)
                            {
                                if (dp.Result is ICustmoLayer)
                                {
                                    //新添加图层
                                    ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                    lays.Visible = true;
                                    lays.Style = userStyle;
                                    lays.Style.PalletPathName = sNowPalFileName;//???
                                    lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                    lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                    baseMap.WMap.AddLayer(lays);

                                    sNowSelLayName = lays.Name;
                                    //-------------------------------------------
                                    object obj = lays.GetParams(null);
                                    if (obj is VISParams)
                                    {
                                        VISParams visP = obj as VISParams;
                                        if (visP != null)
                                        {
                                            ucTrackBar1.iContrastType = 0;
                                            ucTrackBar1.iNowType = iNowType;
                                            ucTrackBar1.dData = visP.dSigns2Data;
                                            ucTrackBar1.rePaint();
                                        }
                                    }
                                    //-------------------------------------------
                                    //2014-08 用于开关“直方图增强”
                                    if (tsmiZFTzq.Checked)
                                    {
                                        lays.SetParams(1);
                                    }
                                    else
                                    {
                                        lays.SetParams(0);
                                    }
                                    //-------------------------------------------
                                    bLayerChanged = true;
                                    //-------------------------------------------
                                    //载入定标数据
                                    //svuObj.sFilePathName = sVissPathName;
                                    //svuObj.getPubDatas();
                                    //svuObj.loadImageDate();
                                    //svuObj.loadGridDate();
                                    //svuObj.getGridProjJW(baseMap.WMap.Coord);

                                    //double d1 = svuObj.getMaxSigns2Val();
                                    //double d2 = svuObj.getMinSigns2Val();                                
                                    //ucTrackBar1.iMaxVal = Convert.ToInt32(d1);
                                }
                            }
                        }//if (!bIsExit)

                        ////20150416 ZJB
                        loadAllExistedAirspace();

                        //=================================================================================                        
                        #region 控制最多载入图层数量

                        int iUserLayer = 0;
                        foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                        {
                            if (!lay.IsBaseMap)
                            {
                                iUserLayer++;
                            }
                        }

                        int m = 0;
                        while (iUserLayer > userSysParams.LoadLayerMax)
                        {
                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    baseMap.WMap.LayerList.RemoveAt(i);
                                    iUserLayer--;

                                    ClearMemory();
                                    break;
                                }
                            }
                            m++;

                            if (m > 10)
                            {
                                break;
                            }
                        }

                        #endregion
                        //=================================================================================
                        if (bLayerChanged)
                        {
                            //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            //{
                            //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            //    //lay.Style.LimitRectangle = true;   //打开显示区域控制
                            //}

                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    string ss = lay.Name;
                                    if (ss.IndexOf("云迹风") > 0)
                                    {
                                        lay.Visible = false;    //隐藏云际风图层
                                    }

                                }
                            }
                            //autoLoadYJFByMapFile(sfi);

                            baseMap.Render();
                        }
                        //=================================================================================                        

                    }//if (sfi != null)

                    #endregion
                }
                else if (nowWorkType == EWorkType.MTSATSec)
                {
                    #region SEC

                    CSecFileInfo sfi = listBoxMap.Items[k] as CSecFileInfo;
                    if (sfi != null)
                    {
                        string sWayName = sfi.sSecLayName;//默认通道地图文件名

                        //根据当前投影换算
                        switch (iNowProj)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                //直接载入位图, SEC 目前没有卫星图
                                return;
                            //break;
                            default:
                                return;
                            //break;
                        }

                        //iNowType = 1;    //1=IR,2=VIS
                        //iNowWay = 0;     //0-3:1~4 通道
                        //iNowProj = 1;    //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星

                        //获取预期的图层名
                        //sWayName = sfi.getVissLayerName(iNowType, iNowWay);
                        sWayName = sfi.getNextLayerSecName(iNowType, iNowWay, iNowProj);

                        //校验是否已经存在
                        bool bIsExit = false;
                        bool bLayerChanged = false;

                        baseMap.ZoomPanTool.bLimitPan = false;//不显示用户图层时可随意拖动，显示用户图层时判断放大和区域、控制拖动
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                lay.Visible = false;    //隐藏图层

                                string ss = lay.Name;
                                if (ss.Equals(sWayName))
                                {
                                    //已经存在                                    
                                    lay.Visible = true;

                                    //lay.Style.LimitRectangle = true;   //打开显示区域控制
                                    //baseMap.ZoomPanTool.bLimitPan = true;//不显示用户图层时可随意拖动，显示用户图层时判断放大和区域、控制拖动

                                    bIsExit = true;
                                    sNowSelLayName = lay.Name;
                                    //break;
                                    //-------------------------------------------
                                    object obj = lay.GetParams(null);
                                    if (obj is VISParams)
                                    {
                                        VISParams visP = obj as VISParams;
                                        if (visP != null)
                                        {
                                            ucTrackBar1.iContrastType = 1;
                                            ucTrackBar1.iNowType = iNowType;
                                            ucTrackBar1.fDataSec = visP.fContrast;
                                            ucTrackBar1.rePaint();
                                        }
                                    }
                                    //-------------------------------------------
                                    bLayerChanged = true;
                                    //-------------------------------------------
                                }
                            }
                        }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)

                        if (!bIsExit)
                        {
                            string sSecPathName = sfi.sPath;

                            //换算文件名
                            //sVissPathName = sfi.getVissPathName(iNowType, iNowWay);
                            //sVissPathName = sfi.sPath;
                            sSecPathName = sfi.getNextSecPathName(iNowType, iNowWay, iNowProj);
                            if (!File.Exists(sSecPathName))
                            {
                                baseMap.Render();
                                ShowStatusInfo("云图数据文件不存在！");// +sSecPathName;
                                //MessageBox.Show("文件不存在！" + Environment.NewLine + sSecPathName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            IDataPlugin dp = PluginManager.getInstance().OpenData(sSecPathName);
                            if (dp != null)
                            {
                                if (dp.Result is ICustmoLayer)
                                {
                                    //新添加图层
                                    ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                    lays.Visible = true;
                                    lays.Style.PalletPathName = sNowPalFileName;
                                    lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                    lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                    baseMap.WMap.AddLayer(lays);

                                    sNowSelLayName = lays.Name;
                                    //-------------------------------------------
                                    object obj = lays.GetParams(null);
                                    if (obj is VISParams)
                                    {
                                        VISParams visP = obj as VISParams;
                                        if (visP != null)
                                        {
                                            ucTrackBar1.iContrastType = 1;
                                            ucTrackBar1.iNowType = iNowType;
                                            ucTrackBar1.fDataSec = visP.fContrast;
                                            ucTrackBar1.rePaint();
                                        }
                                    }
                                    //-------------------------------------------
                                    bLayerChanged = true;
                                    //-------------------------------------------
                                    //载入定标数据
                                    //svuObj.sFilePathName = sVissPathName;
                                    //svuObj.getPubDatas();
                                    //svuObj.loadImageDate();
                                    //svuObj.loadGridDate();
                                    //svuObj.getGridProjJW(baseMap.WMap.Coord);

                                    //double d1 = svuObj.getMaxSigns2Val();
                                    //double d2 = svuObj.getMinSigns2Val();                                
                                    //ucTrackBar1.iMaxVal = Convert.ToInt32(d1);
                                }
                            }
                        }//if (!bIsExit)
                        loadAllExistedAirspace();
                        if (bLayerChanged)
                        {
                            //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            //{
                            //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            //    //lay.Style.LimitRectangle = true;   //打开显示区域控制
                            //}
                            baseMap.Render();
                        }

                    }//if (sfi != null)

                    #endregion
                }

                #endregion

                listMapLays();
            }

            tspBar1.Value = 0;
            tspBar1.Visible = false;
        }

        private void listBoxMap_DoubleClick(object sender, EventArgs e)
        {
            //显示属性
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //生成动画图片
            if (iWorkingState != 0)//0=空闲，1=解析生数据中，2=动画生成中
            {
                ShowStatusInfo("正在工作中...");
                return;
            }
            //iWorkingState = 1;//0=空闲，1=解析生数据中，2=动画生成中
            int iOutType = 0;//0=默认文件夹,1=自定义文件夹
            string sOutFolder = sNowSelLayName.Substring(0, 1);

            #region 计算时限

            //0 前 6小时
            //1 前 5小时
            //2 前 4小时
            //3 前 3小时
            //4 前 2小时
            //5 前 1小时
            //6 后 1小时
            //7 后 2小时
            //8 后 3小时
            //9 后 4小时
            //10 后 5小时
            //11 后 6小时
            //12 高级设置...
            //13 前12小时
            int iTimeSel = comboBox1.SelectedIndex;
            double dHoursLen = -6;
            double dTmp = Convert.ToDouble(iTimeSel);
            if (iTimeSel >= 6)
            {
                dTmp = dTmp + 1;
            }
            dHoursLen = dHoursLen + dTmp;

            DateTime dtStart = new DateTime(2000, 1, 1, 0, 0, 0);
            DateTime dtEndat = new DateTime(2050, 1, 1, 0, 0, 0);

            int iNowMapSelIndex = listBoxMap.SelectedIndex;

            if (iTimeSel == 12)//高级设置...
            {
                #region 高级设置

                iOutType = 1;//0=默认文件夹,1=自定义文件夹
                int k = 0;
                DateTime dtNow = DateTime.Now;
                if ((iNowMapSelIndex >= 0) && (iNowMapSelIndex < listBoxMap.Items.Count))
                {
                    if (nowWorkType == EWorkType.FYMap)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[iNowMapSelIndex] as CSivFileInfo;
                        if (sfi != null)
                        {
                            dtNow = sfi.dtBegin;
                            k++;
                        }
                    }
                }

                if (k <= 0)
                {
                    MessageBox.Show("请选择一个时间！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //选定地图时间做动画子文件夹名
                sOutFolder = sOutFolder + dtNow.ToString("yyMMdd_HHmm");

                frmATimeSel dlgATS = new frmATimeSel();
                dlgATS.dtNowDT = dtNow;
                dlgATS.sOutFolder = sOutFolder;//传递文件夹名
                dlgATS.sActPath = sysCfg.sActPath;

                if (dlgATS.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dtStart = dlgATS.dtStart;
                    dtEndat = dlgATS.dtEndat;
                    sOutFolder = dlgATS.sOutFolder;
                }
                else
                {
                    return;
                }

                #endregion
            }
            else//简易配置：-6 to +6
            {
                #region 简易配置
                iOutType = 0;//0=默认文件夹,1=自定义文件夹

                if ((iNowMapSelIndex >= 0) && (iNowMapSelIndex < listBoxMap.Items.Count))
                {
                    if (nowWorkType == EWorkType.FYMap)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[iNowMapSelIndex] as CSivFileInfo;
                        if (sfi != null)
                        {
                            //取出当前选项时间点
                            if (iTimeSel <= 5)//前
                            {
                                dtEndat = sfi.dtBegin;
                                dtStart = dtEndat.AddHours(dHoursLen);

                                dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
                                //dtEndat = new DateTime(dtEndat.Year, dtEndat.Month, dtEndat.Day, dtEndat.Hour, 59, 59);
                            }
                            else if (iTimeSel == 13) //201407 add ：前12小时
                            {
                                dtEndat = sfi.dtBegin;
                                dtStart = dtEndat.AddHours(-12);
                                dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
                            }
                            else//>=6 后
                            {
                                dtStart = sfi.dtBegin;
                                dtEndat = dtStart.AddHours(dHoursLen);

                                //dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
                                dtEndat = new DateTime(dtEndat.Year, dtEndat.Month, dtEndat.Day, dtEndat.Hour, 59, 59);
                            }

                        }
                    }
                    else if (nowWorkType == EWorkType.MTSATSec)
                    {
                        CSecFileInfo sfi = listBoxMap.Items[iNowMapSelIndex] as CSecFileInfo;
                        if (sfi != null)
                        {
                            //取出当前选项时间点
                            if (iTimeSel <= 5)//前
                            {
                                dtEndat = sfi.dtBegin;
                                dtStart = dtEndat.AddHours(dHoursLen);

                                dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);

                            }
                            else if (iTimeSel == 13) //201407 add ：前12小时
                            {
                                dtEndat = sfi.dtBegin;
                                dtStart = dtEndat.AddHours(-12);
                                dtStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0);
                            }
                            else//>=6 后
                            {
                                dtStart = sfi.dtBegin;
                                dtEndat = dtStart.AddHours(dHoursLen);

                                dtEndat = new DateTime(dtEndat.Year, dtEndat.Month, dtEndat.Day, dtEndat.Hour, 59, 59);
                            }

                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择一个时间！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion
            }

            #endregion
            //------------------------------------------------------------------------
            string sLayType = sNowSelLayName.Substring(0, 1);

            if (nowWorkType == EWorkType.FYMap)
            {
                #region BuildFY

                //加入动画帧队列
                listExportLayNames.Clear();
                for (int k = listBoxMap.Items.Count - 1; k >= 0; k--)
                {
                    CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                    if (sfi != null)
                    {
                        if ((sfi.dtBegin >= dtStart) && (sfi.dtBegin <= dtEndat))
                        {
                            //获取预期的图层名
                            string sTempLayerName = sfi.getVissLayerName(iNowType, iNowWay, iNowProj);
                            //listExportLayNames.Add(sTempLayerName);

                            string ss = sTempLayerName.Substring(0, 1);
                            if (ss.Equals(sLayType, StringComparison.OrdinalIgnoreCase))
                            {
                                listExportLayNames.Add(sTempLayerName);
                            }
                        }

                    }
                }
                //----------------------------------------------------------
                //载入图层
                int iflag = 0;//0=条件,1=全部
                if (iflag == 0)
                {
                    for (int k = 0; k < listBoxMap.Items.Count; k++)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                        if (sfi != null)
                        {
                            string sWayName = sfi.sVissLayName;//默认通道地图文件名

                            //iNowType = 1;//1=IR,2=VIS
                            //iNowWay = 0;//0-3:1~4 通道
                            sWayName = sfi.getVissLayerName(iNowType, iNowWay, iNowProj);

                            //判断是否在动画帧队列中
                            if (listExportLayNames.IndexOf(sWayName) >= 0)
                            {
                                //校验是否已经存在
                                bool bIsExit = false;
                                for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        lay.Visible = false;
                                        string ss = lay.Name;
                                        if (ss.Equals(sWayName))
                                        {
                                            //已经存在
                                            //lay.Visible = true;
                                            bIsExit = true;
                                            //break;
                                        }
                                    }
                                }
                                //-------------------------------------------
                                if (!bIsExit)
                                {
                                    string sVissPathName = sfi.sPath;
                                    //换算文件名
                                    sVissPathName = sfi.getVissPathName(iNowType, iNowWay, iNowProj);

                                    if (File.Exists(sVissPathName))
                                    {
                                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                        if (dp != null)
                                        {
                                            if (dp.Result is ICustmoLayer)
                                            {
                                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                                lays.Visible = false;
                                                lays.Style = userStyle;
                                                lays.Style.PalletPathName = sNowPalFileName;
                                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                                baseMap.WMap.AddLayer(lays);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else//0=条件,1=全部
                {
                    #region 1=全部

                    for (int k = 0; k < listBoxMap.Items.Count; k++)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                        if (sfi != null)
                        {
                            string sWayName = sfi.sVissLayName;//默认通道地图文件名

                            //iNowType = 1;//1=IR,2=VIS
                            //iNowWay = 0;//0-3:1~4 通道
                            sWayName = sfi.getVissLayerName(iNowType, iNowWay, iNowProj);

                            //校验是否已经存在
                            bool bIsExit = false;
                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    lay.Visible = false;
                                    string ss = lay.Name;
                                    if (ss.Equals(sWayName))
                                    {
                                        //已经存在
                                        lay.Visible = true;
                                        bIsExit = true;
                                        //break;
                                    }
                                }
                            }

                            if (!bIsExit)
                            {
                                string sVissPathName = sfi.sPath;
                                //换算文件名
                                sVissPathName = sfi.getVissPathName(iNowType, iNowWay, iNowProj);

                                IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                if (dp != null)
                                {
                                    if (dp.Result is ICustmoLayer)
                                    {
                                        ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                        //lays.Visible = true;//???
                                        lays.Style = userStyle;
                                        lays.Style.PalletPathName = sNowPalFileName;//???
                                        lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                        lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                        baseMap.WMap.AddLayer(lays);
                                    }
                                }
                            }
                            //baseMap.Render();
                        }
                    }//end 载入图层
                    //----------------------------------------------------------
                    //iNowType = 1;//1=IR,2=VIS
                    //iNowWay = 0;//0-3:1~4 通道
                    string sTypeWay = "";
                    if (iNowType == 1)
                    {
                        //iWay;//0-3:1~4 通道
                        sTypeWay = "IR" + (iNowWay + 1).ToString();
                    }
                    else if (iNowType == 2)
                    {
                        sTypeWay = "VIS" + (iNowWay + 1).ToString();
                    }
                    //----------------------------------------------------------
                    //加入动画帧队列
                    listExportLayNames.Clear();
                    for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            //lay.Visible = false;
                            string fn = lay.Name;
                            if (fn.IndexOf(sTypeWay) >= 0)
                                listExportLayNames.Add(fn);
                        }
                    }
                    //----------------------------------------------------------
                    #endregion
                }
                #endregion
            }
            else if (nowWorkType == EWorkType.MTSATSec)
            {
                #region buildMTSAT

                //加入动画帧队列
                listExportLayNames.Clear();
                for (int k = 0; k < listBoxMap.Items.Count; k++)
                {
                    CSecFileInfo sfi = listBoxMap.Items[k] as CSecFileInfo;
                    if (sfi != null)
                    {
                        if ((sfi.dtBegin >= dtStart) && (sfi.dtBegin <= dtEndat))
                        {
                            //获取预期的图层名
                            string sTempLayerName = sfi.getNextLayerSecName(iNowType, iNowWay, iNowProj);
                            //sfi.sPath;

                            //换算文件名
                            //string sSecPathName = sfi.getNextSecPathName(iNowType, iNowWay, iNowProj);

                            listExportLayNames.Add(sTempLayerName);
                        }

                    }
                }
                //----------------------------------------------------------
                //载入图层
                for (int k = 0; k < listBoxMap.Items.Count; k++)
                {
                    CSecFileInfo sfi = listBoxMap.Items[k] as CSecFileInfo;
                    if (sfi != null)
                    {
                        //string sWayName = sfi.sSecLayName;//默认通道地图文件名

                        //获取预期的图层名
                        string sTempLayerName = sfi.getNextLayerSecName(iNowType, iNowWay, iNowProj);

                        //判断是否在动画帧队列中
                        if (listExportLayNames.IndexOf(sTempLayerName) >= 0)
                        {
                            //校验是否已经存在
                            bool bIsExit = false;
                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    lay.Visible = false;
                                    string ss = lay.Name;
                                    if (ss.Equals(sTempLayerName))
                                    {
                                        //已经存在
                                        //lay.Visible = true;
                                        bIsExit = true;
                                        //break;
                                    }
                                }
                            }

                            if (!bIsExit)
                            {
                                string sSecPathName = sfi.sPath;
                                //换算文件名
                                sSecPathName = sfi.getNextSecPathName(iNowType, iNowWay, iNowProj);

                                if (File.Exists(sSecPathName))
                                {
                                    IDataPlugin dp = PluginManager.getInstance().OpenData(sSecPathName);
                                    if (dp != null)
                                    {
                                        if (dp.Result is ICustmoLayer)
                                        {
                                            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                            lays.Visible = false;
                                            lays.Style.PalletPathName = sNowPalFileName;//???
                                            lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                            lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                            baseMap.WMap.AddLayer(lays);
                                        }
                                    }
                                }
                            }

                        }

                    }
                }

                #endregion
            }

            listMapLays();

            if (listExportLayNames.Count <= 0)
            {
                MessageBox.Show("没有地图文件可供生成动画！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //folderBrowserDialog1.Description = "请选择输出目录";
            //if (Convert.ToInt32(folderBrowserDialog1.Tag) == 0)
            //{
            //    folderBrowserDialog1.SelectedPath = sysCfg.sActPath;// Application.StartupPath;
            //}

            //if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //string sTime = DateTime.Now.ToString("yyMMdd_HHmmss");
                //folderBrowserDialog1.Tag = 1;
                //string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();

                //0=默认文件夹,1=自定义文件夹
                if (iOutType == 0)
                {
                    string sTime = DateTime.Now.ToString("yyMMdd_HHmmss");
                    sOutFolder = sOutFolder + sTime;
                }
                else//0=默认文件夹
                {
                    //sOutFolder =  "Act" + sTime + Path.DirectorySeparatorChar.ToString();
                }

                //string sPath = sysCfg.sActPath;
                //sExportImgPaht = sPath + "Act" + sTime + Path.DirectorySeparatorChar.ToString();
                sExportImgPaht = sysCfg.sActPath + sOutFolder + Path.DirectorySeparatorChar.ToString();

                if (MessageBox.Show("即将生成动画文件，请耐心等待，不要进行其他操作。", "生成动画", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    //outSaveAllLayerImage(sPath, "Act" + sTime + Path.DirectorySeparatorChar.ToString());
                    PubUnit.safetyCreateDir(sExportImgPaht);
                    outSaveAllLayerImage();
                }

            }
        }

        private void tsmFY_Click(object sender, EventArgs e)
        {
            tsmMTSAT.Checked = false;
            tsmFY.Checked = true;
            //baseMap.ZoomPanTool.EnablePan = true;
            nowWorkType = EWorkType.FYMap;

            //tabPage1.Text = "FY 数据文件";
            FindLoadFYMapFile();

            baseMap.WMap.DelAllLayer();
            baseMap.Render();

            listMapLays();
        }

        private void tsmMTSAT_Click(object sender, EventArgs e)
        {
            tsmMTSAT.Checked = true;
            tsmFY.Checked = false;
            //baseMap.ZoomPanTool.EnablePan = false;
            nowWorkType = EWorkType.MTSATSec;

            //tabPage1.Text = "MTSAT 数据文件";
            //FindLoadSECMapFile();

            baseMap.WMap.DelAllLayer();
            baseMap.Render();

            listMapLays();

            //MTSAT 没有 极射投影，切换到 兰勃托
            if (iNowProj == 3)//当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            {
                tsbL1.Checked = true;
                tsbM2.Checked = false;
                tsbJ3.Checked = false;
                tsbD4.Checked = false;
                tsbS5.Checked = false;
                if (baseMap.Projection != pLbt1)
                {
                    baseMap.Projection = pLbt1;
                    iNowProj = 1;
                }
            }
        }

        private void btnSetTrackBar_Click(object sender, EventArgs e)
        {
            //ucTrackBar1.setBSVal(1000, 500);
            //ucTrackBar_VisibleChanged(sender, e);
            if (null == ucTrackBar1.dData)
            {
                //Environment.NewLine
                MessageBox.Show("还没有载入地图数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmSetTrackBarVal dlgSTBV = new frmSetTrackBarVal();
            dlgSTBV.iVal1 = ucTrackBar1.iValue1;
            dlgSTBV.iVal2 = ucTrackBar1.iValue2;

            dlgSTBV.iContrastType = ucTrackBar1.iContrastType;// = 1;
            dlgSTBV.iNowType = ucTrackBar1.iNowType;// = iNowType;
            dlgSTBV.dData = ucTrackBar1.dData;// = visP.fContrast;

            //dlgPal.OnApply += new EventHandler(dlgPallet_OnApply);
            if (dlgSTBV.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int iv1 = dlgSTBV.iVal1;
                int iv2 = dlgSTBV.iVal2;
                ucTrackBar1.setBSVal(iv1, iv2);
                ucTrackBar_VisibleChanged(sender, e);
            }

        }

        private void tsmVAreaSys_Click(object sender, EventArgs e)
        {
            //系统视窗
            baseMap.WMap.Coord.ZoomScale = 10.0f;
            PointF pfCen = new PointF(105, 35);
            //baseMap.WMap.Coord.GeoPan(pfCen);
            baseMap.WMap.Coord.Center = pfCen;
            baseMap.Render();

        }

        private void goUserViewCenter()
        {
            //用户视窗
            baseMap.WMap.Coord.ZoomScale = sysCfg.fZoomScale;
            PointF pfCen = new PointF(sysCfg.fUserJX, sysCfg.fUserWY);
            baseMap.WMap.Coord.Center = pfCen;
            baseMap.Render();
        }

        private void tsmVAreaUser_Click(object sender, EventArgs e)
        {
            goUserViewCenter();
        }

        private void tsmVAreaSave_Click(object sender, EventArgs e)
        {
            //保存用户视窗
            PointF pfCen = baseMap.WMap.Coord.Center;
            sysCfg.fUserJX = pfCen.X;
            sysCfg.fUserWY = pfCen.Y;
            sysCfg.fZoomScale = baseMap.WMap.Coord.ZoomScale;
            sysCfg.SaveUserViewCentrePara();

            ShowStatusInfo("用户视窗保存成功");
        }

        /// <summary>
        /// 手工制定调色板名称
        /// </summary>
        /// <param name="sPalName">调色板名称</param>
        /// <param name="iMapType">0=系统图层,1=用户图层</param>
        public void autoLoadPalletPro(string sPalName, int iMapType)
        {
            //sNowPalFileName = sysCfg.sWorkPath + "syspallet\\pal" + sid + ".pal";
            string sPname = sysCfg.sWorkPath + "syspallet\\" + sPalName + ".pal";

            if (File.Exists(sPname))
            {
                int k = 0;
                foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                {
                    if (iMapType == 0)
                    {
                        //系统图层
                        if (lay.IsBaseMap || lay.Name == "飞行航线.DAT")//20150324 wm add
                        {
                            lay.Style.PalletPathName = sPname;
                            k++;
                        }
                    }
                    else
                    {
                        //用户图层
                        if ((!lay.IsBaseMap) && (!lay.Name.Equals("飞行航线.DAT")))//20150324 wm add
                        {
                            lay.Style.PalletPathName = sPname;
                            sNowPalFileName = sPname;
                            k++;
                        }
                    }
                }

                if (k > 0)
                {
                    baseMap.Render();
                }
            }

        }

        private void tsmPal21_Click(object sender, EventArgs e)
        {
            autoLoadPalletPro("land01", 0);
        }

        private void tsmPal22_Click(object sender, EventArgs e)
        {
            autoLoadPalletPro("land02", 0);
        }

        private void tsmPal23_Click(object sender, EventArgs e)
        {
            autoLoadPalletPro("land03", 0);
        }

        private void tsmPal24_Click(object sender, EventArgs e)
        {
            autoLoadPalletPro("land04", 0);
        }

        private void tsmPal25_Click(object sender, EventArgs e)
        {
            autoLoadPalletPro("land05", 0);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!pm.MainForm.MainMenuStrip.Visible)
                {
                    tbFillScreen_Click(sender, e);
                }
            }
        }

        private void tsmiMouseGet_Click(object sender, EventArgs e)
        {
            //鼠标取值
            bMouseGetVal = !bMouseGetVal;
            tsmiMouseGet.Checked = bMouseGetVal;
        }

        private void tsmiMValTM_Click(object sender, EventArgs e)
        {
            //取值框透明
            tsmiMValTM.Checked = !tsmiMValTM.Checked;
            bMouseLablTM = tsmiMValTM.Checked;
            if (null != labMapXYMsg)
            {
                if (bMouseLablTM)
                {
                    labMapXYMsg.BackColor = Color.Transparent;
                }
                else
                {
                    labMapXYMsg.BackColor = SystemColors.Control;
                }
            }
        }


        //wangchangshuai
        private void tsmiHiden_Click(object sender, EventArgs e)
        {
            tsmiHiden.Checked = !tsmiHiden.Checked;
            tsToolBar.Visible = !tsmiHiden.Checked;
        }

        private void tsmiTSml_Click(object sender, EventArgs e)
        {
            setToolButtonViewType(0);
        }

        private void tsmiTBig_Click(object sender, EventArgs e)
        {
            setToolButtonViewType(1);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (null != baseMap)
                baseMap.Render();
        }

        private void tsmiShowPalette_Click(object sender, EventArgs e)
        {
            //显示色条
            bool bLayerChanged = false;
            tsmiShowPalette.Checked = !tsmiShowPalette.Checked;
            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    Style tmpS = lay.Style as Style;//备份各层现有属性设置
                    tmpS.ShowPaletteVitta = tsmiShowPalette.Checked;
                    lay.Style = tmpS;
                    bLayerChanged = true;
                }
            }

            if (bLayerChanged)
            {
                baseMap.Render();
            }
        }

        /// <summary>
        /// 运行中的进程处理
        /// using System.Diagnostics;
        /// </summary>
        public static void KillDealProcess(string sProcessName)
        {
            //遍历所有的进程
            foreach (Process thisproc in System.Diagnostics.Process.GetProcesses())
            {
                //控制台命令，输出进程名称
                //Console.WriteLine(thisproc.ProcessName);

                //判断如果进程名为IntelliTrace
                //QQ程序的进程名就是QQ，不需要带后缀的.exe
                if (thisproc.ProcessName.Equals(sProcessName))
                {
                    //自动关闭进程
                    thisproc.Kill();

                    //控制台命令，输出Killed!
                    //Console.WriteLine("Killed!");

                    //RefreshSystemTray.RefreshSysTray.SysTray.Refresh();//2014-11-19 wm add
                    break;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //检测解析程序是否运行
            timer2.Enabled = false;
            autoTestRunData();
            timer2.Enabled = userSysParams.AutoTestMTSAT;
        }

        private void tsmiDelAct_Click(object sender, EventArgs e)
        {
            //删除动画
            //foreach (DirectoryInfo dirinfo in new DirectoryInfo(sysCfg.sActPath).GetDirectories())
            //{
            //    string ss = dirinfo.Name;

            //    if (dirinfo.CreationTime > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
            //    {
            //        listBox1.Items.Add(ss);
            //    }
            //}
            int iSel = listBox1.SelectedIndex;
            if ((iSel >= 0) && (iSel < listBox1.Items.Count))
            {
                string sFolder = Convert.ToString(listBox1.Items[iSel]);
                //FindFile(sysCfg.sActPath + sFolder);
                if (MessageBox.Show("即将删除动画：" + sFolder, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    //del
                    try
                    {
                        //CMapInfo mi = listMap[iSel] as CMapInfo;
                        foreach (CMapInfo item in listMap)
                        {
                            item.bmpImage.Dispose();
                            item.bmpImage = null;
                        }

                        if (null != pbxMap.Image)
                        {
                            pbxMap.Image.Dispose();
                            pbxMap.Image = null;
                        }

                        listMap.Clear();
                        Directory.Delete(sysCfg.sActPath + sFolder, true);
                        WRFileUnit.writeLogStr("删除动画: " + sysCfg.sActPath + sFolder);
                        listActFolder();
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }

        private void tsmiFBefore_Click(object sender, EventArgs e)
        {
            int k = listBoxMap.SelectedIndex;
            if (k > 0)
            //&& (k < listBoxMap.Items.Count))
            {
                listBoxMap.SelectedIndex = k - 1;
            }
            else
            {
                MessageBox.Show("已到起始！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsmiFNext_Click(object sender, EventArgs e)
        {
            int k = listBoxMap.SelectedIndex;
            if (k < listBoxMap.Items.Count - 1)
            {
                listBoxMap.SelectedIndex = k + 1;
            }
            else
            {
                MessageBox.Show("已到末尾！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsmShowLayerList_Click(object sender, EventArgs e)
        {
            tsmShowLayerList.Checked = !tsmShowLayerList.Checked;
            tabLeftBottom.Visible = tsmShowLayerList.Checked;
            if (tabLeftBottom.Visible)
            {
                scLeft.SplitterDistance = tabControl1.Height - 220;
            }
            else
            {
                scLeft.SplitterDistance = tabControl1.Height;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //刷新文件列表
            FindLoadFYMapFile(1);

            //自动删除超期地图
            //timer3.Enabled = false;
            if (autoDelMapData() > 0)
            {
                FindLoadFYMapFile();
                listMapLays();
                if (listBoxMap.Items.Count > 0)
                {
                    listBoxMap.SelectedIndex = 0;//自动引发：SelectedIndexChanged(sender, e)                    
                }
            }
            //timer3.Enabled = true;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string ss = "";
            //ss = PubUnit.FullStr("123", 10);
            //ss = PubUnit.FullStr("agag", 10," ",false);

            DateTime dt1 = DateTime.Now;
            string sCap = dt1.ToString("yyyy年M月d日HH时mm分") + "FY2D云迹风";
            string sTim = " " + dt1.ToString("yyyy")
                            + PubUnit.FullStr(dt1.ToString("MM"), 3)
                            + PubUnit.FullStr(dt1.ToString("dd"), 3)
                            + PubUnit.FullStr(dt1.ToString("HH"), 3)
                            + "  9999"
                            + PubUnit.FullStr("6545", 8);

            sTim = " " + dt1.ToString("yyyy")
                            + PubUnit.FullStr(dt1.Month.ToString(), 3)
                            + PubUnit.FullStr(dt1.Day.ToString(), 3)
                            + PubUnit.FullStr(dt1.Hour.ToString(), 3)
                            + "  9999"
                            + PubUnit.FullStr("6545", 8);

            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ss = openFileDialog1.FileName;

                IDataPlugin dp = PluginManager.getInstance().OpenData(ss);
                if (dp != null)
                {
                    if (dp.Result is ICustmoLayer)
                    {
                        //新添加图层
                        ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                        lays.Visible = true;
                        //lays.Style = userStyle;
                        //lays.Style.PalletPathName = sNowPalFileName;
                        //lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                        //lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                        baseMap.WMap.AddLayer(lays);
                        sNowSelLayName = lays.Name;
                    }
                    baseMap.Render();

                    listMapLays();
                }
            }
        }

        //--------------------------------------------------------------------------
        #region 列表选中的条目改变时的事件

        private void listBoxYJF_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isShowYJF)
            {
                isShowYJF = true;
                return;
            }
            tsbIR1_ClickDealing();
            tsbIR1.Checked = true;
            tsbIR2.Enabled = false;
            tsbIR3.Enabled = false;
            tsbIR4.Enabled = false;
            tsbVIS1.Enabled = false;
            tsmIR2.Enabled = false;
            tsmIR3.Enabled = false;
            tsmIR4.Enabled = false;
            tsmVIS1.Enabled = false;
            tsbL1.Enabled = true;
            tsbJ3.Enabled = true;
            tsbD4.Enabled = true;
            muL1.Enabled = true;
            muJ3.Enabled = true;
            muD4.Enabled = true;

            ////listBoxMap.ClearSelected();//2014-10-21 wm add
            //显示云迹风
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = listBoxYJF.SelectedIndex;
            if ((k >= 0) && (k < listBoxYJF.Items.Count))
            {
                #region 显示地图

                #region 载入云迹风

                CYJFFileInfo sfi = listBoxYJF.Items[k] as CYJFFileInfo;
                if (sfi != null)
                {
                    //=================================================================================
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sLayName = sfi.sLayName;
                    string sYTName = sfi.getYTName();//2014-10-21 wm add

                    //校验是否已经存在
                    bool bIsExit = false;
                    bool bLayerChanged = false;
                    bool bMainMapOpen = false;//2014-10-21 wm add

                    //删除所有非底图
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除
                        }
                    }

                    if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                    {
                        for (int i = 0; i < listFYYT.Count; i++)
                        {
                            CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                    if (!File.Exists(sVissPathName))
                                    {
                                        ShowStatusInfo("云图数据文件不存在！");
                                        return;
                                    }
                                    IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                    if (dp != null)
                                    {
                                        if (dp.Result is ICustmoLayer)
                                        {
                                            //新添加图层
                                            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                            lays.Visible = true;
                                            lays.Style = userStyle;
                                            lays.Style.PalletPathName = sNowPalFileName;
                                            lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                            lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                            bMainMapOpen = true;//主图打开
                                            baseMap.WMap.AddLayer(lays);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (!bMainMapOpen)//2014-10-21 wm add
                    {
                        //主图未打开
                        bool bTmp = false;
                        for (int i = 0; i < listBoxMap.Items.Count; i++)
                        {
                            CSivFileInfo sfiM = listBoxMap.Items[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    bTmp = true;
                                    bLayerChanged = false;//刷新标致清零
                                    listBoxMap.SelectedIndex = i;
                                    bMainMapOpen = true;//主图打开
                                    break;
                                }
                            }
                        }
                    }

                    if (!bIsExit)
                    {
                        if (chbShowYJF.Checked)
                        {
                            string sPath = sfi.sPath;

                            if (!File.Exists(sPath))
                            {
                                baseMap.Render();
                                ShowStatusInfo("云迹风数据文件不存在！");
                                return;
                            }

                            IDataPlugin dp = PluginManager.getInstance().OpenData(sPath);
                            if (dp != null)
                            {
                                if (dp.Result is ICustmoLayer)
                                {
                                    //新添加图层
                                    ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                    lays.Visible = true;
                                    lays.IsTopLayer = true;

                                    baseMap.WMap.AddLayer(lays);

                                    sNowSelYJFLayName = lays.Name;
                                    bLayerChanged = true;
                                }
                            }
                        }
                    }//if (!bIsExit)
                    else
                    {
                        if (chbShowYJF.Checked)
                        {
                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    // baseMap.WMap.LayerList.Remove(lay);//2014-10-21 wm add

                                    #region 原代码，不用

                                    string ss = lay.Name;
                                    if (ss.IndexOf("云迹风") > 0)
                                    //2014-7-28 改成全部隐藏
                                    {
                                        lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                        lay.Visible = false;    //隐藏图层

                                        if (ss.Equals(sLayName))
                                        {
                                            //已经存在                                    
                                            lay.Visible = true;
                                            lay.IsTopLayer = true;
                                        }
                                    }

                                    #endregion
                                }
                            }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        }
                        baseMap.Render();

                    }

                    //20150324 wm add
                    loadAllExistedAirspace();

                    //=================================================================================                        
                    #region 控制最多载入图层数量

                    int iUserLayer = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (!lay.IsBaseMap)
                        {
                            iUserLayer++;
                        }
                    }

                    int m = 0;
                    while (iUserLayer > userSysParams.LoadLayerMax)
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                iUserLayer--;

                                ClearMemory();
                                break;
                            }
                        }
                        m++;

                        if (m > 10)
                        {
                            break;
                        }
                    }

                    #endregion
                    //=================================================================================
                    if (bLayerChanged)
                    {
                        baseMap.Render();
                    }
                    //=================================================================================                        

                }//if (sfi != null)

                #endregion

                #endregion

                listMapLays();
            }

            tspBar1.Value = 0;
            tspBar1.Visible = false;
            FYtype = "yjf";//2014-11-2,wm add
        }

        private void IsFYYTFileExistAndShowIcon(CSivFileInfo sfiM)
        {
            //先隐藏所有通道图标，2015-6-4,wm add
            tsbIR1.Enabled = false;
            tsbIR2.Enabled = false;
            tsbIR3.Enabled = false;
            tsbIR4.Enabled = false;
            tsbVIS1.Enabled = false;

            tsmIR1.Enabled = false;
            tsmIR2.Enabled = false;
            tsmIR3.Enabled = false;
            tsmIR4.Enabled = false;
            tsmVIS1.Enabled = false;

            tsbL1.Enabled = true;
            tsbJ3.Enabled = true;
            tsbD4.Enabled = true;
            muL1.Enabled = true;
            muJ3.Enabled = true;
            muD4.Enabled = true;

            //------------------------------------------------------------------------
            //判断对应云图各通道文件是否存在，并关联界面通道图标显示，2015-6-4，wm add
            string sVissPathName_VIS = sfiM.getVissPathName(2, 0, iNowProj);
            string sVissPathName_IR4 = sfiM.getVissPathName(1, 3, iNowProj);
            string sVissPathName_IR3 = sfiM.getVissPathName(1, 2, iNowProj);
            string sVissPathName_IR2 = sfiM.getVissPathName(1, 1, iNowProj);
            string sVissPathName_IR1 = sfiM.getVissPathName(1, 0, iNowProj);

            if (File.Exists(sVissPathName_VIS))
            {
                tsbVIS1.Enabled = true; tsmVIS1.Enabled = true;//图标显示
                if (SameCount == 0)//首次点击此条目
                {
                    iNowType = 2; iNowWay = 0;
                }
            }
            if (File.Exists(sVissPathName_IR4))
            {
                tsbIR4.Enabled = true; tsmIR4.Enabled = true;
                if (SameCount == 0)//首次点击此条目
                {
                    iNowType = 1; iNowWay = 3;
                }
            }
            if (File.Exists(sVissPathName_IR3))
            {
                tsbIR3.Enabled = true; tsmIR3.Enabled = true;
                if (SameCount == 0)//首次点击此条目
                {
                    iNowType = 1; iNowWay = 2;
                }
            }
            if (File.Exists(sVissPathName_IR2))
            {
                tsbIR2.Enabled = true; tsmIR2.Enabled = true;
                if (SameCount == 0)//首次点击此条目
                {
                    iNowType = 1; iNowWay = 1;
                }
            }
            if (File.Exists(sVissPathName_IR1))
            {
                tsbIR1.Enabled = true; tsmIR1.Enabled = true;
                if (SameCount == 0)//首次点击此条目
                {
                    iNowType = 1; iNowWay = 0;
                }
            }
            //------------------------------------------------------------------------

            //根据当前通道，重置图标，2015-6-4,wm add
            if (iNowType == 2)
            {
                tsbVIS_ClickDealing();
                tsbVIS1.Checked = true;
            }
            else
            {
                if (iNowWay == 3)
                {
                    tsbIR4_ClickDealing();
                    tsbIR4.Checked = true;
                }
                else if (iNowWay == 1)
                {
                    tsbIR2_ClickDealing();
                    tsbIR2.Checked = true;
                }
                else if (iNowWay == 2)
                {
                    tsbIR3_ClickDealing();
                    tsbIR3.Checked = true;
                }
                else
                {
                    tsbIR1_ClickDealing();
                    tsbIR1.Checked = true;
                }
            }
        }

        private void lBoxLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isShowLB)
            {
                isShowLB = true;
                return;
            }

            //listBoxMap.ClearSelected();//2014-10-21 wm add
            //显示雷暴
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = lBoxLB.SelectedIndex;
            if ((k >= 0) && (k < lBoxLB.Items.Count))
            {
                #region 显示地图
                CLBFileInfo sfi = lBoxLB.Items[k] as CLBFileInfo;
                if (sfi != null)
                {
                    //=================================================================================
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sLayName = sfi.sLayName;
                    string sYTName = sfi.getYTName();

                    //-------------------------------------------------------------------------------
                    //2015-6-4,wm add
                    if ((FYtypeTemp == "lb") && (YTNameTemp == sYTName))//重复点击积冰同一条目或切换通道
                    {
                        SameCount++;
                    }
                    else
                    {
                        //首次点击此条目，则记录此条目标识
                        FYtypeTemp = "lb";
                        YTNameTemp = sYTName;
                        SameCount = 0;
                    }
                    //-------------------------------------------------------------------------------

                    //校验是否已经存在
                    bool bIsExit = false;
                    bool bLayerChanged = false;
                    bool bMainMapOpen = false;

                    //更新图层？
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)//保留底图
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除

                            #region 原代码，不用
                            /*
                                string ss = lay.Name;

                                //2014-7-28 改成全部隐藏
                                {
                                    lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                    lay.Visible = false;    //隐藏图层

                                    //开启主图
                                    if (ss.Equals(sYTName))
                                    {
                                        //lay.IsTopLayer = false;
                                        //lay.Visible = true;
                                        //bMainMapOpen = true;

                                        baseMap.WMap.LayerList.RemoveAt(i);
                                    }

                                    if (ss.Equals(sLayName))
                                    {
                                        //已经存在                                    
                                        //lay.Visible = true;
                                        //lay.IsTopLayer = true;                                                                                
                                        //bLayerChanged = true;
                                        //bIsExit = true;

                                        baseMap.WMap.LayerList.RemoveAt(i);
                                    }
                                }
                                */
                            #endregion
                        }
                    }
                    if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                    {
                        for (int i = 0; i < listFYYT.Count; i++)
                        {
                            CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    IsFYYTFileExistAndShowIcon(sfiM);//2015-6-4,wm add

                                    string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                    if (!File.Exists(sVissPathName))
                                    {
                                        ShowStatusInfo("云图数据文件不存在！");
                                        return;
                                    }
                                    IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                    if (dp != null)
                                    {
                                        if (dp.Result is ICustmoLayer)
                                        {
                                            //新添加图层
                                            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                            lays.Visible = true;
                                            lays.Style = userStyle;
                                            lays.Style.PalletPathName = sNowPalFileName;
                                            lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                            lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                            bMainMapOpen = true;//主图打开
                                            baseMap.WMap.AddLayer(lays);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (!bMainMapOpen)
                    {
                        //主图未打开
                        bool bTmp = false;
                        for (int i = 0; i < listBoxMap.Items.Count; i++)
                        {
                            CSivFileInfo sfiM = listBoxMap.Items[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    bTmp = true;
                                    bLayerChanged = false;//刷新标致清零
                                    listBoxMap.SelectedIndex = i;
                                    bMainMapOpen = true;//主图打开
                                    break;
                                }
                            }
                        }
                    }
                    if (chkShowLB.Checked)
                    {
                        #region 载入雷暴
                        string sPath = sfi.sPath;

                        if (!File.Exists(sPath))
                        {
                            baseMap.Render();
                            ShowStatusInfo("雷暴数据文件不存在！");
                            return;
                        }

                        IDataPlugin dp = PluginManager.getInstance().OpenData(sPath);
                        if (dp != null)
                        {
                            if (dp.Result is ICustmoLayer)
                            {
                                //新添加图层
                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                lays.Visible = true;
                                lays.IsTopLayer = true;

                                baseMap.WMap.AddLayer(lays);

                                //sNowSelYJFLayName = lays.Name;
                                bLayerChanged = true;
                            }
                        }
                        #endregion
                    }

                    //baseMap.Render();

                    //20150416 ZJB add
                    loadAllExistedAirspace();

                    //图层可见 2014-10-21 wm add
                    //for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    //{
                    //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    //    lay.Visible = true;
                    //}
                    //=================================================================================                        
                    #region 控制最多载入图层数量

                    int iUserLayer = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (!lay.IsBaseMap)
                        {
                            iUserLayer++;
                        }
                    }

                    int m = 0;
                    while (iUserLayer > userSysParams.LoadLayerMax)
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                iUserLayer--;

                                ClearMemory();
                                break;
                            }
                        }
                        m++;

                        if (m > 10)
                        {
                            break;
                        }
                    }

                    #endregion
                    //=================================================================================                         
                    if (bLayerChanged)
                    {
                        baseMap.Render();
                    }
                    //=================================================================================                        
                }
                #endregion
                listMapLays();
                tspBar1.Value = 0;
                tspBar1.Visible = false;
                FYtype = "lb";//2014-11-2,wm add
            }
        }

        //三个成员用来确定雷暴积冰海雾同一条目是否初次加载，2015-6-4,wm add
        private string FYtypeTemp = string.Empty;
        private string YTNameTemp = string.Empty;
        private uint SameCount = 0;

        //2014-10-14 wm add
        private void lBoxJB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isShowJB)
            {
                isShowJB = true;
                return;
            }

            //listBoxMap.ClearSelected();//2014-10-21 wm add
            //显示积冰
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = lBoxJB.SelectedIndex;
            if ((k >= 0) && (k < lBoxJB.Items.Count))
            {
                #region 显示地图

                CJBFileInfo sfi = lBoxJB.Items[k] as CJBFileInfo;
                if (sfi != null)
                {
                    //=================================================================================
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sLayName = sfi.sLayName;
                    string sYTName = sfi.getYTName();

                    //-------------------------------------------------------------------------------
                    //2015-6-4,wm add
                    if ((FYtypeTemp == "jb") && (YTNameTemp == sYTName))//重复点击积冰同一条目或切换通道
                    {
                        SameCount++;
                    }
                    else
                    {
                        //首次点击此条目，则记录此条目标识
                        FYtypeTemp = "jb";
                        YTNameTemp = sYTName;
                        SameCount = 0;
                    }
                    //-------------------------------------------------------------------------------

                    bool bMainMapOpen = false;

                    //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除
                        }
                    }
                    if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                    {
                        for (int i = 0; i < listFYYT.Count; i++)
                        {
                            CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    IsFYYTFileExistAndShowIcon(sfiM);//2015-6-4,wm add

                                    string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                    if (!File.Exists(sVissPathName))//防止所有通道文件不存在情况
                                    {
                                        ShowStatusInfo("云图数据文件不存在！");
                                        return;
                                    }
                                    IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                    if (dp != null)
                                    {
                                        if (dp.Result is ICustmoLayer)
                                        {
                                            //新添加图层
                                            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                            lays.Visible = true;
                                            lays.Style = userStyle;
                                            lays.Style.PalletPathName = sNowPalFileName;
                                            lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                            lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                            bMainMapOpen = true;//主图打开
                                            baseMap.WMap.AddLayer(lays);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }


                    //if (!bIsExit)
                    //{
                    #region 载入积冰
                    if (chkShowJB.Checked)
                    {
                        string sPath = sfi.sPath;

                        if (!File.Exists(sPath))
                        {
                            baseMap.Render();
                            ShowStatusInfo("积冰数据文件不存在！");
                            return;
                        }

                        IDataPlugin dp = PluginManager.getInstance().OpenData(sPath);
                        if (dp != null)
                        {
                            if (dp.Result is ICustmoLayer)
                            {
                                //新添加图层
                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                lays.Visible = true;
                                lays.IsTopLayer = true;
                                baseMap.WMap.AddLayer(lays);
                            }
                        }
                    }//if (sfi != null)
                    #endregion

                    //图层可见 2014-10-21 wm add
                    //for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    //{
                    //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    //    lay.Visible = true;
                    //}

                    //20150416 ZJB added
                    loadAllExistedAirspace();

                    //=================================================================================                        
                    #region 控制最多载入图层数量

                    int iUserLayer = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (!lay.IsBaseMap)
                        {
                            iUserLayer++;
                        }
                    }

                    int m = 0;
                    while (iUserLayer > userSysParams.LoadLayerMax)
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                iUserLayer--;

                                ClearMemory();
                                break;
                            }
                        }
                        m++;

                        if (m > 10)
                        {
                            break;
                        }
                    }

                    #endregion
                    //=================================================================================
                    baseMap.Render();
                    //=================================================================================                        
                }
                #endregion
            }

            listMapLays();

            tspBar1.Value = 0;
            tspBar1.Visible = false;
            FYtype = "jb";//2014-11-2,wm add

            #region 积冰原代码，不用
            /*
            if (chkShowJB.Checked)
            {
                if (iNowType == 2)//1=IR,2=VIS
                {

                    ShowStatusInfo("可见光无法执行该操作!");
                    return;
                }

                //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                if (iNowProj != 2 && iNowProj != 4)
                {
                    ShowStatusInfo("该投影下无法执行此操作!");
                    return;
                }

                int k = lBoxJB.SelectedIndex;
                if ((k >= 0) && (k < lBoxJB.Items.Count))
                {
                    string sName = Convert.ToString(lBoxJB.Items[k]);

                    bool bFinded = false;
                    for (int i = 0; i < listBoxMap.Items.Count; i++)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[i] as CSivFileInfo;
                        if (sfi != null)
                        {
                            string sShowName = sfi.ToString();
                            if (sShowName.Equals(sName))
                            {
                                bFinded = true;
                                //listBoxMap_SelectedIndexChanged(sender, e);
                                listBoxMap.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    if (bFinded)
                    {
                        #region 开启显示

                        bool bLayerChanged = true;
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                sName = lay.Name;
                                if (sName.Equals(sNowSelLayName))
                                {
                                    string sParams = "jb:1";
                                    lay.SetParams(sParams);
                                    bLayerChanged = true;
                                }
                            }
                        }

                        if (bLayerChanged)
                        {
                            baseMap.Render();
                        }

                        #endregion
                    }
                }
            }
            */
            #endregion
        }

        //2014-10-14 wm add
        private void lBoxHW_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isShowHW)
            {
                isShowHW = true;
                return;
            }

            //listBoxMap.ClearSelected();//2014-10-21 wm add
            //显示海雾
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = lBoxHW.SelectedIndex;
            if ((k >= 0) && (k < lBoxHW.Items.Count))
            {
                #region 显示地图


                CHWFileInfo sfi = lBoxHW.Items[k] as CHWFileInfo;
                if (sfi != null)
                {
                    //=================================================================================
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sLayName = sfi.sLayName;
                    string sYTName = sfi.getYTName();

                    //-------------------------------------------------------------------------------
                    //2015-6-4,wm add
                    if ((FYtypeTemp == "hw") && (YTNameTemp == sYTName))//重复点击积冰同一条目或切换通道
                    {
                        SameCount++;
                    }
                    else
                    {
                        //首次点击此条目，则记录此条目标识
                        FYtypeTemp = "hw";
                        YTNameTemp = sYTName;
                        SameCount = 0;
                    }
                    //-------------------------------------------------------------------------------

                    //校验是否已经存在
                    bool bIsExit = false;
                    bool bLayerChanged = false;
                    bool bMainMapOpen = false;

                    //只保留 全相同的底图
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除，2014-10-21 wm add

                            #region 原代码，不用
                            /*
                                string ss = lay.Name;

                                //2014-7-28 改成全部隐藏
                                {
                                    lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                    lay.Visible = false;    //隐藏图层

                                    //开启主图
                                    if (ss.Equals(sYTName))
                                    {
                                        //lay.IsTopLayer = false;
                                        //lay.Visible = true;
                                        //bMainMapOpen = true;

                                        baseMap.WMap.LayerList.RemoveAt(i);
                                    }

                                    if (ss.Equals(sLayName))
                                    {
                                        //已经存在                                    
                                        //lay.Visible = true;
                                        //lay.IsTopLayer = true;                                                                                
                                        //bLayerChanged = true;
                                        //bIsExit = true;

                                        baseMap.WMap.LayerList.RemoveAt(i);
                                    }
                                }
                                */
                            #endregion
                        }
                    }

                    if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                    {
                        for (int i = 0; i < listFYYT.Count; i++)
                        {
                            CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    IsFYYTFileExistAndShowIcon(sfiM);//2015-6-4,wm add

                                    string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                    if (!File.Exists(sVissPathName))
                                    {
                                        ShowStatusInfo("云图数据文件不存在！");
                                        return;
                                    }
                                    IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                    if (dp != null)
                                    {
                                        if (dp.Result is ICustmoLayer)
                                        {
                                            //新添加图层
                                            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                            lays.Visible = true;
                                            lays.Style = userStyle;
                                            lays.Style.PalletPathName = sNowPalFileName;
                                            lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                            lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                            bMainMapOpen = true;//主图打开
                                            baseMap.WMap.AddLayer(lays);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (!bMainMapOpen)//主图仍没有到listBoxMap找
                    {
                        //主图未打开
                        bool bTmp = false;
                        for (int i = 0; i < listBoxMap.Items.Count; i++)
                        {
                            CSivFileInfo sfiM = listBoxMap.Items[i] as CSivFileInfo;
                            if (sfiM != null)
                            {
                                string sShowName = sfiM.ToString();
                                if (sShowName.Equals(sYTName))
                                {
                                    bTmp = true;
                                    bLayerChanged = false;//刷新标致清零
                                    listBoxMap.SelectedIndex = i;//加入云图图层在listBoxMap的选中事件
                                    bMainMapOpen = true;//主图打开
                                    break;
                                }
                            }
                        }
                        //将主图放置底部
                        //if (bTmp)
                        //{
                        //    for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        //    {
                        //        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        //        if (!lay.IsBaseMap)
                        //        {
                        //            string ss = lay.Name;
                        //            if (ss.Equals(sYTName))
                        //            {
                        //                lay.IsTopLayer = false;
                        //                lay.Visible = true;
                        //                bMainMapOpen = true;
                        //            }
                        //        }
                        //    }
                        //}
                    }

                    //if (!bIsExit)//加入反演产品图层
                    //{
                    if (chkShowHW.Checked)
                    {
                        #region 载入海雾
                        string sPath = sfi.sPath;

                        if (!File.Exists(sPath))
                        {
                            baseMap.Render();
                            ShowStatusInfo("海雾数据文件不存在！");
                            return;
                        }

                        IDataPlugin dp = PluginManager.getInstance().OpenData(sPath);
                        if (dp != null)
                        {
                            if (dp.Result is ICustmoLayer)
                            {
                                //新添加图层
                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                lays.Visible = true;
                                lays.IsTopLayer = true;

                                baseMap.WMap.AddLayer(lays);

                                //sNowSelYJFLayName = lays.Name;
                                bLayerChanged = true;
                            }
                        }
                        #endregion
                    }//if (sfi != null)

                    //20150416 ZJB added
                    loadAllExistedAirspace();//载入之前设置为可见的所有空域；
                    //}//if (!bIsExit)

                    //图层可见 2014-10-21 wm add
                    //for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    //{
                    //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    //    lay.Visible = true;
                    //}

                    //=================================================================================                        
                    #region 控制最多载入图层数量

                    int iUserLayer = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (!lay.IsBaseMap)
                        {
                            iUserLayer++;
                        }
                    }

                    int m = 0;
                    while (iUserLayer > userSysParams.LoadLayerMax)//max=10
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                iUserLayer--;

                                ClearMemory();
                                break;
                            }
                        }
                        m++;

                        if (m > 10)
                        {
                            break;
                        }
                    }


                    #endregion
                    //=================================================================================                         
                    //if (bLayerChanged)
                    //{
                    baseMap.Render();
                    //}
                    //=================================================================================                        
                }

                #endregion
                listMapLays();
            }

            tspBar1.Value = 0;
            tspBar1.Visible = false;
            FYtype = "hw";//2014-11-2,wm add
            #region 海雾原代码，不用
            /*
            if (chkShowHW.Checked)
            {
                if (iNowType == 2)//1=IR,2=VIS
                {

                    ShowStatusInfo("可见光无法执行该操作!");
                    return;
                }

                //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                if (iNowProj != 2 && iNowProj != 4)
                {
                    ShowStatusInfo("该投影下无法执行此操作!");
                    return;
                }

                int k = lBoxHW.SelectedIndex;
                if ((k >= 0) && (k < lBoxHW.Items.Count))
                {
                    string sName = Convert.ToString(lBoxHW.Items[k]);

                    bool bFinded = false;
                    for (int i = 0; i < listBoxMap.Items.Count; i++)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[i] as CSivFileInfo;
                        if (sfi != null)
                        {
                            string sShowName = sfi.ToString();
                            if (sShowName.Equals(sName))
                            {
                                bFinded = true;
                                //listBoxMap_SelectedIndexChanged(sender, e);
                                listBoxMap.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    if (bFinded)
                    {
                        #region 开启显示

                        bool bLayerChanged = true;
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                sName = lay.Name;
                                if (sName.Equals(sNowSelLayName))
                                {
                                    string sParams = "hw:1";
                                    lay.SetParams(sParams);
                                    bLayerChanged = true;
                                }
                            }
                        }

                        if (bLayerChanged)
                        {
                            baseMap.Render();
                        }

                        #endregion
                    }
                }
            }
            */
            #endregion
        }

        public bool AirLineChoosed = true;

        private void lBoxDZX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isShowDZX)
            {
                isShowDZX = true;
                return;
            }
            tsbIR1_ClickDealing();
            tsbM2_Click(null, null);
            tsbIR1.Checked = true;
            tsbIR2.Enabled = false;
            tsbIR3.Enabled = false;
            tsbIR4.Enabled = false;
            tsbVIS1.Enabled = false;
            tsmIR2.Enabled = false;
            tsmIR3.Enabled = false;
            tsmIR4.Enabled = false;
            tsmVIS1.Enabled = false;
            tsbM2.Checked = true;
            tsbL1.Enabled = false;
            tsbJ3.Enabled = false;
            tsbD4.Enabled = false;
            muL1.Enabled = false;
            muJ3.Enabled = false;
            muD4.Enabled = false;

            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            if (iNowProj != 2)
            {
                MessageBox.Show("只有在红外一的麦卡托投影下才能查看等值线!");
                return;
            }

            //listBoxMap.ClearSelected();//2014-10-21 wm add
            //显示等值线
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(70);
            Application.DoEvents();

            int k = lBoxDZX.SelectedIndex;
            if ((k >= 0) && (k < lBoxDZX.Items.Count))
            {
                string mainMapFilePath;
                CDZXFileInfo fileInfo = lBoxDZX.Items[k] as CDZXFileInfo;
                string mapName = fileInfo.getYTName();
                for (int i = 0; i < listFYYT.Count; i++)
                {
                    CSivFileInfo cFileInfo = listFYYT[i] as CSivFileInfo;
                    if (cFileInfo.sName == mapName)
                    {
                        mainMapFilePath = cFileInfo.getVissPathName(1, 0, 2);
                    }
                }
                #region 显示地图
                CDZXFileInfo dzxFileInfo = lBoxDZX.Items[k] as CDZXFileInfo;
                if (dzxFileInfo != null)
                {
                    //=================================================================================
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sLayName = dzxFileInfo.layerName;
                    string sYTName = dzxFileInfo.getYTName();

                    //校验是否已经存在
                    bool bLayerChanged = false;
                    bool bMainMapOpen = false;

                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除
                        }
                    }
                    if (chkShowDZXyt.Checked)
                    {
                        if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                        {//王猛版本，此版本和主图相无关联
                            for (int i = 0; i < listFYYT.Count; i++)
                            {
                                CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                                if (sfiM != null)
                                {
                                    string sShowName = sfiM.ToString();
                                    if (sShowName.Equals(sYTName))
                                    {
                                        string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                        if (!File.Exists(sVissPathName))
                                        {
                                            ShowStatusInfo("云图数据文件不存在！");
                                            return;
                                        }
                                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                        if (dp != null)
                                        {
                                            if (dp.Result is ICustmoLayer)
                                            {
                                                //新添加图层
                                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                                lays.Visible = true;
                                                lays.Style = userStyle;
                                                lays.Style.PalletPathName = sNowPalFileName;
                                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                                bMainMapOpen = true;//主图打开
                                                baseMap.WMap.AddLayer(lays);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //if (!bMainMapOpen)
                    //{//于元明版本，此版本和主图相关联
                    //    //主图未打开
                    //    for (int i = 0; i < listBoxMap.Items.Count; i++)
                    //    {
                    //        CSivFileInfo sfiM = listBoxMap.Items[i] as CSivFileInfo;
                    //        if (sfiM != null)
                    //        {
                    //            string sShowName = sfiM.ToString();
                    //            if (sShowName.Equals(sYTName))
                    //            {
                    //                bLayerChanged = false;//刷新标致清零
                    //                listBoxMap.SelectedIndex = i;
                    //                bMainMapOpen = true;//主图打开
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    if (chkShowDZX.Checked)
                    {
                        #region 载入等值线，添加新图层
                        if (chkShowJB.Checked)
                        {
                            string filePath = dzxFileInfo.filePath;

                            if (!File.Exists(filePath))
                            {
                                baseMap.Render();
                                ShowStatusInfo("等值线数据文件不存在！");
                                return;
                            }

                            IDataPlugin dp = PluginManager.getInstance().OpenData(filePath);
                            if (dp != null)
                            {
                                if (dp.Result is ICustmoLayer)
                                {
                                    //新添加图层
                                    ICustmoLayer layer = (ICustmoLayer)(dp.Result);
                                    layer.Visible = true;
                                    layer.IsTopLayer = true;
                                    baseMap.WMap.AddLayer(layer);
                                    bLayerChanged = true;
                                }
                            }
                        }
                        #endregion
                    }
                    baseMap.Render();
                    //=================================================================================                        
                    #region 控制最多载入图层数量

                    int iUserLayer = 0;
                    foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                    {
                        if (!lay.IsBaseMap)
                        {
                            iUserLayer++;
                        }
                    }

                    int m = 0;
                    while (iUserLayer > userSysParams.LoadLayerMax)
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                iUserLayer--;
                                ClearMemory();
                                break;
                            }
                        }
                        m++;
                        if (m > 10)
                        {
                            break;
                        }
                    }

                    #endregion
                    //=================================================================================                         
                    if (bLayerChanged)
                    {
                        baseMap.Render();
                    }
                    //=================================================================================
                }
                #endregion

                listMapLays();
            }

            tspBar1.Value = 0;
            tspBar1.Visible = false;
            FYtype = "dzx";
            //------------------------------------------------------------------------------------------------------------------------------------------

            #region 旧代码
            //if (chkShowDZX.Checked)
            //{
            //    if (iNowType == 2)//1=IR,2=VIS
            //    {

            //        ShowStatusInfo("可见光无法执行该操作!");
            //        return;
            //    }

            //    //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            //    if (iNowProj != 2 && iNowProj != 4)
            //    {
            //        ShowStatusInfo("该投影下无法执行此操作!");
            //        return;
            //    }

            //    int k = lBoxDZX.SelectedIndex;
            //    if ((k >= 0) && (k < lBoxDZX.Items.Count))
            //    {
            //        string sName = Convert.ToString(lBoxDZX.Items[k]);

            //        bool bFinded = false;
            //        for (int i = 0; i < listBoxMap.Items.Count; i++)
            //        {
            //            CSivFileInfo sfi = listBoxMap.Items[i] as CSivFileInfo;
            //            if (sfi != null)
            //            {
            //                string sShowName = sfi.ToString();
            //                if (sShowName.Equals(sName))
            //                {
            //                    bFinded = true;
            //                    //listBoxMap_SelectedIndexChanged(sender, e);
            //                    listBoxMap.SelectedIndex = i;
            //                    break;
            //                }
            //            }
            //        }

            //        if (bFinded)
            //        {
            //            #region 开启等值线显示

            //            bool bLayerChanged = true;
            //            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            //            {
            //                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
            //                if (!lay.IsBaseMap)
            //                {
            //                    sName = lay.Name;
            //                    if (sName.Equals(sNowSelLayName))
            //                    {
            //                        string sParams = "dzx:1";
            //                        //2014-9-4 add 不画底图、直接绘等值线
            //                        if (!chkShowDZXyt.Checked)
            //                        {
            //                            sParams = "dzx:2";
            //                        }

            //                        lay.SetParams(sParams);
            //                        bLayerChanged = true;
            //                    }
            //                }
            //            }

            //            if (bLayerChanged)
            //            {
            //                baseMap.Render();
            //            }

            //            #endregion
            //        }
            //    }
            //}
            #endregion
        }

        //2014-10-31 wm add
        private void lBoxLogicYT_SelectedIndexChanged(object sender, EventArgs e)
        {
            tsbIR1_ClickDealing();
            tsbIR1.Checked = true;
            tsbIR2.Enabled = false;
            tsbIR3.Enabled = false;
            tsbIR4.Enabled = false;
            tsbVIS1.Enabled = false;
            tsmIR2.Enabled = false;
            tsmIR3.Enabled = false;
            tsmIR4.Enabled = false;
            tsmVIS1.Enabled = false;
            tsbL1.Enabled = true;
            tsbJ3.Enabled = true;
            tsbD4.Enabled = true;
            muL1.Enabled = true;
            muJ3.Enabled = true;
            muD4.Enabled = true;

            //载入逻辑运算地图
            //显示地图
            tspBar1.Maximum = 100;
            SetProgressBarVal(20);
            tspBar1.Visible = true;
            Application.DoEvents();

            SetProgressBarVal(60);
            Application.DoEvents();

            int k = lBoxLogicYT.SelectedIndex;
            if ((k >= 0) && (k < lBoxLogicYT.Items.Count))
            {
                bool bMainMapOpen = false;
                CSivFileInfo sfi = lBoxLogicYT.Items[k] as CSivFileInfo;
                string YTName = sfi.sName;
                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        baseMap.WMap.LayerList.Remove(lay);//非底图全部移除
                    }
                }
                #region 添加主图
                if (!bMainMapOpen)//优先从反演云图列表找到对应云图，2014-10-27 wm add
                {
                    for (int i = 0; i < listFYYT.Count; i++)
                    {
                        CSivFileInfo sfiM = listFYYT[i] as CSivFileInfo;
                        if (sfiM != null)
                        {
                            string sShowName = sfiM.sName;
                            if (sShowName.Equals(YTName))
                            {
                                string sVissPathName = sfiM.getVissPathName(iNowType, iNowWay, iNowProj);
                                if (!File.Exists(sVissPathName))
                                {
                                    ShowStatusInfo("云图数据文件不存在！");
                                    return;
                                }
                                IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                if (dp != null)
                                {
                                    if (dp.Result is ICustmoLayer)
                                    {
                                        //新添加图层
                                        ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                        lays.Visible = true;
                                        lays.Style = userStyle;
                                        lays.Style.PalletPathName = sNowPalFileName;
                                        lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                        lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                        bMainMapOpen = true;//主图打开
                                        baseMap.WMap.AddLayer(lays);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                #endregion
                baseMap.Render();
            }
        }
        private void listBoxLogic_SelectedIndexChanged(object sender, EventArgs e)
        {//载入逻辑运算地图

            tsbIR1_ClickDealing();
            tsbIR1.Checked = true;
            tsbIR2.Enabled = false;
            tsbIR3.Enabled = false;
            tsbIR4.Enabled = false;
            tsbVIS1.Enabled = false;
            tsmIR2.Enabled = false;
            tsmIR3.Enabled = false;
            tsmIR4.Enabled = false;
            tsmVIS1.Enabled = false;
            tsbL1.Enabled = true;
            tsbJ3.Enabled = true;
            tsbD4.Enabled = true;
            muL1.Enabled = true;
            muJ3.Enabled = true;
            muD4.Enabled = true;

            int k = listBoxLogic.SelectedIndex;
            if ((k >= 0) && (k < listBoxLogic.Items.Count))
            {
                #region 显示地图

                if (nowWorkType == EWorkType.FYMap)
                {
                    CSivFileInfo sfi = listBoxLogic.Items[k] as CSivFileInfo;
                    if (sfi != null)
                    {
                        for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                        {
                            ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                            if (!lay.IsBaseMap)
                            {
                                baseMap.WMap.LayerList.RemoveAt(i);
                                i--;
                            }
                        }
                        //===========================================================
                        #region  载入逻辑减产品
                        string sVissPathName = sfi.sPath;
                        if (!File.Exists(sVissPathName))
                        {
                            baseMap.Render();
                            ShowStatusInfo("逻辑减数据文件不存在！");
                            return;
                        }
                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                        if (dp != null)
                        {
                            if (dp.Result is ICustmoLayer)
                            {
                                //新添加图层
                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                lays.Visible = true;
                                lays.IsTopLayer = true;
                                //-------------------------------------------
                                if (ucTrackBar1.iValue2 == 0)
                                {
                                    ucTrackBar1.iValue2 = 1;
                                }

                                lays.Style = userStyle;
                                lays.Style.PalletPathName = sNowPalFileName;//???
                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;

                                baseMap.WMap.AddLayer(lays);

                                sNowSelLayName = lays.Name;
                                //-------------------------------------------
                                object obj = lays.GetParams(null);
                                if (obj is VISParams)
                                {
                                    VISParams visP = obj as VISParams;
                                    if (visP != null)
                                    {
                                        ucTrackBar1.iContrastType = 0;
                                        ucTrackBar1.iNowType = iNowType;
                                        ucTrackBar1.dData = visP.dSigns2Data;
                                        ucTrackBar1.rePaint();
                                    }
                                }
                            }
                        }
                        #endregion
                        //=================================================================================                        
                        #region 控制最多载入图层数量

                        int iUserLayer = 0;
                        foreach (ICustmoLayer lay in baseMap.WMap.LayerList)
                        {
                            if (!lay.IsBaseMap)
                            {
                                iUserLayer++;
                            }
                        }

                        int m = 0;
                        while (iUserLayer > userSysParams.LoadLayerMax)
                        {
                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    baseMap.WMap.LayerList.RemoveAt(i);
                                    iUserLayer--;

                                    ClearMemory();
                                    break;
                                }
                            }
                            m++;

                            if (m > 10)
                            {
                                break;
                            }
                        }
                        #endregion

                    }//if (sfi != null)
                }
                //=================================================================================
                baseMap.Render();
                //=================================================================================   

                #endregion

                if ((k >= 0) && (k < listBoxLogic.Items.Count))
                {
                    listBoxLogic.SelectedIndex = k;
                }

                listMapLays();
                Refresh_lBoxLogicYT(k);//刷新相关云图列表,2014-10-31 wm add
                baseMap.Render();
            }

            FYtype = "logic";
        }

        //--------------------------------------------------------------------------

        private void chbShowYJF_CheckedChanged(object sender, EventArgs e)
        {
            if (!chbShowYJF.Checked)
            {
                bool bLayerChanged = CloseAllYJFLayer();

                if (bLayerChanged)
                {
                    baseMap.Render();
                }
            }
            else
            {
                listBoxYJF_SelectedIndexChanged(sender, e);
            }
        }

        private void chkShowLB_CheckedChanged(object sender, EventArgs e)
        {
            //显示雷暴
            if (!chkShowLB.Checked)
            {
                bool bLayerChanged = false;
                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        baseMap.WMap.LayerList.RemoveAt(i);
                        bLayerChanged = true;
                    }
                }

                if (bLayerChanged)
                {
                    //baseMap.Render();
                    ClearMemory();
                    listBoxMap_SelectedIndexChanged(sender, e);
                }
            }
            else
            {
                lBoxLB_SelectedIndexChanged(sender, e);
            }
        }

        private void chkShowJB_CheckedChanged(object sender, EventArgs e)
        {
            //显示积冰
            if (!chkShowJB.Checked)
            {
                bool bLayerChanged = false;
                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        baseMap.WMap.LayerList.RemoveAt(i);
                        bLayerChanged = true;
                    }
                }

                if (bLayerChanged)
                {
                    //baseMap.Render();
                    ClearMemory();
                    listBoxMap_SelectedIndexChanged(sender, e);
                }
            }
            else
            {
                lBoxJB_SelectedIndexChanged(sender, e);
            }
        }

        private void chkShowHW_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkShowDZX_CheckedChanged(object sender, EventArgs e)
        {
            lBoxDZX_SelectedIndexChanged(sender, e);
        }

        #endregion
        //--------------------------------------------------------------------------

        /// <summary>
        /// 关闭所有云迹风图层
        /// </summary>
        /// <returns></returns>
        public bool CloseAllYJFLayer()
        {
            bool bLayerChanged = false;

            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    string ss = lay.Name;
                    if (ss.IndexOf("云迹风") > 0)
                    {
                        //lay.Style.LimitRectangle = false;   //关闭显示区域控制
                        lay.Visible = false;    //隐藏图层
                        bLayerChanged = true;
                    }

                }
            }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)

            return bLayerChanged;
        }

        #region 自动显示云际风（暂时不用 王昌帅）
        public void autoLoadYJFByMapFile(CSivFileInfo sMapfi)
        {
            bool bHav = false;
            if (chbShowYJF.Checked)
            {
                DateTime dtMap = sMapfi.dtBegin;
                string sMapName = sMapfi.sName;

                for (int j = 0; j < listBoxYJF.Items.Count; j++)
                {
                    CYJFFileInfo sfi = listBoxYJF.Items[j] as CYJFFileInfo;
                    if (sfi != null)
                    {
                        //iNowType = 1; //1=IR,2=VIS
                        //iNowWay = 0;  //0-3:1~4 通道
                        string sLayName = sfi.sName;//sLayName

                        #region 判断类型、通道、时间

                        if (sMapName.Substring(0, 1).Equals("f"))
                        {
                            //2013-10-11_1503|FY-2E_IR1
                            if (sLayName.IndexOf("|FY") > 0)
                            {
                                //1=IR,2=VIS
                                if (iNowType == 1)
                                {
                                    if (sLayName.IndexOf("_IR" + Convert.ToString(iNowWay + 1)) > 0)
                                    {
                                        TimeSpan ts1 = new TimeSpan(dtMap.Ticks - sfi.dtBegin.Ticks);
                                        if (Math.Abs(ts1.TotalMinutes) < 10)
                                        {
                                            bHav = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (sLayName.IndexOf("_VIS" + Convert.ToString(iNowWay + 1)) > 0)
                                    {
                                        TimeSpan ts1 = new TimeSpan(dtMap.Ticks - sfi.dtBegin.Ticks);
                                        if (Math.Abs(ts1.TotalMinutes) < 10)
                                        {
                                            bHav = true;
                                        }
                                    }
                                }
                            }

                        }
                        else if (sMapName.Substring(0, 1).Equals("m"))
                        {
                            //2013-10-11_0702|MTSAT_IR1
                            if (sLayName.IndexOf("|MTSAT") > 0)
                            {
                                //1=IR,2=VIS
                                if (iNowType == 1)
                                {
                                    if (sLayName.IndexOf("_IR" + Convert.ToString(iNowWay + 1)) > 0)
                                    {
                                        TimeSpan ts1 = new TimeSpan(dtMap.Ticks - sfi.dtBegin.Ticks);
                                        if (Math.Abs(ts1.TotalMinutes) < 10)
                                        {
                                            bHav = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (sLayName.IndexOf("_VIS" + Convert.ToString(iNowWay + 1)) > 0)
                                    {
                                        TimeSpan ts1 = new TimeSpan(dtMap.Ticks - sfi.dtBegin.Ticks);
                                        if (Math.Abs(ts1.TotalMinutes) < 10)
                                        {
                                            bHav = true;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        if (bHav)
                        {
                            //校验是否已经存在
                            bool bIsExit = false;
                            //bool bLayerChanged = false;

                            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    string ss = lay.Name;
                                    if (ss.IndexOf("云迹风") > 0)
                                    {
                                        //lay.Style.LimitRectangle = false;   //关闭显示区域控制
                                        lay.Visible = false;    //隐藏图层

                                        if (ss.Equals(sLayName))
                                        {
                                            //已经存在                                    
                                            lay.Visible = true;
                                            lay.IsTopLayer = true;

                                            sNowSelYJFLayName = lay.Name;
                                            //bLayerChanged = true;
                                            bIsExit = true;
                                        }
                                    }

                                }
                            }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)

                            if (!bIsExit)
                            {
                                string sPath = sfi.sPath;

                                if (!File.Exists(sPath))
                                {
                                    //baseMap.Render();                                    
                                    return;
                                }

                                IDataPlugin dp = PluginManager.getInstance().OpenData(sPath);
                                if (dp != null)
                                {
                                    if (dp.Result is ICustmoLayer)
                                    {
                                        //新添加图层
                                        ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                                        lays.Visible = true;
                                        lays.IsTopLayer = true;

                                        baseMap.WMap.AddLayer(lays);

                                        sNowSelYJFLayName = lays.Name;
                                        //bLayerChanged = true;
                                    }
                                }
                            }//if (!bIsExit)

                            return;
                        }
                    }
                }//end for listBoxYJF.Items

                //由外层负责刷新地图
            }//end if

            if (!bHav)
            {
                //隐藏所有云迹风
                CloseAllYJFLayer();
            }
        }
        #endregion

        private void tsmiDelYJF_Click(object sender, EventArgs e)
        {
            //删除云迹风
            int k = listBoxYJF.SelectedIndex;
            if ((k >= 0) && (k < listBoxYJF.Items.Count))
            {
                CYJFFileInfo sfi = listBoxYJF.Items[k] as CYJFFileInfo;
                if (sfi != null)
                {
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sShowName = sfi.ToString();
                    string sLayName = sfi.sLayName;

                    if (MessageBox.Show("确定要删除云迹风：" + sShowName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                        == System.Windows.Forms.DialogResult.OK)
                    {
                        //del
                        try
                        {
                            WRFileUnit.writeLogStr("删除云迹风： " + sLayName);

                            File.Delete(sfi.sPath);

                            //移除图层列表
                            bool bLayerChanged = false;
                            for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    if (lay.Name.Equals(sLayName))
                                    {
                                        baseMap.WMap.LayerList.RemoveAt(i);
                                        bLayerChanged = true;
                                        break;
                                    }
                                }
                            }

                            //刷新文件列表
                            FindLoadYJFFileList();
                            listMapLays();

                            if (bLayerChanged)
                            {
                                baseMap.Render();
                            }
                        }
                        catch (Exception ex)
                        {
                            string ss = ex.ToString();
                        }
                    }
                }
            }
        }


        #region 逻辑运算减

        private void tsmiLJJ_Click(object sender, EventArgs e)
        {
            //逻辑减
            string sMapPathMain_L = string.Empty;    //被减数
            string sMapPathSub_L = string.Empty;    //减数
            string sMapPathMain_M = string.Empty;
            string sMapPathSub_M = string.Empty;
            string sMapPathMain_J = string.Empty;
            string sMapPathSub_J = string.Empty;
            string sMapPathMain_D = string.Empty;
            string sMapPathSub_D = string.Empty;

            int iParam = 0;  //阈值

            frmLogicOperation dlgSelLJJ = new frmLogicOperation();
            dlgSelLJJ.iNowType = iNowType;
            dlgSelLJJ.iNowWay = iNowWay;
            dlgSelLJJ.iNowProj = iNowProj;

            dlgSelLJJ.setMapFileList(listBoxMap);
            if (dlgSelLJJ.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //dlgSelYJF.Dispose();   
                sMapPathMain_L = dlgSelLJJ.sMapPathMain_L;
                sMapPathSub_L = dlgSelLJJ.sMapPathSub_L;

                sMapPathMain_M = dlgSelLJJ.sMapPathMain_M;
                sMapPathSub_M = dlgSelLJJ.sMapPathSub_M;

                sMapPathMain_J = dlgSelLJJ.sMapPathMain_J;
                sMapPathSub_J = dlgSelLJJ.sMapPathSub_J;

                sMapPathMain_D = dlgSelLJJ.sMapPathMain_D;
                sMapPathSub_D = dlgSelLJJ.sMapPathSub_D;

                iParam = dlgSelLJJ.iParam;  //阈值
            }
            else
            {
                return;
            }

            //if (MessageBox.Show("开始进行逻辑计算，请耐心等待完成，不要进行其它操作！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //    == DialogResult.No)
            //{
            //    return;
            //}

            ShowStatusInfo("开始生成...");
            Application.DoEvents();

            //开始计算
            tspBar1.Maximum = 100;
            tspBar1.Value = 0;
            tspBar1.Visible = true;

            string fileName1 = string.Empty, fileName2 = string.Empty, fileName3 = string.Empty, fileName4 = string.Empty;

            #region 生成过程
            if (sMapPathMain_L != null && sMapPathSub_L != null)
            {
                fileName1 = CreateLogic(sMapPathMain_L, sMapPathSub_L, iParam);
            }
            tspBar1.Value = 20;
            if (sMapPathMain_M != null && sMapPathSub_M != null)
            {
                fileName2 = CreateLogic(sMapPathMain_M, sMapPathSub_M, iParam);
            }
            tspBar1.Value = 40;
            if (sMapPathMain_J != null && sMapPathSub_J != null)
            {
                fileName3 = CreateLogic(sMapPathMain_J, sMapPathSub_J, iParam);
            }
            tspBar1.Value = 60;
            if (sMapPathMain_D != null && sMapPathSub_D != null)
            {
                fileName4 = CreateLogic(sMapPathMain_D, sMapPathSub_D, iParam);
            }
            tspBar1.Value = 80;
            #endregion

            tspBar1.Value = 100;
            tspBar1.Visible = false;
            ShowStatusInfo("生成完毕。");

            FindLoadLogicFileList2();

            if (selectedType == "logic")
            {
                for (int i = 0; i < listBoxLogic.Items.Count; i++)
                {
                    CSivFileInfo fInfo = listBoxLogic.Items[i] as CSivFileInfo;
                    string showName = fInfo.ToString();
                    if (fileName1.Contains(showName) || fileName2.Contains(showName) || fileName3.Contains(showName) || fileName4.Contains(showName))
                    {
                        listBoxLogic.SelectedIndex = i;
                        break;
                    }
                }
            }

            ClearMemory();
        }

        //逻辑减生成过程，2014-11-4 wm add
        public string CreateLogic(string sMapPathMain, string sMapPathSub, int iParam)
        {
            string fileName = string.Empty;
            LayerVISS lvis1 = new LayerVISS();//
            lvis1.Load(sMapPathMain);
            SVissrUnit svu1 = lvis1.visObj.svuObj;

            LayerVISS lvis2 = new LayerVISS();//
            lvis2.Load(sMapPathSub);
            SVissrUnit svu2 = lvis2.visObj.svuObj;

            int iLen1 = svu1.bVissData.Length;
            int iLen2 = svu2.bVissData.Length;

            byte[] bVissDataNew = new byte[iLen1];

            for (int i = 0; i < iLen1; i++)
            {
                byte bVal = svu1.bVissData[i];
                if (i >= 0 && i < iLen2)
                {
                    byte bVal2 = svu2.bVissData[i];
                    int iTmp = Math.Abs(bVal - bVal2);
                    //相差不多就去掉，相差较大才保留
                    if (iTmp < iParam)
                    {
                        //原有点
                        bVissDataNew[i] = 0;
                    }
                    else
                    {
                        //>=阈值  新增点
                        bVissDataNew[i] = bVal;
                    }
                }
            }

            #region 载入原主图数据头

            FileStream fs = new FileStream(sMapPathMain, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            //====================================================================
            int ilen = 48 + 10 + 4 * 256;	//先读出头+最长定标数据
            byte[] bbuf = br.ReadBytes(ilen);
            //====================================================================
            //13    2     协议版本，默认1
            //1=全图单通道数据，2=简单差值单通道数据，3=三角变换单通道数据,4=分通道分投影数据,5=MTSAT格式数据
            int iStructType = bbuf[13];

            int iType = bbuf[19]; //1=IR,2=VIS
            int iWay = bbuf[20];//0-3:1~4 通道

            //投影方式, 1: 兰布托投影, 2: 麦卡托投影, 3: 极射赤面投影, 4: 等经纬度投影
            int iProjectType = bbuf[46];
            //====================================================================
            int iSigns2Len = 4 * 256 + 10;

            if (iStructType == 4)//协议4 分通道分投影数据
            {

                if (iType == 1)//1=IR,2=VIS
                {
                    //IR
                    iSigns2Len = 4 * 256 + 10;
                }
                else
                {
                    //VIS
                    iSigns2Len = 4 * 64 + 10;
                }
            }
            else if (iStructType == 5)//协议5 MTSAT格式数据
            {
                //64,256,1024
                //bSigns2Data[0] = 0x01;
                int iSignsFlag = bbuf[48];
                if (iSignsFlag == 0)
                {
                    iSigns2Len = 4 * 64 + 10;//		
                }
                else if (iSignsFlag == 1)
                {
                    iSigns2Len = 4 * 256 + 10;//		
                }
                else if (iSignsFlag == 2)
                {
                    iSigns2Len = 4 * 1024 + 10;//		
                }
            }

            //确定文件头长度
            int iDataStart = 48 + iSigns2Len;

            #endregion

            string sFileName = Path.GetFileNameWithoutExtension(sMapPathMain);
            sFileName = sFileName + "_" + iStructType + "_" + iType + "_" + iWay + "_" + iProjectType + ".opm";

            #region 生成文件名

            sFileName = "";
            if (iStructType == 4)
            {
                //4=分通道分投影数据
                sFileName = sFileName + "f";
            }
            else if (iStructType == 5)
            {
                //5=MTSAT格式数据
                sFileName = sFileName + "m";
            }

            //sFileName = sFileName + svu1.dtBegin.ToString("yyMMdd_HHmm")
            //                + "-" + svu2.dtBegin.ToString("HHmm");
            sFileName = sFileName + svu1.dtBegin.ToString("yyMMddHHmm")
                            + "-" + svu2.dtBegin.ToString("yyMMddHHmm");//更改文件名，2014-10-31 wm add

            //1=IR,2=VIS
            if (iType == 1)
            {
                sFileName = sFileName + "[" + iParam + "I" + Convert.ToString(iWay + 1) + Convert.ToString(iProjectType) + "].opm";////阈值
            }
            else
            {
                sFileName = sFileName + "[" + iParam + "V" + Convert.ToString(iWay + 1) + Convert.ToString(iProjectType) + "].opm";////阈值
            }

            fileName = sFileName;
            #endregion

            //生成新图像文件，保存云图，使用主图的基础数据    @"E:\云导风\SatellitePower 0717\ljj\" 
            string sOutPathName = userSysParams.LogicOutPatn + sFileName;

            FileStream fs2 = new FileStream(sOutPathName, FileMode.Create, FileAccess.Write);

            //头数据
            //fs2.Write(svu.bHead_3, 0, svu.bHead_3.Length);
            ////西东地平点，没有作用了

            ////10+   4*64    VIS   定标数据2
            ////      4*1024  IR
            //fs2.Write(svu.bSigns2Data, 0, svu.bSigns2Data.Length);

            ////200   预留空字节
            //byte[] btmp = new byte[200];
            //fs2.Write(btmp, 0, 200);

            ////图像像素数据
            //fs2.Write(svu.bLineProData, 0, svu.bLineProData.Length);
            //--------------------------------------------------------
            fs2.Write(bbuf, 0, iDataStart);

            //200   预留空字节
            byte[] btmp = new byte[200];
            fs2.Write(btmp, 0, 200);

            //图像像素数据
            fs2.Write(bVissDataNew, 0, iLen1);

            fs2.Close();
            return fileName;
        }



        #region 逻辑减checkBox控件—chkShowLogic，2014-10-31 wm del
        /*
        private void chkShowLogic_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowLogic.Checked)
            {
                bool bLayerChanged = false;
                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        baseMap.WMap.LayerList.RemoveAt(i);
                        bLayerChanged = true;
                    }
                }//for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)

                if (bLayerChanged)
                {
                    //baseMap.Render();
                    ClearMemory();
                    listBoxMap_SelectedIndexChanged(sender, e);
                }
            }
        }
        */
        #endregion

        private void tsmiDelLJJ_Click(object sender, EventArgs e)
        {
            //删除逻辑减图
            int k = listBoxLogic.SelectedIndex;
            if ((k >= 0) && (k < listBoxLogic.Items.Count))
            {
                CSivFileInfo sfi = listBoxLogic.Items[k] as CSivFileInfo;
                if (sfi != null)
                {
                    //iNowType = 1; //1=IR,2=VIS
                    //iNowWay = 0;  //0-3:1~4 通道
                    string sShowName = sfi.ToString();
                    string sLayName = sShowName;// sfi.sLayName;

                    if (MessageBox.Show("确定要删除逻辑减结果：" + sShowName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                        == System.Windows.Forms.DialogResult.OK)
                    {
                        //del
                        try
                        {
                            WRFileUnit.writeLogStr("删除逻辑减： " + sLayName.Substring(0, 22));

                            //2014-11-4 wm add
                            string path = userSysParams.LogicOutPatn;
                            string name26 = sShowName.Substring(0, 27);//匹配前27个字符
                            foreach (FileInfo fi in new DirectoryInfo(path).GetFiles())
                            {
                                if (fi.Name.Length < 27)//防止某些文件名太小导致错误，2014-11-19 wm add
                                {
                                    continue;
                                }
                                else
                                {
                                    string name26_ = fi.Name.Substring(0, 27);
                                    string sPathName = fi.FullName;
                                    if (name26_.Equals(name26))
                                    {
                                        try
                                        {
                                            File.Delete(sPathName);
                                            WRFileUnit.writeLogStr("删除成功:" + sPathName);
                                        }
                                        catch (System.Exception ex)
                                        {
                                            WRFileUnit.writeLogStr("删除失败:" + sPathName);
                                        }
                                    }
                                }
                            }
                            //File.Delete(sfi.sPath);

                            //移除图层列表
                            bool bLayerChanged = false;
                            for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                            {
                                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                if (!lay.IsBaseMap)
                                {
                                    if (lay.Name.Equals(sLayName))
                                    {
                                        baseMap.WMap.LayerList.RemoveAt(i);
                                        bLayerChanged = true;
                                        break;
                                    }
                                }
                            }

                            //刷新文件列表
                            FindLoadLogicFileList();
                            listMapLays();
                            lBoxLogicYT.Items.Clear();//清空相关云图列表，2014-11-4 wm add

                            if (bLayerChanged)
                            {
                                baseMap.Render();
                            }
                        }
                        catch (Exception ex)
                        {
                            string ss = ex.ToString();
                        }
                    }
                }
            }
        }

        #endregion

        private void tsmiDelMap_Click(object sender, EventArgs e)
        {
            //删除地图
            int iSel = listBoxMap.SelectedIndex;
            if ((iSel >= 0) && (iSel < listBoxMap.Items.Count))
            {
                CSivFileInfo sfi = listBoxMap.Items[iSel] as CSivFileInfo;
                bool h_flag = HaveFYProduct(iSel);//判断是否有反演产品存在
                if (sfi != null)
                {
                    if (!h_flag)//不存在反演产品
                    {
                        #region 删除云图
                        string sShowName = sfi.ToString();
                        if (MessageBox.Show("确定要删除云图：" + sShowName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            //del
                            try
                            {
                                string sLayName = sfi.ToString();
                                WRFileUnit.writeLogStr("删除云图: " + sLayName);

                                List<string> lstMaps = new List<string>();

                                #region 保存文件列表
                                if (sfi.iStructType == 5)//MTSAT
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 11));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 12));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 13));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 14));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 10));
                                    }
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 11));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 12));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 13));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 14));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 10));
                                }
                                else//FY
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 5));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 6));
                                        lstMaps.Add(sfi.getVissPathName(1, i, 7));
                                    }
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 5));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 6));
                                    lstMaps.Add(sfi.getVissPathName(2, 0, 7));
                                }
                                #endregion

                                int iDelCount = 0;
                                //Directory.Delete(sysCfg.sActPath + sLayName, true);
                                for (int i = 0; i < lstMaps.Count; i++)
                                {
                                    string sPathName = lstMaps[i];
                                    if (File.Exists(sPathName))
                                    {
                                        File.Delete(sPathName);
                                        iDelCount++;
                                    }
                                }

                                //移除图层列表
                                bool bLayerChanged = false;
                                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        baseMap.WMap.LayerList.RemoveAt(i);
                                        bLayerChanged = true;
                                    }
                                }

                                //刷新文件列表
                                FindLoadFYMapFile();
                                listMapLays();

                                if (bLayerChanged)
                                {
                                    baseMap.Render();
                                }
                            }
                            catch (Exception ex)
                            {
                                string ss = ex.ToString();
                            }
                        }
                        #endregion
                    }
                    else//存在反演产品
                    {

                        string sShowName = sfi.ToString();
                        if (MessageBox.Show("确定要删除云图：" + sShowName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            bool delFYProduct = false;
                            if (MessageBox.Show("云图" + sShowName + "存在反演产品，是否同时删除反演产品？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                                == System.Windows.Forms.DialogResult.OK)
                            {
                                delFYProduct = true;
                            }
                            if (delFYProduct)
                            {
                                //del
                                #region 删除云图
                                try
                                {
                                    string sLayName = sfi.ToString();
                                    WRFileUnit.writeLogStr("删除云图: " + sLayName);

                                    List<string> lstMaps = new List<string>();

                                    #region 保存文件列表
                                    if (sfi.iStructType == 5)//MTSAT
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 11));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 12));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 13));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 14));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 10));
                                        }
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 11));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 12));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 13));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 14));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 10));
                                    }
                                    else//FY
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            lstMaps.Add(sfi.getVissPathName(1, i, 1));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 2));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 3));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 4));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 5));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 6));
                                            lstMaps.Add(sfi.getVissPathName(1, i, 7));
                                        }
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 1));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 2));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 3));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 4));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 5));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 6));
                                        lstMaps.Add(sfi.getVissPathName(2, 0, 7));
                                    }
                                    #endregion

                                    int iDelCount = 0;
                                    //Directory.Delete(sysCfg.sActPath + sLayName, true);
                                    for (int i = 0; i < lstMaps.Count; i++)
                                    {
                                        string sPathName = lstMaps[i];
                                        if (File.Exists(sPathName))
                                        {
                                            File.Delete(sPathName);
                                            iDelCount++;
                                        }
                                    }

                                    //移除图层列表
                                    bool bLayerChanged = false;
                                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                                    {
                                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                        if (!lay.IsBaseMap)
                                        {
                                            baseMap.WMap.LayerList.RemoveAt(i);
                                            bLayerChanged = true;
                                        }
                                    }

                                    //刷新文件列表
                                    FindLoadFYMapFile();
                                    listMapLays();

                                    if (bLayerChanged)
                                    {
                                        baseMap.Render();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string ss = ex.ToString();
                                }
                                #endregion

                                #region 删除反演云图列表对应项
                                for (int i = 0; i < listFYYT.Count; i++)
                                {
                                    if (listFYYT[i] == sfi)
                                    {
                                        listFYYT.RemoveAt(i);
                                    }
                                }
                                #endregion

                                #region 删除反演产品
                                for (int k = 0; k < fyProduct_id.Count; k++)
                                {
                                    if (fyProduct_id[k] >= 9000000)//等值线
                                    {
                                        int dzx_index = fyProduct_id[k] - 9000000;
                                        DeleteDZX(dzx_index);
                                    }
                                    if (fyProduct_id[k] >= 8000000)//海雾
                                    {
                                        int hw_index = fyProduct_id[k] - 8000000;
                                        DeleteHW(hw_index);
                                    }
                                    else if (fyProduct_id[k] >= 7000000)//积冰
                                    {
                                        int jb_index = fyProduct_id[k] - 7000000;
                                        DeleteJB(jb_index);
                                    }
                                    else if (fyProduct_id[k] >= 6000000)//雷暴
                                    {
                                        int lb_index = fyProduct_id[k] - 6000000;
                                        DeleteLB(lb_index);
                                    }
                                    else if (fyProduct_id[k] >= 5000000)//云迹风
                                    {
                                        int yjf_index = fyProduct_id[k] - 5000000;
                                        DeleteYJF(yjf_index);
                                    }
                                }
                                #endregion
                            }
                            if (!delFYProduct)
                            {
                                #region 删除云图列表对应项
                                listBoxMap.Items.RemoveAt(iSel);
                                //listBoxMap.Refresh();
                                #endregion

                                //移除图层列表
                                bool bLayerChanged = false;
                                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        baseMap.WMap.LayerList.RemoveAt(i);
                                        bLayerChanged = true;
                                    }
                                }

                                listMapLays();

                                if (bLayerChanged)
                                {
                                    baseMap.Render();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void tsmitsmiAutoSaveAll_Click(object sender, EventArgs e)
        {
            //智能保存/批量保存
            if (iWorkingState != 0)//0=空闲，1=解析生数据中，2=动画生成中
            {
                ShowStatusInfo("正在工作中...");
                return;
            }
            //------------------------------------------------------------------------            
            sExportImgPaht = userSysParams.AutoSaveImgPath;
            PubUnit.safetyCreateDir(sExportImgPaht);

            if (nowWorkType == EWorkType.FYMap)
            {
                #region BuildFY

                //加入帧队列
                listExportLayNames.Clear();
                for (int k = listBoxMap.Items.Count - 1; k >= 0; k--)
                {
                    CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                    if (sfi != null)
                    {
                        #region 添加VIS图层，20150506，wm add

                        //获取预期的图层名
                        string sFilePathNameVis = sfi.getVissPathName(2, 0, iNowProj);
                        if (File.Exists(sFilePathNameVis))
                        {
                            string sTempLayerNameVis = sfi.getVissLayerName(2, 0, iNowProj);

                            //listExportLayNames.Add(sTempLayerName);

                            //string ss = sTempLayerName.Substring(0, 1);
                            //if (ss.Equals(sLayType, StringComparison.OrdinalIgnoreCase))
                            {
                                //判断同名文件是否存在
                                //userSysParams.AutoSaveImgPath + "图层名";
                                string sOutPathNameVis = sExportImgPaht + sTempLayerNameVis + ".jpg";

                                if (!File.Exists(sOutPathNameVis))//防止重复！
                                {
                                    listExportLayNames.Add(sTempLayerNameVis);//添加的是图层名
                                }
                            }
                        }

                        #endregion

                        #region 添加IR1或IR2图层，20150506，wm add
                        //获取预期的图层名
                        string sFilePathName = sfi.getVissPathName(1, 0, iNowProj);//IR1
                        if (File.Exists(sFilePathName))//IR1存在
                        {
                            string sTempLayerName = sfi.getVissLayerName(1, 0, iNowProj);

                            //判断同名文件是否存在
                            //userSysParams.AutoSaveImgPath + "图层名";
                            string sOutPathName = sExportImgPaht + sTempLayerName + ".jpg";
                            if (!File.Exists(sOutPathName))
                            {
                                listExportLayNames.Add(sTempLayerName);
                            }
                        }
                        else//IR1不存在
                        {
                            sFilePathName = sfi.getVissPathName(1, 1, iNowProj);//IR2
                            if (File.Exists(sFilePathName))//IR2存在
                            {
                                string sTempLayerName = sfi.getVissLayerName(1, 1, iNowProj);

                                string sOutPathName = sExportImgPaht + sTempLayerName + ".jpg";
                                if (!File.Exists(sOutPathName))
                                {
                                    listExportLayNames.Add(sTempLayerName);
                                }
                            }

                        }
                        #endregion
                    }
                }
                //----------------------------------------------------------
                //载入图层
                //int iflag = 0;//0=条件,1=全部
                if (listExportLayNames.Count > 0)
                {
                    for (int k = 0; k < listBoxMap.Items.Count; k++)
                    {
                        CSivFileInfo sfi = listBoxMap.Items[k] as CSivFileInfo;
                        if (sfi != null)
                        {
                            #region 保存VIS图片，20150506，wm add
                            //添加VIS图层,20150430，wm add

                            string sWayNameVis = sfi.sVissLayName;//默认通道地图文件名

                            //iNowType = 1;//1=IR,2=VIS
                            //iNowWay = 0;//0-3:1~4 通道
                            sWayNameVis = sfi.getVissLayerName(2, 0, iNowProj);

                            //判断是否在动画帧队列中
                            if (listExportLayNames.IndexOf(sWayNameVis) >= 0)
                            {
                                //校验是否已经存在
                                bool bIsExit = false;
                                //找云图
                                for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        lay.Visible = false;
                                        string ss = lay.Name;
                                        if (ss.Equals(sWayNameVis))
                                        {
                                            //已经存在
                                            //lay.Visible = true;
                                            bIsExit = true;
                                        }
                                    }
                                }
                                //-------------------------------------------
                                if (!bIsExit)
                                {
                                    string sVissPathName = sfi.sPath;
                                    //换算文件名
                                    sVissPathName = sfi.getVissPathName(2, 0, iNowProj);

                                    if (File.Exists(sVissPathName))
                                    {
                                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                        if (dp != null)
                                        {
                                            if (dp.Result is ICustmoLayer)
                                            {
                                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                                lays.Visible = false;
                                                lays.Style = userStyle;
                                                lays.Style.PalletPathName = sNowPalFileName;
                                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                                baseMap.WMap.AddLayer(lays);
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            string sWayName = sfi.sVissLayName;//默认通道地图文件名

                            #region 保存IR1图片，20150506，wm add
                            //iNowType = 1;//1=IR,2=VIS
                            //iNowWay = 0;//0-3:1~4 通道
                            sWayName = sfi.getVissLayerName(1, 0, iNowProj);

                            //判断是否在动画帧队列中
                            if (listExportLayNames.IndexOf(sWayName) >= 0)
                            {
                                //校验是否已经存在
                                bool bIsExit = false;
                                for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        lay.Visible = false;
                                        string ss = lay.Name;
                                        if (ss.Equals(sWayName))
                                        {
                                            //已经存在
                                            //lay.Visible = true;
                                            bIsExit = true;
                                        }
                                    }
                                }
                                //-------------------------------------------
                                if (!bIsExit)
                                {
                                    string sVissPathName = sfi.sPath;
                                    //换算文件名
                                    sVissPathName = sfi.getVissPathName(1, 0, iNowProj);

                                    if (File.Exists(sVissPathName))
                                    {
                                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                        if (dp != null)
                                        {
                                            if (dp.Result is ICustmoLayer)
                                            {
                                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                                lays.Visible = false;
                                                lays.Style = userStyle;
                                                lays.Style.PalletPathName = sNowPalFileName;//???
                                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                                baseMap.WMap.AddLayer(lays);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 保存IR2图片，20150506，wm add
                            //iNowType = 1;//1=IR,2=VIS
                            //iNowWay = 0;//0-3:1~4 通道
                            sWayName = sfi.getVissLayerName(1, 1, iNowProj);

                            //判断是否在动画帧队列中
                            if (listExportLayNames.IndexOf(sWayName) >= 0)
                            {
                                //校验是否已经存在
                                bool bIsExit = false;
                                for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
                                {
                                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                                    if (!lay.IsBaseMap)
                                    {
                                        lay.Visible = false;
                                        string ss = lay.Name;
                                        if (ss.Equals(sWayName))
                                        {
                                            //已经存在
                                            //lay.Visible = true;
                                            bIsExit = true;
                                        }
                                    }
                                }
                                //-------------------------------------------
                                if (!bIsExit)
                                {
                                    string sVissPathName = sfi.sPath;
                                    //换算文件名
                                    sVissPathName = sfi.getVissPathName(1, 1, iNowProj);

                                    if (File.Exists(sVissPathName))
                                    {
                                        IDataPlugin dp = PluginManager.getInstance().OpenData(sVissPathName);
                                        if (dp != null)
                                        {
                                            if (dp.Result is ICustmoLayer)
                                            {
                                                ICustmoLayer lays = (ICustmoLayer)(dp.Result);

                                                lays.Visible = false;
                                                lays.Style = userStyle;
                                                lays.Style.PalletPathName = sNowPalFileName;//???
                                                lays.Style.ShowMaxVal = ucTrackBar1.iValue1;
                                                lays.Style.ShowMinVal = ucTrackBar1.iValue2;
                                                baseMap.WMap.AddLayer(lays);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }//endif (sfi != null)
                    }
                }
                #endregion
            }

            listMapLays();

            if (listExportLayNames.Count <= 0)
            {
                //MessageBox.Show("没有地图文件可供生成动画！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStatusInfo("保存完毕");
                return;
            }

            //if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //string sTime = DateTime.Now.ToString("yyMMdd_HHmmss");
                //folderBrowserDialog1.Tag = 1;
                //string sPath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar.ToString();

                //if (MessageBox.Show("即将生成图像，请耐心等待，不要进行其他操作。", "生成动画", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                //    == System.Windows.Forms.DialogResult.OK)
                //{                
                autoSaveAllLayerImageNowView();
                //}
            }
        }

        public int autoSaveAllLayerImageNowView()
        {
            if (listExportLayNames.Count <= 0)
            {
                return 0;
            }

            int iRet = 0;
            //-------------------------------------------------------------------
            iExportImgFlag = 2;//开始标志，1=生成动画，2=自动批量保存图像
            iExportImgCount = 0;
            //-------------------------------------------------------------------
            //tspBar1.Maximum = listExportLayNames.Count;
            tspBar1.Visible = true;

            iExportImgIndex = 0;
            string sFirst = listExportLayNames[iExportImgIndex];

            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    lay.Visible = false;//隐藏其他非底图图层，20150711 wm add

                    string fn = lay.Name;
                    if (fn.Equals(sFirst))
                    {
                        sExportImgFileName = fn;

                        //一定要放在baseMap.Render()之前
                        iExportImgCount++;
                        iExportImgIndex++;

                        SetProgressBarVal(iExportImgCount);
                        ShowStatusInfo("正在生成第 " + iExportImgCount + " 幅");

                        Application.DoEvents();

                        lay.Visible = true;//仅显示云图图层，20150711 wm add
                        baseMap.Render();//MapCtrl.Render()->WeatherMap.Render()->MainForm.WMap_OnMapRendre()
                        break;
                    }

                }
            }
            //-------------------------------------------------------------------
            iRet = iExportImgIndex;
            return iRet;
        }

        private void autoBatSaveImg()
        {
            //调整可视区再保存
            goUserViewCenter();
            tsmitsmiAutoSaveAll_Click(null, null);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (!userSysParams.AutoSaveImg)
            {
                timer4.Enabled = false;
                return;
            }

            //定时批量保存图片
            timer4.Enabled = false;
            int intHour = DateTime.Now.Hour;

            int iRunH = 1;
            int iRunM = 0;

            DateTime dtRun = Convert.ToDateTime("1:00");
            if (DateTime.TryParse(userSysParams.AutoSaveImgTime, out dtRun))
            {
                iRunH = dtRun.Hour;
                iRunM = dtRun.Minute;
            }


            //每日1：00触发执行
            if (intHour == iRunH)
            {
                int intMinute = DateTime.Now.Minute;
                //int intSecond = DateTime.Now.Second;
                int iDt = Math.Abs(iRunM - intMinute);
                if (iDt <= 5)
                {
                    autoBatSaveImg();
                }
            }
            timer4.Enabled = true;

        }

        #region 云图扩展处理

        #region 雷暴积冰海雾等值线排序函数

        private static int SortYJFFileDesc(CYJFFileInfo a1, CYJFFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }

        private static int SortLBFileDesc(CLBFileInfo a1, CLBFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }

        //2014-10-14 wm add
        private static int SortJBFileDesc(CJBFileInfo a1, CJBFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }

        //2014-10-14 wm add
        private static int SortHWFileDesc(CHWFileInfo a1, CHWFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }

        private static int SortDZXFileDesc(CDZXFileInfo a1, CDZXFileInfo a2)
        {
            //if (a1.dtBegin>a2.dtBegin)
            //    return 1;
            //int k = a1.dtBegin.CompareTo(a2.dtBegin);
            int k = a2.dtBegin.CompareTo(a1.dtBegin);
            return k;
        }
        #endregion

        #region 雷暴积冰海雾等值线列表载入函数

        /// <summary>
        /// 列出云迹风文件
        /// </summary>
        private void FindLoadYJFFileList()
        {
            ShowStatusInfo("正在载入云迹风文件列表...");

            int k = listBoxYJF.SelectedIndex;//为了再次定位            
            listBoxYJF.Items.Clear();
            listYJFData.Clear();

            string sYJFPath = sysCfg.sYJFMapPath;//CSysConfig.sAppRootPath + Path.DirectorySeparatorChar.ToString() + "mapyjf";

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sYJFPath))
            {
                ShowStatusInfo("云迹风文件路径无效！");
                return;
            }

            //foreach (FileInfo fi in new DirectoryInfo(sysCfg.sIR1Path).GetFiles("*.siv"))
            foreach (FileInfo fi in new DirectoryInfo(sYJFPath).GetFiles("*.yjf"))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CYJFFileInfo sfi = new CYJFFileInfo();
                sfi.sPath = sPathName;
                sfi.loadHead();

                if (sfi.bIsMapFile)
                {
                    if (sfi.iDataRowCount > 0)
                    {
                        //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                        {
                            listYJFData.Add(sfi);
                        }
                    }
                }
            }

            //列表排序
            listYJFData.Sort(SortYJFFileDesc);

            //展现地图列表
            for (int i = 0; i < listYJFData.Count; i++)
            {
                CYJFFileInfo sfi = listYJFData[i];
                listBoxYJF.Items.Add(sfi);
            }

            ShowStatusInfo("云迹风文件列表载入完毕");
        }

        /// <summary>
        /// 载入雷暴图数据
        /// </summary>
        private void FindLoadLeiBaoFileList()
        {
            ShowStatusInfo("正在载入雷暴文件列表...");

            int k = lBoxLB.SelectedIndex;//为了再次定位            
            lBoxLB.Items.Clear();
            listLBData.Clear();

            string sLBPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "maplb" + Path.DirectorySeparatorChar.ToString();

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sLBPath))
            {
                ShowStatusInfo("雷暴文件路径无效！");
                return;
            }

            //foreach (FileInfo fi in new DirectoryInfo(sysCfg.sIR1Path).GetFiles("*.siv"))
            foreach (FileInfo fi in new DirectoryInfo(sLBPath).GetFiles("*.mlb"))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CLBFileInfo sfi = new CLBFileInfo();
                sfi.sPath = sPathName;
                sfi.loadHead();

                if (sfi.bIsMapFile)
                {
                    //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        listLBData.Add(sfi);
                    }
                }
            }
            //列表排序
            listLBData.Sort(SortLBFileDesc);

            //展现地图列表
            for (int i = 0; i < listLBData.Count; i++)
            {
                CLBFileInfo sfi = listLBData[i];
                lBoxLB.Items.Add(sfi);
            }

            ShowStatusInfo("雷暴文件列表载入完毕");
        }
        /// <summary>
        /// 载入积冰图数据,2014-10-14 wm add
        /// </summary>
        private void FindLoadJBFileList()
        {
            ShowStatusInfo("正在载入积冰文件列表...");

            int k = lBoxJB.SelectedIndex;//为了再次定位            
            lBoxJB.Items.Clear();
            listJBData.Clear();

            string sJBPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapjb" + Path.DirectorySeparatorChar.ToString();

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sJBPath))
            {
                ShowStatusInfo("积冰文件路径无效！");
                return;
            }

            //foreach (FileInfo fi in new DirectoryInfo(sysCfg.sIR1Path).GetFiles("*.siv"))
            foreach (FileInfo fi in new DirectoryInfo(sJBPath).GetFiles("*.mlb"))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CJBFileInfo sfi = new CJBFileInfo();
                sfi.sPath = sPathName;
                sfi.loadHead();

                if (sfi.bIsMapFile)
                {
                    //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        listJBData.Add(sfi);
                    }
                }
            }
            //列表排序
            listJBData.Sort(SortJBFileDesc);

            //展现地图列表
            for (int i = 0; i < listJBData.Count; i++)
            {
                CJBFileInfo sfi = listJBData[i];
                lBoxJB.Items.Add(sfi);
            }

            ShowStatusInfo("积冰文件列表载入完毕");
        }

        /// <summary>
        /// 载入海雾图数据,2014-10-14 wm add
        /// </summary>
        private void FindLoadHaiWuFileList()
        {
            ShowStatusInfo("正在载入陆海雾文件列表...");

            int k = lBoxHW.SelectedIndex;//为了再次定位            
            lBoxHW.Items.Clear();
            listHWData.Clear();

            string sHWPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "maphw" + Path.DirectorySeparatorChar.ToString();

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sHWPath))
            {
                ShowStatusInfo("陆海雾文件路径无效！");
                return;
            }

            //foreach (FileInfo fi in new DirectoryInfo(sysCfg.sIR1Path).GetFiles("*.siv"))
            foreach (FileInfo fi in new DirectoryInfo(sHWPath).GetFiles("*.mlb"))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CHWFileInfo sfi = new CHWFileInfo();
                sfi.sPath = sPathName;
                sfi.loadHead();

                if (sfi.bIsMapFile)
                {
                    //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        listHWData.Add(sfi);
                    }
                }
            }
            //列表排序
            listHWData.Sort(SortHWFileDesc);

            //展现地图列表
            for (int i = 0; i < listHWData.Count; i++)
            {
                CHWFileInfo sfi = listHWData[i];
                lBoxHW.Items.Add(sfi);
            }

            ShowStatusInfo("陆海雾文件列表载入完毕");
        }

        /// <summary>
        /// 载入等值线数据,2014-10-14 wm add
        /// </summary>
        private void FindLoadDZXFileList()
        {
            ShowStatusInfo("正在载入等值线文件列表...");

            int k = lBoxDZX.SelectedIndex;//为了再次定位            
            lBoxDZX.Items.Clear();
            listDZXData.Clear();

            string sDZXPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapdzx" + Path.DirectorySeparatorChar.ToString();

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sDZXPath))
            {
                ShowStatusInfo("等值线文件路径无效！");
                return;
            }

            foreach (FileInfo fileInfo in new DirectoryInfo(sDZXPath).GetFiles("*.bmp"))
            {
                string sShowName = fileInfo.Name;//解析文件名
                string fullPathName = fileInfo.FullName;

                CDZXFileInfo dzxFileInfo = new CDZXFileInfo();
                dzxFileInfo.filePath = fullPathName;
                dzxFileInfo.parseFileName();

                if (dzxFileInfo.bIsMapFile)
                {
                    if (dzxFileInfo.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        listDZXData.Add(dzxFileInfo);
                    }
                }
            }
            //列表排序
            listDZXData.Sort(SortDZXFileDesc);

            //展现地图列表
            for (int i = 0; i < listDZXData.Count; i++)
            {
                CDZXFileInfo sfi = listDZXData[i];
                lBoxDZX.Items.Add(sfi);
            }

            ShowStatusInfo("等值线文件列表载入完毕");
        }

        /// <summary>
        /// 载入逻辑运算云图
        /// </summary>
        private void FindLoadLogicFileList()
        {
            ShowStatusInfo("正在载入逻辑减文件列表...");

            int k = listBoxLogic.SelectedIndex;//为了再次定位            
            listBoxLogic.Items.Clear();
            listMapData.Clear();

            //string sIR1P = sysCfg.getIR1Path(userSysParams);//20140513 del
            string sIR1P = userSysParams.LogicOutPatn;// @"E:\云导风\SatellitePower 0717\ljj\";

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sIR1P))
            {
                ShowStatusInfo("数据文件路径无效！");
                return;
            }

            foreach (FileInfo fi in new DirectoryInfo(sIR1P).GetFiles("*.opm", SearchOption.AllDirectories))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CSivFileInfo sfi = new CSivFileInfo();
                sfi.sCustomType = "logicJian";
                sfi.sPath = sPathName;
                sfi.sDefRootPath = userSysParams.sMapPath;
                sfi.loadHead();

                //mi.bmpImage = new Bitmap(sPathName);
                if (sfi.bIsMapFile)
                {
                    //2014-10-31 wm del
                    //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        #region 判断列表中是否已存在

                        bool bListExist = false;
                        foreach (CSivFileInfo item in listMapData)
                        {
                            if (item.ToString().Equals(sfi.ToString()))
                            {
                                bListExist = true;
                                break;
                            }
                        }

                        if (!bListExist)
                        {
                            //1=IR,2=VIS
                            //0-3:1~4 通道
                            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                            if (sfi.iType == iNowType
                                && sfi.iWay == iNowWay
                                && sfi.iProjectType == iNowProj)
                            {
                                listMapData.Add(sfi);
                            }
                        }

                        #endregion
                    }
                }
            }

            //列表排序
            listMapData.Sort(SortFileDesc);

            //展现地图列表
            for (int i = 0; i < listMapData.Count; i++)
            {
                CSivFileInfo sfi = listMapData[i];
                sfi.sCustomType = "logicJian";
                listBoxLogic.Items.Add(sfi);
            }
            if (k >= 0 && k < listBoxLogic.Items.Count)
            {
                listBoxLogic.SelectedIndex = k;
            }

            ShowStatusInfo("逻辑减文件列表载入完毕");
        }
        /// <summary>
        /// 载入逻辑运算云图
        /// </summary>
        private void FindLoadLogicFileList2()
        {
            ShowStatusInfo("正在载入逻辑减文件列表...");

            int k = listBoxLogic.SelectedIndex;//为了再次定位            
            listBoxLogic.Items.Clear();
            listMapData.Clear();

            //string sIR1P = sysCfg.getIR1Path(userSysParams);//20140513 del
            string sIR1P = userSysParams.LogicOutPatn;// @"E:\云导风\SatellitePower 0717\ljj\";

            //if (!Directory.Exists(sysCfg.sIR1Path))
            if (!Directory.Exists(sIR1P))
            {
                ShowStatusInfo("数据文件路径无效！");
                return;
            }

            foreach (FileInfo fi in new DirectoryInfo(sIR1P).GetFiles("*.opm", SearchOption.AllDirectories))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;

                CSivFileInfo sfi = new CSivFileInfo();
                sfi.sCustomType = "logicJian";
                sfi.sPath = sPathName;
                sfi.sDefRootPath = userSysParams.sMapPath;
                sfi.loadHead();

                //mi.bmpImage = new Bitmap(sPathName);
                if (sfi.bIsMapFile)
                {
                    //2014-10-31 wm del
                    //if (sfi.dtBegin > (DateTime.Now.AddDays(-userSysParams.NearMayDays)))
                    {
                        #region 判断列表中是否已存在

                        bool bListExist = false;
                        foreach (CSivFileInfo item in listMapData)
                        {
                            if (item.ToString().Equals(sfi.ToString()))
                            {
                                bListExist = true;
                                break;
                            }
                        }

                        if (!bListExist)
                        {
                            //1=IR,2=VIS
                            //0-3:1~4 通道
                            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                            if (sfi.iType == iNowType
                                && sfi.iWay == iNowWay
                                && sfi.iProjectType == iNowProj)
                            {
                                listMapData.Add(sfi);
                            }
                        }

                        #endregion
                    }
                }
            }

            //列表排序
            listMapData.Sort(SortFileDesc);

            //展现地图列表
            for (int i = 0; i < listMapData.Count; i++)
            {
                CSivFileInfo sfi = listMapData[i];
                sfi.sCustomType = "logicJian";
                listBoxLogic.Items.Add(sfi);
            }

            ShowStatusInfo("逻辑减文件列表载入完毕");
        }
        #endregion

        public void SortListBox(ListBox lBox, bool bDesc = true)
        {
            if (null == lBox)
            {
                return;
            }

            if (lBox.Items.Count <= 1)
            {
                //一项不用排序
                return;
            }

            ArrayList myAL = new ArrayList();
            myAL.AddRange(lBox.Items);
            if (bDesc)
            {
                IComparer myComparer = new CReverserComparer();
                myAL.Sort(myComparer);
            }
            else
            {
                myAL.Sort();
            }

            object[] objAry = myAL.ToArray();
            lBox.Items.Clear();
            lBox.Items.AddRange(objAry);
        }

        /// <summary>
        /// 直方图增强
        /// </summary>
        private void tsmiZFTzq_Click(object sender, EventArgs e)
        {
            //直方图增强
            int iZFTFlag = 0;
            tsmiZFTzq.Checked = !tsmiZFTzq.Checked;
            if (tsmiZFTzq.Checked)
            {
                iZFTFlag = 1;
            }
            else
            {
                iZFTFlag = 0;
            }

            string sParams = "zftzq:" + iZFTFlag;

            bool bLayerChanged = true;
            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (!lay.IsBaseMap)
                {
                    //lay.SetParams(iZFTFlag);                    
                    lay.SetParams(sParams);
                    bLayerChanged = true;
                }
            }

            if (bLayerChanged)
            {
                baseMap.Render();
            }
        }

        #region 点击生成云际风、雷暴、积冰、海雾、等值线 事件
        public frmSelYJF dlgSelYJF;//2014-11-21 wm add
        private void btnMakeYJF_Click(object sender, EventArgs e)
        {
            if (tspBar_yjf.Visible)
            {
                if (MessageBox.Show("当前云际风反演正在后台执行，您的操作将终止它，确认终止吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    return;
                }
                else
                {
                    if (makeYJFThread != null)
                    {
                        makeYJFThread.Abort();
                    }
                }
            }
            //生成云迹风
            string sMapPath1 = string.Empty;    //时间靠后的，如：10点
            string sMapPath2 = string.Empty;    //时间中间的，如： 9点
            string sMapPath3 = string.Empty;    //时间靠前的，如： 8点

            dlgSelYJF = new frmSelYJF();//2014-10-27 wm add
            dlgSelYJF.iNowType = 1;
            dlgSelYJF.iNowWay = 0;
            dlgSelYJF.setMapFileList(listBoxMap);
            if (dlgSelYJF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sMapPath1 = dlgSelYJF.sMapPath1;
                sMapPath2 = dlgSelYJF.sMapPath2;
                sMapPath3 = dlgSelYJF.sMapPath3;
                //dlgSelYJF.Dispose();                
            }
            else
            {
                return;
            }

            ShowStatusInfo("开始生成云际风...");
            Application.DoEvents();

            PointF pfLT = new PointF(0, 0);
            PointF pfRB = new PointF(0, 0);
            int iW = 0;
            int iH = 0;

            DateTime dt1 = DateTime.Now;//时间靠后的，如：10点
            DateTime dt2 = DateTime.Now;//时间中间的，如： 9点
            DateTime dt3 = DateTime.Now;//时间靠前的，如： 8点

            LayerVISS lvis1 = new LayerVISS();//时间靠后的，如：10点
            LayerVISS lvis2 = new LayerVISS();//时间中间的，如： 9点
            LayerVISS lvis3 = new LayerVISS();//时间靠前的，如： 8点

            #region 生成过程

            //图1
            lvis1.Load(sMapPath1);
            SVissrUnit svu1 = lvis1.visObj.svuObj;
            dt1 = svu1.dtBegin;
            pfLT = new PointF(svu1.fMLeft, svu1.fMTop);
            pfRB = new PointF(svu1.fMRight, svu1.fMBotton);

            iW = svu1.iMWidth;
            iH = svu1.iMHigh;

            sSalelliteName = svu1.sSatelliteName;
            iType = svu1.iType;  //1=IR,2=VIS
            iWay = svu1.iWay;    //0-3:1~4 通道

            //图2
            lvis2.Load(sMapPath2);
            dt2 = lvis2.visObj.svuObj.dtBegin;

            //图3
            lvis3.Load(sMapPath3);
            dt3 = lvis3.visObj.svuObj.dtBegin;

            //开始计算
            tspBar_yjf.Value = 0;
            tspBar_yjf.Visible = true;

            //纬度校正
            if (sSalelliteName.Equals("MTSAT"))
            {
                pfRB.Y = 0;//-10
            }

            //存下新生成的时间，用于定位新生成的条目
            newItemNameYJF = dt1.ToString("yyyy-MM-dd HH_mm_ss");

            string sOutPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapyjf" + Path.DirectorySeparatorChar.ToString();
            cloudWind = new AnalyCloudWind(tspBar_yjf, this, dt1, sOutPath, sSalelliteName, iType, iWay);
            cloudWind.visObjBase = lvis1.visObj;//时间靠后的，如：10点
            cloudWind.puserSysParams = userSysParams;
            cloudWind.LoadCloudFile(lvis3.visObj.svuObj.bVissData
                            , lvis2.visObj.svuObj.bVissData
                            , lvis1.visObj.svuObj.bVissData,
                            iW, iH,
                            pfLT.X, pfRB.X,
                            pfRB.Y, pfLT.Y,
                            dt3, dt2, dt1);
            //dt1, dt2, dt3);
            dateTime_yjf_begin = dt1;

            //在cw对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            cloudWind.mainThread = new AnalyCloudWind.updateDelegate(updateProgressBar_yjf);
            cloudWind.completeThread = new AnalyCloudWind.completeDelegate(onMakeDataCompleted_yjf);
            cloudWind.setMaxThread = new AnalyCloudWind.setMaxDelegate(setMax_yjf);

            //创建一个无参数的线程,这个线程执行AnalyCloudWind类中的testFunction方法。
            makeYJFThread = new Thread(new ThreadStart(cloudWind.MakeCloudWind));
            //启动线程，启动之后线程才开始执行
            makeYJFThread.Start();

            #endregion
        }

        /// <summary>
        /// 雷暴点击生成函数
        /// </summary>
        public FrmSelectYT dlgSelectLB;
        private void tsmiCSlb_Click(object sender, EventArgs e)
        {
            if (tspBar_lb.Visible)
            {
                if (MessageBox.Show("当前雷暴反演正在后台执行，您的操作将终止它，确认终止吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    return;
                }
                else
                {
                    if (makeLBThread != null)
                    {
                        makeLBThread.Abort();
                    }
                }
            }

            string sMapPath1 = "";
            string sMapPath2 = "";
            string sMapPath3 = "";

            dlgSelectLB = new FrmSelectYT(1);
            dlgSelectLB.iNowType = 1;
            dlgSelectLB.iNowWay = 0;
            dlgSelectLB.setMapFileList(listBoxMap);
            if (dlgSelectLB.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sMapPath1 = dlgSelectLB.sMapPathIR1;
                sMapPath2 = dlgSelectLB.sMapPathIR2;
                sMapPath3 = dlgSelectLB.sMapPathIR3;
            }
            else
            {
                return;
            }

            ShowStatusInfo("开始生成雷暴...");
            Application.DoEvents();

            string sSalelliteName = "";
            int iType = 1;//1=IR,2=VIS
            int iWay = 0;//0-3:1~4 通道

            PointF pfLT = new PointF(0, 0);
            PointF pfRB = new PointF(0, 0);
            int iW = 0;
            int iH = 0;

            DateTime dt1 = DateTime.Now;
            LayerVISS lvis1 = new LayerVISS();
            LayerVISS lvis2 = new LayerVISS();
            LayerVISS Lvis3 = new LayerVISS();

            #region 生成过程

            //IR1
            lvis1.Load(sMapPath1);
            SVissrUnit svu1 = lvis1.visObj.svuObj;
            dt1 = svu1.dtBegin;
            pfLT = new PointF(svu1.iMLeft, svu1.iMTop);
            pfRB = new PointF(svu1.iMRight, svu1.iMBotton);

            iW = svu1.iMWidth;
            iH = svu1.iMHigh;

            sSalelliteName = svu1.sSatelliteName;
            iType = svu1.iType;  //1=IR,2=VIS
            iWay = svu1.iWay;    //0-3:1~4 通道

            double[,] T1 = new double[iW, iH];
            //IR2
            lvis2.Load(sMapPath2);
            SVissrUnit svu2 = lvis2.visObj.svuObj;
            double[,] T2 = new double[iW, iH];

            //WV
            Lvis3.Load(sMapPath3);
            SVissrUnit svu3 = Lvis3.visObj.svuObj;
            double[,] T3 = new double[iW, iH];

            for (int j = 0; j < iH; j++)
            {
                for (int i = 0; i < iW; i++)
                {
                    int index = j * iW + i;
                    T1[i, j] = svu1.getSigns2ValByIndex(svu1.bVissData[index]);
                    T2[i, j] = svu2.getSigns2ValByIndex(svu2.bVissData[index]);
                    T3[i, j] = svu3.getSigns2ValByIndex(svu3.bVissData[index]);
                }
            }

            //开始计算
            tspBar_lb.Value = 0;
            tspBar_lb.Visible = true;

            //纬度校正
            if (sSalelliteName.Equals("MTSAT"))
            {
                pfRB.Y = 0;//-10
            }

            //存下新生成的时间，用于定位新生成的条目
            newItemNameLB = dt1.ToString("yyyy-MM-dd HH_mm_ss");

            string sOutPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "maplb" + Path.DirectorySeparatorChar.ToString();
            thunder = new AnalyThunderStorm(tspBar_lb, this, dt1, sOutPath, sSalelliteName, iType, iWay);

            thunder.SetAnalyData(T1, T2, T3, iW, iH, pfLT.X, pfRB.X, pfRB.Y, pfLT.Y);

            //在ats对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            thunder.mainThread = new AnalyThunderStorm.updateDelegate(updateProgressBar_lb);
            thunder.completeThread = new AnalyThunderStorm.completeDelegate(onMakeDataCompleted_lb);
            thunder.setMaxThread = new AnalyThunderStorm.setMaxDelegate(setMax_lb);

            //创建一个无参数的线程,这个线程执行AnalyThunderStorm类中的testFunction方法。
            makeLBThread = new Thread(new ThreadStart(thunder.BuildLBMap));
            //启动线程，启动之后线程才开始执行
            makeLBThread.Start();
            //------------------------------------------------------------------------------

            #endregion

        }

        /// <summary>
        /// 积冰点击生成函数
        /// </summary>
        public FrmSelectYT dlgSelectJB;
        private void tsmiCSjb_Click(object sender, EventArgs e)
        {
            if (tspBar_jb.Visible)
            {
                if (MessageBox.Show("当前积冰反演正在后台执行，您的操作将终止它，确认终止吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    return;
                }
                else
                {
                    if (makeJBThread != null)
                    {
                        makeJBThread.Abort();
                    }
                }
            }
            ////测算积冰
            //if (iNowType != 1 || iNowWay != 0)//1=IR,2=VIS
            //{
            //    MessageBox.Show("只能在红外一通道下执行该操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            //if (iNowProj != 2 && iNowProj != 4)
            //{
            //    MessageBox.Show("该投影下无法执行此操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //if (!chkShowJB.Checked)
            //{
            //    MessageBox.Show("请勾选“显示积冰”复选框!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            string mapPathSelectedIR1 = string.Empty;    //选中的云图
            string mapPathSelectedVIS = string.Empty;    //选中的云图
            dlgSelectJB = new FrmSelectYT(2);//2014-10-27 wcs add
            dlgSelectJB.iNowType = 1;
            dlgSelectJB.iNowWay = 0;
            dlgSelectJB.setMapFileList(listBoxMap);
            if (dlgSelectJB.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mapPathSelectedIR1 = dlgSelectJB.sMapPathIR1;
                mapPathSelectedVIS = dlgSelectJB.sMapPathVis1;
            }
            else
            {
                return;
            }

            //CSivFileInfo sfi = listBoxMap.SelectedItem as CSivFileInfo;
            //if (null == sfi)
            //{
            //    MessageBox.Show("请在列表中选择云图!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //sfiJB = sfi;//2014-10-27 wm add

            //2014-10-14 wm add
            ShowStatusInfo("开始生成积冰...");
            Application.DoEvents();

            string sSalelliteName = "";
            int iType = 1;//1=IR,2=VIS
            int iWay = 0;//0-3:1~4 通道

            PointF pfLT = new PointF(0, 0);
            PointF pfRB = new PointF(0, 0);
            int iW = 0;
            int iH = 0;

            DateTime dt = DateTime.Now;

            //iNowType:IR=1,VIS=2
            //iNowWay:0~3
            //iNowProj:投影方式
            LayerVISS lvis_IR1 = new LayerVISS();
            lvis_IR1.Load(mapPathSelectedIR1);
            SVissrUnit svuIR1 = lvis_IR1.visObj.svuObj;

            LayerVISS lvis_VIS = new LayerVISS();
            lvis_VIS.Load(mapPathSelectedVIS);
            SVissrUnit svuVIS = lvis_VIS.visObj.svuObj;

            dt = svuIR1.dtBegin;
            pfLT = new PointF(svuIR1.iMLeft, svuIR1.iMTop);
            pfRB = new PointF(svuIR1.iMRight, svuIR1.iMBotton);

            iW = svuIR1.iMWidth;
            iH = svuIR1.iMHigh;

            sSalelliteName = svuIR1.sSatelliteName;
            iType = svuIR1.iType;  //1=IR,2=VIS
            iWay = svuIR1.iWay;    //0-3:1~4 通道

            //开始计算
            tspBar_jb.Value = 0;
            tspBar_jb.Visible = true;

            //纬度校正
            if (sSalelliteName.Equals("MTSAT"))
            {
                pfRB.Y = 0;//-10
            }

            //存下新生成的时间，用于定位新生成的条目
            newItemNameJB = dt.ToString("yyyy-MM-dd HH_mm_ss");

            string sOutPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapjb" + Path.DirectorySeparatorChar.ToString();
            ice = new AnalyIce(tspBar_jb, this, dt, sOutPath, sSalelliteName, iType, iWay);
            ice.visObjBase = lvis_IR1.visObj;

            //积冰新算法反演参数，20150320，wm add
            ice.visObjBase5 = lvis_VIS.visObj;
            ice.DayT1 = userSysParams.DayT1;
            ice.DayT2 = userSysParams.DayT2;
            ice.DayT3 = userSysParams.DayT3;
            ice.NightT1 = userSysParams.NightT1;
            ice.NightT2 = userSysParams.NightT2;

            ////2015-1-5,wm add
            //ice.Height1 = userSysParams.YDG_HighMeter;
            //ice.Height2 = userSysParams.YDG_LowMeter;
            //ice.Height3 = userSysParams.YDG_FloorMeter;
            //ice.T1 = userSysParams.YDG_HighVal;
            //ice.T2 = userSysParams.YDG_LowVal;
            //ice.T3 = userSysParams.YDG_FloorVal;

            ice.SetAnalyData(lvis_IR1.visObj.svuObj.bVissData, lvis_VIS.visObj.svuObj.bVissData, iW, iH, pfLT.X, pfRB.X, pfRB.Y, pfLT.Y, dt);

            //在ai对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            ice.mainThread = new AnalyIce.updateDelegate(updateProgressBar_jb);
            ice.completeThread = new AnalyIce.completeDelegate(onMakeDataCompleted_jb);
            ice.setMaxThread = new AnalyIce.setMaxDelegate(setMax_jb);

            //创建一个无参数的线程,这个线程执行AnalyIce类中的testFunction方法。
            makeJBThread = new Thread(new ThreadStart(ice.BuildJB));
            //启动线程，启动之后线程才开始执行
            makeJBThread.Start();
        }

        /// <summary>
        /// 海雾点击生成函数
        /// </summary>
        public FrmSelectYT dlgSelectHW;
        private void tsmiCShw_Click(object sender, EventArgs e)
        {
            if (tspBar_hw.Visible)
            {
                if (MessageBox.Show("当前陆海雾反演正在后台执行，您的操作将终止它，确认终止吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    return;
                }
                else
                {
                    if (makeHWThread != null)
                    {
                        makeHWThread.Abort();
                    }
                }
            }
            //测算陆/海雾
            //if (iNowType != 1 || iNowWay != 0)//1=IR,2=VIS
            //{
            //    MessageBox.Show("只能在红外一通道下执行该操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            ////当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            //if (iNowProj != 2 && iNowProj != 4)
            //{
            //    MessageBox.Show("该投影下无法执行此操作!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (!chkShowHW.Checked)
            {
                MessageBox.Show("请勾选“显示陆海雾”复选框!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            string mapPathSelectedIR1 = string.Empty;    //选中的云图
            string mapPathSelectedIR4 = string.Empty;    //选中的云图
            string mapPathSelectedVis1 = string.Empty;    //选中的云图
            dlgSelectHW = new FrmSelectYT(4);//2014-10-27 wcs add
            dlgSelectHW.iNowType = 1;
            dlgSelectHW.iNowWay = 0;
            dlgSelectHW.setMapFileList(listBoxMap);
            if (dlgSelectHW.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mapPathSelectedIR1 = dlgSelectHW.sMapPathIR1;
                mapPathSelectedIR4 = dlgSelectHW.sMapPathIR4;
                mapPathSelectedVis1 = dlgSelectHW.sMapPathVis1;
            }
            else
            {
                return;
            }


            //CSivFileInfo sfi = listBoxMap.SelectedItem as CSivFileInfo;
            //if (null == sfi)
            //{
            //    MessageBox.Show("请在列表中选择云图!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //sfiHW = sfi;//2014-10-27 wm add

            //2014-10-14 wm add
            ShowStatusInfo("开始生成陆海雾...");
            Application.DoEvents();


            //iNowType:IR=1,VIS=2
            //iNowWay:0~3
            //iNowProj:投影方式
            LayerVISS layerIR1 = new LayerVISS();
            LayerVISS layerIR4 = new LayerVISS();
            LayerVISS layerVis1 = new LayerVISS();

            layerIR1.Load(mapPathSelectedIR1);
            layerIR4.Load(mapPathSelectedIR4);
            layerVis1.Load(mapPathSelectedVis1);

            string sSalelliteName = layerIR1.visObj.sSalelliteName;
            int iType = 1;//1=IR,2=VIS
            int iWay = 0;//0-3:1~4 通道


            //开始计算
            tspBar_hw.Value = 0;
            tspBar_hw.Visible = true;


            //存下新生成的时间，用于定位新生成的条目
            newItemNameHW = layerIR1.visObj.dtBegin.ToString("yyyy-MM-dd HH_mm_ss");

            string sOutPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "maphw" + Path.DirectorySeparatorChar.ToString();
            seaFog = new AnalyFog2(tspBar_hw, this, sOutPath, sSalelliteName, iType, iWay, layerIR1, layerIR4, layerVis1);

            //海雾,20150417 wm add
            seaFog.AngelDayNight = userSysParams.AngelDayNight;
            seaFog.AngelSummer = userSysParams.AngelSummer;
            seaFog.AngelWinter = userSysParams.AngelWinter;
            seaFog.DcdNight = userSysParams.DcdNight;
            seaFog.DcdDawn = userSysParams.DcdDawn;
            seaFog.DcdDay = userSysParams.DcdDay;
            seaFog.DayVisMin = userSysParams.DayVisMin;
            seaFog.DayVisMax = userSysParams.DayVisMax;

            //在ai对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            seaFog.mainThread = new AnalyFog2.updateDelegate(updateProgressBar_hw);
            seaFog.completeThread = new AnalyFog2.completeDelegate(onMakeDataCompleted_hw);
            seaFog.setMaxThread = new AnalyFog2.setMaxDelegate(setMax_hw);

            //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
            makeHWThread = new Thread(new ThreadStart(seaFog.BuildHW));
            //启动线程，启动之后线程才开始执行
            makeHWThread.Start();
        }

        /// <summary>
        /// 等值线点击生成函数
        /// </summary>
        public FrmSelectYT dlgSelectDZX;
        private void tsmiDZX_Click(object sender, EventArgs e)
        {
            //绘制等值线
            if (tspBar_dzx.Visible)
            {
                if (MessageBox.Show("当前等值线绘制正在后台执行，您的操作将终止它，确认终止吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    return;
                }
                else
                {
                    if (makeDZXThread != null)
                    {
                        makeDZXThread.Abort();
                    }
                }
            }

            //当前云图及通道：iNowType(1:IR,2:VISS) iNowWay(0:IR1,1:IR2....)
            //if (iNowType != 1 || iNowWay != 0)
            //{
            //    MessageBox.Show("该通道下无法执行此操作，只有红外一能绘制等值线!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            ////当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
            //if (iNowProj != 2)
            //{
            //    MessageBox.Show("该投影下无法执行此操作，只有麦卡托投影能绘制等值线!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (!chkShowDZX.Checked)
            {
                MessageBox.Show("请勾选“显示等值线”复选框!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string mapPathSelected = string.Empty;    //选中的云图
            dlgSelectDZX = new FrmSelectYT(3);//2014-10-27 wcs add
            dlgSelectDZX.iNowType = 1;
            dlgSelectDZX.iNowWay = 0;
            dlgSelectDZX.setMapFileList(listBoxMap);
            if (dlgSelectDZX.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mapPathSelected = dlgSelectDZX.sMapPathIR1;
            }
            else
            {
                return;
            }

            //CSivFileInfo mainMapFileInfo = listBoxMap.SelectedItem as CSivFileInfo;
            //if (null == mainMapFileInfo)
            //{
            //    MessageBox.Show("请在云图列表中选择云图!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            ShowStatusInfo("开始生成等值线...");
            Application.DoEvents();

            string sSalelliteName = "";
            int iType = 1;//1=IR,2=VIS
            int iWay = 0;//0-3:1~4 通道

            PointF pfLT = new PointF(0, 0);
            PointF pfRB = new PointF(0, 0);
            int iW = 0;
            int iH = 0;

            DateTime dt = DateTime.Now;

            //iNowType:1=IR,2=VIS
            //iNowWay:0~3
            //iNowProj:投影方式
            //string sMapPath_IR1 = mainMapFileInfo.getVissPathName(iNowType, iNowWay, iNowProj);
            LayerVISS lvis_IR1 = new LayerVISS();
            lvis_IR1.Load(mapPathSelected);
            SVissrUnit svu = lvis_IR1.visObj.svuObj;

            dt = svu.dtBegin;
            pfLT = new PointF(svu.fMLeft, svu.fMTop);
            pfRB = new PointF(svu.fMRight, svu.fMBotton);

            iW = svu.iMWidth;
            iH = svu.iMHigh;

            sSalelliteName = svu.sSatelliteName;
            iType = svu.iType;  //1=IR,2=VIS
            iWay = svu.iWay;    //0-3:1~4 通道

            //开始计算
            tspBar_dzx.Value = 0;
            tspBar_dzx.Visible = true;

            //纬度校正
            if (sSalelliteName.Equals("MTSAT"))
            {
                pfRB.Y = 0;//-10
            }

            //存下新生成的时间，用于定位新生成的条目
            newItemNameDZX = dt.ToString("yyyy-MM-dd HH_mm_ss");

            string sOutPath = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "mapdzx" + Path.DirectorySeparatorChar.ToString();
            contour = new AnalyContour(tspBar_jb, this, dt, sOutPath, sSalelliteName, iType, iWay, svu, WenDuJG);
            contour.SetAnalyData(lvis_IR1.visObj.svuObj.bVissData, iW, iH, pfLT.X, pfRB.X, pfRB.Y, pfLT.Y, dt);

            //在ai对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。
            contour.mainThread = new AnalyContour.updateDelegate(updateProgressBar_dzx);
            contour.completeThread = new AnalyContour.completeDelegate(onMakeDataCompleted_dzx);
            contour.setMaxThread = new AnalyContour.setMaxDelegate(setMax_dzx);

            //创建一个无参数的线程,这个线程执行AnalyFog类中的testFunction方法。
            makeDZXThread = new Thread(new ThreadStart(contour.plotContour));
            //启动线程，启动之后线程才开始执行
            makeDZXThread.Start();



            //==============================================================

            //string sShowName = mainMapFileInfo.ToString();

            //bool bLayerChanged = true;
            //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            //{
            //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
            //    if (!lay.IsBaseMap)
            //    {
            //        string sName = lay.Name;
            //        if (sName.Equals(sNowSelLayName))
            //        {
            //            if (lBoxDZX.Items.IndexOf(sShowName) < 0)
            //            {
            //                lBoxDZX.Items.Add(sShowName);

            //                SortListBox(lBoxDZX);

            //                WRFileUnit.saveListBoxToFile(lBoxDZX, "mapDZX.dat");
            //            }

            //            string sParams = "dzx:1";

            //            //2014-9-4 add 不画底图、直接绘等值线
            //            if (!chkShowDZXyt.Checked)
            //            {
            //                sParams = "dzx:2";
            //            }

            //            lay.SetParams(sParams);
            //            bLayerChanged = true;
            //        }
            //    }
            //}

            //if (bLayerChanged)
            //{
            //    baseMap.Render();
            //}
        }

        #endregion
        #endregion

        #region 通用删除菜单控制

        /// <summary>
        /// 右键选中条目的函数
        /// </summary>
        private bool selectItem(object sender, MouseEventArgs e, ListBox lBox)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lBox.IndexFromPoint(e.Location);
                if (index >= 0)
                {
                    lBox.SelectedIndex = index;
                    return true;
                }
                return false;
            }
            return true;
        }

        private void lBoxDZX_Enter(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "dzx";
            tsmiPubDel.Text = "删除等值线";
        }

        private void lBoxDZX_MouseDown(object sender, MouseEventArgs e)
        {
            tsmiPubDel.Tag = "dzx";
            tsmiPubDel.Text = "删除等值线";
            selectItem(sender, e, lBoxDZX);
        }

        private void lBoxDZX_Leave(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "-";
            tsmiPubDel.Text = "-";
        }
        //-----------------------------------------------------------
        private void lBoxHW_Enter(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "hw";
            tsmiPubDel.Text = "删除陆海雾";
        }

        private void lBoxHW_MouseDown(object sender, MouseEventArgs e)
        {
            tsmiPubDel.Tag = "hw";
            tsmiPubDel.Text = "删除陆海雾";
            selectItem(sender, e, lBoxHW);
        }

        private void lBoxHW_Leave(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "-";
            tsmiPubDel.Text = "-";
        }
        //-----------------------------------------------------------
        private void lBoxJB_Enter(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "jb";
            tsmiPubDel.Text = "删除积冰";
        }

        private void lBoxJB_MouseDown(object sender, MouseEventArgs e)
        {
            tsmiPubDel.Tag = "jb";
            tsmiPubDel.Text = "删除积冰";
            selectItem(sender, e, lBoxJB);
        }

        private void lBoxJB_Leave(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "-";
            tsmiPubDel.Text = "-";
        }
        //-----------------------------------------------------------
        private void lBoxLB_Enter(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "lb";
            tsmiPubDel.Text = "删除雷暴";
        }

        private void lBoxLB_MouseDown(object sender, MouseEventArgs e)
        {
            tsmiPubDel.Tag = "lb";
            tsmiPubDel.Text = "删除雷暴";
            selectItem(sender, e, lBoxLB);
        }

        private void lBoxLB_Leave(object sender, EventArgs e)
        {
            tsmiPubDel.Tag = "-";
            tsmiPubDel.Text = "-";
        }
        //-----------------------------------------------------------
        /// <summary>
        /// 公共删除方法
        /// </summary>
        private void tsmiPubDel_Click(object sender, EventArgs e)
        {
            bool bIsDel = false;
            string sType = Convert.ToString(tsmiPubDel.Tag);
            if (sType.Equals("lb"))
            {
                int k = lBoxLB.SelectedIndex;
                if ((k >= 0) && (k < lBoxLB.Items.Count))
                {
                    CLBFileInfo sfi = lBoxLB.Items[k] as CLBFileInfo;
                    if (sfi != null)
                    {
                        if (MessageBox.Show("确定要删除雷暴：" + sfi.sName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            string sLayName = sfi.sLayName;
                            string sYTName = sfi.getYTName();

                            if (File.Exists(sfi.sPath))
                            {
                                try
                                {
                                    File.Delete(sfi.sPath);
                                    FindLoadLeiBaoFileList();
                                    if (k < lBoxLB.Items.Count)
                                    {
                                        lBoxLB.SelectedIndex = k;
                                    }
                                    else if (lBoxLB.Items.Count > 0)
                                    {
                                        lBoxLB.SelectedIndex = lBoxLB.Items.Count - 1;
                                    }
                                    bIsDel = true;
                                }
                                catch (Exception err)
                                {
                                    MessageBox.Show(err.Message);
                                }
                            }
                        }
                    }
                }

            }
            else if (sType.Equals("jb"))
            {
                int k = lBoxJB.SelectedIndex;
                if ((k >= 0) && (k < lBoxJB.Items.Count))
                {
                    CJBFileInfo sfi = lBoxJB.Items[k] as CJBFileInfo;
                    if (sfi != null)
                    {
                        if (MessageBox.Show("确定要删除积冰：" + sfi.sName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            string sLayName = sfi.sLayName;
                            string sYTName = sfi.getYTName();

                            if (File.Exists(sfi.sPath))
                            {
                                try
                                {
                                    File.Delete(sfi.sPath);
                                    FindLoadJBFileList();
                                    if (k < lBoxJB.Items.Count)
                                    {
                                        lBoxJB.SelectedIndex = k;
                                    }
                                    else if (lBoxJB.Items.Count > 0)
                                    {
                                        lBoxJB.SelectedIndex = lBoxJB.Items.Count - 1;
                                    }
                                    bIsDel = true;
                                }
                                catch (Exception err)
                                {
                                    MessageBox.Show(err.Message);
                                }
                            }
                        }
                    }
                }

            }
            else if (sType.Equals("hw"))
            {
                int k = lBoxHW.SelectedIndex;
                if ((k >= 0) && (k < lBoxHW.Items.Count))
                {
                    CHWFileInfo sfi = lBoxHW.Items[k] as CHWFileInfo;
                    if (sfi != null)
                    {
                        if (MessageBox.Show("确定要删除海雾：" + sfi.sName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            string sLayName = sfi.sLayName;
                            string sYTName = sfi.getYTName();

                            if (File.Exists(sfi.sPath))
                            {
                                try
                                {
                                    File.Delete(sfi.sPath);
                                    FindLoadHaiWuFileList();
                                    if (k < lBoxHW.Items.Count)
                                    {
                                        lBoxHW.SelectedIndex = k;
                                    }
                                    else if (lBoxHW.Items.Count > 0)
                                    {
                                        lBoxHW.SelectedIndex = lBoxHW.Items.Count - 1;
                                    }
                                    bIsDel = true;
                                }
                                catch (Exception err)
                                {
                                    MessageBox.Show(err.Message);
                                }
                            }
                        }
                    }
                }
            }
            else if (sType == "dzx")
            {
                int k = lBoxDZX.SelectedIndex;
                if ((k >= 0) && (k < lBoxDZX.Items.Count))
                {
                    CDZXFileInfo fileInfo = lBoxDZX.Items[k] as CDZXFileInfo;
                    if (fileInfo != null)
                    {
                        if (MessageBox.Show("确定要删除等值线：" + fileInfo.showName, "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            if (fileInfo.isFileExist())
                            {
                                try
                                {
                                    File.Delete(fileInfo.filePath);
                                    FindLoadDZXFileList();
                                    if (k < lBoxDZX.Items.Count)
                                    {
                                        lBoxDZX.SelectedIndex = k;
                                    }
                                    else if (lBoxDZX.Items.Count > 0)
                                    {
                                        lBoxDZX.SelectedIndex = lBoxDZX.Items.Count - 1;
                                    }
                                    bIsDel = true;
                                }
                                catch (Exception err)
                                {
                                    MessageBox.Show(err.Message);
                                }
                            }
                        }
                    }
                }
            }

            if (bIsDel)
            {
                //移除图层
                bool bLayerChanged = false;
                for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                {
                    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                    if (!lay.IsBaseMap)
                    {
                        baseMap.WMap.LayerList.RemoveAt(i);
                        bLayerChanged = true;
                    }
                }

                if (bLayerChanged)
                {
                    //baseMap.Render();
                    ClearMemory();
                    listBoxMap_SelectedIndexChanged(sender, e);
                }
                int k = -1;
                //根据类型，显示这些当前选中的产品
                if (sType.Equals("lb"))
                {
                    k = lBoxLB.SelectedIndex;
                    lBoxLB_SelectedIndexChanged(null, null);
                }
                else if (sType.Equals("jb"))
                {
                    k = lBoxJB.SelectedIndex;
                    lBoxJB_SelectedIndexChanged(null, null);
                }
                else if (sType.Equals("hw"))
                {
                    k = lBoxHW.SelectedIndex;
                    lBoxHW_SelectedIndexChanged(null, null);
                }
                else if (sType == "dzx")
                {
                    k = lBoxDZX.SelectedIndex;
                    lBoxDZX_SelectedIndexChanged(null, null);
                }
                if (k < 0)
                {
                    //只保留底图
                    for (int i = baseMap.WMap.LayerList.Count - 1; i >= 0; i--)
                    {
                        ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                        if (!lay.IsBaseMap)
                        {
                            baseMap.WMap.LayerList.Remove(lay);//非底图全部移除，2014-10-21 wm add
                        }
                    }
                    baseMap.Render();
                    return;
                }
            }
        }
        #endregion


        #region 云图测量分析

        private DrawParams _drawParams = new DrawParams();
        private DrawParams _drawPRet;

        private void OnDrawEnd(DrawParams dpIn)
        {
            int k = dpIn.listPoints.Count;
            if (k == 2)
            {
                //直线测距标志，0=空状态，1=测距状态
                if (iGetLineDist == 0)
                {
                    tsmiDDCT.Visible = false;
                    tsmiYTDB.Visible = false;
                    tsmiYLFX.Visible = false;

                    _drawPRet = dpIn;

                    //iDrawType 绘图类型，0=不绘图，1=绘直线,2=绘曲线，3=绘矩形，4=绘填充矩形，5=绘圆，6=绘填充圆,9=手绘任意线
                    if (null != _drawPRet)
                    {
                        if (_drawPRet.iDrawType == 1)
                        {
                            //iNowProj 当前投影：1=兰勃托,2=麦卡托,3=极射,4=经纬度,5=卫星
                            //定点采样 不支持 1,2,3
                            //if (iNowProj == 4)
                            //{
                            //    tsmiDDCT.Visible = true;
                            //}
                            tsmiDDCT.Visible = true;
                        }
                        else if (_drawPRet.iDrawType == 3)
                        {
                            tsmiYTDB.Visible = true;
                            tsmiYLFX.Visible = true;
                        }
                        Point p = new Point(MousePosition.X, MousePosition.Y);

                        //根据iDrawAirLine参数确定是否绘制的是飞行航线，然后决定弹出哪个对话框，20150324 wm add
                        if (!_drawParams.iDrawAirLine)
                        {
                            contextMenuStrip1.Show(p);
                        }
                        else
                        {
                            contextMenuStripAirLine.Show(p);
                        }

                    }
                    return;
                }
                else
                {
                    //1=测距状态
                    if (dpIn.iDrawType == 1)
                    {
                        //测距 ###
                        if (dpIn.listPoints.Count == 2)
                        {
                            string sInfo = "";
                            Point p1 = dpIn.listPoints[0];
                            Point p2 = dpIn.listPoints[1];

                            PointF pf1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                            PointF pf2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                            //sInfo = "  起点: " + string.Format("经度: {0:f2}, 纬度: {1:f2}", pf1.X, pf1.Y) + Environment.NewLine;
                            sInfo = "  起点: " + string.Format("{0,6:f2},{1,6:f2}  ", pf1.X, pf1.Y)
                                + Environment.NewLine + Environment.NewLine;
                            sInfo = sInfo + "  终点: " + string.Format("{0,6:f2},{1,6:f2}  ", pf2.X, pf2.Y)
                                + Environment.NewLine + Environment.NewLine;

                            //ToString("f2");//保留两位小数，四舍五入
                            sInfo = sInfo + "  距离: " + PubUnit.getEarthDistance(pf1, pf2).ToString("f2") + " km";

                            MessageBox.Show(sInfo, "测距", MessageBoxButtons.OK);//MessageBoxIcon.Information
                        }
                    }
                }
                //if (dpIn.iDrawType == 1)
                //{
                //    toolStripButton5_Click_1(null, null);
                //}
                //else if (dpIn.iDrawType == 3)
                //{
                //    toolStripButton6_Click(null, null);
                //}
            }
        }

        private void tsbGetLen_Click(object sender, EventArgs e)
        {
            //2点测距
            //0=不绘图，1=绘直线,2=绘曲线，3=绘矩形，4=绘填充矩形，5=绘圆，6=绘填充圆,9=手绘任意线
            if (!tsbGetLen.Checked)
            {
                _drawParams.iDrawType = 1;
                tsbDrawLine.Checked = false;
                tsbDrawRect.Checked = false;
                tsbGetLen.Checked = true;
                iGetLineDist = 1;//直线测距标志，0=空状态，1=测距状态
            }
            else
            {
                _drawParams.iDrawType = 0;
                tsbDrawLine.Checked = false;
                tsbDrawRect.Checked = false;
                tsbGetLen.Checked = false;
                iGetLineDist = 0;//直线测距标志，0=空状态，1=测距状态
            }
            tsmiCPcj.Checked = tsbGetLen.Checked;
            tsmiCPzx.Checked = false;
            tsmiCPjx.Checked = false;

            _drawParams.iDrawAirLine = false;//20150324 wm add

            _drawParams.eventDrawed -= OnDrawEnd;
            _drawParams.eventDrawed += OnDrawEnd;
            baseMap.ZoomPanTool.setDrawParams(_drawParams);
        }

        private void tsbDrawLine_Click(object sender, EventArgs e)
        {
            //0=不绘图，1=绘直线,2=绘曲线，3=绘矩形，4=绘填充矩形，5=绘圆，6=绘填充圆,9=手绘任意线
            if (!tsbDrawLine.Checked)
            {
                _drawParams.iDrawType = 1;
                tsbDrawLine.Checked = true;
                tsbDrawRect.Checked = false;
                tsbGetLen.Checked = false;
            }
            else
            {
                _drawParams.iDrawType = 0;
                tsbDrawLine.Checked = false;
                tsbDrawRect.Checked = false;
                tsbGetLen.Checked = false;
            }
            tsmiCPcj.Checked = false;
            tsmiCPzx.Checked = tsbDrawLine.Checked;
            tsmiCPjx.Checked = false;

            _drawParams.iDrawAirLine = false;//20150324 wm add

            iGetLineDist = 0;//直线测距标志，0=空状态，1=测距状态
            _drawParams.eventDrawed -= OnDrawEnd;
            _drawParams.eventDrawed += OnDrawEnd;
            baseMap.ZoomPanTool.setDrawParams(_drawParams);
        }




        private void tsbDrawRect_Click(object sender, EventArgs e)
        {
            //0=不绘图，1=绘直线,2=绘曲线，3=绘矩形，4=绘填充矩形，5=绘圆，6=绘填充圆,9=手绘任意线
            if (!tsbDrawRect.Checked)
            {
                _drawParams.iDrawType = 3;
                tsbDrawLine.Checked = false;
                tsbDrawRect.Checked = true;
                tsbGetLen.Checked = false;
                //不打勾

            }
            else
            {
                _drawParams.iDrawType = 0;
                tsbDrawLine.Checked = false;
                tsbDrawRect.Checked = false;
                tsbGetLen.Checked = false;
            }
            tsmiCPcj.Checked = false;
            tsmiCPzx.Checked = false;
            tsmiCPjx.Checked = tsbDrawRect.Checked;

            _drawParams.iDrawAirLine = false;//20150324 wm add

            iGetLineDist = 0;//直线测距标志，0=空状态，1=测距状态
            _drawParams.eventDrawed -= OnDrawEnd;
            _drawParams.eventDrawed += OnDrawEnd;
            baseMap.ZoomPanTool.setDrawParams(_drawParams);
        }

        private void tsmiDDCT_Click(object sender, EventArgs e)
        {
            //定点采样

            if (iNowProj == 1 || iNowProj == 3)
            {
                MessageBox.Show("请在等经纬度投影或麦卡托投影下使用此功能！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //if (_drawPRet.iDrawType == 1)
            //{
            //    toolStripButton5_Click_1(null, null);
            //}
            //else if (_drawPRet.iDrawType == 3)
            //{
            //    toolStripButton6_Click(null, null);
            //}
            //-----------------------------------------------
            if (null != _drawPRet)
            {
                if (_drawPRet.listPoints.Count == 2)
                {
                    //屏幕点
                    Point p1 = _drawPRet.listPoints[0];
                    Point p2 = _drawPRet.listPoints[1];

                    //经纬度坐标
                    //PointF pf1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                    //PointF pf2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                    if (null != baseMap.WMap.BufferBmp)
                    {
                        Point pLT = new Point();
                        Point pRB = new Point();

                        pLT.X = p1.X < p2.X ? p1.X : p2.X;
                        pLT.Y = p1.Y < p2.Y ? p1.Y : p2.Y;

                        pRB.X = p1.X > p2.X ? p1.X : p2.X;
                        pRB.Y = p1.Y > p2.Y ? p1.Y : p2.Y;

                        Bitmap bmpRect = PubUnit.copyBitmapRect(baseMap.WMap.BufferBmp, pLT, pRB);
                        //bmpRect.Save(@"G:\aa.bmp");

                        CSivFileInfo sfi1 = listBoxMap.SelectedItem as CSivFileInfo;
                        if (sfi1 != null)
                        {
                            string sMapPath = sfi1.getVissPathName(iNowType, iNowWay, iNowProj);

                            LayerVISS lvis1 = new LayerVISS();
                            lvis1.Load(sMapPath);
                            //SVissrUnit svu1 = lvis1.visObj.svuObj;

                            frmFXLine fLine = new frmFXLine();
                            fLine.pnt1 = p1;
                            fLine.pnt2 = p2;
                            fLine.pfJW1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                            fLine.pfJW2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                            fLine.pLT = pLT;
                            fLine.pRB = pRB;

                            fLine.pfJWLT = baseMap.WMap.Coord.ScreenToWorld(pLT);
                            fLine.pfJWRB = baseMap.WMap.Coord.ScreenToWorld(pRB);

                            fLine.iNowType = iNowType;
                            fLine.sMapPathName = sMapPath;
                            //fLine.svuObj = svu1;
                            fLine.visObj = lvis1.visObj;
                            fLine.bmpRect = bmpRect;
                            fLine.ShowDialog();
                        }
                    }
                }
            }
        }

        public Bitmap bmpRect1;
        public Bitmap bmpRect2;
        public Point pRectLT;
        public Point pRectRB;

        private void tsmiYTDB_Click(object sender, EventArgs e)
        {
            if (iNowProj != 4)
            {
                MessageBox.Show("请在等经纬度投影下使用此功能！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //云类对比
            if (null == _drawPRet)
                return;

            if (null != _drawPRet)
            {
                if (_drawPRet.listPoints.Count == 2)
                {
                    //屏幕点
                    Point p1 = _drawPRet.listPoints[0];
                    Point p2 = _drawPRet.listPoints[1];

                    //经纬度坐标,2014-9-20 wm add
                    PointF pf1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                    PointF pf2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                    if (null != baseMap.WMap.BufferBmp)
                    {
                        Point pLT = new Point();
                        Point pRB = new Point();

                        pLT.X = p1.X < p2.X ? p1.X : p2.X;
                        pLT.Y = p1.Y < p2.Y ? p1.Y : p2.Y;

                        pRB.X = p1.X > p2.X ? p1.X : p2.X;
                        pRB.Y = p1.Y > p2.Y ? p1.Y : p2.Y;

                        //2014-9-21 wm add
                        CSivFileInfo sfi2 = listBoxMap.SelectedItem as CSivFileInfo;
                        if (sfi2 == null)
                        {
                            MessageBox.Show("未检测到相应通道的云图！", "提示");
                            return;
                        }
                        if (sfi2 != null)
                        {
                            //判断所有通道文件是否存在,2014-12-17,wm add
                            bool[] ex = new bool[5] { true, true, true, true, true };

                            string re = CheckAllChannel(sfi2, iNowProj);
                            if (re.IndexOf("1") >= 0)
                                ex[0] = false;
                            if (re.IndexOf("2") >= 0)
                                ex[1] = false;
                            if (re.IndexOf("3") >= 0)
                                ex[2] = false;
                            if (re.IndexOf("4") >= 0)
                                ex[3] = false;
                            if (re.IndexOf("5") >= 0)
                                ex[4] = false;

                            frmFXSelWay fsetway = new frmFXSelWay();
                            fsetway.iNowType = iNowType;
                            fsetway.iNowWay = iNowWay;
                            fsetway.iNowProj = iNowProj;
                            fsetway.pf1 = pf1;
                            fsetway.pf2 = pf2;
                            fsetway.sfi = sfi2;
                            for (int i = 0; i < 5; i++)
                            {
                                fsetway.exist[i] = ex[i];
                            }
                            fsetway.ShowDialog();
                        }
                        //Application.DoEvents();
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void tsmiYLFX_Click(object sender, EventArgs e)
        {
            //FCM云类分析
            if (iNowType != 2)
            {
                MessageBox.Show("请在可见光通道下使用此功能！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (null == _drawPRet)
                return;

            if (null != _drawPRet)
            {
                if (_drawPRet.listPoints.Count == 2)
                {
                    //屏幕点
                    Point p1 = _drawPRet.listPoints[0];
                    Point p2 = _drawPRet.listPoints[1];

                    //经纬度坐标,2014-9-20 wm add
                    PointF pf1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                    PointF pf2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                    if (null != baseMap.WMap.BufferBmp)
                    {
                        Point pLT = new Point();
                        Point pRB = new Point();

                        pLT.X = p1.X < p2.X ? p1.X : p2.X;
                        pLT.Y = p1.Y < p2.Y ? p1.Y : p2.Y;

                        pRB.X = p1.X > p2.X ? p1.X : p2.X;
                        pRB.Y = p1.Y > p2.Y ? p1.Y : p2.Y;

                        //2014-9-21 wm add
                        CSivFileInfo sfi2 = listBoxMap.SelectedItem as CSivFileInfo;
                        if (sfi2 == null)
                        {
                            MessageBox.Show("未检测到相应通道的云图！", "提示");
                            return;
                        }
                        if (sfi2 != null)
                        {
                            //td1、td2分别为通道1和通道2
                            //{1,0}=IR1,{1,1}=IR2,{1,2}=IR3,{1,3}=IR4,{2,0}=VIS
                            int[] td1 = new int[2] { 1, 0 };
                            int[] td2 = new int[2] { 2, 0 };
                            //iNowType:IR=1,VIS=2
                            //iNowWay:0~3
                            //iNowProj:投影方式
                            string sMapPath_IR1 = sfi2.getVissPathName(td1[0], td1[1], iNowProj);
                            string sMapPath_VIS = sfi2.getVissPathName(td2[0], td2[1], iNowProj);

                            LayerVISS lvis_IR1 = new LayerVISS();
                            LayerVISS lvis_VIS = new LayerVISS();

                            lvis_IR1.Load(sMapPath_IR1);
                            lvis_VIS.Load(sMapPath_VIS);
                            //SVissrUnit svu1 = lvis1.visObj.svuObj;
                            FCM_VVnum fcm_vvnum = new FCM_VVnum();
                            fcm_vvnum.yx1 = TD_youxiaoyuzhi(td1);
                            fcm_vvnum.yx2 = TD_youxiaoyuzhi(td2);
                            fcm_vvnum.pf1 = pf1;
                            fcm_vvnum.pf2 = pf2;
                            fcm_vvnum.sMapPathName_IR1 = sMapPath_IR1;
                            fcm_vvnum.sMapPathName_VIS = sMapPath_VIS;
                            fcm_vvnum.visObj_IR1 = lvis_IR1.visObj;
                            fcm_vvnum.visObj_VIS = lvis_VIS.visObj;
                            fcm_vvnum.Show();

                        }
                        //Application.DoEvents();
                    }
                }
                else
                {
                    return;
                }
            }
        }

        //DBSCAN云类分析,2014-10-9,wm add
        private void tsmiDBSCAN_Click(object sender, EventArgs e)
        {

            if (null == _drawPRet)
                return;

            if (null != _drawPRet)
            {
                if (_drawPRet.listPoints.Count == 2)
                {
                    //屏幕点
                    Point p1 = _drawPRet.listPoints[0];
                    Point p2 = _drawPRet.listPoints[1];

                    //经纬度坐标,2014-9-20 wm add
                    PointF pf1 = baseMap.WMap.Coord.ScreenToWorld(p1);
                    PointF pf2 = baseMap.WMap.Coord.ScreenToWorld(p2);

                    if (null != baseMap.WMap.BufferBmp)
                    {
                        Point pLT = new Point();
                        Point pRB = new Point();

                        pLT.X = p1.X < p2.X ? p1.X : p2.X;
                        pLT.Y = p1.Y < p2.Y ? p1.Y : p2.Y;

                        pRB.X = p1.X > p2.X ? p1.X : p2.X;
                        pRB.Y = p1.Y > p2.Y ? p1.Y : p2.Y;

                        //2014-9-21 wm add
                        CSivFileInfo sfi3 = listBoxMap.SelectedItem as CSivFileInfo;
                        if (sfi3 == null)
                        {
                            MessageBox.Show("未检测到相应通道的云图！", "提示");
                            return;
                        }
                        if (sfi3 != null)
                        {
                            //iNowType:IR=1,VIS=2
                            //iNowWay:0~3
                            //iNowProj:投影方式
                            string sMapPath_IR1 = sfi3.getVissPathName(1, 0, iNowProj);

                            LayerVISS lvis_IR1 = new LayerVISS();
                            lvis_IR1.Load(sMapPath_IR1);

                            frmFXCloudTypeDBSCAN dbscan = new frmFXCloudTypeDBSCAN();
                            dbscan.pf1 = pf1;
                            dbscan.pf2 = pf2;
                            dbscan.sMapPathName_IR1 = sMapPath_IR1;
                            dbscan.visObj_IR1 = lvis_IR1.visObj;
                            dbscan.ShowDialog();
                        }
                        //Application.DoEvents();
                    }
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        //根据数组判断通道有效点阈值，2014-9-22 wm add
        public int TD_youxiaoyuzhi(int[] td)
        {
            if (td[0] == 1 && td[1] == 0)//IR1
            {
                return 110;
            }
            else if (td[0] == 1 && td[1] == 1)//IR2
            {
                return 110;
            }
            else if (td[0] == 1 && td[1] == 2)//IR3
            {
                return 200;
            }
            else if (td[0] == 1 && td[1] == 3)//IR4
            {
                return 210;
            }
            else//VIS
            {
                return 20;
            }
        }

        //2014-9-18 wm add
        /// <summary>
        /// 温度间隔改变响应事件
        /// </summary>
        private void WenDuJianGe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WenDuJianGe.Items.Count > 0 && WenDuJianGe.SelectedItem != null)
            {
                switch (WenDuJianGe.SelectedIndex)
                {
                    case 0:
                        WenDuJG = 5;
                        break;
                    case 1:
                        WenDuJG = 10;
                        break;
                    case 2:
                        WenDuJG = 15;
                        break;
                    case 3:
                        WenDuJG = 20;
                        break;
                    default:
                        WenDuJG = 5;
                        break;
                }
            }
        }

        //2014-10-21 wm add
        private void timer_LBJBHW_Tick(object sender, EventArgs e)
        {
            //timer_LBJBHW.Enabled = false;
            if (lBoxLB.SelectedIndex < 0)
            {
                labLBJD1.Visible = false;
                labLBWD1.Visible = false;
            }
            else
            {
                labLBJD1.Visible = true;
                labLBWD1.Visible = true;
            }
            if (lBoxJB.SelectedIndex < 0)
            {
                labJBJD1.Visible = false;
                labJBWD1.Visible = false;
            }
            else
            {
                labJBJD1.Visible = true;
                labJBWD1.Visible = true;
            }
            if (lBoxHW.SelectedIndex < 0)
            {
                labHWJD1.Visible = false;
                labHWWD1.Visible = false;
            }
            else
            {
                labHWJD1.Visible = true;
                labHWWD1.Visible = true;
            }
        }

        #region 线程监听函数（云际风、雷暴、积冰、海雾、等值线、FCM、DBSCAN）
        //=============================================================================================
        #region 云际风监听函数开始
        public void setMax_yjf(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_yjf.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(cloudWind.setMaxThread, new object[] { iMax });
            }
            else
            {
                tspBar_yjf.Maximum = iMax;
            }
        }

        public void updateProgressBar_yjf(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_yjf.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(cloudWind.mainThread, new object[] { value });
            }
            else
            {
                tspBar_yjf.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted_yjf()
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_yjf.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(cloudWind.completeThread, new object[] { });
            }
            else
            {
                //------------------------------------------------------------------------------
                ShowStatusInfo("正在输出...");
                Application.DoEvents();
                //保存输出文件
                bool flag = cloudWind.SaveAsProduct();

                //2014-11-21 wm add
                if (flag)
                {
                    ADD_listFYYT(dlgSelYJF.first);//将云图添加到反演云图列表
                    FindLoadYJFFileList();
                    if (selectedType == "wind")
                    {
                        isShowYJF = true;
                    }
                    else
                    {
                        isShowYJF = false;
                    }
                    for (int i = 0; i < listBoxYJF.Items.Count; i++)
                    {
                        CYJFFileInfo fInfo = listBoxYJF.Items[i] as CYJFFileInfo;
                        if (fInfo.sPath.Contains(newItemNameYJF))
                        {
                            listBoxYJF.SelectedIndex = i;
                            break;
                        }
                    }
                    ShowStatusInfo("云迹风生成完毕！");
                }
                else
                {
                    ShowStatusInfo("云迹风生成失败！");
                }
                tspBar_yjf.Value = 0;
                tspBar_yjf.Visible = false;
                ClearMemory();
            }
        }
        #endregion
        //=============================================================================================

        //=============================================================================================
        #region 雷暴监听函数开始
        public void setMax_lb(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_lb.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(thunder.setMaxThread, new object[] { iMax });
            }
            else
            {
                tspBar_lb.Maximum = iMax;
            }
        }

        public void updateProgressBar_lb(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_lb.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(thunder.mainThread, new object[] { value });
            }
            else
            {
                tspBar_lb.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted_lb()
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_lb.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(thunder.completeThread, new object[] { });
            }
            else
            {
                ShowStatusInfo("正在输出...");
                Application.DoEvents();

                //保存输出文件
                bool flag = thunder.SaveAsProduct();

                //2014-11-21 wm add
                if (flag)
                {
                    ADD_listFYYT(dlgSelectLB.first);//将云图添加到反演云图列表
                    //重新载入雷暴图列表
                    FindLoadLeiBaoFileList();
                    if (selectedType == "thunder")
                    {
                        isShowLB = true;
                    }
                    else
                    {
                        isShowLB = false;
                    }
                    for (int i = 0; i < lBoxLB.Items.Count; i++)
                    {
                        CLBFileInfo fInfo = lBoxLB.Items[i] as CLBFileInfo;
                        if (fInfo.sPath.Contains(newItemNameLB))
                        {
                            lBoxLB.SelectedIndex = i;
                            break;
                        }
                    }
                    ShowStatusInfo("雷暴生成完毕！");
                }
                else
                {
                    ShowStatusInfo("该时段内未检测到雷暴！");
                }
                //------------------------------------------------------------------------------
                tspBar_lb.Value = 0;
                tspBar_lb.Visible = false;
                ClearMemory();
            }
        }
        #endregion
        //=============================================================================================

        //=============================================================================================
        #region 积冰监听函数开始
        public void setMax_jb(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_jb.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(ice.setMaxThread, new object[] { iMax });
            }
            else
            {
                tspBar_jb.Maximum = iMax;
            }
        }

        public void updateProgressBar_jb(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_jb.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(ice.mainThread, new object[] { value });
            }
            else
            {
                tspBar_jb.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted_jb()
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_jb.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(ice.completeThread, new object[] { });
            }
            else
            {
                ShowStatusInfo("正在输出...");
                Application.DoEvents();

                //保存输出文件
                bool flag = ice.SaveAsProduct();

                //2014-11-21 wm add
                if (flag)
                {
                    ADD_listFYYT(dlgSelectJB.first);
                    //重新载入积冰图列表
                    FindLoadJBFileList();

                    if (selectedType == "ice")
                    {
                        isShowJB = true;
                    }
                    else
                    {
                        isShowJB = false;
                    }
                    for (int i = 0; i < lBoxJB.Items.Count; i++)
                    {
                        CJBFileInfo fInfo = lBoxJB.Items[i] as CJBFileInfo;
                        if (fInfo.sPath.Contains(newItemNameJB))
                        {
                            lBoxJB.SelectedIndex = i;
                            break;
                        }
                    }
                    ShowStatusInfo("积冰生成完毕！");
                }
                else
                {
                    ShowStatusInfo("该时段内未检测到积冰！");
                }

                //------------------------------------------------------------------------------
                tspBar_jb.Value = 0;
                tspBar_jb.Visible = false;
                ClearMemory();
            }
        }
        #endregion
        //=============================================================================================

        //=============================================================================================
        #region 海雾监听函数开始
        public void setMax_hw(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_hw.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(seaFog.setMaxThread, new object[] { iMax });
            }
            else
            {
                tspBar_hw.Maximum = iMax;
            }
        }

        public void updateProgressBar_hw(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_hw.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(seaFog.mainThread, new object[] { value });
            }
            else
            {
                tspBar_hw.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted_hw()
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_hw.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(seaFog.completeThread, new object[] { });
            }
            else
            {
                ShowStatusInfo("正在输出...");
                Application.DoEvents();

                //保存输出文件
                bool flag = seaFog.SaveAsProduct();

                //2014-11-21 wm add
                if (flag)
                {
                    ADD_listFYYT(dlgSelectHW.first);
                    //重新载入积冰图列表
                    FindLoadHaiWuFileList();
                    if (selectedType == "fog")
                    {
                        isShowHW = true;
                    }
                    else
                    {
                        isShowHW = false;
                    }
                    for (int i = 0; i < lBoxHW.Items.Count; i++)
                    {
                        CHWFileInfo fInfo = lBoxHW.Items[i] as CHWFileInfo;
                        if (fInfo.sPath.Contains(newItemNameHW))
                        {
                            lBoxHW.SelectedIndex = i;
                            break;
                        }
                    }
                    ShowStatusInfo("陆海雾生成完毕！");
                }
                else
                {
                    ShowStatusInfo("该时段内未检测到陆海雾！");
                }

                //------------------------------------------------------------------------------
                tspBar_hw.Value = 0;
                tspBar_hw.Visible = false;
                ClearMemory();
            }
        }
        #endregion
        //=============================================================================================

        //=============================================================================================
        #region 等值线监听函数开始
        public void setMax_dzx(int iMax)
        {
            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_dzx.Control.InvokeRequired)
            {
                ////this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(contour.setMaxThread, new object[] { iMax });
            }
            else
            {
                tspBar_dzx.Maximum = iMax;
            }
        }

        public void updateProgressBar_dzx(int value)
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_dzx.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(contour.mainThread, new object[] { value });
            }
            else
            {
                tspBar_dzx.Value = value;
            }
            //Application.DoEvents();
        }


        public void onMakeDataCompleted_dzx()
        {

            //判断该方法是否被主线程调用，也就是创建labMessage1控件的线程，当控件的InvokeRequired属性为ture时，说明是被主线程以外的线程调用。如果不加判断，会造成异常
            if (this.tspBar_dzx.Control.InvokeRequired)
            {
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i)
                this.Invoke(contour.completeThread, new object[] { });
            }
            else
            {
                //------------------------------------------------------------------------------
                ShowStatusInfo("正在输出...");
                Application.DoEvents();

                //保存输出文件
                bool flag = contour.SaveAsProduct();

                //2014-11-21 wm add
                if (flag)
                {
                    ADD_listFYYT(dlgSelectDZX.first);
                    FindLoadDZXFileList();

                    if (selectedType == "contour")
                    {
                        isShowYJF = true;
                    }
                    else
                    {
                        isShowYJF = false;
                    }
                    for (int i = 0; i < lBoxDZX.Items.Count; i++)
                    {
                        CDZXFileInfo fInfo = lBoxDZX.Items[i] as CDZXFileInfo;
                        if (fInfo.filePath.Contains(newItemNameDZX))
                        {
                            lBoxDZX.SelectedIndex = i;
                            break;
                        }
                    }
                    ShowStatusInfo("等值线生成完毕！");
                }
                else
                {
                    ShowStatusInfo("等值线生成失败！");
                }

                tspBar_dzx.Value = 0;
                tspBar_dzx.Visible = false;
                ClearMemory();
            }
        }
        #endregion
        //=============================================================================================
        #endregion

        //2014-10-26 wm add
        //判断ListBoxMap索引index项是否有反演产品
        public List<int> fyProduct_id;//记录对应云图的反演产品标号，YJF：5000000+，LB：6000000+，JB：7000000+，HW:8000000+

        /// <summary>
        /// 判断主图ListBoxMap索引index项是否有反演产品
        /// </summary>
        public bool HaveFYProduct(int index)
        {
            fyProduct_id = new List<int>();
            string YunTuName = listBoxMap.Items[index].ToString();

            //判断云迹风
            for (int k = 0; k < listBoxYJF.Items.Count; k++)
            {
                CYJFFileInfo sfi = listBoxYJF.Items[k] as CYJFFileInfo;
                if (sfi != null)
                {
                    if (sfi.getYTName() == YunTuName)
                    {
                        int id = 5000000 + k;
                        fyProduct_id.Add(id);
                    }
                }
            }

            //判断雷暴
            for (int k = 0; k < lBoxLB.Items.Count; k++)
            {
                CLBFileInfo sfi = lBoxLB.Items[k] as CLBFileInfo;
                if (sfi != null)
                {
                    if (sfi.getYTName() == YunTuName)
                    {
                        int id = 6000000 + k;
                        fyProduct_id.Add(id);
                        break;
                    }
                }
            }

            //判断积冰
            for (int k = 0; k < lBoxJB.Items.Count; k++)
            {
                CJBFileInfo sfi = lBoxJB.Items[k] as CJBFileInfo;
                if (sfi != null)
                {
                    if (sfi.getYTName() == YunTuName)
                    {
                        int id = 7000000 + k;
                        fyProduct_id.Add(id);
                        break;
                    }
                }
            }

            //判断海雾
            for (int k = 0; k < lBoxHW.Items.Count; k++)
            {
                CHWFileInfo sfi = lBoxHW.Items[k] as CHWFileInfo;
                if (sfi != null)
                {
                    if (sfi.getYTName() == YunTuName)
                    {
                        int id = 8000000 + k;
                        fyProduct_id.Add(id);
                        break;
                    }
                }
            }
            //判断等值线
            for (int k = 0; k < lBoxDZX.Items.Count; k++)
            {
                CDZXFileInfo sfi = lBoxDZX.Items[k] as CDZXFileInfo;
                if (sfi != null)
                {
                    if (sfi.getYTName() == YunTuName)
                    {
                        int id = 9000000 + k;
                        fyProduct_id.Add(id);
                        break;
                    }
                }
            }

            if (fyProduct_id.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //判断逻辑减，2014-10-31 wm add

        /// <summary>
        /// 判断主图ListBoxMap索引index项是否有逻辑减产品
        /// </summary>
        public bool HaveLogicFile(int index)
        {
            string YunTuName = listBoxMap.Items[index].ToString();
            string f1, f2;//逻辑减涉及的两个云图名称
            foreach (FileInfo fi in new DirectoryInfo(userSysParams.LogicOutPatn).GetFiles("*.opm", SearchOption.AllDirectories))
            {
                string sShowName = fi.Name;//解析文件名
                string sPathName = fi.FullName;
                f1 = "f20" + sShowName.Substring(1, 2) + "-" + sShowName.Substring(3, 2) + "-" + sShowName.Substring(5, 2) + "_" + sShowName.Substring(7, 4);
                f2 = "f20" + sShowName.Substring(12, 2) + "-" + sShowName.Substring(14, 2) + "-" + sShowName.Substring(16, 2) + "_" + sShowName.Substring(18, 4);
                if (YunTuName.Equals(f1) || YunTuName.Equals(f2))
                {
                    return true;
                }
            }
            return false;
        }

        //刷新lBoxLogicYT，找到逻辑减对应的两个云图，2014-10-31 wm add
        public void Refresh_lBoxLogicYT(int index)
        {
            lBoxLogicYT.Items.Clear();
            string sShowName = listBoxLogic.Items[index].ToString();
            string f1 = "f20" + sShowName.Substring(1, 2) + "-" + sShowName.Substring(3, 2) + "-" + sShowName.Substring(5, 2) + "_" + sShowName.Substring(7, 4);
            string f2 = "f20" + sShowName.Substring(12, 2) + "-" + sShowName.Substring(14, 2) + "-" + sShowName.Substring(16, 2) + "_" + sShowName.Substring(18, 4);
            int count = 0;
            for (int i = 0; i < listFYYT.Count; i++)
            {
                string YTName = listFYYT[i].sName;
                if (YTName.Equals(f1))
                {
                    lBoxLogicYT.Items.Add(listFYYT[i]);
                    count++;
                }
                if (YTName.Equals(f2))
                {
                    lBoxLogicYT.Items.Add(listFYYT[i]);
                    count++;
                }
                if (count >= 2)
                {
                    break;
                }
            }
            if (count < 2)
            {
                ShowStatusInfo("云图缺失，未能添加到相关云图列表！");
            }
        }

        #region 根据索引index删除指定的反演产品

        //删除云迹风,2014-10-26,wm add
        public void DeleteYJF(int index)
        {
            if ((index >= 0) && (index < listBoxYJF.Items.Count))
            {
                CYJFFileInfo sfi = listBoxYJF.Items[index] as CYJFFileInfo;
                if (sfi != null)
                {
                    string sLayName = sfi.sLayName;
                    try
                    {
                        WRFileUnit.writeLogStr("删除云迹风： " + sLayName);
                        File.Delete(sfi.sPath);
                        //刷新文件列表
                        listBoxYJF.Items.RemoveAt(index);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }
        //删除雷暴,2014-10-26,wm add
        public void DeleteLB(int index)
        {
            if ((index >= 0) && (index < lBoxLB.Items.Count))
            {
                CLBFileInfo sfi = lBoxLB.Items[index] as CLBFileInfo;
                if (sfi != null)
                {
                    string sLayName = sfi.sLayName;
                    try
                    {
                        WRFileUnit.writeLogStr("删除雷暴： " + sLayName);
                        File.Delete(sfi.sPath);
                        //刷新文件列表
                        lBoxLB.Items.RemoveAt(index);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }
        //删除积冰,2014-10-26,wm add
        public void DeleteJB(int index)
        {
            if ((index >= 0) && (index < lBoxJB.Items.Count))
            {
                CJBFileInfo sfi = lBoxJB.Items[index] as CJBFileInfo;
                if (sfi != null)
                {
                    string sLayName = sfi.sLayName;
                    try
                    {
                        WRFileUnit.writeLogStr("删除积冰： " + sLayName);
                        File.Delete(sfi.sPath);
                        //刷新文件列表
                        lBoxJB.Items.RemoveAt(index);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }
        //删除海雾,2014-10-26,wm add
        public void DeleteHW(int index)
        {
            if ((index >= 0) && (index < lBoxHW.Items.Count))
            {
                CHWFileInfo sfi = lBoxHW.Items[index] as CHWFileInfo;
                if (sfi != null)
                {
                    string sLayName = sfi.sLayName;
                    try
                    {
                        WRFileUnit.writeLogStr("删除海雾： " + sLayName);
                        File.Delete(sfi.sPath);
                        //刷新文件列表
                        lBoxHW.Items.RemoveAt(index);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }
        //删除等值线,2014-11-03,wcs add
        public void DeleteDZX(int index)
        {
            if ((index >= 0) && (index < lBoxDZX.Items.Count))
            {
                CDZXFileInfo fileInfo = lBoxDZX.Items[index] as CDZXFileInfo;
                if (fileInfo != null)
                {
                    string sLayName = fileInfo.layerName;
                    try
                    {
                        WRFileUnit.writeLogStr("删除海雾： " + sLayName);
                        File.Delete(fileInfo.filePath);
                        //刷新文件列表
                        lBoxDZX.Items.RemoveAt(index);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
        }
        #endregion

        //为反演云图列表添加项,2014-10-26 wm add
        public bool ADD_listFYYT(CSivFileInfo csfi)
        {
            bool bListExist = false;
            //按名字去除重复项
            foreach (CSivFileInfo item in listFYYT)
            {
                if (item.sName.Equals(csfi.sName))
                {
                    bListExist = true;
                    break;
                }
            }
            if (!bListExist)
            {
                listFYYT.Add(csfi);
                return true;
            }
            return false;
        }

        //切换投影通道反演产品，2014-11-2
        public string FYtype;
        public void ChangeFYTD(object sender, EventArgs e, string type)
        {
            switch (type)
            {
                case "yjf":
                    {
                        listBoxYJF_SelectedIndexChanged(sender, e);
                        break;
                    }
                case "lb":
                    {
                        lBoxLB_SelectedIndexChanged(sender, e);
                        break;
                    }
                case "jb":
                    {
                        lBoxJB_SelectedIndexChanged(sender, e);
                        break;
                    }
                case "hw":
                    {
                        lBoxHW_SelectedIndexChanged(sender, e);
                        break;
                    }
                case "dzx":
                    {
                        lBoxDZX_SelectedIndexChanged(sender, e);
                        break;
                    }
                case "logic":
                    {
                        //lBoxLogicYT_SelectedIndexChanged(sender, e);
                        FindLoadLogicFileList();//载入逻辑运算云图
                        break;
                    }
                default:
                    {
                        listBoxMap_SelectedIndexChanged(sender, e);
                        break;
                    }
            }
        }

        private void chkShowDZXyt_CheckedChanged(object sender, EventArgs e)
        {
            lBoxDZX_SelectedIndexChanged(sender, e);
        }

        private void listBoxMap_MouseDown(object sender, MouseEventArgs e)
        {
            selectItem(sender, e, listBoxMap);
        }

        private void listBoxYJF_MouseDown(object sender, MouseEventArgs e)
        {
            selectItem(sender, e, listBoxYJF);
        }

        private void listBoxLogic_MouseDown(object sender, MouseEventArgs e)
        {
            selectItem(sender, e, listBoxLogic);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                //动画界面
                listActFolder();
                setToolBarBntState(1);
                return;
            }
            //停止播放
            btnStop_Click(sender, e);
            setToolBarBntState(0);
            if (tabControl1.SelectedIndex == 2)
            {
                tabControl2_SelectedIndexChanged(null, null);
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                tabControl3_SelectedIndexChanged(null, null);

                FindLoadLogicFileList2();
            }
            else
            {
                selectedType = string.Empty;
            }

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = tabControl2.SelectedIndex;
            switch (index)
            {
                case 0:
                    selectedType = "wind";
                    break;
                case 1:
                    selectedType = "thunder";
                    break;
                case 2:
                    selectedType = "ice";
                    break;
                case 3:
                    selectedType = "fog";
                    break;
                default:
                    break;
            }
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = tabControl3.SelectedIndex;
            switch (index)
            {
                case 0:
                    selectedType = "logic";
                    break;
                case 1:
                    selectedType = "contour";
                    break;
                default:
                    break;
            }
        }

        //2014-11-22 wm add
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            FindLoadFYMapFile();
        }

        //检测所有通道是否存在,2014-12-17 wm add
        private string CheckAllChannel(CSivFileInfo sfi, int iProj)
        {
            string result = "";
            string sMapPathIR1 = sfi.getVissPathName(1, 0, iProj);
            string sMapPathIR2 = sfi.getVissPathName(1, 1, iProj);
            string sMapPathIR3 = sfi.getVissPathName(1, 2, iProj);
            string sMapPathIR4 = sfi.getVissPathName(1, 3, iProj);
            string sMapPathVis1 = sfi.getVissPathName(2, 0, iProj);
            if (!File.Exists(sMapPathIR1))
            {
                result += "1";
            }
            if (!File.Exists(sMapPathIR2))
            {
                result += "2";
            }
            if (!File.Exists(sMapPathIR3))
            {
                result += "3";
            }
            if (!File.Exists(sMapPathIR4))
            {
                result += "4";
            }
            if (!File.Exists(sMapPathVis1))
            {
                result += "5";
            }
            return result;
        }

        #endregion

        #region AreaForm“保存”按钮后面的执行 added by ZJB 20150412
        private void addArea_Click(object sender, EventArgs e)
        {
            AreaForm.editMode = false;
            AreaForm addareaform = new AreaForm();

            addareaform.newDraw += new AreaForm.DrawAirSpace(DrawAirspace);
            addareaform.deleteOldLayer += new AreaForm.deleteLayer(deleteTheOldLayer);
            addareaform.ShowDialog();

        }
        public void deleteTheOldLayer(string aname)
        {
            if (AirSpaceItem.DropDownItems.Count > 0)
            {
                for (int i = 0; i < AirSpaceItem.DropDownItems.Count; i++)
                {
                    if (AirSpaceItem.DropDownItems[i].Text == aname)
                    {
                        AirSpaceItem.DropDownItems.RemoveAt(i);
                        break;
                    }
                }
            }

            string oldFile = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\" + aname + ".ASD";
            if (File.Exists(oldFile))//同时删除原来的checked中保存的图层数据文件;
            {
                File.Delete(oldFile);
            }

            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (lay.Name.Equals(aname + ".ASD"))
                {
                    baseMap.WMap.LayerList.RemoveAt(i);
                }

            }


        }
        public void DrawAirspace(string aname)
        {
            #region 载入飞行航线
            //if (!File.Exists(Application.StartupPath + @"\workspace\飞行航线.DAT"))
            //{
            //    return false;
            //}

            ////已存在该图层则不添加，20150324 wm add
            //for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            //{
            //    ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
            //    if (lay.Name.Equals("飞行航线.DAT"))
            //    {
            //        lay.IsTopLayer = true;//设置为首层！
            //        return true;
            //    }
            //}
            //if (!AirLineChoosed)
            //{
            //    return;
            //}
            string filePath = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + aname + ".ASD";
            IDataPlugin dp = PluginManager.getInstance().OpenData(filePath);
            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
            if (dp != null)
            {
                if (dp.Result is ICustmoLayer)
                {
                    //新添加图层

                    lays.Visible = true;
                    lays.IsTopLayer = true;

                    baseMap.WMap.AddLayer(lays);
                    //return true;
                }
            }
            //return false;
            bool layerSeted = false;
            string layerset = Application.StartupPath + Path.DirectorySeparatorChar + "layerSet.cfg";
            string[] lines = File.ReadAllLines(layerset);
            float penWidth;
            int penColor;
            foreach (string text in lines)
            {
                string[] aline = text.Split('|');
                if (aline[0] == aname + ".ASD")
                {
                    layerSeted = true;
                    float.TryParse(aline[1], out penWidth);
                    int.TryParse(aline[2], out penColor);
                    lays.Style.Pen.Width = penWidth;
                    lays.Style.Pen.Color = Color.FromArgb(penColor);
                    break;
                }

            }
            if (layerSeted == false)
            {
                lays.Style.Pen.Width = 2.0f;
                lays.Style.Pen.Color = Color.Red;
            }

            baseMap.Render();
            addAreaItem(aname);
            #endregion
        }
        public void setAirspaceVIsible(string s, bool visible)
        {
            for (int i = 0; i < baseMap.WMap.LayerList.Count; i++)
            {
                ICustmoLayer lay = baseMap.WMap.LayerList[i] as ICustmoLayer;
                if (lay.Name.Equals(s + ".ASD"))
                {
                    baseMap.WMap.LayerList.RemoveAt(i);
                }

            }
            string filePath = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + s + ".ASD";
            string file_checked = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\" + s + ".ASD";
            //string file = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + aname + ".ASD";
            string sFilePathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "layerSet.cfg";
            string AirspaceName = s + ".ASD";
            float penWidth = 2.0f;//笔宽
            int penColorInt = Color.Red.ToArgb();//笔颜色int
            if (File.Exists(sFilePathName))//配置文件文件存在,20150330，wm add
            {
                try
                {
                    List<string> listLaySet = new List<string>();

                    FileStream aFile = new FileStream(sFilePathName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(aFile);
                    while (!sr.EndOfStream)
                    {
                        // 这里处理每一行
                        string strLine = sr.ReadLine();
                        listLaySet.Add(strLine);
                    }
                    sr.Close();
                    aFile.Close();
                    for (int i = 0; i < listLaySet.Count; i++)
                    {
                        string item = listLaySet[i];
                        string[] strParams = item.Split('|');
                        if (strParams[0].Equals(AirspaceName))//通过AirspaceName分别判断每一个空域文件，达到对每一个空域分别设置的目的。
                        {
                            float.TryParse(strParams[1], out penWidth);
                            int.TryParse(strParams[2], out penColorInt);
                        }
                    }
                }
                catch (Exception aex)
                {
                    MessageBox.Show(aex.ToString());
                }
            }

            IDataPlugin dp = PluginManager.getInstance().OpenData(filePath);
            ICustmoLayer lays = (ICustmoLayer)(dp.Result);
            if (dp != null)
            {
                if (dp.Result is ICustmoLayer)
                {
                    //新添加图层

                    lays.Visible = visible;
                    lays.IsTopLayer = true;

                    baseMap.WMap.AddLayer(lays);
                    //return true;
                }
            }
            lays.Style.Pen.Width = penWidth;
            lays.Style.Pen.Color = Color.FromArgb(penColorInt);



            baseMap.Render();

        }
        public void Item_click(object sender, EventArgs e)
        {
            ToolStripMenuItem airspaceItem = (ToolStripMenuItem)(sender);
            airspaceItem.Checked = !airspaceItem.Checked;
            bool visible = airspaceItem.Checked;
            string s = airspaceItem.Text;
            string checkedfolder = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\";
            if (!Directory.Exists(checkedfolder))
            {
                Directory.CreateDirectory(checkedfolder);
            }
            string name = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + s + ".ASD";
            string newName = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\" + s + ".ASD";
            visible = airspaceItem.Checked;
            setAirspaceVIsible(airspaceItem.Text, visible);
            if (airspaceItem.Checked == true)
            {
                //File.Create(newName);
                if (File.Exists(name))
                {
                    if (!File.Exists(newName))
                        File.Copy(name, newName);

                }
                else
                {
                    MessageBox.Show("未找到文件：" + name, "文件错误");
                }

            }
            else
            {
                if (File.Exists(newName))
                    File.Delete(newName);
            }
            //MessageBox.Show((airspaceItem.Checked).ToString());


            //MessageBox.Show(s);
        }
        public void addAreaItem(string name)
        {
            ToolStripMenuItem it = new ToolStripMenuItem();
            it.Text = name;

            AirSpaceItem.DropDownItems.AddRange(new ToolStripMenuItem[] { it });
            it.Checked = true;

            it.Click += new System.EventHandler(Item_click);
            string origionname = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + name + ".ASD";
            string newName = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\" + name + ".ASD";
            string checkedFolder = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\";
            if (!Directory.Exists(checkedFolder))
            {
                Directory.CreateDirectory(checkedFolder);
            }
            if (!File.Exists(newName))
            {
                File.Copy(origionname, newName);
            }

        }
        #endregion

        #region 显示所有已设置为可见的图层 loadAllExistedAirspace

        public void loadAllExistedAirspace()
        {
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\"))
                Directory.CreateDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\");
            string[] files = Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\");
            //theAreaList = File.ReadAllLines(AreaForm.filename);
            AirSpaceItem.DropDownItems.Clear();
            string name;
            foreach (string aname in files)
            {
                name = aname.ToLower().Replace(".asd", "");
                name = Path.GetFileNameWithoutExtension(name);
                //MessageBox.Show(name);
                ToolStripMenuItem it = new ToolStripMenuItem();
                it.Text = name;
                AirSpaceItem.DropDownItems.AddRange(new ToolStripItem[] { it });
                it.Click += new System.EventHandler(Item_click);
                string file_checked = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\checked\\" + name + ".ASD";
                string file = Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\" + name + ".ASD";
                string sFilePathName = Application.StartupPath + Path.DirectorySeparatorChar.ToString() + "layerSet.cfg";
                string AirspaceName = name + ".ASD";
                float penWidth = 2.0f;//笔宽
                int penColorInt = Color.Red.ToArgb();//笔颜色int
                if (File.Exists(sFilePathName))//配置文件文件存在,20150330，wm add
                {
                    try
                    {
                        List<string> listLaySet = new List<string>();

                        FileStream aFile = new FileStream(sFilePathName, FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(aFile);
                        while (!sr.EndOfStream)
                        {
                            // 这里处理每一行
                            string strLine = sr.ReadLine();
                            listLaySet.Add(strLine);
                        }
                        sr.Close();
                        aFile.Close();
                        for (int i = 0; i < listLaySet.Count; i++)
                        {
                            string item = listLaySet[i];
                            string[] strParams = item.Split('|');
                            if (strParams[0].Equals(AirspaceName))//通过AirspaceName分别判断每一个空域文件，达到对每一个空域分别设置的目的。
                            {
                                float.TryParse(strParams[1], out penWidth);
                                int.TryParse(strParams[2], out penColorInt);
                            }
                        }
                    }
                    catch (Exception aex)
                    {
                        MessageBox.Show(aex.ToString());
                    }
                }
                else
                {
                    penWidth = 2.0f;
                    penColorInt = Color.Red.ToArgb();
                }
                if (File.Exists(file_checked))
                {

                    it.Checked = true;
                    IDataPlugin dp = PluginManager.getInstance().OpenData(file_checked);
                    ICustmoLayer lays = (ICustmoLayer)(dp.Result);
                    if (dp != null)
                    {
                        if (dp.Result is ICustmoLayer)
                        {
                            //新添加图层

                            lays.Visible = true;
                            lays.IsTopLayer = true;

                            baseMap.WMap.AddLayer(lays);
                            //return true;
                        }
                    }
                    lays.Style.Pen.Width = penWidth;
                    lays.Style.Pen.Color = Color.FromArgb(penColorInt);
                }
                else
                    continue;




            }
        }
        #endregion

        public void reDraw()
        {

            baseMap.Render();
        }
        private void DeleteAirspace_Click(object sender, EventArgs e)
        {
            DeleteAirspace DeleteForm = new DeleteAirspace();
            DeleteForm.deleteLayer += new DeleteAirspace.delete(deleteTheOldLayer);
            DeleteForm.aRender += new DeleteAirspace.render(reDraw);
            DeleteForm.ShowDialog();
        }

        public delegate void modeSelect();
        public modeSelect editTheAirspace;

        private void editAirspace_Click(object sender, EventArgs e)
        {
            AreaForm.editMode = true;
            DirectoryInfo filePath = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar + "workspace\\Airspaces\\");

            if (filePath.GetFiles().Length == 0)
            {
                MessageBox.Show("没有可编辑的空域存在！", "错误");
            }
            else
            {
                AreaForm aform = new AreaForm();
                aform.aRender += new AreaForm.render(reDraw);
                aform.newDraw += new AreaForm.DrawAirSpace(DrawAirspace);
                aform.deleteOldLayer += new AreaForm.deleteLayer(deleteTheOldLayer);
                aform.ShowDialog();
            }

            //editTheAirspace();
        }





    }

    //========================================启动画面，2015-1-10，wm add========================================
    #region 启动画面
    public class mycontext : SplashScreenApplicationContext
    {
        protected override void OnCreateSplashScreenForm()
        {
            this.SplashScreenForm = new SplashScreen();//启动窗体
        }

        protected override void OnCreateMainForm()
        {
            this.PrimaryForm = new MainForm();//主窗体
        }

        protected override void SetSeconds()
        {
            this.SecondsShow = 1;//启动窗体显示的时间(秒)
        }
    }

    //启动画面虚基类,启动画面会停留一段时间，该时间是设定的时间和主窗体构造所需时间两个的最大值
    public abstract class SplashScreenApplicationContext : ApplicationContext
    {
        private Form _SplashScreenForm;//启动窗体
        private Form _PrimaryForm;//主窗体
        private System.Timers.Timer _SplashScreenTimer;
        private int _SplashScreenTimerInterVal = 5000;//默认是启动窗体显示5秒
        private bool _bSplashScreenClosed = false;
        private delegate void DisposeDelegate();//关闭委托，下面需要使用控件的Invoke方法，该方法需要这个委托

        public SplashScreenApplicationContext()
        {
            this.ShowSplashScreen();//这里创建和显示启动窗体
            this.MainFormLoad();//这里创建和显示启动主窗体
        }

        protected abstract void OnCreateSplashScreenForm();

        protected abstract void OnCreateMainForm();

        protected abstract void SetSeconds();

        protected Form SplashScreenForm
        {
            set
            {
                this._SplashScreenForm = value;
            }
        }

        protected Form PrimaryForm
        {//在派生类中重写OnCreateMainForm方法，在MainFormLoad方法中调用OnCreateMainForm方法
            //  ,在这里才会真正调用Form1(主窗体)的构造函数，即在启动窗体显示后再调用主窗体的构造函数
            //  ，以避免这种情况:主窗体构造所需时间较长,在屏幕上许久没有响应，看不到启动窗体       
            set
            {
                this._PrimaryForm = value;
            }
        }

        protected int SecondsShow
        {//未设置启动画面停留时间时，使用默认时间
            set
            {
                if (value != 0)
                {
                    this._SplashScreenTimerInterVal = 1000 * value;
                }
            }
        }

        private void ShowSplashScreen()
        {
            this.SetSeconds();
            this.OnCreateSplashScreenForm();
            this._SplashScreenTimer = new System.Timers.Timer(((double)(this._SplashScreenTimerInterVal)));
            _SplashScreenTimer.Elapsed += new System.Timers.ElapsedEventHandler(new System.Timers.ElapsedEventHandler(this.SplashScreenDisplayTimeUp));

            this._SplashScreenTimer.AutoReset = false;
            Thread DisplaySpashScreenThread = new Thread(new ThreadStart(DisplaySplashScreen));

            DisplaySpashScreenThread.Start();
        }

        private void DisplaySplashScreen()
        {
            this._SplashScreenTimer.Enabled = true;
            Application.Run(this._SplashScreenForm);
        }

        private void SplashScreenDisplayTimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._SplashScreenTimer.Dispose();
            this._SplashScreenTimer = null;
            this._bSplashScreenClosed = true;
        }

        private void MainFormLoad()
        {
            this.OnCreateMainForm();

            while (!(this._bSplashScreenClosed))
            {
                Application.DoEvents();
            }

            DisposeDelegate SplashScreenFormDisposeDelegate = new DisposeDelegate(this._SplashScreenForm.Dispose);
            this._SplashScreenForm.Invoke(SplashScreenFormDisposeDelegate);
            this._SplashScreenForm = null;


            //必须先显示，再激活，否则主窗体不能在启动窗体消失后出现
            this._PrimaryForm.Show();
            this._PrimaryForm.Activate();

            this._PrimaryForm.Closed += new EventHandler(_PrimaryForm_Closed);

        }

        private void _PrimaryForm_Closed(object sender, EventArgs e)
        {
            base.ExitThread();
        }
    }
    #endregion
}