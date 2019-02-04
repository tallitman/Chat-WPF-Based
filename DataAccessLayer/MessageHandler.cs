using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ILogger;

namespace DataAccessLayer
{
    public class MessageHandler
    {
        private List<IQueryAction> _filters; //list of filters we add to queries
        private Logger _logger;
        private string connetion_string = null;
        private string server_address = "ise172.ise.bgu.ac.il,1433\\DB_LAB";
       //string server_address = "localhost\\SQLExpress;"; 
        private const string _lastDateQuery = "SELECT TOP(1) [SendTime] FROM[MS3].[dbo].[Messages] order by SendTime DESC";//predefined query order by sendtime
        private string database_name = "MS3";
        private string user_name = "publicUser";
        private string password = "isANerd";
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader data_reader;
       


        public MessageHandler(Logger logger)
        {
            _logger = logger;
            _filters = new List<IQueryAction>();//init filters list
                connetion_string = $"Data Source={server_address};Initial Catalog={database_name };User ID={user_name};Password={password}";
                connection = new SqlConnection(connetion_string);
        }
        //retrieve messages by date query
        public List<IMessage> retrieveMessages(DateTime lastDate)//for timer 
        {
            _logger.logInfoMessage("time " + server_address);
            Query query = createQuery();
            query.addDate(lastDate);
            query.makeQuery();
            _logger.logInfoMessage(query.queryString);
            //send this retrieve query tocommunicate to make the sql request 
            return communicate(query);
        }
        //retrieve messages with default query
        public List<IMessage> retrieveMessages()//for apply changes
        {
            _logger.logInfoMessage("ragil" + server_address);
            Query query = createQuery();
            query.makeQuery();

            return communicate(query);
        }
        //this method open connection and execute the request in front of the server DB
        public List<IMessage> communicate(Query query)
        {
            List<IMessage> messages = new List<IMessage>(); //this list will get the messages from the db
            try
            {
                _logger.logInfoMessage("trying to communicate with the server and make sql request");
                connection.Open();
        
                _logger.logInfoMessage("connected to: ----retreiveapply " + server_address);
                _logger.logInfoMessage(query.queryString);
                command = new SqlCommand(query.queryString, connection);
                SqlParameter SendTime = new SqlParameter(@"SendTime", SqlDbType.DateTime, 64);
                SqlParameter Group_Id = new SqlParameter(@"GroupId", SqlDbType.Int, 20);
                SqlParameter Nickname = new SqlParameter(@"Nickname", SqlDbType.Char, 8);
                SendTime.Value = query.dateTime;
                Group_Id.Value = query.gid;
                Nickname.Value = query.nickname;
                command.Parameters.Add(SendTime);
                command.Parameters.Add(Group_Id);
                command.Parameters.Add(Nickname);
                command.Prepare();
                data_reader = command.ExecuteReader();
                IMessage msg; //get a single message row from db
                while (data_reader.Read())
                {
                    DateTime dateFacturation = new DateTime();
                    if (!data_reader.IsDBNull(1))
                        dateFacturation = data_reader.GetDateTime(1); 
                    Guid guid = new Guid();
                    if (!data_reader.IsDBNull(0))
                        guid = Guid.Parse(data_reader.GetValue(0).ToString());
                    msg = new IMessage(guid, data_reader.GetValue(3).ToString(), data_reader.GetValue(4).ToString(), data_reader.GetValue(2).ToString(), dateFacturation);
                    messages.Add(msg);//add message to the list
                }
                _logger.logInfoMessage("check " + server_address);
                data_reader.Close();
                command.Dispose();
                connection.Close();
                _logger.logInfoMessage("Disconnected from: " + server_address);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - communicate try connection not closed");
                Console.WriteLine(ex.ToString());
                _logger.logInfoMessage("bad attempt to make a sql request in communicate");   

            }
            return messages;
    
        }

