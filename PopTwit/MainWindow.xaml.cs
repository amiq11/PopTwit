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
using Tweetinvi;


namespace PopTwit
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotKey hotkey;
        private TweetController controller;
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            InitializeBinding();

            controller = new TweetController();
            controller.ProcessStream();
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
            string text = TweetBox.Text;
            
            if (controller.Update(text))
            {
                TweetBox.Clear();
                Hide();
            }
            else
            {
                Console.WriteLine("Something Wrong!");
            }
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
                this.TweetBox.Clear();
                this.TweetBox.Focus();
            }
        }
    }
}
