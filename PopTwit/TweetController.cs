using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

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

        public ILoggedUser me = null;

        public string MyScreenName
        {
            get
            {
                if (me == null) return "";
                else return me.ScreenName;
            }
        }

        public IList<ITweet> ReplyList { get; protected set; }

        private Tweetinvi.Core.Interfaces.oAuth.IOAuthCredentials credential;
        private TweetAuthWindow taw;
        public TweetController()
        {
            // リプライ一覧の作成
            ReplyList = new List<ITweet>();

            // APIキーが登録されていなかったらAuthの画面を開く
            if (AccessToken.Length == 0 || AccessSecret.Length == 0)
            {
                taw = new TweetAuthWindow(this);
                taw.Show();
            }
            else
            {
                RenewCredential();
                me = User.GetLoggedUser();
                if (me != null)
                {
                    var tl = me.GetMentionsTimeline();
                    foreach (var t in tl)
                    {
                        if (t.InReplyToUserId == me.UserIdentifier.Id)
                        {
                            ReplyList.Insert(0, t);
                        }
                    }
                }
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

            me = User.GetLoggedUser();
            if (me != null)
            {
                var tl = me.GetMentionsTimeline();
                foreach (var t in tl)
                {
                    if (t.InReplyToUserId == me.UserIdentifier.Id)
                    {
                        ReplyList.Insert(0, t);
                    }
                }
            }
        }

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
                        if (idStrs.Contains("@"+MyScreenName))
                        {
                            Console.WriteLine(MyScreenName + "!!");
                            NotifyIconWrapper notify = (NotifyIconWrapper)Application.Current.Properties["notifyIcon"];
                            notify.ShowPopup(t.Creator.Name + " [" + t.Creator.ScreenName + "]: " + t.Text);
                            ReplyList.Add(t);
                        }
                    }
                };

                stream.TweetCreatedByMe += (sender, e) =>
                {
                    var t = e.Tweet;
                    Console.WriteLine("tweeted by: " + t.Creator.ScreenName + ", reply to: " + t.InReplyToScreenName + ", status: " + t.Text);
                    NotifyIconWrapper nofity = (NotifyIconWrapper)Application.Current.Properties["notifyIcon"];
                    nofity.ShowPopup(t.Creator.Name + " [" + t.Creator.ScreenName + "]: " + t.Text);
                };
                stream.StartStreamAsync();
            //});
        }


        public ITweet Update(string tweet, ITweet replyTo = null)
        {
            ITweet t = Tweet.CreateTweet(tweet);
            TwitterCredentials.ExecuteOperationWithCredentials(credential, () =>
            {
                if (replyTo != null)
                {
                    Tweet.PublishTweetInReplyTo(t, replyTo);
                }
                else
                {
                    Tweet.PublishTweet(t);
                }
            });
            return t;
        }
    }
}
