using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace tgbot1
{
    enum BD_mod { GET, SET }
    enum BD_Comand { Creply, Delreply, Sreply, nul }
    enum BD_Type { Sm, Tm }
    enum BD_Mesege { Text, Photo }
    class ALO_Props
    {
        protected static void Create_props()
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

        protected static void Replase_Props()
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
        private static TelegramBotClient botClient;
        private static BD bD = new();
        public static string? BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string? BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "1.0.0.1"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "final, release"; // тута лежит инфосборк если нада можно взять

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

            Console.WriteLine($"Название бота: {BotName}");
            Console.WriteLine("бот работает");
            Console.ReadLine();
        } // конструктор класа где всё вызывается и задоётся при создании класса

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            if (update.Type == UpdateType.Message)
                if (update.Message != null)
                    Comand(update.Message);
        }  // апдейт метод особо тут не кулюмай, если надо чота большое сделать выноси в метод. сис инфо в пример

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }// не трож, оно тебя сожрёт!!!

        public static void Comand(Message message)
        {
            string[] comands = new string[] { "/sreply", "/creply-sm", "/creply-tm", "/delreply-sm", "/delreply-tm", "/start", "/help", "/botinfo" };
            if (message.Type == MessageType.Text)
                if (message.Text?.ToCharArray()[0] == '/')
                    ForComands(message, comands, BD_Mesege.Text);
                else
                    ForBd(message);
            else if (message.Type == MessageType.Photo)
                if (message.Caption?.ToCharArray()[0] == '/')
                    ForComands(message, comands, BD_Mesege.Photo);
            return;
        } // тута все команды
        public static void Answers(Message message)
        {
            Console.WriteLine($"кто-то написал '{message.Text}' в чате {message.Chat.Id}.");
        }
        public static async void ForBd(Message message)
        {
            if (message.Text != null)
            {
                string[] creplym = message.Text.ToLower().Split('\n');
                string? a = bD.BD_Initialize(message.Chat.Id.ToString(), creplym[0], BD_Comand.nul);
                if (a != null)
                {
                    if (!a.EndsWith(".jpg"))
                    {
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: a, replyToMessageId: message.MessageId);
                        Console.WriteLine($"бот ответил кому-то '{a}' на сообщение '{message.Text}' в чате {message.Chat.Id}.");
                    }
                    else
                    {
                        await using Stream stream = System.IO.File.OpenRead($"photo_memory//{a}");
                        await botClient.SendPhotoAsync(chatId: message.Chat, photo: new InputOnlineFile(content: stream, fileName: $"photo_memory//{a}"), replyToMessageId: message.MessageId);
                        Console.WriteLine($"бот скинул нюдсы кому-то на сообщение '{message.Text}' в чате {message.Chat.Id}.");
                    }
                }
                else
                {
                    Answers(message);
                }
            }
        }
        public static async void ForComands(Message message, string[] comands, BD_Mesege bD_Mesege)
        {
            for (int i = 0; i < comands.Length; i++)
            {
                if (bD_Mesege == BD_Mesege.Text)
                    if (message.Text != null)
                        if (message.Text.ToLower().StartsWith(comands[i]))
                            switch (comands[i])
                            {
                                case "/sreply":
                                    var sreply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/sreply", BD_Comand.Sreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (sreply != null)
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat, sreply, disableNotification: true, replyToMessageId: message.MessageId);
                                        Console.WriteLine($"кто-то посмотрел ответы в чате {message.Chat.Id}.");
                                        return;
                                    }
                                    await botClient.SendTextMessageAsync(message.Chat, "не найдено", disableNotification: true);
                                    return;
                                case "/creply-sm":
                                    Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                                    return;
                                case "/creply-tm":
                                    Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                                    return;
                                case "/delreply-sm":
                                    Creply_sm(message.Text, message.Chat.Id.ToString(), "/delreply-sm", BD_Comand.Delreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда удалена(если конечно она была)", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то удалил ответ в чате {message.Chat.Id}.");
                                    return;
                                case "/delreply-tm":
                                    Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-tm", BD_Comand.Delreply, BD_Type.Tm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда удалена(если конечно она была)", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то удалил ответ в чате {message.Chat.Id}.");
                                    return;
                                case "/start":
                                    await botClient.SendTextMessageAsync(message.Chat, "Это бот. НЕ ЧЕЛОВЕК. Чтобы понять нажмите \n/help", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то вызвал start в чате {message.Chat.Id}.");
                                    return;
                                case "/help":
                                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help\n/botinfo\n/sreply\n/creply-sm|tm\n/delreply-sm|tm", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то вызвал help в чате {message.Chat.Id}.");
                                    return;
                                case "/botinfo":
                                    await botClient.SendTextMessageAsync(message.Chat, "Подождите, собираю информацию о системе...", disableNotification: true);
                                    Console.WriteLine($"кто-то вызвал botinfo в чате {message.Chat.Id}.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(message.Chat.Id), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                            }
                if (bD_Mesege == BD_Mesege.Photo)
                    if (message.Caption != null)
                        if (message.Caption.ToLower().StartsWith(comands[i]))
                            switch (comands[i])
                            {
                                case "/creply-sm":
                                    string filename1 = SavePhotoAsync(message).Result;
                                    Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Photo, filename1);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                                    return;
                                case "/creply-tm":
                                    string filename2 = SavePhotoAsync(message).Result;
                                    Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Photo, filename2);
                                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                                    return;
                            }
            }
        }
        private static string? Creply_sm(string creply, string Id, string comand, BD_Comand bD_Comand, BD_Type bD_Type, BD_Mesege bD_Mesege, string? photoname)
        {
            int comandL = comand.Length;
            creply = creply.Substring(comandL);
            string[] creplym = new string[100];
            if (bD_Mesege == BD_Mesege.Text)
            {
                creplym = creply.Split('\n');
                creplym[0].ToLower();
                for (int i = 0; i < creplym.Length; i++)
                    creplym[i] = creplym[i].Trim();
            }
            if (bD_Mesege == BD_Mesege.Photo)
            {
                creplym = creply.Split('\n');
                creplym[0].ToLower();
                for (int i = 0; i < creplym.Length; i++)
                    if (i != 1)
                        creplym[i] = creplym[i].Trim();
                    else
                        creplym[i] = photoname ?? "";
            }
            if (bD_Comand == BD_Comand.Sreply)
                return bD.BD_Initialize(Id, creplym[0], BD_Comand.Sreply);
            bD.BD_Initialize(Id, creplym, bD_Comand, bD_Type);
            return null;
        }
        private static async Task<string> SavePhotoAsync(Message message)
        {
            var fileId = message.Photo.Last().FileId;
            var fileInfo = await botClient.GetFileAsync(fileId);
            var filePath = fileInfo.FilePath;
            string destinationFilePath = $"photo_memory\\{message.Photo.Last().FileId}.jpg";
            await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
            await botClient.DownloadFileAsync(
                filePath: filePath,
                destination: fileStream);
            return $"{message.Photo.Last().FileId}.jpg";
        }
        private static string SiseInfo(long id)
        {
            using Process process = Process.GetCurrentProcess();
            using PerformanceCounter mem = new("Memory", "Available MBytes");
            return $"Информация о боте"
                   + $"\nИмя: {BotName}"
                   + $"\nВерсия: {BotVersion}"
                   + $"\nИнформация о сборке: {Infosbork}"
                   + $"\nChatId: {id}"
                   + $"\nОС Хоста: {getOSInfo()}"
                   + $"\nИмя Хоста: {Environment.MachineName}"
                   + $"\nПроц Хоста: {GetHardwareInfo("Win32_Processor", "Name")[0]}"
                   + $"\nКол-во свободной оперативной памяти: {mem.NextValue()} MB"
                   + $"\nКол-во оперативной памяти занимаемой ботом: {process.PrivateMemorySize64 / 1024 / 1024} MB"
                   + $"\nкодер: Muly"
                   + $"\nидеи и консультация: CyanRed";
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
                using (RegistryKey? regKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        try
                        {
                            string? name = regKey.GetValue("ProductName").ToString();
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
    }

    class BD
    {
        public string? BD_Initialize(string chat_name, string qu, BD_Comand comand)
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
                            int num = 0;
                            int phots = 0;
                            while (reader.Read()) // построчно считываем данные
                            {
                                if (reader[1].ToString() == chat_name && reader[2].ToString() == qu)
                                {
                                    if (!reader[3].ToString().EndsWith(".jpg"))
                                        ret += reader[3].ToString() + "\n";
                                    else
                                        phots++;
                                    num++;
                                }
                            }
                            if (num == 0) return null;
                            return ret + $"\nфотов: {phots}";
                        }
                        List<string?> strings = new List<string?>();
                        while (reader.Read()) // построчно считываем данные
                        {
                            Regex wordFilter = new Regex($"{reader[2]}");
                            if (reader[1].ToString() == chat_name && reader[2].ToString() == qu && reader[4].ToString() == "Sm")
                                strings.Add(reader[3].ToString());
                            else if (wordFilter.IsMatch(qu) && reader[4].ToString() == "Tm")
                                strings.Add(reader[3].ToString());
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
        public void BD_Initialize(string chat_name, string[] Cr_qu, BD_Comand comand, BD_Type type)
        {
            if (Cr_qu.Length < 2 && comand == BD_Comand.Creply)
                return;
            string? sqlExpression = null;
            using (var connection = new SqliteConnection("Data Source=memory.db"))
            {
                connection.Open();
                SqliteCommand? command = null;
                switch (comand)
                {
                    case BD_Comand.Creply:
                        if (Cr_qu.Length == 2)
                            sqlExpression = $"INSERT INTO creply (re, q, type, chatId) VALUES (@re, @q, '{type}', {chat_name})";
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
                            sqlExpression = $"DELETE  FROM creply WHERE chatId={chat_name} AND q= @q AND type= '{type}'";
                        else
                        {
                            sqlExpression = $"DELETE  FROM creply WHERE chatId={chat_name} AND q= @q AND re = @re AND type= '{type}'";
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
            }
        }
    }
}