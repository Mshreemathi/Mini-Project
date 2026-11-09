using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace IPLProject
{
    public static class Db
    {
        public static SqlConnection GetConnection()
        {
            var setting = ConfigurationManager.ConnectionStrings["IPLDb"];
            if (setting == null) throw new InvalidOperationException("Connection string 'IPLDb' not found.");
            return new SqlConnection(setting.ConnectionString);
        }

        private static void AddParameters(SqlCommand cmd, object[] values)
        {
            if (values == null) return;
            for (int i = 0; i < values.Length; i++)
            {
                string name = "@p" + i;
                int marker = cmd.CommandText.IndexOf("?", StringComparison.Ordinal);
                if (marker >= 0) cmd.CommandText = cmd.CommandText.Remove(marker, 1).Insert(marker, name);
                cmd.Parameters.AddWithValue(name, values[i] ?? DBNull.Value);
            }
        }

        public static DataTable GetTable(string sql, params object[] values)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                AddParameters(cmd, values);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static object ExecuteScalar(string sql, params object[] values)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                AddParameters(cmd, values);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static int ExecuteNonQuery(string sql, params object[] values)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                AddParameters(cmd, values);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