        public bool sendMessage(IMessage msg)
        {
            
                try
                {
                    connection.Open();
                    Console.WriteLine("connected to: " + server_address);
                    _logger.logInfoMessage("connected to: " + server_address);
                    SqlCommand commandMsg = new SqlCommand(null, connection);
                    // Create and prepare an SQL statement.
                    commandMsg.CommandText =
                        "INSERT INTO [Messages] ([Guid],[User_Id],[SendTime],[Body])" +
                        "VALUES (@Guid,@User_Id,@SendTime,@Body)";
                    // SqlParameter Id = new SqlParameter(@"Id", SqlDbType.Int, 20);
                    SqlParameter Guid = new SqlParameter(@"Guid", SqlDbType.Text, 68);
                    SqlParameter User_Id = new SqlParameter(@"User_Id", SqlDbType.Int, 8);
                    SqlParameter SendTime = new SqlParameter(@"SendTime", SqlDbType.DateTime, 64);
                    SqlParameter Body = new SqlParameter(@"Body", SqlDbType.Text, 100);

                    Guid.Value = msg._Id.ToString();
                    User_Id.Value = findUserId(msg._GroupID, msg._UserName);
                    DateTime dateTime = msg._Date;
                    SendTime.Value = dateTime;
                    Body.Value = msg._MessageContent;

                    commandMsg.Parameters.Add(Guid);
                    commandMsg.Parameters.Add(User_Id);
                    commandMsg.Parameters.Add(SendTime);
                    commandMsg.Parameters.Add(Body);
                    // Call Prepare after setting the Commandtext and Parameters.
                    commandMsg.Prepare();

                    //  int num_rows_changed = command.ExecuteNonQuery();
                    int num_rows_changed = commandMsg.ExecuteNonQuery();
                    Console.WriteLine(num_rows_changed + "");

                    commandMsg.Dispose();
                    connection.Close();
                    _logger.logInfoMessage("Disconnected from: " + server_address);
                    Console.WriteLine($"ExecuteNonQuery in SqlCommand executed!! {num_rows_changed.ToString()} row was changes\\inserted");
                _logger.logInfoMessage("made sql reuest successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            
           
        }


        
        /// <summary>
        /// updates the message with the same GUID as in 'msg' with msg body and time values 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool updateMessage(IMessage msg)
        {
            try
            {
                connection.Open();
                Console.WriteLine("connected to: " + server_address);
                _logger.logInfoMessage("connected to: " + server_address);
                SqlCommand commandMsg = new SqlCommand(null, connection);
                // Create and prepare an SQL statement.
                commandMsg.CommandText =
                    "UPDATE [Messages] " +
                    "SET SendTime=@SendTime, Body=@Body " +
                    "WHERE Guid=@Guid";
                
                SqlParameter SendTime = new SqlParameter(@"SendTime", SqlDbType.DateTime, 64);
                SqlParameter Body = new SqlParameter(@"Body", SqlDbType.NChar, 100);
                SqlParameter Guid = new SqlParameter(@"Guid", SqlDbType.Char, 68);

                
                DateTime dateTime = msg._Date;
                SendTime.Value = dateTime;
                Body.Value = msg._MessageContent;
                Guid.Value = msg._Id.ToString();

                commandMsg.Parameters.Add(SendTime);
                commandMsg.Parameters.Add(Body);
                commandMsg.Parameters.Add(Guid);
                // Call Prepare after setting the Commandtext and Parameters.
                commandMsg.Prepare();

                //  int num_rows_changed = command.ExecuteNonQuery();
                int num_rows_changed = commandMsg.ExecuteNonQuery();
                Console.WriteLine(num_rows_changed + "");

                commandMsg.Dispose();
                connection.Close();
                _logger.logInfoMessage("Disconnected from: " + server_address);
                Console.WriteLine($"ExecuteNonQuery in SqlCommand executed!! {num_rows_changed.ToString()} row was changes\\inserted");
                _logger.logInfoMessage("updated message in db");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - could not send message to SQL");
                Console.WriteLine(ex.ToString());
                _logger.logInfoMessage("failed to update a message in DB");
                return false;
            }

        }

        /// <summary>
        /// find the user id as written in users table
        /// </summary>
        /// <param name="G_id"></param>
        /// <param name="nickName"></param>
        /// <returns></returns>
        private int findUserId(string G_id, string nickName)
        {
            int res = 0;
            try
            {
                string query = "SELECT Users.id from [Users] WHERE [Users].Group_Id=@groupId AND [Users].Nickname=@userName";
                SqlParameter groupId = new SqlParameter(@"groupId", SqlDbType.Int, 20);
                SqlParameter userName = new SqlParameter(@"userName", SqlDbType.Char, 8);
                groupId.Value = Int32.Parse(G_id);
                userName.Value = nickName;
                command = new SqlCommand(query, connection);
                command.Parameters.Add(groupId);
                command.Parameters.Add(userName);            
                data_reader = command.ExecuteReader();
                while (data_reader.Read())
                {
                    if (!data_reader.IsDBNull(0))
                        res =Int32.Parse(data_reader.GetValue(0).ToString());
                }
                data_reader.Close();
                command.Dispose();
                _logger.logInfoMessage("Disconnected from: " + server_address);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - could not find user ID in SQL");
                Console.WriteLine(ex.ToString());
                _logger.logInfoMessage("bad attempt");   
                return res;

            }
        }

        /// <summary>
        /// create a query using the filters saved in _filters
        /// </summary>
        /// <returns></returns>
        private Query createQuery()
        {
            Query newQuery = new Query();
            foreach (IQueryAction action in _filters)
                action.execute(newQuery);
            clearFilters();
            return newQuery;
        }
        /// <summary>
        /// clear the filters list
        /// </summary>
        private void clearFilters()
        {
            _filters.Clear();
        }
        /// <summary>
        /// making a g_id filter 
        /// </summary>
        /// <param name="gid"></param>
        public void addGroupFilter(string gid)
        {
            int groupID;

            bool intCheck  = Int32.TryParse(gid, out groupID);
            if (intCheck)
            {
                GroupFilter groupFilter = new GroupFilter(groupID);
                _filters.Add(groupFilter);
            }
        }
        /// <summary>
        /// making a nickname filter
        /// </summary>
        /// <param name="nickname"></param>
        public void addNicknameFilter(string nickname)
        {
            NicknameFilter nicknameFilter = new NicknameFilter(nickname);
            _filters.Add(nicknameFilter);
        }


    }

}
