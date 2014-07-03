using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using Tweetinvi;

namespace PopTwit
{
    public delegate void ReplyEventHandler(Tweetinvi.Core.Interfaces.ITweet tweet);

    public class TweetController
    {
        static public string ConsumerKey { get { return "Ul0HawBthgiGnd7ar1qtJfIRX"; } }
        static public string ConsumerSecret { get { return "JzSYSA7wvYGL3qsJqpNcSFDgIuxgW8NFH3w8Bq71enVzbJlxaD"; } }
        static public string AccessToken { 
            get { return Properties.Settings.Default.AccessToken; }
            set { 
                Properties.Settings.Default.AccessToken = value;
                Properties.Settings.Default.Save();
            }
        }
        static public string AccessSecret
        {
            get { return Properties.Settings.Default.AccessSecret; }
            set
            {
                Properties.Settings.Default.AccessSecret = value;
                Properties.Settings.Default.Save();
            }
        }

        private Tweetinvi.Core.Interfaces.oAuth.IOAuthCredentials credential;
        private TweetAuthWindow taw;
        public TweetController()
        {
            if (AccessToken.Length == 0 || AccessSecret.Length == 0)
            {
                taw = new TweetAuthWindow(this);
                taw.Show();
            }
            else
            {
                Console.WriteLine("Token: " + TweetController.AccessToken);
                Console.WriteLine("Secret: " + TweetController.AccessSecret);
                credential = TwitterCredentials.CreateCredentials(AccessToken, AccessSecret, ConsumerKey, ConsumerSecret);
                TwitterCredentials.SetCredentials(credential);
            }
        }

        public void Reauthorize()
        {
            taw = new TweetAuthWindow(this);
            taw.Show();
        }

        public void RenewCredential() {
            credential = TwitterCredentials.CreateCredentials(AccessToken, AccessSecret, ConsumerKey, ConsumerSecret);
            TwitterCredentials.SetCredentials(credential);
        }

        public event ReplyEventHandler OnReply;

        public void ProcessStream()
        {
            //TwitterCredentials.ExecuteOperationWithCredentials(credential, () =>
            //{
                var stream = Stream.CreateUserStream();
                
                stream.TweetCreatedByFriend += (sender, e) =>
                {
                    var t = e.Tweet;
                    Console.WriteLine("tweeted by: " + t.Creator.ScreenName + ", reply to: " + t.InReplyToScreenName + ", status: " + t.Text);
                    Regex replyToRegex = new Regex("^(@[a-z0-9_-]+ )+", RegexOptions.IgnoreCase);
                    Match m = replyToRegex.Match(t.Text);
                    if (m.Success)
                    {
                        string[] idStrs = m.Value.Split(' ');
                        Console.WriteLine("Reply to: '" + m.Value + "'");
                        if (idStrs.Contains("@amiq11"))
                        {
                            Console.WriteLine("AMIQ!!");
                            NotifyIconWrapper notify = (NotifyIconWrapper)Application.Current.Properties["notifyIcon"];
                            notify.ShowPopup(t.Creator.Name + " [" + t.Creator.ScreenName + "]: " + t.Text);
                        }
                    }
                };
                stream.StartStreamAsync();
            //});
        }


        public bool Update(string tweet)
        {
            Tweetinvi.Core.Interfaces.ITweet t = Tweet.CreateTweet(tweet);
            TwitterCredentials.ExecuteOperationWithCredentials(credential, () =>
            {
                Tweet.PublishTweet(t);
            });
            return t.IsTweetPublished;
        }
    }
}
