using System;
using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot1
{

    class ALO_Props
    {
        public string Token { get; private set; }

        public ALO_Props()
        {
            Token = Get_props();
        }

        private void Create_props()
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
                sw.WriteLine("Token : [ctrl+v Token]");
                sw.Close();
                Console.WriteLine("Текстовый документ был создан/перезаписан\nПроверте путь Properties\\Props.txt");
                Console.ReadLine();
            }
        }
        private void Replase_Props()
        {
            System.IO.File.Delete("Properties\\Props.txt");
            Create_props();
        }

        private string Get_props()
        {
            try
            {
                StreamReader streamReader = new StreamReader("Properties\\Props.txt");
                string token = streamReader.ReadToEnd();
                token = token.Trim();
                token = token.TrimStart('T', 'o', 'k', 'e', 'n', ' ', ':', ' ');
                token = token.Trim(new Char[] { ' ', '[', ']' });
                
                if (token.Length == 45)
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
                Create_props();
                throw;
            }
        }
    }

    class Tgbot
    {
        static void Main(string[] args)
        {
            ALO_bot bot = new ALO_bot(new ALO_Props().Token);
        }
    }

    class ALO_bot
    {
        public ALO_bot(string Token)
        {
            Console.WriteLine(Token);
            Console.ReadLine();
        }
    }
}

