using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace tgbot
{ 
    class TgbotCore
    {
        private static TelegramBotClient botClient;
        public static int Timemesege { get; private set; }
        public static long UpTime { get; private set; }
        public static string BotToken { get; private set; } = ""; // тута лежит токен если нада можно взять
        public static string BotName { get; private set; } = ""; // тута лежит имя если нада можно взять
        public static string BotVersion { get; } = "1.1.4.13"; // тута лежит версия если нада можно взять
        public static string Infosbork { get; } = "final, dev, gold"; // тута лежит инфосборк если нада можно взять


        public TgbotCore(string token, string name)
        {
            BotToken = token;
            BotName = name;
            botClient = new TelegramBotClient(BotToken);
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Thread myThread = new Thread(Timer);
            myThread.Start();
            Console.WriteLine($"Название бота: {BotName}");
            Console.WriteLine("бот работает");
        } // конструктор класа где всё вызывается и задоётся при создании класса

        public void Timer()
        {
            while (true) {
                Timemesege++;
                UpTime++;
                Thread.Sleep(1000);
            }
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
                if (update.Message != null)
                    await Comand(update.Message);
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private static async Task Comand(Message message)
        {
            string[] comands = new string[] { "/sreply", "/creply-sm", "/creply-tm", "/delreply-sm", "/delreply-tm", "/start", "/help", "/botinfo", "/chatid", "/cringe" };
            string? Username = message.From?.FirstName;
            if (Username == null)
                Username = "кто-то";
            string? Chat_Name = message.Chat.Username;
            if (Chat_Name == null)
                Chat_Name = message.Chat.Id.ToString();
            if (message.Type == MessageType.Text)
                if (message.Text?.ToCharArray()[0] == '/')
                    await ForComands(message, comands, BD_Mesege.Text, Username, Chat_Name);
                else
                    await ForBd(message, Username, Chat_Name);
            else if (message.Type == MessageType.Photo)
                if (message.Caption?.ToCharArray()[0] == '/')
                    await ForComands(message, comands, BD_Mesege.Photo, Username, Chat_Name);
        return;
        } // тута все команды

        public static void Answers(Message message, string Username, string Chat_Name)
        {
            Console.WriteLine($"[{DateTime.Now}] '{Username}' написал '{message.Text}' в чате '{Chat_Name}'.");
        }

        public static async Task ForBd(Message message, string Username, string Chat_Name)
        {
            var Date = DateTime.Now;
            if (message.Text != null) {
                string[] creplym = message.Text.ToLower().Split('\n');
                string messageret = TgbotDB.Read(message.Chat.Id.ToString(), creplym[0], BD_Comand.nul);
                if (messageret != null) {
                    if (!messageret.EndsWith(".jpg")) {
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: messageret, replyToMessageId: message.MessageId);
                        Console.WriteLine($"[{Date}] бот ответил '{Username}' '{messageret}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                    }
                    else {
                        await using Stream stream = System.IO.File.OpenRead($"photo_memory//{messageret}");
                        await botClient.SendPhotoAsync(chatId: message.Chat, photo: new InputOnlineFile(content: stream, fileName: $"photo_memory//{messageret}"), replyToMessageId: message.MessageId);
                        Console.WriteLine($"[{Date}] бот скинул нюдсы '{Username}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                    }
                }
                else {
                    Answers(message, Username, Chat_Name);
                }
            }
        }

        public static async Task ForComands(Message message, string[] comands, BD_Mesege bD_Mesege, string Username, string Chat_Name)
        {
            var Date = DateTime.Now;
            string creply = "";
            for (int i = 0; i < comands.Length; i++)
            {
                if (bD_Mesege == BD_Mesege.Text)
                    if (message.Text != null)
                        if (message.Text.ToLower().StartsWith(comands[i]))
                            switch (comands[i]) {
                                case "/sreply":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/sreply", BD_Comand.Sreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "") {
                                      await botClient.SendTextMessageAsync(message.Chat, "не найдено", disableNotification: true);
                                      return;
                                    }
                                    await botClient.SendTextMessageAsync(message.Chat, creply, disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}]'{Username}' посмотрел ответы в чате '{Chat_Name}'");
                                    return;
                                case "/creply-sm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                      Console.WriteLine($"[{Date}] '{Username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/creply-tm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                      Console.WriteLine($"[{Date}] '{Username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-sm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/delreply-sm", BD_Comand.Delreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-tm":
                                    creply = Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-tm", BD_Comand.Delreply, BD_Type.Tm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                    Console.WriteLine($"[{Date}] '{Username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/start":
                                    await botClient.SendTextMessageAsync(message.Chat, "Это бот. НЕ ЧЕЛОВЕК. Чтобы понять нажмите \n/help", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал start в чате '{Chat_Name}'.");
                                    return;
                                case "/help":
                                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help - выводит это меню\n/cringe - поиск изображений через Яндекс\n/sreply [слово] - показать ответы\n/creply-sm|tm [слово на которое ответить]\n[чем ответить, обязательно через перенос строки (Ctrl+Enter на десктопе)]\nПример использования:\n /creply-sm ALO\nАЛО АЛО\nТак бот будет отвечать на сообщение, где только слово АЛО, /creply-tm испоьзовать так же, но так бот ищет АЛО во всех сообщениях, вне зависимости от того, написано ли в них что то ещё. Так же вместо ответа второй строкой вы можете прикрепить картинку, и бот будет отвечать картинкой\n/delreply-sm|tm - использовать так же, как и /creply, только delreply удаляет ответ", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал help в чате '{Chat_Name}'.");
                                    return;
                                case "/botinfo":
                                    await botClient.SendTextMessageAsync(message.Chat, "Подождите, собираю информацию о системе...", disableNotification: true);
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал botinfo в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(message.Chat.Id), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/chatid":
                                    Console.WriteLine($"[{Date}] '{Username}' вызвал chatid в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: message.Chat.Id.ToString(), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/cringe":
                                    string cringe = message.Text.Substring(comands[i].Length);
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
        private static string Creply_sm(string creply, string Id, string comand, BD_Comand bD_Comand, BD_Type bD_Type, BD_Mesege bD_Mesege, string? photoname)
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
                if (crepym.Length == 1) {
                    creplym = new string[2];
                    creplym[0] = crepym[0];
                    creplym[1] = "";
                }
                else if (crepym.Length == 2) {
                    creplym = new string[3];
                    creplym[0] = crepym[0];
                    creplym[1] = "";
                    creplym[2] = crepym[1];
                }
                creplym[0].ToLower();
                for (int i = 0; i < creplym.Length; i++) {
                    if (i != 1)
                        creplym[i] = creplym[i].Trim();
                    else
                        creplym[i] = photoname ?? "";
                }
            }
            if (bD_Comand == BD_Comand.Sreply)
                return TgbotDB.Read(Id, creplym[0], BD_Comand.Sreply);
            if (TgbotDB.AddCommand(Id, creplym, bD_Comand, bD_Type))
                return "";
            return "команда создана";
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
            var ts = TimeSpan.FromSeconds(UpTime);
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
