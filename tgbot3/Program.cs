﻿using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot1
{
    enum BD_mod { GET, SET }
    enum BD_Comand { Creply, Delreply, Sreply, nul }
    enum BD_Type { Sm }
    class ALO_Props
    {
        protected void Create_props()
        {
            if (!Directory.Exists("Properties"))
                Directory.CreateDirectory("Properties"); // создаёт папку если таковой нет
            if (!System.IO.File.Exists("Properties\\set.txt"))
            {
                var file = System.IO.File.CreateText("Properties\\set.txt");
                file.Close();
                StreamWriter sw = new StreamWriter("Properties\\set.txt");
                sw.WriteLine("Token : [ctrl+v Token]\nBotName : [BotName]"); // что будет в документе
                Console.WriteLine("Текстовый документ был создан/перезаписан\nПроверте путь Properties\\set.txt");
                sw.Close();
                Console.ReadLine();
            }
        } // создаёт документ

        protected void Replase_Props()
        {
            if (!System.IO.File.Exists("Properties\\set.txt"))
                return;
            System.IO.File.Delete("Properties\\set.txt");
            Create_props();
        } // перезаписывает

        public string Get_props(int i, string lastname)
        {
            try
            {
                char[] chars = lastname.ToCharArray();
                string[] file = System.IO.File.ReadAllLines("Properties\\set.txt");
                string props = file[i];
                props = props.TrimStart(chars); // удаляет 1 слово
                props = props.Trim(new char[] { ' ', ':', ' ', '[', ']' }); // защита от дибила
                if (lastname == "Token" && !(props.Length == 46)) // мало букв
                {
                    Console.WriteLine("Не коректные данные");
                    Replase_Props();
                }
                return props;
            }
            catch (Exception)
            {
                Exception();
                throw;
            }
        }

        protected void Exception()
        {
            Console.WriteLine("Не коректные данные");
            Replase_Props();
            Create_props();
        }
    } // класс получения настроек

    class Tgbot : ALO_Props
    {
        static void Main(string[] args)
        {
            var Alo_props = new ALO_Props();
            new ALO_bot(Alo_props.Get_props(0, "Token"), Alo_props.Get_props(1, "BotName")); // создаём класс ALO_bot и в конструктор добовляем токен и имя
        }
    } // Main тут методы не создавать

    class ALO_bot
    {
        static TelegramBotClient botClient;
        private static BD bD = new();

        public static string? BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string? BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "0.0.2.12-beta"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "beta, debug, non-release"; // тута лежит инфосборк если нада можно взять

        public ALO_bot(string Token, string Name)
        {
            BotToken = Token;
            BotName = Name;
            botClient = new TelegramBotClient(Token);
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Console.WriteLine("бот работает");
            Console.ReadLine();
        } // конструктор класа где всё вызывается и задоётся при создании класса

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            if (update.Type == UpdateType.Message)
                Comand(update.Message);
        }  // апдейт метод особо тут не кулюмай, если надо чота большое сделать выноси в метод. сис инфо в пример

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }// не трож, оно тебя сожрёт!!!

        public static async void Comand(Message message)
        {
            try
            {
                if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 7)) == "/sreply")
                {
                    var sreply = Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/sreply", BD_Comand.Sreply);
                    if (sreply != null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, sreply, disableNotification: true);
                        Console.WriteLine($"кто-то посмотрел ответы в чате {message.Chat.Id}.");
                        return;
                    }
                    await botClient.SendTextMessageAsync(message.Chat, "не найдено", disableNotification: true);
                    return;
                }
                if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 10)) == "/creply-sm")
                {
                    Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply);
                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true);
                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                    return;
                }
                if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 12)) == "/delreply-sm")
                {
                    Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-sm", BD_Comand.Delreply);
                    await botClient.SendTextMessageAsync(message.Chat, "команда удалена", disableNotification: true);
                    Console.WriteLine($"кто-то удалил ответ в чате {message.Chat.Id}.");
                    return;
                }
            }
            catch (Exception)
            {
            }
            if (message.Text?.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Это бот. НЕ ЧЕЛОВЕК. Чтобы понять нажмите \n/help", disableNotification: true);
                Console.WriteLine($"кто-то вызвал start в чате {message.Chat.Id}.");
                return;
            }
            if (message.Text?.ToLower() == "/help")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help\n/sisinfo\n/sreply\n/creply-sm\n/delreply-sm", disableNotification: true);
                Console.WriteLine($"кто-то вызвал help в чате {message.Chat.Id}.");
                return;
            }
            if (message.Text?.ToLower() == "/sisinfo")
            {
                await botClient.SendTextMessageAsync(message.Chat, "подождите...", disableNotification: true);
                Console.WriteLine($"кто-то вызвал sisinfo в чате {message.Chat.Id}.");
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(), disableNotification: true);
                return;
            }
            else
            {
                ForBd(message);
            }
            return;
        } // тута все команды
        public static void Answers(Message message)
        {
            Console.WriteLine($"кто-то написал '{message.Text}' в чате {message.Chat.Id}.");
        }
        public static async void ForBd(Message message)
        {
            string[]? creplym = message.Text?.ToLower().Split('\n');
            string? a = bD.BD_Initialize(message.Chat.Id.ToString(), creplym[0], BD_Comand.nul);
            if (a != null)
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: a, replyToMessageId: message.MessageId);
                Console.WriteLine($"бот ответил кому-то '{a}' на сообщение '{message.Text}' в чате {message.Chat.Id}.");
            }
            else
            {
                Answers(message);
            }
        }

        private static string? Creply_sm(string creply, string Id, string comand, BD_Comand bD_Comand)
        {
            int comandL = comand.Length;
            creply = creply.Substring(comandL);
            string[] creplym = creply.Split('\n');
            for (int i = 0; i < creplym.Length; i++)
                creplym[i] = creplym[i].Trim();
            if (bD_Comand == BD_Comand.Sreply)
                return bD.BD_Initialize(Id, creplym[0], BD_Comand.Sreply);
            bD.BD_Initialize(Id, creplym, bD_Comand, BD_Type.Sm);
            return null;
        }

        private static string SiseInfo()
        {
            using Process process = Process.GetCurrentProcess();
            using PerformanceCounter mem = new("Memory", "Available MBytes");
            return $"Информация о боте"
                   + $"\nИмя: {BotName}"
                   + $"\nВерсия: {BotVersion}"
                   + $"\nИнформация о сборке: {Infosbork}"
                   + $"\nОС Хоста: {getOSInfo()}"
                   + $"\nИмя Хоста: {Environment.MachineName}"
                   + $"\nПроц Хоста: {GetHardwareInfo("Win32_Processor", "Name")[0]}"
                   + $"\nКол-во свободной оперативной памяти: {mem.NextValue()} MB"
                   + $"\nКол-во оперативной памяти занимаемой ботом: {process.PrivateMemorySize64 / 1024 / 1024} MB";
        } // метод сис инфо

        static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            using ManagementObjectSearcher searcher = new("SELECT * FROM " + WIN32_Class);
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        } // получение проца

        private static string getOSInfo()
        {
            {
                string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        try
                        {
                            string name = regKey.GetValue("ProductName").ToString();
                            if (name == "") return "Значение отсутствует";
                            else
                                return $"{name}";
                        }
                        catch (Exception ex)
                        {
                            return ex.Message;
                        }
                    }
                    else
                        return "Не удалось получить значение ключа в реестре";
                }
            } // получение винды
        } // получение винды

    }// это база

    class BD
    {
        public string? BD_Initialize(string chat_name, string? qu, BD_Comand comand)
        {
            string sqlExpression = "SELECT * FROM creply";
            using (var connection = new SqliteConnection("Data Source=memory.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        if (comand == BD_Comand.Sreply)
                        {
                            string ret = $"Ниже ответы на \"{qu}\"\r\n———————————\n";
                            while (reader.Read()) // построчно считываем данные
                            {
                                if (reader[1].ToString() == chat_name & reader[2].ToString() == qu)
                                {
                                    ret += reader[3].ToString() + "\n";
                                }
                            }
                            return ret;
                        }
                        List<string?> strings = new List<string?>();
                        while (reader.Read()) // построчно считываем данные
                        {
                            if (reader[1].ToString() == chat_name & reader[2].ToString() == qu)
                            {
                                strings.Add(reader[3].ToString());
                            }
                        }
                        int ListLeng = strings.Count;
                        if (ListLeng == 0) return null;
                        Random random = new Random();
                        int rand = random.Next(ListLeng);
                        return strings[rand];
                    }
                    return null;
                }
            }
        }
        public void BD_Initialize(string chat_name, string[]? Cr_qu, BD_Comand comand, BD_Type type)
        {
            if (Cr_qu?.Length < 2 && comand == BD_Comand.Creply)
                return;
            switch (type)
            {
                case BD_Type.Sm:
                    string? sqlExpression = null;
                    using (var connection = new SqliteConnection("Data Source=memory.db"))
                    {
                        connection.Open();
                        SqliteCommand? command = null;
                        switch (comand)
                        {
                            case BD_Comand.Creply:
                                sqlExpression = $"INSERT INTO creply (re, q, chatId) VALUES (@re, @q, {chat_name})";
                                command = new SqliteCommand(sqlExpression, connection);
                                SqliteParameter re = new SqliteParameter("@re", Cr_qu[1]);
                                command.Parameters.Add(re);
                                SqliteParameter q = new SqliteParameter("@q", Cr_qu[0]);
                                command.Parameters.Add(q);
                                break;
                            case BD_Comand.Delreply:
                                sqlExpression = $"DELETE  FROM creply WHERE chatId={chat_name} AND q= @q";
                                command = new SqliteCommand(sqlExpression, connection);
                                SqliteParameter t = new SqliteParameter("@q", Cr_qu[0]);
                                command.Parameters.Add(t);
                                break;
                        }
                        int number = command.ExecuteNonQuery();
                        Console.WriteLine($"изменено объектов: {number}");
                    }
                    break;
            }
        }
    }
}