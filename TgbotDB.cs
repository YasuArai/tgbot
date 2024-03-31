using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace tgbot
{
  enum BD_mod { GET, SET }
  enum BD_Comand { Creply, Delreply, Sreply, nul }
  enum BD_Type { Sm, Tm }
  enum BD_Mesege { Text, Photo }

    class TgbotDB
    {
        public static string Read(string chatname, string qu, BD_Comand dbcommand)
        {
            string sqlExpression = "SELECT * FROM creply";
            using (var connection = new SqliteConnection("Data Source=memory.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) // если есть данные
                    {
                        return "";
                    }
                    if (dbcommand == BD_Comand.Sreply)
                    {
                      string ret = $"Ниже ответы на \"{qu}\"\r\n———————————\n";
                      int num = 0;
                      int phots = 0;
                      while (reader.Read()) // построчно считываем данные
                      {
                        if (reader[1].ToString() == chatname && reader[2].ToString() == qu)
                        {
                          if (!reader[3].ToString().EndsWith(".jpg"))
                            ret += reader[3].ToString() + "\n";
                          else {
                            phots++;
                          }
                          num++;
                        }
                      }
                      if (num == 0)
                        return "";
                      return ret + $"\nфотов: {phots}";
                    }
                        List<string?> strings = new List<string?>();
                        while (reader.Read()) // построчно считываем данные
                        {
                            Regex wordFilter = new Regex($"{reader[2]}");
                            if (reader[1].ToString() == chatname && reader[2].ToString() == qu && reader[4].ToString() == "Sm")
                                strings.Add(reader[3].ToString());
                            else if (reader[1].ToString() == chatname && wordFilter.IsMatch(qu) && reader[4].ToString() == "Tm")
                                strings.Add(reader[3].ToString());
                        }
                        int ListLeng = strings.Count;
                        if (ListLeng == 0)
                            return "";
                        Random random = new Random();
                        int rand = random.Next(ListLeng);
                        return strings[rand];
                    }
            }
        }
        public static bool AddCommand(string chatname, string[] Cr_qu, BD_Comand dbcommand, BD_Type type)
        {
            if (Cr_qu.Length < 2 && dbcommand == BD_Comand.Creply)
                return true;
            if (Cr_qu[0] == "" || Cr_qu[0] == " ")
                return true;
            if (type == BD_Type.Tm && Cr_qu[0].Length < 3)
                return true;
            string? sqlExpression = null;
            using (var connection = new SqliteConnection("Data Source=memory.db"))
            {
                connection.Open();
                SqliteCommand? command = null;
                switch (dbcommand)
                {
                    case BD_Comand.Creply:
                        if (Cr_qu.Length == 2)
                            sqlExpression = $"INSERT INTO creply (re, q, type, chatId) VALUES (@re, @q, '{type}', {chatname})";
                        if (Cr_qu.Length == 3)
                            sqlExpression = $"INSERT INTO creply (re, q, type, chatId) VALUES (@re, @q, '{type}', {Cr_qu[2]})";
                        command = new SqliteCommand(sqlExpression, connection);
                        SqliteParameter re = new SqliteParameter("@re", Cr_qu[1]);
                        command.Parameters.Add(re);
                        SqliteParameter q = new SqliteParameter("@q", Cr_qu[0]);
                        command.Parameters.Add(q);
                        break;
                    case BD_Comand.Delreply:
                        if (Cr_qu.Length == 1)
                            sqlExpression = $"DELETE  FROM creply WHERE chatId={chatname} AND q= @q AND type= '{type}'";
                        else
                        {
                            sqlExpression = $"DELETE  FROM creply WHERE chatId={chatname} AND q= @q AND re = @re AND type= '{type}'";
                            command = new SqliteCommand(sqlExpression, connection);
                            SqliteParameter g = new SqliteParameter("@q", Cr_qu[0]);
                            command.Parameters.Add(g);
                            SqliteParameter i = new SqliteParameter("@re", Cr_qu[1]);
                            command.Parameters.Add(i);
                            break;
                        }
                        command = new SqliteCommand(sqlExpression, connection);
                        SqliteParameter t = new SqliteParameter("@q", Cr_qu[0]);
                        command.Parameters.Add(t);
                        break;
                }
                int number = command.ExecuteNonQuery();
                Console.WriteLine($"изменено объектов: {number}");
                if (number == 0)
                    return true;
            }
            return false;
        }
    }
}
