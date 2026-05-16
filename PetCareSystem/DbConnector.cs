using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace PetCareSystem
{
    public class DbConnector
    {
        // Connection string for local XAMPP/MySQL environment
        private static string connString = "server=localhost;database=pet_clinic_system;uid=root;pwd=;";

        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database Connection Error: " + ex.Message);
                return null;
            }
        }
    }
}