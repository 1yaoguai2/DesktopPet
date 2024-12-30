using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopPetWindows
{
    public partial class Form1 : Form
    {
        //播放定时器
        private Timer _timer;
        private int _timerInterval = 100; //ms

        //切换定时器
        private Timer _cutTimer;
        private int _cutTimerInterval = 5000; //ms

        private Image[] frames1 =
        {
            Player1.SNinja_swordidle_1,
            Player1.SNinja_swordidle_2,
            Player1.SNinja_swordidle_3
        };

        private Image[] frames2 =
        {
            Run1.SNinja_run_03,
            Run1.SNinja_run_04,
            Run1.SNinja_run_05,
            Run1.SNinja_run_06,
            Run1.SNinja_run_07,
            Run1.SNinja_run_08,
            Run1.SNinja_run_09,
            Run1.SNinja_run_10,
            Run1.SNinja_run_11

        };

        private Image[] frames3 =
        {
            Attack1.SNinja_Atk_1,
            Attack1.SNinja_Atk_2,
            Attack1.SNinja_Atk_3,
            Attack1.SNinja_Atk_4,
            Attack1.SNinja_Atk_5,
            Attack1.SNinja_Atk_6
        };

        private List<Image[]> allFrames = new List<Image[]>();
        private int framesCount;

        private Image[] currentFrames;
        private int currentFrameCount = 0;

        public Form1()
        {
            InitializeComponent();
            this.TransparencyKey = BackColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.ShowInTaskbar = false;

            allFrames.Add(frames1);
            allFrames.Add(frames2);
            allFrames.Add(frames3);

            CreateMenuStrips();

            InitializeTimer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Tick += Timer_Tick;
            }
            else _timer.Stop();
            if (currentFrames == null)
            {
                framesCount = 0;
                currentFrames = allFrames[framesCount];
            }
            currentFrameCount = 0;
            _timer.Interval = _timerInterval;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.BackgroundImage = currentFrames[currentFrameCount];
            this.BackgroundImageLayout = ImageLayout.Center;
            this.TopMost = true;

            currentFrameCount = ++currentFrameCount % currentFrames.Length;
        }

        private void CreateMenuStrips()
        {
            ToolStripMenuItem cutMenuItem = new ToolStripMenuItem();
            cutMenuItem.Text = "切换";
            cutMenuItem.Click += MenuIteamCutFrame_Click;
            contextMenuStrip1.Items.Add(cutMenuItem);

            ToolStripMenuItem settingMenuItem = new ToolStripMenuItem();
            settingMenuItem.Text = "设置";
            contextMenuStrip1.Items.Add(settingMenuItem);

            ToolStripMenuItem playSpeedAdd = new ToolStripMenuItem();
            playSpeedAdd.Text = "加快动画速度";
            playSpeedAdd.Click += MenuIteamAddSpeed_Click;
            InitAutoRunStatu(playSpeedAdd);
            settingMenuItem.DropDownItems.Add(playSpeedAdd);

            ToolStripMenuItem playSpeedSubtruct = new ToolStripMenuItem();
            playSpeedSubtruct.Text = "减慢动画速度";
            playSpeedSubtruct.Click += MenuIteamSubtractSpeed_Click;
            InitAutoRunStatu(playSpeedSubtruct);
            settingMenuItem.DropDownItems.Add(playSpeedSubtruct);

            ToolStripMenuItem openMenuItem = new ToolStripMenuItem();
            openMenuItem.Text = "开机自启";
            openMenuItem.Click += MenuItem开机自启_Click;
            InitAutoRunStatu(openMenuItem);
            settingMenuItem.DropDownItems.Add(openMenuItem);

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
            exitMenuItem.Text = "退出";
            exitMenuItem.Click += MenuIteam退出_Click;
            contextMenuStrip1.Items.Add(exitMenuItem);

        }


        #region 鼠标拖动实现

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int IParam);

        //定义常量
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HT_CAPTION = 0x0002;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                //
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        #region 菜单按钮事件
        private void MenuIteamCutFrame_Click(object sender, EventArgs e)
        {
            framesCount++;
            framesCount = framesCount == allFrames.Count ? 0 : framesCount;
            currentFrames = allFrames[framesCount];
            InitializeTimer();
        }
        private void MenuIteamAddSpeed_Click(object sender, EventArgs e)
        {
            _timerInterval -= _timerInterval > 50 ? 50 : 0;
            InitializeTimer();
        }
        private void MenuIteamSubtractSpeed_Click(object sender, EventArgs e)
        {
            _timerInterval += _timerInterval < 300 ? 50 : 0;
            InitializeTimer();
        }
        private void MenuItem开机自启_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            if (item.Checked)
            {
                AddAutoRun();
            }
            else
            {
                RemoveAutoRun();
            }
        }
        private void MenuIteam退出_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 开机自启
        /// <summary>
        /// 开机自启状态判断
        /// </summary>
        /// <param name="menuItem"></param>
        private void InitAutoRunStatu(ToolStripMenuItem menuItem)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (registryKey != null)
            {
                // 检查是否存在名为 "BaichuiMonitor" 的键
                if (registryKey.GetValue("BaichuiMonitor") == null)
                    menuItem.Checked = false;
                else
                    menuItem.Checked = true;
                // 关闭注册表项
                registryKey.Close();
            }
        }

        /// <summary>
        /// 添加开机自启
        /// </summary>
        private void AddAutoRun()
        {
            //获取当前应用程序的路径
            string localPath = Application.ExecutablePath;
            if (!System.IO.File.Exists(localPath))//判断指定文件是否存在
                return;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (registryKey != null)
            {
                // 检查是否存在名为 "BaichuiMonitor" 的键
                if (registryKey.GetValue("BaichuiMonitor") == null)
                {
                    registryKey.SetValue("BaichuiMonitor", Application.ExecutablePath);//"BaichuiMonitor"可以自定义
                }
                // 关闭注册表项
                registryKey.Close();
            }
        }

        /// <summary>
        /// 移除开机自启
        /// </summary>
        private void RemoveAutoRun()
        {
            // 打开注册表项 "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey != null)
            {
                // 检查是否存在名为 "BaichuiMonitor" 的键
                if (registryKey.GetValue("BaichuiMonitor") != null)
                {
                    // 删除 "BaichuiMonitor" 键
                    registryKey.DeleteValue("BaichuiMonitor");
                }

                // 关闭注册表项
                registryKey.Close();
            }
        }
        #endregion
    }
}