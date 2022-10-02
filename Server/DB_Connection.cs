using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Project_2022_
{
    internal class DB_Connection
    {
        MySqlConnection connection;
        public DB_Connection(string ip, string db_user, string database, string db_password)
        {
            string db = "server=" + ip + ";user=" + db_user + ";database=" + database + ";password=" + db_password + ";";
            connection = new MySqlConnection(db);    // Соединение с БД
            connection.Open();
        }
        public int request_auth(string login, string password)
        {
            int f = 0;
            string request = "SELECT `id`, `login`, `online` FROM `authorization` WHERE `login` = '" + login + "' and `password` = '" + password + "'";
            MySqlCommand command = new MySqlCommand(request, connection);   // Отправляем запрос
            MySqlDataReader reader = command.ExecuteReader();               // Читаем ответ

            while (reader.Read())
            {
                if (reader[1].ToString() == login)
                {
                    f = 1;
                }
                if (reader[2].ToString() == "1" && reader[1].ToString() == login)
                {
                    f = 2;
                }
            }
            reader.Close();

            if (f == 1)
            {
                request = "UPDATE `authorization` SET `online` = '1' WHERE `login` = '" + login + "'";
                command = new MySqlCommand(request, connection);
                command.ExecuteNonQuery();
            }

            return f;
        }
        public bool request_registr(string login, string password)
        {
            bool f = false;
            string request = "SELECT `login` FROM `authorization` WHERE `login` = '" + login + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            MySqlDataReader reader = command.ExecuteReader();               // Отправляем запрос

            while (reader.Read())
            {
                if (reader[0].ToString() == login)
                    f = true;
                else f = false;
            }
            reader.Close();

            if (f)
                return false;
            request = "INSERT INTO `authorization` (`login`, `password`, `online`) VALUES ('" + login + "', '" + password + "', '1')";
            command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();                                      // Отправляем запрос
            return true;
        }

        public bool request_logout(string login)
        {
            try
            {
                string request = "UPDATE `authorization` SET `online` = '0' WHERE `login` = '" + login + "'";
                MySqlCommand command = new MySqlCommand(request, connection);
                command.ExecuteNonQuery();                                      // Отправляем запрос
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public List<(int, int, string)> request_UsersList()
        {
            List<(int, int, string)> users = new List<(int, int, string)>();
            string request = "SELECT * FROM `authorization`";
            MySqlCommand command = new MySqlCommand(request, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add((Convert.ToInt32(reader[0]), Convert.ToInt32(reader[3]), reader[1].ToString()));
            }
            reader.Close();
            return users;
        }

        public List<(string, string, string, int)> request_MessageList(string user1, string user2)
        {
            List<(string, string, string, int)> messages = new List<(string, string, string, int)>();
            string request;
            if (user2 == "Global")
            {
                request = "SELECT `date_time`, `sender`, `message`, `id` FROM `chatmessages` WHERE `recipient` = 'Global'";
                MySqlCommand command = new MySqlCommand(request, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add((Convert.ToString(reader[0]), Convert.ToString(reader[1]), Convert.ToString(reader[2]), Convert.ToInt32(reader[3])));
                }
                reader.Close();
            }
            else
            {
                request = "SELECT `date_time`, `sender`, `message`, `id` FROM `chatmessages` WHERE `sender` = '" + user1 + "' AND `recipient` = '" + user2 + "' or `sender` = '" + user2 + "' AND `recipient` = '" + user1 + "'";
                MySqlCommand command = new MySqlCommand(request, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add((Convert.ToString(reader[0]), Convert.ToString(reader[1]), Convert.ToString(reader[2]), Convert.ToInt32(reader[3])));
                }
                reader.Close();
            }

            return messages;
        }

        public void request_MessageAdd(string sender, string recipient, string message)
        {
            string request = "INSERT INTO `chatmessages` (`sender`, `recipient`, `message`) VALUES ('" + sender + "', '" + recipient + "', '" + message + "')";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void request_MessageDelete(string id, string user)
        {
            string request = "DELETE FROM `chatmessages` WHERE `id` = '" + id + "' AND `sender` = '" + user + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void request_UserDelete(string user)
        {
            Console.WriteLine("User deleted: " + user);
            string request = "DELETE FROM `authorization` WHERE `login` = '" + user + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void close()
        {
            connection.Close();
        }
    }
}
