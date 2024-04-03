using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace tgbot
{ 
    static class TgbotCore
    {
        private static TelegramBotClient botClient;
        public static ulong   UpTime   { get; private set; }
        public static string? BotToken { get; private set; }
        public static string? BotName  { get; private set; }
        public static string BotVersion { get; } = "1.1.5.0";
        public static string Infosbork  { get; } = "final, dev, gold";

        public static void Start( string token, string name )
        {
            BotName = name;
            BotToken = token;
            botClient = new TelegramBotClient(BotToken);
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions { AllowedUpdates = {} };
            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Thread myThread = new Thread(Timer);
            myThread.Start();
        }

        public static void Stop()
        {
            BotName = null;
            BotToken = null;
            UpTime = 0;
        }

        private static void Timer()
        {
            while (true) {
                UpTime++;
                Thread.Sleep(1000);
            }
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient c, Update u, CancellationToken ct)
        {
            if (u.Type == UpdateType.Message)
                if (u.Message != null)
                    await Command(u.Message);
        }

        private static async Task HandleErrorAsync(ITelegramBotClient c, Exception e, CancellationToken ct)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }

        private static async Task Command(Message message)
        {
            string[] commands = new string[] { "/sreply", "/creply-sm", "/creply-tm", "/delreply-sm", "/delreply-tm", "/start", "/help", "/botinfo", "/chatid", "/cringe" };
            string? username = message.From?.FirstName;
            if (username == null)
                username = "кто-то";
            string? Chat_Name = message.Chat.Username;
            if (Chat_Name == null)
                Chat_Name = message.Chat.Id.ToString();
            if (message.Type == MessageType.Text)
                if (message.Text?.ToCharArray()[0] == '/')
                    await ForComands(message, commands, BD_Mesege.Text, username, Chat_Name);
                else
                    await ForBd(message, username, Chat_Name);
            else if (message.Type == MessageType.Photo)
                if (message.Caption?.ToCharArray()[0] == '/')
                    await ForComands(message, commands, BD_Mesege.Photo, username, Chat_Name);
        return;
        } // тута все команды

        private static async Task ForBd(Message message, string username, string Chat_Name)
        {
            var Date = DateTime.Now;
            if (message.Text != null) {
                string[] creplym = message.Text.ToLower().Split('\n');
                string messageret = TgbotDB.Read(message.Chat.Id.ToString(), creplym[0], BD_Comand.nul);
                if (messageret == "")
                    return;
                if (!messageret.EndsWith(".jpg")) {
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: messageret, replyToMessageId: message.MessageId);
                Console.WriteLine($"[{Date}] бот ответил '{username}' '{messageret}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                }
                else {
                await using Stream stream = System.IO.File.OpenRead($"photo_memory//{messageret}");
                await botClient.SendPhotoAsync(chatId: message.Chat, photo: new InputOnlineFile(content: stream, fileName: $"photo_memory//{messageret}"), replyToMessageId: message.MessageId);
                Console.WriteLine($"[{Date}] бот отправил фото '{username}' на сообщение '{message.Text}' в чате {Chat_Name}.");
                }
            }
        }

        public static async Task ForComands(Message message, string[] commands, BD_Mesege bD_Mesege, string username, string Chat_Name)
        {
            var Date = DateTime.Now;
            string creply = "";
            for (int i = 0; i < commands.Length; i++)
            {
                if (bD_Mesege == BD_Mesege.Text)
                    if (message.Text != null)
                        if (message.Text.ToLower().StartsWith(commands[i]))
                            switch (commands[i]) {
                                case "/sreply":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/sreply", BD_Comand.Sreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "") {
                                      await botClient.SendTextMessageAsync(message.Chat, "не найдено", disableNotification: true);
                                      return;
                                    }
                                    await botClient.SendTextMessageAsync(message.Chat, creply, disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}]'{username}' посмотрел ответы в чате '{Chat_Name}'");
                                    return;
                                case "/creply-sm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                      Console.WriteLine($"[{Date}] '{username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/creply-tm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                      Console.WriteLine($"[{Date}] '{username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-sm":
                                    creply = Creply_sm(message.Text, message.Chat.Id.ToString(), "/delreply-sm", BD_Comand.Delreply, BD_Type.Sm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/delreply-tm":
                                    creply = Creply_sm(message.Text.ToLower(), message.Chat.Id.ToString(), "/delreply-tm", BD_Comand.Delreply, BD_Type.Tm, BD_Mesege.Text, null);
                                    if (creply == "")
                                      await botClient.SendTextMessageAsync(message.Chat, "Ответ не удалён\nВозможно, он не существует", disableNotification: true, replyToMessageId: message.MessageId);
                                    else
                                    Console.WriteLine($"[{Date}] '{username}' удалил ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/start":
                                    await botClient.SendTextMessageAsync(message.Chat, "Это бот. НЕ ЧЕЛОВЕК. Чтобы понять нажмите \n/help", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{username}' вызвал start в чате '{Chat_Name}'.");
                                    return;
                                case "/help":
                                    await botClient.SendTextMessageAsync(message.Chat, "Список команд:\n/help - выводит это меню\n/cringe - поиск изображений через Яндекс\n/sreply [слово] - показать ответы\n/creply-sm|tm [слово на которое ответить]\n[чем ответить, обязательно через перенос строки (Ctrl+Enter на десктопе)]\nПример использования:\n /creply-sm ALO\nАЛО АЛО\nТак бот будет отвечать на сообщение, где только слово АЛО, /creply-tm испоьзовать так же, но так бот ищет АЛО во всех сообщениях, вне зависимости от того, написано ли в них что то ещё. Так же вместо ответа второй строкой вы можете прикрепить картинку, и бот будет отвечать картинкой\n/delreply-sm|tm - использовать так же, как и /creply, только delreply удаляет ответ", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{username}' вызвал help в чате '{Chat_Name}'.");
                                    return;
                                case "/botinfo":
                                    await botClient.SendTextMessageAsync(message.Chat, "Подождите, собираю информацию о системе...", disableNotification: true);
                                    Console.WriteLine($"[{Date}] '{username}' вызвал botinfo в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: SiseInfo(message.Chat.Id), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/chatid":
                                    Console.WriteLine($"[{Date}] '{username}' вызвал chatid в чате '{Chat_Name}'.");
                                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: message.Chat.Id.ToString(), disableNotification: true, replyToMessageId: message.MessageId);
                                    return;
                                case "/cringe":
                                    string cringe = message.Text.Substring(commands[i].Length);
                                    return;
                            }
                if (bD_Mesege == BD_Mesege.Photo)
                    if (message.Caption != null)
                        if (message.Caption.ToLower().StartsWith(commands[i]))
                            switch (commands[i])
                            {
                                case "/creply-sm":
                                    string filename1 = SavePhotoAsync(message).Result;
                                    string? a = Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-sm", BD_Comand.Creply, BD_Type.Sm, BD_Mesege.Photo, filename1);
                                    await botClient.SendTextMessageAsync(message.Chat, a ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{username}' создал ответ в чате '{Chat_Name}'.");
                                    return;
                                case "/creply-tm":
                                    string filename2 = SavePhotoAsync(message).Result;
                                    string? b = Creply_sm(message.Caption, message.Chat.Id.ToString(), "/creply-tm", BD_Comand.Creply, BD_Type.Tm, BD_Mesege.Photo, filename2);
                                    await botClient.SendTextMessageAsync(message.Chat, b ?? "Ответ не создан\nПроверьте правильность ввода", disableNotification: true, replyToMessageId: message.MessageId);
                                    Console.WriteLine($"[{Date}] '{username}' создал в чате '{Chat_Name}'.");
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
