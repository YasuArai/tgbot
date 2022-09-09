using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot1
{
    class Tgbot
    {
        public static string Token { get; private set; }

        private static string Get_props()
        {
            StreamReader streamReader = new StreamReader("Properties\\Props.txt");
            string token = streamReader.ReadToEnd();
            string Trimtoken = token.TrimStart('T','o','k','e','n',' ',':',' '); 
            streamReader.Close();
            return Trimtoken;
        }
        static void Main(string[] args)
        {
            Token = Get_props();
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

