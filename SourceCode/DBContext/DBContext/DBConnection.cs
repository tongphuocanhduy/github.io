using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBContext
{
    public class DBConnection
    {
        public static DataTable QueryBySELECT(string sql)
        {
            // Lấy ra đối tượng Connection kết nối vào DB.
            SqlConnection connection = DBUtils.GetDBConnection();
            connection.Open();
            try
            {
                using (var adapter = new SqlDataAdapter(sql, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                // Đóng kết nối.
                connection.Close();
                // Hủy đối tượng, giải phóng tài nguyên.
                connection.Dispose();
            }

            return null;
        }

        public static int QueryByINSERT(string sql)
        {
            SqlConnection connection = DBUtils.GetDBConnection();
            connection.Open();
            try
            {    
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;                 
 
                // Thực thi Command (Dùng cho delete, insert, update).
                int rowCount = cmd.ExecuteNonQuery();
 
                return rowCount;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            { 
                connection.Close(); 
                connection.Dispose();
                connection = null;
            }
            return 0;
        }
    }
}
