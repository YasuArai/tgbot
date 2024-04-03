using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net;

using tgbot;

namespace TgBot_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string token   = TgbotProps.GetProps(0, "Token");
            string botname = TgbotProps.GetProps(1, "BotName");
            TgbotCore.Start(token, botname);
        }
    }
}
