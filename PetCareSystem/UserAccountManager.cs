using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace PetCareSystem
{
    public class UserAccountManager
    {
        public static string CurrentUserFullName { get; private set; } = "System Administrator";


        public static bool Authenticate(string user, string pass)
        {
            using (var conn = DbConnector.GetConnection())
            {
                if (conn == null) return false;
                string query = "SELECT full_name FROM users WHERE username=@u AND password=@p AND status='Active'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@p", pass);
                object? result = cmd.ExecuteScalar();
                if (result != null)
                {
                    CurrentUserFullName = result.ToString() ?? "System Administrator";
                    return true;
                }
                return false;
            }
        }


        public static bool AddAccount(string user, string fullName, string pass, string answer)
        {
            using (var conn = DbConnector.GetConnection())
            {
                if (conn == null) return false; // Null check added
                string query = "INSERT INTO users (username, full_name, password, recovery_answer) VALUES (@u, @f, @p, @a)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@f", fullName);
                cmd.Parameters.AddWithValue("@p", pass);
                cmd.Parameters.AddWithValue("@a", answer);
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public static bool UpdateProfile(int id, string fullName, string pass)
        {
            using (var conn = DbConnector.GetConnection())
            {
                if (conn == null) return false; // Null check added
                string query = "UPDATE users SET full_name=@f, password=@p WHERE user_id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@f", fullName);
                cmd.Parameters.AddWithValue("@p", pass);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public static bool SetAccountStatus(int id, string status)
        {
            using (var conn = DbConnector.GetConnection())
            {
                if (conn == null) return false; // Null check added
                string query = "UPDATE users SET status=@s WHERE user_id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public static DataTable GetAccountList(string search = "")
        {
            using (var conn = DbConnector.GetConnection())
            {
                DataTable dt = new DataTable();
                if (conn == null) return dt; // Null check added
                string query = "SELECT user_id, username, full_name, status FROM users WHERE username LIKE @s OR full_name LIKE @s";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
        }

        public static string? RecoverPassword(string user, string answer)
        {
            using (var conn = DbConnector.GetConnection())
            {
                if (conn == null) return null;
                string query = "SELECT password FROM users WHERE username=@u AND recovery_answer=@a";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@a", answer);
                object? result = cmd.ExecuteScalar(); // 'object?' for null safety
                return result?.ToString();
            }
        }


        public static bool UpdatePassword(string user, string answer, string newPass)
        {
            try
            {
                using (var conn = DbConnector.GetConnection())
                {
                    if (conn == null) return false;

                    // This matches your column names: recovery_answer and password
                    string query = "UPDATE users SET password=@p WHERE username=@u AND recovery_answer=@a";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@p", newPass);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@a", answer);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}