using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public TweetController controller { get; private set; }
        public static readonly ICommand TweetCmd = new TweetCommand();
        public static readonly ICommand NextReplyCmd = new NextReplyCommand();
        public static readonly ICommand PrevReplyCmd = new PrevReplyCommand();
        public static readonly ICommand ClearTweetCmd = new ClearTweetCommand();

        private ITweet replyTarget = null;
        private int replyTargetPos = -1;

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            InitializeBinding();
            controller = new TweetController();
            controller.ProcessStream();
            ((TweetCommand)TweetCmd).RegisterWindow(this);
            ((NextReplyCommand)NextReplyCmd).RegisterWindow(this);
            ((PrevReplyCommand)PrevReplyCmd).RegisterWindow(this);
            ((ClearTweetCommand)ClearTweetCmd).RegisterWindow(this);
            
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
            ITweet t = controller.Update(text, replyTarget);
            if (t.IsTweetPublished)
            {
                ResetTweet();
                Hide();
                //if (notify != null) notify.ShowPopup(t.Creator.ScreenName + "[" + t.Creator.Name + "]: " + text);
            }
            else
            {
                Console.WriteLine("Something Wrong!");
                if (notify != null) notify.ShowPopup("Tweet Failed..");

            }
        }

        private void SetReply()
        {
            if (replyTarget == null)
            {
                ResetTweet();
            }
            else
            {
                // 下のリプライ画面
                ReplyBlock.Text = replyTarget.Creator.Name + " [" + replyTarget.Creator.ScreenName + "]: " + replyTarget.Text;

                // ツイートのテキストボックス
                Regex replyToRegex = new Regex("^(@[a-z0-9_-]+ )+", RegexOptions.IgnoreCase);
                Match m = replyToRegex.Match(replyTarget.Text);
                if (m.Success)
                {
                    string replyText = m.Value;
                    TweetBox.Text  = "@" + replyTarget.Creator.ScreenName + " ";
                    TweetBox.Text += replyText.Replace("@"+controller.MyScreenName+" ", "");
                    TweetBox.SelectionStart = TweetBox.Text.Length;
                }
            }
        }

        private void ResetTweet()
        {
            replyTarget = null;
            replyTargetPos = -1;
            ReplyBlock.Text = (string)Resources["UsageString"];
            TweetBox.Text = "";
            TweetBox.SelectionStart = 0;
        }

        private void SelectNextReply()
        {
            var replys = controller.ReplyList;
            if (replyTargetPos == 0 || replys.Count == 0)
            {
                replyTarget = null;
                replyTargetPos = -1;
            }
            else {
                if (replyTargetPos < 0)
                    replyTargetPos = replys.Count - 1;
                else
                    replyTargetPos--;
                replyTarget = replys[replyTargetPos];
            }
        }

        private void SelectPrevReply()
        {
            var replys = controller.ReplyList;
            if (replyTargetPos == replys.Count-1  || replys.Count == 0)
            {
                replyTarget = null;
                replyTargetPos = -1;
            }
            else
            {
                replyTargetPos++;
                replyTarget = replys[replyTargetPos];
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
        public class NextReplyCommand : ICommand
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
                window.SelectNextReply();
                window.SetReply();
            }
        }
        public class PrevReplyCommand : ICommand
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
                window.SelectPrevReply();
                window.SetReply();
            }
        }
        public class ClearTweetCommand : ICommand
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
                window.ResetTweet();
            }
        }
                
    }
}
