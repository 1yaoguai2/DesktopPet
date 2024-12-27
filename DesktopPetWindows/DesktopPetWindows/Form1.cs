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
        //切换定时器
        private Timer _cutTimer;


        private int frameCount = 0;

        private Image[] frames =
        {
            Player1.SNinja_swordidle_1,
            Player1.SNinja_swordidle_2,
            Player1.SNinja_swordidle_3
        };


        private List<Image[]> allFrames = new List<Image[]>();


        public Form1()
        {
            InitializeComponent();
            this.TransparencyKey = BackColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            contextMenuStrip1.ItemClicked += 退出ToolStripMenuIteam_Click;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = 100;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.BackgroundImage = frames[frameCount];
            this.BackgroundImageLayout = ImageLayout.Center;
            this.TopMost = true;

            frameCount = ++frameCount % frames.Length;
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


        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            //_timer.Stop();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            //_timer.Start();
        }

        private void 退出ToolStripMenuIteam_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
         ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue("BaichuiMonitor", Application.ExecutablePath);//"BaichuiMonitor"可以自定义
        }
    }
}