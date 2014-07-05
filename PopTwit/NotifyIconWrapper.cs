using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopTwit
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            // イベントハンドラの設定
            toolStripMenuItemShow.Click += toolStripMenuItemShow_Click;
            toolStripMenuItemExit.Click += toolStripMenuItemExit_Click;
            toolStripMenuItemAuth.Click += toolStripMenuItemAuth_Click;

            if (win.controller.MyScreenName != "") {
                ShowPopup("Launched as " + win.controller.MyScreenName);
            }
            else
            {
                ShowPopup("Launched but cannot log in correctly");
            }
        }

        // 常駐させるウィンドウはここで保持する
        private MainWindow win = new MainWindow();

        void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        void toolStripMenuItemAuth_Click(object sender, EventArgs e)
        {
            win.Reauthorize();
        }        

        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            // ウィンドウ表示&最前面に持ってくる
            if (win.WindowState == System.Windows.WindowState.Minimized)
                win.WindowState = System.Windows.WindowState.Normal;

            win.Show();
            win.Activate();
            // タスクバーでの表示をする
            win.ShowInTaskbar = true;
        }

        public void ShowPopup(string text)
        {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.ShowBalloonTip(500, "Tweet", text, System.Windows.Forms.ToolTipIcon.None);
        }
        //public NotifyIconWrapper(IContainer container)
        //{
        //    container.Add(this);

        //    InitializeComponent();
        //}
    }
}
