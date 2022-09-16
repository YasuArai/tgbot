using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Management;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace tgbot1
{
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
        public static string BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "0.0.2.2-alpha"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "< code > alpha, debug, non-release</code>"; // тута лежит инфосборк если нада можно взять

        public static List<string[]> cr = new List<string[]>();

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
            if (System.IO.File.Exists("Properties\\base.txt"))
            {
                for (int i = 0; i < System.IO.File.ReadAllLines("Properties\\base.txt").Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        cr.Add(Get_props(i));
                    }
                }
            }
            Console.WriteLine("бот работает");
            Console.ReadLine();
        } // конструктор класа где всё вызывается и задоётся при создании класса

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text?.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/сисинфо\n/creply-sm\n/savebas");
                    return;
                }
                if (message.Text?.ToLower() == "/сисинфо")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "подождите...", disableNotification: false);
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(), disableNotification: false);
                    return;
                }
                for (int i = 0; i < cr.Count; i++)
                {
                    if (message.Text?.ToLower() == cr[i][0])
                    {
                        await botClient.SendTextMessageAsync(message.Chat, cr[i][1]);
                        return;
                    }
                }
                if (message.Text?.Length >= 10)
                {
                    if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 10)) == "/creply-sm")
                    {
                        Creply_sm(message.Text.ToLower());
                        new SaveB(cr);
                        await botClient.SendTextMessageAsync(message.Chat, "команда создана. требуется перезапуск бота");
                        return;
                    }
                    return;
                }
            }
        }  // апдейт метод особо тут не кулюмай, если надо чота большое сделать выноси в метод. сис инфо в пример

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }// не трож, оно тебя сожрёт!!!

        private static string SiseInfo() // метод сис инфо
        {
            using Process process = Process.GetCurrentProcess();
            using PerformanceCounter mem = new PerformanceCounter("Memory", "Available MBytes");
            return $"Информация о боте"
                   + $"\nИмя: {BotName}"
                   + $"\nВерсия: {BotVersion}"
                   + $"\nИнформация о сборке: {Infosbork}"
                   + $"\nОС Хоста: {getOSInfo()}"
                   + $"\nИмя Хоста: {Environment.MachineName}"
                   + $"\nПроц Хоста: {GetHardwareInfo("Win32_Processor", "Name")[0]}"
                   + $"\nКол-во свободной оперативной памяти: {mem.NextValue()} MB"
                   + $"\nКол-во оперативной памяти занимаемой ботом: {process.PrivateMemorySize64 / 1024 / 1024} MB";
        }

        static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);
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
            OperatingSystem os = Environment.OSVersion;
            Version vs = os.Version;

            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            if (operatingSystem != "")
            {
                operatingSystem = "Windows " + operatingSystem;
                if (os.ServicePack != "")
                {
                    operatingSystem += " " + os.ServicePack;
                }
            }
            return operatingSystem;
        } // получение винды

        private static void Creply_sm(string creply)
        {
            creply = creply.Substring(10);
            string[] creplym = creply.Split('\n');
            cr.Add(creplym);
        }

        private string[] Get_props(int i)
        {
            string[] file = System.IO.File.ReadAllLines("Properties\\base.txt");
            string[] props = new string[2];
            props[0] = file[i];
            props[1] = file[i + 1];
            return props;
        }

    }// это база

    class SaveB
    {
        private List<string[]> cr = new List<string[]>();

        public SaveB(List<string[]> cr)
        {
            this.cr = cr;
            if (!System.IO.File.Exists("Properties\\base.txt"))
                Create_props();
            else
                Replase_Props();
        }

        protected void Create_props()
        {
            if (!Directory.Exists("Properties"))
                Directory.CreateDirectory("Properties"); // создаёт папку если таковой нет
            if (!System.IO.File.Exists("Properties\\base.txt"))
            {
                var file = System.IO.File.CreateText("Properties\\base.txt");
                file.Close();
                StreamWriter sw = new StreamWriter("Properties\\base.txt");
                foreach (var lisrcr in cr)
                {
                    sw.WriteLine($"{lisrcr[0].Trim()}\n{lisrcr[1].Trim()}"); // что будет в документе
                }
                Console.WriteLine("База данных была создана/перезаписана\nПроверте путь Properties\\base.txt");
                sw.Close();
                return;
            }
            return;
        } // создаёт документ

        protected void Replase_Props()
        {
            if (!System.IO.File.Exists("Properties\\base.txt"))
                return;
            System.IO.File.Delete("Properties\\base.txt");
            Create_props();
        } // перезаписывает

    }
}