using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using com.sun.org.apache.bcel.@internal.generic;

namespace TameIrofText
{
    public class TweetDataStore
    {
        private DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(TweetCollection));

        public void Save(IEnumerable<Tweet> tweet, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                _serializer.WriteObject(fs, new TweetCollection(tweet));
            }
        }

        public IEnumerable<Tweet> Restore(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var restore = _serializer.ReadObject(fs) as TweetCollection;
                return restore != null ? restore.Tweets : null;
            }
        }
    }
}
