using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBContext
{
    public class DBSQLServerUtils
    {
        public static SqlConnection
            GetDBConnection(string datasource, string database, string username, string password)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                                + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }

        public static SqlConnection
            GetDBConnection(string datasource, string database)
        {

            string connString = @"Server=" + datasource + ";Database="
                                + database + ";Trusted_Connection=True;";

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }
    }
}