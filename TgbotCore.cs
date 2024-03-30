using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace tgbot
{
    enum BD_mod { GET, SET }
    enum BD_Comand { Creply, Delreply, Sreply, nul }
    enum BD_Type { Sm, Tm }
    enum BD_Mesege { Text, Photo }

    internal class ALO_bot
    {
        private static TelegramBotClient botClient;
        private static BD bD = new();
        public static int Nummesege { get; private set; }
        public static int NumCringe { get; private set; }
        public static int Timemesege { get; private set; }
        public static long UpTime { get; private set; }
        public static int CringeRand { get; private set; }
        public static string BotToken { get; private set; } // тута лежит токен если нада можно взять
        public static string? BotName { get; private set; } // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "1.1.4.9"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "final, release"; // тута лежит инфосборк если нада можно взять

        public ALO_bot(string Token, string Name, int Cringe)
        {
            BotToken = Token;
            BotName = Name;
            CringeRand = Cringe;
            Start_bot();
            Thread myThread = new Thread(Timer);
            myThread.Start();
            Console.WriteLine($"Название бота: {BotName}");
            Console.WriteLine("бот работает");
            while (true)
                Consosmesege();
        } // конструктор класа где всё вызывается и задоётся при создании класса
        public static void Start_bot()
        {
            botClient = new TelegramBotClient(BotToken);
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
        }
        public void Timer()
        {
            while (true)
            {
                Timemesege++;
                UpTime++;
                Thread.Sleep(1000);
            }
        }
        public async void Consosmesege()
        {
            try
            {
                string[]? strm = Console.ReadLine()?.Split(" ");
                string str = "";
                for (int i = 1; i < strm?.Length; i++) str += strm[i] + " ";
                await botClient.SendTextMessageAsync(chatId: strm?[0] ?? "", text: "CONSOLE: " + str);
            }
            catch
            {
                Console.WriteLine("чот не так");
            }
        }
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
            Thread.Sleep(900000);
            Start_bot();

        }// не трож, оно тебя сожрёт!!!

        public static void Comand(Message message)
        {
            string[] comands = new string[] { "/sreply", "/creply-sm", "/creply-tm", "/delreply-sm", "/delreply-tm", "/start", "/help", "/botinfo", "/chatid", "/cringe" };
            string? Username = message.From?.FirstName;
            if (Username == null) Username = "кто-то";
            string? Chat_Name = message.Chat.Username;
            if (Chat_Name == null) Chat_Name = message.Chat.Id.ToString();
            if (message.Type == MessageType.Text)
                if (message.Text?.ToCharArray()[0] == '/')
                    ForComands(message, comands, BD_Mesege.Text, Username, Chat_Name);
                else
                    ForBd(message, Username, Chat_Name);
            else if (message.Type == MessageType.Photo)
                if (message.Caption?.ToCharArray()[0] == '/')
                    ForComands(message, comands, BD_Mesege.Photo, Username, Chat_Name);
            return;
        } // тута все команды
        public static void Answers(Message message, string Username, string Chat_Name)
        {
            Console.WriteLine($"[{DateTime.Now}] '{Username}' написал '{message.Text}' в чате '{Chat_Name}'.");
        }
        public static async Task<bool> BezSpama(Message message)
        {
            Nummesege++;
            if (Nummesege > 11 & Timemesege > 30)
            {
                Nummesege = Timemesege = 0;
                using (var stream = System.IO.File.OpenRead($"photo_memory//spam.mp3"))
                    message = await botClient.SendVoiceAsync(chatId: message.Chat, voice: stream, duration: 8);
                return true;
            }
            if (Timemesege > 30)
                Nummesege = Timemesege = 0;
            return false;
        }
        public static async void ForRandomImage(Message message, string crige)
        {
            bool cringe;
            do
                cringe = await RandomImage(message, crige);
            while (cringe);
        }
        public static async Task<bool> RandomImage(Message message, string crige)
        {
            if (crige == "")
                crige = "empty message";
            if (NumCringe > 20)
            {
                NumCringe = 0;
                return false;
            }
            string url = $"https://yandex.ru/images/search?text={crige}";
            Random random = new Random();
            string xPath = $"//div[@class='serp-item serp-item_type_search serp-item_group_search serp-item_pos_{random.Next(1, 30)} serp-item_scale_yes justifier__item i-bem']";
            string html = string.Empty;
            string userAgent = Leaf.xNet.Http.RandomUserAgent();
            using Leaf.xNet.HttpRequest request = new Leaf.xNet.HttpRequest();
            {
                request.UserAgent = userAgent;
                html = request.Get(url).ToString();
            }
            HtmlAgilityPack.HtmlDocument HTML_document = new HtmlAgilityPack.HtmlDocument();
            HTML_document.LoadHtml(html);
            HtmlAgilityPack.HtmlNode node = HTML_document.DocumentNode.SelectSingleNode(xPath);
            Console.Write(0);
            if (node == null)
            {
                NumCringe++;
                return true;
            }
            Console.Write(1);
            string json = node.Attributes[1].Value;
            Root item = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(json);
            string? DownLoadurl = item.SerpItem.preview[0].origin?.url;
            if (DownLoadurl == null)
            {
                NumCringe++;
                return true;
            }
            Console.Write(DownLoadurl);
            if (!DownLoadurl.EndsWith(".jpg"))
            {
                NumCringe++;
                return true;
            }
            Console.WriteLine(2);
            try
            {
                using (WebClient client = new WebClient())
                    client.DownloadFile(new Uri(DownLoadurl), $"photo_memory//{message.MessageId}.jpg");
                await using Stream stream = System.IO.File.OpenRead($"photo_memory//{message.MessageId}.jpg");
                await botClient.SendPhotoAsync(chatId: message.Chat, photo: new InputOnlineFile(content: stream, fileName: $"photo_memory//{message.MessageId}.jpg"), replyToMessageId: message.MessageId);
                return false;
            }
            catch
            {
                NumCringe++;
                return true;
            }

            
        }
        public static async void ForBd(Message message, string Username, string Chat_Name)
        {
            var Date = DateTime.Now;
            if (message.Text != null)
            {
                Random random = new Random();
                bool rand = random.Next(1, CringeRand) == 1;
                string[] creplym = message.Text.ToLower().Split('\n');
                string? messageret = bD.BD_Initialize(message.Chat.Id.ToString(), creplym[0], BD_Comand.nul);
                if (rand && message.Text.Length < 10) ForRandomImage(message, message.Text);
                if (messageret != null)
                {
                    bool boo = await BezSpama(message);
                    if (boo) return;
                    if (!messageret.EndsWith(".jpg"))
                    {
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: messageret, replyToMessageId: message.MessageId);
                        Console.WriteLine($"[{Date}] бот ответил '{Username}' '{messageret}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                    }
                    else
                    {
                        await using Stream stream = System.IO.File.OpenRead($"photo_memory//{messageret}");
                        await botClient.SendPhotoAsync(chatId: message.Chat, photo: new InputOnlineFile(content: stream, fileName: $"photo_memory//{messageret}"), replyToMessageId: message.MessageId);
                        Console.WriteLine($"[{Date}] бот скинул нюдсы '{Username}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                    }
                }
                else
                {
                    Answers(message, Username, Chat_Name);
                }
            }
        }
        public static async void ForComands(Message message, string[] comands, BD_Mesege bD_Mesege, string Username, string Chat_Name)
        {
            var Date = DateTime.Now;
            for (int i = 0; i < comands.Length; i++)
            {
                if (bD_Mesege == BD_Mesege.Text)
                    if (message.Text != null)
                        if (message.Text.ToLower().StartsWith(comands[i]))
                            switch (comands[i])
                            {
                                case "/sreply":
                                    bool bolo = await BezSpama(message);
                                    if (bolo) return;
                                    var sreply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/sreply", BD_Comand.Sreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (sreply != null)
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat, sreply, disableNotification: true, replyToMessageId: message.MessageId);
                                        Console.WriteLine($"[{Date}]'{Username}' посмотрел ответы в чате '{Chat_Name}'");
                                        return;
                                    }
                                    await botClient.SendTextMessageAsync(message.Chat, "не найдено", disableNotification: true);
                                    return;
                                case "/creply-sm":
                                    string? a = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, a ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/creply-tm":
                                    string? b = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, b ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-sm":
                                    string? c = Creply_sm(message.Text, message.Chat.Id.ToString(), "/delreply-sm", BD_Comand.Delreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, c ?? "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-tm":
                                    string? d = Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-tm", BD_Comand.Delreply, BD_Type.Tm, BD_Mesege.Text, null);
                                    await botClient.SendTextMessageAsync(message.Chat, d ?? "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/start":
                                    await botClient.SendTextMessageAsync(message.Chat, "Это бот. НЕ ЧЕЛОВЕК. Чтобы понять нажмите \n/help", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал start в чате '{Chat_Name}'.");
                                    return;
                                case "/help":
                                    bool boo = await BezSpama(message);
                                    if (boo) return;
                                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help - выводит это меню\n/cringe - поиск изображений через Яндекс\n/sreply [слово] - показать ответы\n/creply-sm|tm [слово на которое ответить]\n[чем ответить, обязательно через перенос строки (Ctrl+Enter на десктопе)]\nПример использования:\n /creply-sm ALO\nАЛО АЛО\nТак бот будет отвечать на сообщение, где только слово АЛО, /creply-tm испоьзовать так же, но так бот ищет АЛО во всех сообщениях, вне зависимости от того, написано ли в них что то ещё. Так же вместо ответа второй строкой вы можете прикрепить картинку, и бот будет отвечать картинкой\n/delreply-sm|tm - использовать так же, как и /creply, только delreply удаляет ответ", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал help в чате '{Chat_Name}'.");
                                    return;
                                case "/botinfo":
                                    bool bo = await BezSpama(message);
                                    if (bo) return;
                                    await botClient.SendTextMessageAsync(message.Chat, "Подождите, собираю информацию о системе...", disableNotification: true);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал botinfo в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(message.Chat.Id), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/chatid":
                                    bool bq = await BezSpama(message);
                                    if (bq) return;
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал chatid в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: message.Chat.Id.ToString(), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/cringe":
                                    bool bp = await BezSpama(message);
                                    if (bp) return;
                                    string cringe = message.Text.Substring(comands[i].Length);
                                    if (cringe.Length < 15) ForRandomImage(message, cringe);
                                    return;
                            }
                if (bD_Mesege == BD_Mesege.Photo)
                    if (message.Caption != null)
                        if (message.Caption.ToLower().StartsWith(comands[i]))
                            switch (comands[i])
                            {
                                case "/creply-sm":
                                    string filename1 = SavePhotoAsync(message).Result;
                                    string? a = Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Photo, filename1);
                                    await botClient.SendTextMessageAsync(message.Chat, a ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/creply-tm":
                                    string filename2 = SavePhotoAsync(message).Result;
                                    string? b = Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Photo, filename2);
                                    await botClient.SendTextMessageAsync(message.Chat, b ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' создал в чате '{Chat_Name}'.");
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
                string[] crepym = creply.Split('\n');
                if (crepym.Length == 1)
                {
                    creplym = new string[2];
                    creplym[0] = crepym[0];
                    creplym[1] = "";
                }
                else if (crepym.Length == 2)
                {
                    creplym = new string[3];
                    creplym[0] = crepym[0];
                    creplym[1] = "";
                    creplym[2] = crepym[1];
                }
                creplym[0].ToLower();
                for (int i = 0; i < creplym.Length; i++)
                    if (i != 1)
                        creplym[i] = creplym[i].Trim();
                    else
                        creplym[i] = photoname ?? "";
            }
            if (bD_Comand == BD_Comand.Sreply)
                return bD.BD_Initialize(Id, creplym[0], BD_Comand.Sreply);
            return bD.BD_Initialize(Id, creplym, bD_Comand, bD_Type);
        }
        private static async Task<string> SavePhotoAsync(Message message)
        {
            var fileId = message.Photo.Last().FileId;
            var fileInfo = await botClient.GetFileAsync(fileId);
            var filePath = fileInfo.FilePath;
            string destinationFilePath = $"photo_memory\\{message.Photo.Last().FileId}.jpg";
            await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
            await botClient.DownloadFileAsync(filePath: filePath, destination: fileStream);
            return $"{message.Photo.Last().FileId}.jpg";
        }
        private static string SiseInfo(long id)
        {
            using Process process = Process.GetCurrentProcess();
            var ts = TimeSpan.FromSeconds(UpTime);
            using PerformanceCounter mem = new("Memory", "Available MBytes");
            return $"Информация о боте"
                   + $"\nВремя на сервере: {DateTime.Now}"
                   + $"\nИмя: {BotName}"
                   + $"\nВерсия: {BotVersion}"
                   + $"\nИнформация о сборке: {Infosbork}"
                   + $"\nChatId: {id}"
                   + $"\nUpTime: {ts.Days} д. {ts.Hours} ч. {ts.Minutes} м. {ts.Seconds} с."
                   + $"\nкодер: Muly"
                   + $"\nидеи и консультация: CyanRed";
        } // метод сис инфо
    } 
}
