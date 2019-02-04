using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    //UserHandler class's responsibly is to write Users to the file-system and read from it.
    public class UserHandler
    {
        private string connetion_string = null;
        private string sql_query = null;
        private string server_address = "ise172.ise.bgu.ac.il,1433\\DB_LAB";

        //string server_address = "localhost\\SQLExpress;";

        private string database_name = "MS3";
        private string user_name = "publicUser";
        private string password = "isANerd";
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader data_reader;
        public UserHandler()
        {
            connetion_string = $"Data Source={server_address};Initial Catalog={database_name };User ID={user_name};Password={password}";
            connection = new SqlConnection(connetion_string);
        }

        /// <summary>
        /// checks if there is a match between a registerd user in the data base to the received user details.
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="G_id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool login(string nickName, string G_id, string password)
        {
            bool res = false;
            try
            {
                connection.Open();
                Console.WriteLine("connected to: " + server_address);              
                sql_query = "SELECT * FROM [Users] Where Group_Id=@GroupId AND Nickname = @Nickname AND Password = @Password";
                SqlParameter Group_Id = new SqlParameter(@"GroupId", SqlDbType.Int, 20);
                SqlParameter Nickname = new SqlParameter(@"Nickname", SqlDbType.Char, 8);
                SqlParameter Password = new SqlParameter(@"Password", SqlDbType.Char, 64);
                command = new SqlCommand(sql_query, connection);
                Group_Id.Value = Int32.Parse(G_id);
                Nickname.Value = nickName;
                Password.Value = password;
                command.Parameters.Add(Group_Id);
                command.Parameters.Add(Nickname);
                command.Parameters.Add(Password);           
                data_reader = command.ExecuteReader();
                if (data_reader.HasRows)
                {
                    res = true;
                    while (data_reader.Read())
                    {
                        Console.WriteLine("User ID: " + data_reader.GetValue(0) + ", Group Id: " + data_reader.GetValue(1) + ", NickName: " + data_reader.GetValue(2) + ", password:" + data_reader.GetValue(3)); //fill code instead of ????????? to read order id. ********************
                    }
                }
                data_reader.Close();
                command.Dispose();
                connection.Close();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.ToString());
                return res;
            }
         
        }
        /// <summary>
        /// first checks if there is a user with these details in the data base,if there is no such user, insert the received details into users table (register as a user).
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="G_id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool register(string nickName, string G_id, string password)
        {
            try
            {
                if (login(nickName, G_id, password))
                {
                    connection.Close();
                    return false;
                }
                connection.Open();
                Console.WriteLine("connected to: " + server_address);

                // Create and prepare an SQL statement.
                // Use should never use something like: query = "insert into table values(" + value + ");" 
                // Especially when selecting. More about it on the lab about security.
                command = new SqlCommand(null, connection);
                command.CommandText =
                    "INSERT INTO [Users] ([Group_Id],[Nickname],[Password])" + // Fill code here. SQL query for inserting values into customer table *******************************************************
                    "VALUES (@Group_Id,@Nickname,@Password)";
                
                SqlParameter Group_Id = new SqlParameter(@"Group_Id", SqlDbType.Int, 20);
                SqlParameter Nickname = new SqlParameter(@"Nickname", SqlDbType.Char, 8);
                SqlParameter Password = new SqlParameter(@"Password", SqlDbType.Char, 64);
                Group_Id.Value =Int32.Parse(G_id);
                Nickname.Value = nickName;
                Password.Value = password;
                command.Parameters.Add(Group_Id);
                command.Parameters.Add(Nickname);
                command.Parameters.Add(Password);
                command.Prepare();
                int num_rows_changed = command.ExecuteNonQuery();              
                Console.WriteLine(num_rows_changed + "");
                command.Dispose();
                connection.Close();
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
        /// returns a list of users with their nickname and group id for each.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> getUsers()
        {
            List<KeyValuePair<string, string>> usersList = new List<KeyValuePair<string, string>>();
            try
            {
                connection.Open();
                Console.WriteLine("connected to: " + server_address);
                sql_query = "SELECT Group_Id, RTRIM(Nickname) FROM [Users]";
                string group = "";
                string nickname = "";
                command = new SqlCommand(sql_query, connection);
                data_reader = command.ExecuteReader();
                if (data_reader.HasRows)
                {
                    while (data_reader.Read())
                    {
                        group = data_reader.GetValue(0).ToString();
                        nickname = data_reader.GetValue(1).ToString();
                        KeyValuePair<string,string> temp= new KeyValuePair<string,string>(group, nickname);
                        usersList.Add(temp);
          
                    }
                }
                data_reader.Close();
                command.Dispose();
                connection.Close();
                return usersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.ToString());
                return usersList;
            }

        }
    }
}
