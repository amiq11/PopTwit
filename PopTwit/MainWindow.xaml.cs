using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
//using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace PopTwit
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotKey hotkey;
        private TweetController controller;
        public static readonly ICommand TweetCmd = new TweetCommand();

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            InitializeBinding();
            controller = new TweetController();
            controller.ProcessStream();
            ((TweetCommand)TweetCmd).RegisterWindow(this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // クローズ処理をキャンセルして、タスクバーの表示も消す
            e.Cancel = true;
            this.WindowState = System.Windows.WindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void InitializeBinding()
        {
            this.hotkey = new HotKey(Key.ImeConvert, KeyModifier.None, OnHotKeyHandler);
        }

        private void clickedTweetButton(object sender, RoutedEventArgs e)
        {
            Tweet();
        }

        private void Tweet()
        {
            string text = TweetBox.Text;
            NotifyIconWrapper notify = System.Windows.Application.Current.Properties["notifyIcon"] as NotifyIconWrapper;
            ITweet t = controller.Update(text);
            if (t.IsTweetPublished)
            {
                TweetBox.Clear();
                Hide();
                //if (notify != null) notify.ShowPopup(t.Creator.ScreenName + "[" + t.Creator.Name + "]: " + text);
            }
            else
            {
                Console.WriteLine("Something Wrong!");
                if (notify != null) notify.ShowPopup("Tweet Failed..");

            }
        }

        public void Reauthorize()
        {
            controller.Reauthorize();
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            if (this.IsActive)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.TweetBox.Focus();
            }
        }

        public class TweetCommand : ICommand
        {
            MainWindow window;
            public void RegisterWindow(MainWindow window)
            {
                this.window = window;
            }
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                window.Tweet();
            }
        }
    }
}
