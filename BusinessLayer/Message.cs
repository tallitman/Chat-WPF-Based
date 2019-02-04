using System;
using DataAccessLayer;

namespace BusinessLayer
{
    [Serializable]
    public class Message : IEquatable<Message>
    {
        #region properties       
        private string _body;            //Message content
        public string body
        {
            get { return _body; }
        }
        private string _nickname;        //The user who sent the message
        public string nickname
        {
            get { return _nickname; }
        }
        private string _g_Id;
        public string g_ID
        {
            get { return _g_Id; }
        }
        private Guid _GUID;               //Message Guid
        public Guid GUID
        {
            get { return _GUID; }
        }
        private DateTime _date;           //Message's date
        public DateTime date
        {
            get { return _date; }
        }
        #endregion

        /// <summary>
        /// creates a message from an existing IMassage retrieved from the server
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Message factory(IMessage msg)
        {
            return new Message(msg);
        }
        /// <summary>
        /// Checks validity of the input , if its fine so make an message object by the constructor.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="g_id"></param>
        /// <param name="nickname"></param>
        /// <param name="charLimitPerMsg"></param>
        /// <returns></returns>
        public static Message factory(string msg, string url, string g_id, string nickname, int charLimitPerMsg, DateTime utc)
        {
            if (checkValidity(msg, charLimitPerMsg))
            {
                Guid guid;
                guid = Guid.NewGuid();
                IMessage IMsg = new IMessage (guid, g_id, nickname, msg, utc);
                Message m = new Message(IMsg);
                return m;
            }
            return null;
        }
        /// <summary>
        /// constructor of message class
        /// </summary>
        /// <param name="message"></param>
        private Message(IMessage message)
        {
            _body = message._MessageContent;
            _nickname = message._UserName;
            _GUID = message._Id;
            _date = message._Date;
            _g_Id = message._GroupID;
        }

        public Message(string body ,string nick,Guid GUID,DateTime date,string g_id)
        {
            this._body = body;
            this._nickname = nick;
            this._GUID = GUID;
            this._date = date;
            this._g_Id = g_id;
        }

        /// <summary>
        /// characters check - if there's an illegal character or msg is too long, print an error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="charLimitPerMsg"></param>
        /// <returns></returns>
        private static bool checkValidity(string message, int charLimitPerMsg)
        {
            if (charLimitPerMsg != 0 && message.Length > charLimitPerMsg)
            {
                return false;
            }
            return true;
        }

        public void updateBody(string updatedBody)
        {
            this._body = updatedBody;
        }
        public void updateDate(DateTime newDateTime)
        {
            this._date = newDateTime;
        }


        /// <summary>
        /// prints message details.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ans = String.Format("[{1} - {0} ({3})] : {2}", nickname, date.ToString(), body, g_ID);
            return ans;   
        }
        /// <summary>
        /// equals method implements by GUID
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Message other)
        {
            if (other == null) return false;
            return _GUID == other.GUID;

        }

    }  
}
