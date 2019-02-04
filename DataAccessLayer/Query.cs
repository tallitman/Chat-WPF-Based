using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
   public class Query
    {
        private const string _select = "SELECT Top 200 RTRIM([Messages].Guid),[Messages].SendTime,RTRIM([Messages].Body), [users].Group_Id,RTRIM ([Users].Nickname)  ";
        private const string _from = "FROM [Messages],[Users] ";
        private string _where = "WHERE Messages.User_Id = Users.Id ";
        private string _query="";
        public string queryString { get { return _query; } set { _query = value; } }
        private int _gid=0;
        private DateTime _date=DateTime.UtcNow;
        private string _nickname="";
        public int gid
        {
            get { return _gid; }
            set { _gid = value; }
        }
        public string nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }
        public DateTime dateTime
        {
            get { return _date; }
            set { _date = value; }
        }

        /// <summary>
        /// adds a date to the query
        /// </summary>
        /// <param name="lastTime"></param>
        public void addDate(DateTime lastTime)
        {
            _date = lastTime;
            //  string sqlFormattedDate = lastTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _where += "AND [Messages].SendTime >= @SendTime ";


        }
        /// <summary>
        /// adds a filter to the query
        /// </summary>
        /// <param name="filter"></param>
        public void addFilter(string filter)
        {
            _where += filter;
        }
        /// <summary>
        /// makes the final query
        /// </summary>
        public void makeQuery()
        {
            _query = _select + _from + _where+ "ORDER BY SendTime DESC";
        }
    }
}
