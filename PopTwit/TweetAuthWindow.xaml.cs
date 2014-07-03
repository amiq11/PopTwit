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
using System.Windows.Shapes;
using OAuth;
using PopTwit;
namespace PopTwit
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetAuthWindow : Window
    {
        static private readonly string RequestTokenUrl = "https://api.twitter.com/oauth/request_token";
        static private readonly string AccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        static private readonly string AuthorizeUrlStub = "https://api.twitter.com/oauth/authorize?oauth_token=";
        TweetController tc;
        OAuth.Manager oauth;
        public TweetAuthWindow(TweetController tc)
        {
            InitializeComponent();
            this.tc = tc;
            oauth = new OAuth.Manager();
            oauth["consumer_key"] = TweetController.ConsumerKey;
            oauth["consumer_secret"] = TweetController.ConsumerSecret;
            oauth.AcquireRequestToken(RequestTokenUrl, "POST");
            var url = AuthorizeUrlStub + oauth["token"];
            Console.WriteLine(url);
            authBrowser.Navigate(url);
        }

        private void PinCommitButtonClicked(object sender, RoutedEventArgs e)
        {
            oauth.AcquireAccessToken(AccessTokenUrl, "POST", PinTextBox.Text);
            TweetController.AccessToken = oauth["token"];
            TweetController.AccessSecret = oauth["token_secret"];
            Console.WriteLine("Token: " + TweetController.AccessToken);
            Console.WriteLine("Secret: " + TweetController.AccessSecret);
            tc.RenewCredential();
            Hide();
        }
    }
}