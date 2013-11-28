

using System.Collections.Generic;

using java.io;
using java.util;
using twitter4j;

namespace TameIrofText
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var file = new File("twitter4j.properties");
            var prop = new Properties();
            InputStream inputStream = null;
            OutputStream outputStream = null;
            try
            {
                if (file.exists())
                {
                    inputStream = new FileInputStream(file);
                    prop.load(inputStream);
                }
                if (args.Length < 2)
                {
                    if (null == prop.getProperty("oauth.consumerKey")
                        && null == prop.getProperty("oauth.consumerSecret"))
                    {
                        // consumer key/secret are not set in twitter4j.properties

                        System.Console.WriteLine(
                            "Usage: java twitter4j.examples.oauth.GetAccessToken [consumer key] [consumer secret]");
                        System.Environment.Exit(-1);
                    }
                }
                else
                {
                    prop.setProperty("oauth.consumerKey", args[0]);
                    prop.setProperty("oauth.consumerSecret", args[1]);
                    outputStream = new FileOutputStream("twitter4j.properties");
                    prop.store(outputStream, "twitter4j.properties");
                }
            }
            catch (IOException ioe)
            {
                ioe.printStackTrace();
                System.Environment.Exit(-1);
            }
            finally
            {
                if (inputStream != null)
                {
                    try
                    {
                        inputStream.close();
                    }
                    catch (IOException)
                    {
                    }
                }
                if (outputStream != null)
                {
                    try
                    {
                        outputStream.close();
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            try
            {

                // gets Twitter instance with default credentials
                var twitter = new TwitterFactory().getInstance();
                var user = twitter.verifyCredentials();
                var statuses = twitter.getHomeTimeline();
                System.Console.WriteLine("Showing @" + user.getScreenName() + "'s home timeline.");
                foreach (var status in statuses.ToEnumerable<Status>())
                {
                    System.Console.WriteLine("@" + status.getUser().getScreenName() + " - " + status.getText());
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
