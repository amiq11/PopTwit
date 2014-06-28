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
using Tweetinvi;

namespace PopTwit
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string consumerKey    = "Ul0HawBthgiGnd7ar1qtJfIRX";
        private string consumerSecret = "JzSYSA7wvYGL3qsJqpNcSFDgIuxgW8NFH3w8Bq71enVzbJlxaD";
        private string accessToken    = "196993405-2KVq1DrwBjzJl4o4zpHsePRkxnWru1rh3vXH9QPV";
        private string accessSecret   = "hem7LrhhE9LK569vNVcaIApDko1Fk5CIDoUlxl9eiIPe3";

        private HotKey hotkey;

        public MainWindow()
        {
            InitializeComponent();
            TwitterCredentials.SetCredentials(accessToken, accessSecret, consumerKey, consumerSecret);
            this.Hide();
            InitializeBinding();
        }

        private void InitializeBinding()
        {
            this.hotkey = new HotKey(Key.ImeConvert, KeyModifier.None, OnHotKeyHandler);
        }

        private void clickedTweetButton(object sender, RoutedEventArgs e)
        {
            string text = TweetBox.Text;
            Tweetinvi.Core.Interfaces.ITweet tweet = Tweet.PublishTweet(text);
            if (tweet.IsTweetPublished)
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
