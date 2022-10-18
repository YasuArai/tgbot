using System.Diagnostics;
using System.Management;
using System.Xml;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace tgbot1
{
    enum BD_mod { GET, SET }
    enum BD_Type { Creply_Sm, Delreply_Sm }
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
        static BD bD = new BD();

        public static string BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "0.0.1.3-beta"; // тута лежит версия если нада можно взять
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
            {
                var message = update.Message;
                Comand(message);
                ForBd(message);
                Answers(message);
            }
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
                if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 10)) == "/creply-sm")
                {
                    if (cal_bd(message)) return;
                    Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/creply-sm", BD_Type.Creply_Sm);
                    await botClient.SendTextMessageAsync(message.Chat, "команда создана", disableNotification: true);
                    Console.WriteLine($"кто-то создал ответ в чате {message.Chat.Id}.");
                    return;
                }
                if (message.Text?.ToLower().Substring(0, message.Text.Length - (message.Text.Length - 12)) == "/delreply-sm")
                {
                    if (cal_bd(message)) return;
                    Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-sm", BD_Type.Delreply_Sm);
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
                await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help\n/sisinfo\n/creply-sm\n/delreply-sm", disableNotification: true);
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
            return;
        } // тута все команды
        public static async void Answers(Message message)
        {
            Console.WriteLine($"кто-то написал '{message.Text}' в чате {message.Chat.Id}.");
        }
        public static async void ForBd(Message message)
        {
            if (message.Chat.Id.ToString().ToCharArray()[0] != '-') return;
            string? a = bD.BD_Initialize(message.Chat.Id.ToString(), message.Text?.ToLower(), null, BD_mod.GET, BD_Type.Creply_Sm);
            if (a != null)
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: a, replyToMessageId: message.MessageId);
                Console.WriteLine($"бот ответил кому-то '{a}' на сообщение '{message.Text}' в чате {message.Chat.Id}.");
            }
        }

        private static void Creply_sm(string creply, string Id, string comand, BD_Type bD_Type)
        {
            int comandL = comand.Length;
            creply = creply.Substring(comandL);
            string[] creplym = creply.Split('\n');
            for (int i = 0; i < creplym.Length; i++)
            {
                creplym[i] = creplym[i].Trim();
            }
            string? a = bD.BD_Initialize(Id, null, creplym, BD_mod.SET, bD_Type);
        }
        private static bool cal_bd(Message message)
        {
            if (message.Chat.Id.ToString().ToCharArray()[0] != '-')
            {
                botClient.SendTextMessageAsync(chatId: message.Chat, text: "Иди нахуй! На тебя одного создавать базу данных не буду!", replyToMessageId: message.MessageId);
                Console.WriteLine($"уёбок в чате: {message.Chat.Id}");
                return true;
            }
            return false;
        }

        private static string SiseInfo()
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
        } // метод сис инфо

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

    }// это база

    class BD
    {
        public string BD_Initialize(string chat_name, string? qu, string[] Cr_qu, BD_mod mod, BD_Type type)
        {
            XmlDocument memory_doc = new XmlDocument();
            if (!(File.Exists($"{chat_name}_memory.xml")))
            {
                var file = System.IO.File.CreateText($"{chat_name}_memory.xml");
                file.Close();
                StreamWriter sw = new StreamWriter($"{chat_name}_memory.xml");
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<creply>\r\n</creply>");
                sw.Close();
            }
            memory_doc.Load($"{chat_name}_memory.xml");
            XmlElement? memory_el = memory_doc.DocumentElement;
            switch (mod)
            {
                case BD_mod.GET:
                    if (memory_el != null)
                    {
                        foreach (XmlElement xnode in memory_el)
                        {
                            XmlNode? attr = xnode.Attributes.GetNamedItem("name");
                            if (attr.InnerText == qu)
                                return xnode.FirstChild.InnerText;
                        }
                    }
                    return null;
                    break;
                case BD_mod.SET:
                    switch (type)
                    {
                        case BD_Type.Creply_Sm:
                            XmlElement quElem = memory_doc.CreateElement("qu");
                            XmlAttribute nameAttr = memory_doc.CreateAttribute("name");
                            XmlElement ansElem = memory_doc.CreateElement("ans");
                            XmlText nameText = memory_doc.CreateTextNode(Cr_qu[0]);
                            var ansText = memory_doc.CreateTextNode(Cr_qu[1]);
                            nameAttr.AppendChild(nameText);
                            ansElem.AppendChild(ansText);
                            quElem.Attributes.Append(nameAttr);
                            quElem.AppendChild(ansElem);
                            memory_el?.AppendChild(quElem);
                            memory_doc.Save($"{chat_name}_memory.xml");
                            return null;
                            break;
                        case BD_Type.Delreply_Sm:
                            if (memory_el != null)
                            {
                                foreach (XmlElement element in memory_el)
                                    if (element.Attributes.GetNamedItem("name")?.InnerText == Cr_qu[0]) if (element != null) memory_el?.RemoveChild(element);
                                memory_doc.Save($"{chat_name}_memory.xml");
                            }
                            return null;
                            break;
                    }
                    return null;
                    break;
                default:
                    return null;
                    break;
            }
        } // работает
    }
}