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
        private static void Main(string[] args)
        {
            try
            {
                // Get tokenizer
                var tokenizer = Tokenizer.builder().build();

                // gets Twitter instance with default credentials
                var twitter = new TwitterFactory().getInstance();
                var user = twitter.verifyCredentials();


                CliConsole.WriteLine("Showing @" + user.getScreenName() + "'s home timeline.");
                var i = 1;
                while (true)
                {
                    var statuses = twitter.getHomeTimeline(new Paging(i));

                    foreach (var status in statuses.ToEnumerable<Status>())
                    {
                        var text = tokenizer.tokenize(status.getText()).ToEnumerable<Token>().Where(t =>t.getAllFeaturesArray()[0] == "名詞" && t.isKnown());

                        CliConsole.WriteLine("@" + status.getUser().getScreenName() + " - " );
                        foreach (var token in text)
                        {
                            CliConsole.WriteLine("\t" + token.getAllFeatures() + " : " + token.getSurfaceForm());
                        }
                    }

                    if (CliConsole.ReadKey().KeyChar == 'q')
                    {
                        break;
                    }

                    i++;
                }
            }
            catch (TwitterException te)
            {
                te.printStackTrace();
                CliConsole.WriteLine("Failed to get timeline: " + te.getMessage());
                System.Environment.Exit(-1);
            }
        }
    }


    public static class JavaListExtention
    {
        public static IEnumerable<T> ToEnumerable<T>(this List jList)
        {
            var iterator = jList.iterator();

            while (iterator.hasNext())
            {
                yield return (T) iterator.next();
            }
        }
    }
}
