using System;
using System.Collections.Generic;
using System.Linq;
using java.util;
using org.atilika.kuromoji;
using twitter4j;

using CliConsole = System.Console;

namespace TameIrofText
{
    internal class Program
    {
        private static readonly TweetDataStore _store = new TweetDataStore();

        private static void Main(string[] args)
        {
            // Get tokenizer
            var tokenizer = Tokenizer.builder().build();
            
            var timeLine = GetTimeline();

            var wordsFromAllTweets = new Dictionary<long, List<string>>();
            var wordsFromAllIrof = new List<string>();

            foreach (var tweet in timeLine.Distinct())
            {
                var tokens = ExtractNoun(tokenizer.tokenize(tweet.Text)).ToArray();
                var nouns = tokens.Select(token => token.getSurfaceForm()).ToList();
                if (!wordsFromAllTweets.ContainsKey(tweet.Id))
                {
                    wordsFromAllTweets.Add(tweet.Id,nouns);
                }

                if (tweet.User == "irof")
                {
                    wordsFromAllIrof.AddRange(tokens.Select(token => token.getSurfaceForm()));
                }
            }

            var irofRank = new Dictionary<string, double>();
            var n = wordsFromAllTweets.Count;
            foreach (var wordFromIrof in wordsFromAllIrof.Distinct())
            {
                var rank = CalculateRank(wordsFromAllIrof, wordFromIrof, wordsFromAllTweets, n);

                irofRank.Add(wordFromIrof,rank);
            }

            foreach (var rank in irofRank.OrderByDescending(pair => pair.Value))
            {
                CliConsole.WriteLine(rank.Key + ": " + rank.Value);
            }

            CliConsole.ReadKey();
        }

        private static double CalculateRank(List<string> wordsFromAllIrof, string wordFromIrof, Dictionary<long, List<string>> wordsFromAllTweets, int n)
        {
            var tf = wordsFromAllIrof.Count(s => s == wordFromIrof);
            var df = wordsFromAllTweets.Count(pair => pair.Value.Contains(wordFromIrof));
            var rank = tf*Math.Log(n/(double) df);
            return rank;
        }

        private static IEnumerable<Token> ExtractNoun(List rawTokens)
        {
            return rawTokens.ToEnumerable<Token>()
                .Where(token => token.getAllFeaturesArray()[0] == "名詞");
        }

        private static List<Tweet> GetTimeline()
        {
            List<Tweet> statusStore = null;

            try
            {
                // gets Twitter instance with default credentials
                var twitter = new TwitterFactory().getInstance();
                var user = twitter.verifyCredentials();

                try
                {
                    statusStore = _store.Restore("data").ToList();
                }
                catch
                {
                    statusStore = twitter.getHomeTimeline(new Paging(1, 200))
                        .ToEnumerable<Status>().Where(status => !status.isRetweet())
                        .Select(status =>
                            new Tweet(
                                status.getUser().getScreenName(),
                                status.getText(),
                                status.getId())).ToList();
                }


                while (true)
                {
                    var lastId = statusStore.Last().Id;

                    var paging = new Paging(1, 200, 1, lastId + 1);
                    var tl = twitter.getHomeTimeline(paging).ToEnumerable<Status>();
                    var tweetData = tl.Where(status => !status.isRetweet())
                        .Select(status =>
                            new Tweet(
                                status.getUser().getScreenName(),
                                status.getText(),
                                status.getId())).ToArray();

                    if (!tweetData.Any()) { break;}

                    statusStore.AddRange(tweetData);
                }
            }
            catch (TwitterException te)
            {
                te.printStackTrace();
                CliConsole.WriteLine("Failed to get timeline: " + te.getMessage());
            }
            finally
            {
                _store.Save(statusStore, "data");
            }

            return statusStore;
        }
    }

    public static class JavaListExtention
    {
        public static System.Collections.Generic.IEnumerable<T> 
            ToEnumerable<T>(this java.util.List jList)
        {
            var iterator = jList.iterator();

            while (iterator.hasNext())
            {
                yield return (T) iterator.next();
            }
        }
    }
}
