using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;

using tgbot;
using tgbot.props;

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
