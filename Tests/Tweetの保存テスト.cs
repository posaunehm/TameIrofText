using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TameIrofText;

namespace Tests
{
    [TestClass]
    public class Tweetの保存テスト
    {
        [TestMethod]
        public void ユーザPiyoとHogeのTweetを保存する()
        {
            var sut = new TweetDataStore();

            sut.Save(new[]
            {
                new Tweet("piyo", "piyopiyo",12345L),
                new Tweet("hoge", "fuga",6789L)
            }, "test");

            var result = sut.Restore("test").ToArray();

            result[0].User.Is("piyo");
            result[0].Text.Is("piyopiyo");
            result[0].Id.Is(12345L);
            result[1].User.Is("hoge");
            result[1].Text.Is("fuga");
            result[1].Id.Is(6789L);

        }
    }
}
