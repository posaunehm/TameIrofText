using System.Collections.Generic;
using twitter4j;

namespace TameIrofText
{
    internal class Program
    {
        private static void Main(string[] args)
        {   
            try
            {
                // gets Twitter instance with default credentials
                var twitter = new TwitterFactory().getInstance();
                var user = twitter.verifyCredentials();

                
                System.Console.WriteLine("Showing @" + user.getScreenName() + "'s home timeline.");
                int i = 1;
                while (true)
                {
                    var statuses = twitter.getHomeTimeline(new Paging(i));

                    foreach (var status in statuses.ToEnumerable<Status>())
                    {
                        System.Console.WriteLine("@" + status.getUser().getScreenName() + " - " + status.getText());
                    }

                    if (System.Console.ReadKey().KeyChar == 'q')
                    {
                        break;
                    }

                    i++;
                }
            }
            catch (TwitterException te)
            {
                te.printStackTrace();
                System.Console.WriteLine("Failed to get timeline: " + te.getMessage());
                System.Environment.Exit(-1);
            }
        }
    }


    public static class JavaListExtention
    {
        public static IEnumerable<T> ToEnumerable<T>(this java.util.List jList)
        {
            var iterator = jList.iterator();

            while (iterator.hasNext())
            {
                yield return (T) iterator.next();
            }
        }
    }
}
