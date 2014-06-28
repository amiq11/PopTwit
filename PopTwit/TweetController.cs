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
    delegate void ReplyEventHandler(Tweetinvi.Core.Interfaces.ITweet tweet);

    class TweetController
    {
        private string consumerKey = "Ul0HawBthgiGnd7ar1qtJfIRX";
        private string consumerSecret = "JzSYSA7wvYGL3qsJqpNcSFDgIuxgW8NFH3w8Bq71enVzbJlxaD";
        private string accessToken = "196993405-2KVq1DrwBjzJl4o4zpHsePRkxnWru1rh3vXH9QPV";
        private string accessSecret = "hem7LrhhE9LK569vNVcaIApDko1Fk5CIDoUlxl9eiIPe3";
        private Tweetinvi.Core.Interfaces.oAuth.IOAuthCredentials credential;
        public TweetController()
        {
            credential = TwitterCredentials.CreateCredentials(accessToken, accessSecret, consumerKey, consumerSecret);
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
