using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLayer;
using ILogger;
using DataAccessLayer;
using DataAccessLayer;
using System.IO;
namespace ClientTests
{

    
    //Before performing the following tests, please be sure that the system is connected to the server and the users and messages files are deleted from testProject. 
    [TestClass()]
    public class ChatRoom_Tests
    {
        //assistance attributes for this test class
        private ChatRoom chatroom;
        private User user;
        private string url;
        private Logger log;

        [TestInitialize()]
        public void setUp()
        {
            deleteFile();// perform the precondition automatically and delete users file
            log = Logger.Instance;
            this.chatroom = new ChatRoom(log);
            url=chatroom.getUrl();
        }
        //The following test verifies the constructor of Chatroom class fields initialized correctly
        [TestMethod()]
        public void TestChatroomConstruction()
        {
            //logger field
            Assert.IsNotNull(chatroom.GetLogger());
            //at first when registration were not performed expected that user is not loogged in
            user = chatroom.getLoggedInUser();
            Assert.IsNull(user);
            //handlers fields test- verify their fields are initialized
            Assert.IsNotNull(chatroom.GetMessageHandler());
            Assert.IsNotNull(chatroom.GetUserHandler());

            //here we verifies the types of the assigned fields
            MessageHandler mh = new MessageHandler(log);
            UserHandler uh = new UserHandler();
            Assert.ReferenceEquals(chatroom.GetMessageHandler(), mh);
            Assert.ReferenceEquals(chatroom.GetUserHandler(), uh);
        }

 
        //Precondition there is no users file in: ISE172_project\Client\ProjectTest\bin\Debug\Files ot this file is empty.
        [TestMethod()]
        public void TestRegistrationMethod()
        {

            //the following is performs a positive tests.
            //more negative tests should be in the tests of the GUI (in check validity func.). 
            bool isRegistered;
            /*

            //checks the functionality of registration with valid values of nickname and group id.
            string functionalCheck = "q";
            isRegistered = chatroom.registration(functionalCheck, "10");
            //Assert.IsTrue(isRegistered);

            //check a registration with a nickname that is already in use
            string alreadyUsed = functionalCheck;
            isRegistered = chatroom.registration(alreadyUsed, "10");
            Assert.IsFalse(isRegistered);

            //negative tests:
            //checks registration with an exist username which is not registered to the inserted group id 
            isRegistered = chatroom.registration(alreadyUsed, "1");
            Assert.IsTrue(isRegistered);

            //checks the edge values of g_id registration:
            isRegistered = chatroom.registration("test1", "40");
            Assert.IsTrue(isRegistered);
            isRegistered = chatroom.registration("test2", "1");
            Assert.IsTrue(isRegistered);
        */
        }

        //the negative tests should be givven in the GUI func that check the validity of the input 
        //PreCondition: The registration function is working well and users file is empty or not exist.
        [TestMethod()]
        public void login_and_logout_Tests()
        {
            /*
            //positive tests:
            //the following test the functionality of the login case
            string nick = "nick";
            bool isLoggedIn;
            chatroom.registration(nick, "2");//first registration with this user details
            //login with a registered user
            isLoggedIn = chatroom.login(nick, "2");
            Assert.IsTrue(isLoggedIn);

            //verifies the correct nickname has been saved
            StringAssert.Equals(this.chatroom.getLoggedInUser(), "nick");

            //negative tests:
            //checks  how the software deals with not registered user input:
            isLoggedIn = chatroom.login("notRegistered", "2");
            Assert.IsFalse(isLoggedIn);

            //checks logging out of a logged in user
            this.user = chatroom.getLoggedInUser();
            chatroom.logout();
            this.user = chatroom.getLoggedInUser();
            Assert.IsNull(user);
            */
        }

        //in the following we simulate a case when the system couldn't connect to the server while trying to retrieve.
        [TestMethod()]
        public void retrieveMessagesFailingTest()
        {/*
            chatroom.setUrl("http://notexistaddress.bgu.ac.il");
            //the following checks that in case of disconnection with the server false flag which signs connection with a server is sent
            Assert.IsFalse(chatroom.retrieveMessages());
            */
        }

        //Precondition: The application is connected to a server or a machine which simulates the server operations.
        [TestMethod()]
        public void retrieveMessagesTest()
        {/*
            //the following is a positive test. it checks the ability to get new Imessages from the server after a successful connecting session to the server
            Assert.IsTrue(chatroom.retrieveMessages());
            */
        }

        //precondition: communication layer works fine and the registration and login functionalities works well,and there is no user registered with name tester in group #3
        [TestMethod()]
        public void sendMessageTest()
        {/*
            chatroom.registration("tester", "3");
            chatroom.login("tester", "3");
            //the following tests the functionality of send a message operation from the chatroom.

            Message m = chatroom.getLoggedInUser().send("this is a send message test from chatroom", url);
            Guid guid = Guid.NewGuid();

            DateTime utc = new DateTime();
            IMessage IMsg = new IMessage(guid, "4", "tester2", "message",utc);
            Message l = new Message(IMsg);
            Assert.ReferenceEquals(m, l);//verifies the received is a message whih means the message "m" sent successfully to the server 
            StringAssert.Equals(m.body, "this is a send message test from chatroom"); // this verifies that the message wrapped well with the correct body as it sent by the user.
            */

        }
        [TestMethod()]
        public void sendMessageFailedTest()
        {/*
            chatroom.registration("tester", "3");
            chatroom.login("tester", "3");
            //the following tests the functionality of send a message operation from the chatroom.
            string res = chatroom.send("ssבדיקה");
            Assert.AreNotEqual("Your message was successfuly delivered", res);
        */}
        /// <summary>
        /// the following get a path of a file to delete and delete it if exist
        /// </summary>
        public void deleteFile()
        {
            if (File.Exists(@"Files\users.bin"))
            {
                File.Delete(@"Files\users.bin");
            }
        }
    }


}
