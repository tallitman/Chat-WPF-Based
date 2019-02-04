using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace BusinessLayer
{
    class MessageConvertor
    {

        /// <summary>
        /// Convert IMessage message to an Message object.
        /// </summary>
        /// <param name="imsg"></param>
        /// <returns>Message object</returns>
        public Message convertToMessage(IMessage imsg)
        {
            return Message.factory(imsg);
        }
        /// <summary>
        /// Convert Message message to an IMessage object.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>IMessage object</returns>
        public IMessage convertToIMessage(Message msg)
        {
            if (msg == null)
                return null;
            Guid GUID = msg.GUID;
            DateTime date = msg.date;
            string nickname = msg.nickname;
            string g_Id = msg.g_ID;
            string body = msg.body;
            IMessage iMsg = new IMessage(GUID, g_Id, nickname, body, date);
            return iMsg;
        }
        /// <summary>
        /// Convert IMessage message to an Message object.
        /// </summary>
        /// <param name="imsg"></param>
        /// <param name="charLimitPerMsg"></param>
        /// <returns>ReadOnlyMessage object</returns>
        public ReadOnlyMessage convertToReadOnly(Message msg)
        {
            if (msg == null)
                return null;
            Guid GUID = msg.GUID;
            DateTime date = msg.date.ToLocalTime();        
            string nickname = msg.nickname;         
            string g_Id = msg.g_ID;
            string body = msg.body;
            ReadOnlyMessage roMsg = new ReadOnlyMessage(GUID, date, nickname, g_Id, body);
            return roMsg;
        }
        /// <summary>
        /// this method receive a readOnlyMessage type message and returns it as a Message type
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Message convertFromReadOnlyToMessage(ReadOnlyMessage msg)
        {
          string body = msg.body;
            string nickname = msg.nickname;
            Guid GUID = msg.GUID;
            DateTime date = msg.date;
            string g_Id = msg.g_ID;
            Message message = new Message(body, nickname, GUID, date, g_Id);
            return message;

        }
    }


}
