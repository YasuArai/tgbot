using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot3
{
    class tgbot3
    {
        public static string machineName = Environment.MachineName;
        public static string versionNumber = "0.0.0.5-alpha";
        public static string buildInfoString = "<code>alpha, debug, non-release</code>";
        public static OperatingSystem osVersion = Environment.OSVersion;

        public async Task MainAsync()
        {
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
                updateHandler: HandleUpdateAsyn.HandleUpdateAsync,
        pollingErrorHandler: HandlePollingErrorAsyn.HandlePollingErrorAsync,
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

        }
    }
}

