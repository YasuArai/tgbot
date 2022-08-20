using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace tgbot3
{
    internal class HandleUpdateAsyn : tgbot3
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");


            switch (messageText)
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
    }
}
