using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace tgbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient("5561996874:AAH1NHrQLjiI5KTqda05AmYSGy__uUEG9Ho");

            var me = botClient.GetMeAsync();
            Console.WriteLine($"Бот запущен, id бота {me.Id}");
        }
    }
}
