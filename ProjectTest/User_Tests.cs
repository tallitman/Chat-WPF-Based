using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLayer;
using ILogger;



namespace ProjectTest
{
    [TestClass]
    public class User_Tests
    {
        private string url;
       
        [TestInitialize()]
        public void setUp()
        {
            Logger log = Logger.Instance;
            ChatRoom chatroom = new ChatRoom(log);
            url = chatroom.getUrl();
        }
        //PreCondition: communication Layer works correctly
        [TestMethod()]
        public void TestUserSend()
        {/*
            Message received;
            User testUser = new User("tester6", "6", 10);

            //checks the fuctionality of send method of user class and verifies the correct string were sent and saved in the body of Message type instance
            received = testUser.send("is valid", url);// a valid message content
            StringAssert.Equals("is valid", received.body);

            //non valid content for sent message
            received = testUser.send("is not valid", url);// a not valid message content
            Assert.IsNull(received);*/
        }
    }
    //the following is a stub for Message class we will use it when checking the send messages functionality, in case the Message class is not 100% working well
    //in that case we will change the signature of the method factory from Message.factory to MessageStub.factory inside the send method.
    class MessageStub
    {

        public Message factory(string msg, string url, string g_id, string nickname, int charLimitPerMsg)
        {
            /*
            if (msg.Equals("is valid") && msg.Length <= charLimitPerMsg)
            {
                IMessage IMsg = Communication.Instance.Send(url, "20", "tester6", msg);
                Message m = new Message(IMsg);
                return m;
            }
            */
            return null;

        }
    }
}