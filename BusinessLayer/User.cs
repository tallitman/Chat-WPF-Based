using System;

namespace BusinessLayer
{
    [Serializable]
    
    public class User : IEquatable<User>          //User class responsible to interacts with the server
    {
        #region properties   
        private int _charLimitPerMessage;       //this holds the limit configured for a certain user
        public int charLimitPerMessage
        {
            get { return _charLimitPerMessage; }
        }
        private string _nickName;
        public string nickName
        {
            get
            {
                return _nickName;
            }
        }
        private string _g_id;
        public string g_id
        {
            get
            {
                return _g_id;
            }
        }
        private int _numOfMessages;             //amount of messages that sent from the certain user.
        public int numOfMessages
        {
            get
            {
                return _numOfMessages;
            }
        }
        #endregion
        /// <summary>
        /// constructs a new user
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="g_id"></param>
        /// <param name="charLimitPerMessage"></param>
        public User(string nickName, string g_id, int charLimitPerMessage)
        {
            _nickName = nickName;
            _g_id = g_id;
            _charLimitPerMessage = charLimitPerMessage;
        }
        /// <summary>
        /// send method recieves a message string and uses factory method in Message class, in order to create a Message which includes this string
        /// </summary>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <returns>Message object wrapped with obtained string</returns>
        public Message send(string message, string url, DateTime localTime)
        {
            return Message.factory(message, url, _g_id, _nickName, _charLimitPerMessage,localTime.ToUniversalTime());
        }

        /// <summary>
        /// checks if a given User object equals to current User
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(User other)
        {
            if (other == null) return false;
            string tGID = other.g_id;
            string tNickname = other.nickName;
            return tGID == g_id && tNickname == nickName;
        }
    }
}
