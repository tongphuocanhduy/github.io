using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBContext
{
    public static class DBUtils
    {
        public static string DATASOURCE;
        public static string DATABASE;
        public static string USERNAME;
        public static string PASSWORD;

        public static SqlConnection GetDBConnection()
        {
            string datasource = @"192.168.205.135\SQLEXPRESS";

            string database = "simplehr";
            string username = "sa";
            string password = "1234";

            //return DBSQLServerUtils.GetDBConnection(datasource,  database, username, password);

            return DBSQLServerUtils.GetDBConnection(DATASOURCE, DATABASE);
        }
    }
}
