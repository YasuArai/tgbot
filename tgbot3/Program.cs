using System;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


Console.WriteLine("Говнобот by CyanRed");
Console.WriteLine("если вы это читаете, то namespaces инициализированы");
//Console.WriteLine($"ключи командной строки {cliArgs}");
string cliArgs;
//try
//{
//    cliArgs = Convert.ToString(args[0]);
    
//}
//catch (Exception)
//{
//    Console.WriteLine("Ключи командной строки не обнаружены или написаны неправильно");
//};


Console.WriteLine("инициализация переменных и счётчиков производительности хост-машины");
string machineName = Environment.MachineName;
string versionNumber = "0.0.0.3-alpha";
string buildInfoString = "<code>alpha, debug, non-release</code>";
var osVersion = Environment.OSVersion;
//if(cliArgs == "/verbose")
//{
 //   Console.WriteLine("Режим отладки включен");
//};
Console.WriteLine("Инициализация базы данных");
var connection = new SqliteConnection("Data Source=usersdata.db");
Console.WriteLine("База данных инициализирована.");
Console.WriteLine("Инициализация токена");
string botToken = "[УДАЛЕНО]";
Console.WriteLine("переменная токена инициализирована");
var botClient = new TelegramBotClient(botToken);
Console.WriteLine("токен инициализирован");
//if (cliArgs == "/verbose")
//{
//    Console.WriteLine("Токен бота: " + botToken);
//};
Console.WriteLine("Подключение базы данных...");
connection.Open();
Console.WriteLine("База данных подключена.");
Console.WriteLine("Подключение к сети, и запуск бота...");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();


Console.WriteLine($"Бот запущен, и работает");
//if (cliArgs == "/verbose")
//{
//    Console.WriteLine($"Подключен как @{me.Username} с id {me.Id} на хосте {machineName} c {osVersion}");
//};
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

   
    switch(messageText)
    {
        case "/сисинфо":
            botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "<b>Собираю информацию о системе...</b>",
            parseMode: ParseMode.Html,
            disableNotification: true,
            replyToMessageId: update.Message.MessageId,
            cancellationToken: cancellationToken);
           // if (cliArgs == "/verbose")
           // {
           //     Message sentMessage = await botClient.SendTextMessageAsync(
            //chatId: chatId,
           // text: $"Информация о боте\nБот без названия\nИмя хоста: {machineName}\nВерсия: {versionNumber}\nВерсия ОС: {osVersion}\nИнформация о сборке:\n{buildInfoString}\n \n<b>Shh...\nLet's not leak my hard work</b>\n \n<b>Режим verbose включён!</b>",
           // parseMode: ParseMode.Html,
           // disableNotification: true,
           // cancellationToken: cancellationToken);
            //    break;
           // };
            
           
                Message sentMessage1 = await botClient.SendTextMessageAsync(
                chatId: chatId,
                 text: $"Информация о боте\nБот без названия\nИмя хоста: {machineName}\nВерсия: {versionNumber}\nВерсия ОС: {osVersion}\nИнформация о сборке:\n{buildInfoString}\n \n<b>Shh...\nLet's not leak my hard work</b>\n",
                parseMode: ParseMode.Html,
                disableNotification: true,
                cancellationToken: cancellationToken);
                break;
            
        default:
            // Echo received message text
            sentMessage1 = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "You said:\n" + messageText,
            cancellationToken: cancellationToken);
            break;
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

