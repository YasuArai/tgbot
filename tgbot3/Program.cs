using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.InteropServices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace tgbot1
{
    class Alo_token : ALO_Props
    {
        public override string Get_props()
        {
            try
            {
                string[] file = System.IO.File.ReadAllLines("Properties\\set.txt");
                string token;
                token = file[0];
                token = token.TrimStart('T', 'o', 'k', 'e', 'n', ' ', ':', ' '); // удаляет 1 слово
                token = token.Trim(new Char[] { ' ', '[', ']' }); // защита от дибила

                if (!(token.Length == 46)) // мало букв
                {
                    Console.WriteLine("Не коректные данные");
                    Replase_Props();
                }
                return token;
            }
            catch (Exception)
            {
                Console.WriteLine("Не коректные данные");
                Replase_Props();
                Create_props();
                throw;
            }
        }
    }

    class Alo_BotName : ALO_Props
    {
        public override string Get_props()
        {
            try
            {
                string[] file = System.IO.File.ReadAllLines("Properties\\set.txt");
                string name;
                name = file[1];
                name = name.TrimStart('B', 'o', 't', 'N', 'a', 'm', 'e', ':', ' '); // удаляет 1 слово
                name = name.Trim(new Char[] { ' ', '[', ']' }); // защита от дибила
                return name;
            }
            catch (Exception)
            {
                Console.WriteLine("Не коректные данные");
                Replase_Props();
                Create_props();
                throw;
            }
        }
    }

    abstract class ALO_Props
    {
        protected void Create_props()
        {
            if (!Directory.Exists("Properties"))
            {
                Directory.CreateDirectory("Properties"); // создаёт папку если таковой нет
            }
            if (!System.IO.File.Exists("Properties\\set.txt"))
            {
                var file = System.IO.File.CreateText("Properties\\set.txt");
                file.Close();
                StreamWriter sw = new StreamWriter("Properties\\set.txt");
                sw.WriteLine("Token : [ctrl+v Token]\nBotName : [BotName]"); // что будет в документе
                sw.Close();
                Console.WriteLine("Текстовый документ был создан/перезаписан\nПроверте путь Properties\\set.txt");
                Console.ReadLine();
            }
        } // создаёт документ

        protected void Replase_Props()
        {
            if (!System.IO.File.Exists("Properties\\set.txt"))
            {
                return;
            }
            System.IO.File.Delete("Properties\\set.txt");
            Create_props();
        } // перезаписывает

        public abstract string Get_props();
    }  // абстрактный класс получения настроек

    class Tgbot
    {
        static void Main(string[] args)
        {
            new ALO_bot(new Alo_token().Get_props(), new Alo_BotName().Get_props()); // создаём класс ALO_bot и в конструктор добовляем токен и имя
        }
    } // Main

    class ALO_bot
    {
        static TelegramBotClient botClient;
        public static string BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "0.0.0.6-alpha"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "< code > alpha, debug, non-release</code>"; // тута лежит инфосборк если нада можно взять

        public ALO_bot(string Token, string Name)
        {
            BotToken = Token;
            BotName = Name;
            botClient = new TelegramBotClient(Token);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
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
                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/сисинфо");
                    return;
                }
                if (message.Text?.ToLower() == "/сисинфо")
                {
#pragma warning disable CS4014
                    botClient.SendTextMessageAsync(message.Chat, "подождите...", disableNotification: false);
#pragma warning restore CS4014
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(), disableNotification: false);
                    return;
                }

              //  await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }  // апдейт метод особо тут не кулюмай, если надо чота большое сделать выноси в метод. сис инфо в пример

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }// не трож, оно тебя сожрёт!!!

        private static string SiseInfo() // метод сис инфо
        {
            Process process = Process.GetCurrentProcess();
            PerformanceCounter mem = new PerformanceCounter("Memory", "Available MBytes");
            return $"Информация о боте"
                   + $"\nИмя: {BotName}"
                   + $"\nВерсия: {BotVersion}"
                   + $"\nИнформация о сборке: {Infosbork}"
                   + $"\nОС Хоста: {getOSInfo()}"
                   + $"\nИмя Хоста: {Environment.MachineName}"
                   + $"\nПроц Хоста: {GetHardwareInfo("Win32_Processor", "Name")[0]}"
                   + $"\nКол-во свободной оперативной памяти: {mem.NextValue()} MB"
                   + $"\nКол-во оперативной памяти занимаемой ботом: {Math.Ceiling((double)(process.PrivateMemorySize64/1024/1024))} MB";
        }

        static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

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
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
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
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        } // получение винды
    } // это база
}