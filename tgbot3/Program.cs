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
    class Tgbot3
    {
        public string Token { get; private set; }

        static void Main()
        {
            ALO_bot bot = new ALO_bot(Token);
        }
    }
    class ALO_bot
    {
        public ALO_bot(string Token)
        {

        }
    }
}

