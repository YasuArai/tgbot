using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
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
            StreamReader streamReader = new StreamReader("Properties\\Props.txt");
            try
            {
                string[] file = System.IO.File.ReadAllLines("Properties\\Props.txt");
                string token = streamReader.ReadToEnd();
                token = token.Trim();
                token = token.Remove(file[0].Length, file[1].Length + 1);
                token = token.TrimStart('T', 'o', 'k', 'e', 'n', ' ', ':', ' ');
                token = token.Trim(new Char[] { ' ', '[', ']' });

                if (!(token.Length == 46))
                {
                    Console.WriteLine("Не коректные данные");
                    streamReader.Close();
                    Replase_Props();
                }
                streamReader.Close();
                return token;
            }
            catch (Exception)
            {
                Console.WriteLine("Не коректные данные");
                streamReader.Close();
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
            StreamReader streamReader = new StreamReader("Properties\\Props.txt");
            try
            {
                string[] file = System.IO.File.ReadAllLines("Properties\\Props.txt");
                string name = streamReader.ReadToEnd();
                name = name.Trim();
                name = name.Remove(0, file[0].Length + 1);
                name = name.TrimStart('B', 'o', 't', 'N', 'a', 'm', 'e', ':', ' ');
                name = name.Trim(new Char[] { ' ', '[', ']' });

                streamReader.Close();
                return name;
            }
            catch (Exception)
            {
                Console.WriteLine("Не коректные данные");
                streamReader.Close();
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
                Directory.CreateDirectory("Properties");
            }
            if (!System.IO.File.Exists("Properties\\Props.txt"))
            {
                var file = System.IO.File.CreateText("Properties\\Props.txt");
                file.Close();
                StreamWriter sw = new StreamWriter("Properties\\Props.txt");
                sw.WriteLine("Token : [ctrl+v Token]\nBotName : [BotName]");
                sw.Close();
                Console.WriteLine("Текстовый документ был создан/перезаписан\nПроверте путь Properties\\Props.txt");
                Console.ReadLine();
            }
        }
        protected void Replase_Props()
        {
            if (!System.IO.File.Exists("Properties\\Props.txt"))
            {
                return;
            }
            System.IO.File.Delete("Properties\\Props.txt");
            Create_props();
        }

        public abstract string Get_props();
    }

    class Tgbot
    {
        static void Main(string[] args)
        {
            new ALO_bot(new Alo_token().Get_props(), new Alo_BotName().Get_props());
        }
    }

    class ALO_bot
    {
        static TelegramBotClient botClient;
        public static string BotToken { get; private set; }
        public static string BotName { get; private set; }
        public static string BotVersion { get; private set; }
        public static string Infosbork { get; private set; }

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
        }

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
                    await botClient.SendTextMessageAsync(message.Chat, SiseInfo());
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }// не трож, оно тебя сожрёт!!!

        private static string SiseInfo()
        {
            return $"{BotToken}\n{BotName}";
        }
    }
}

