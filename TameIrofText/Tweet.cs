using System.Collections.Generic;
using System.Runtime.Serialization;
using com.sun.tools.javac.util;

namespace TameIrofText
{
    [DataContract]
    public class TweetCollection
    {
        [DataMember]
        private List<Tweet> _tweetList;


        public TweetCollection(IEnumerable<Tweet> tweets)
        {
            _tweetList = new List<Tweet>(tweets);
        }

        public IEnumerable<Tweet> Tweets
        {
            get { return _tweetList.AsReadOnly(); }
        } 
    }

    [DataContract]
    public class Tweet
    {
        protected bool Equals(Tweet other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public Tweet(string user, string text, long id)
        {
            Text = text;
            User = user;
            Id = id;
        }

        [DataMember]
        public string User { get; private set; }
        [DataMember]
        public string Text { get; private set; }
        [DataMember]
        public long Id { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tweet) obj);
        }
    }
}