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
            new ALO_bot(ALO_Props.Get_props(0, "Token"), ALO_Props.Get_props(1, "BotName"));
        }
    }
}
