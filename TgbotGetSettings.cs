namespace tgbot.props
{
    internal class ALO_Props
    {
        protected static void Create_props()
        {
            if (!Directory.Exists("Properties"))
                Directory.CreateDirectory("Properties"); // создаёт папку если таковой нет
            if (!System.IO.File.Exists("Properties\\set.txt"))
            {
                var file = System.IO.File.CreateText("Properties\\set.txt");
                file.Close();
                StreamWriter sw = new StreamWriter("Properties\\set.txt");
                sw.WriteLine("Token : [ctrl+v Token]\nBotName : [BotName]\nCringeRand : [50]"); // что будет в документе
                Console.WriteLine("Текстовый документ был создан/перезаписан\nПроверте путь Properties\\set.txt");
                sw.Close();
                Console.ReadLine();
            }
        } // создаёт документ

        protected static void Replase_Props()
        {
            if (!System.IO.File.Exists("Properties\\set.txt"))
                return;
            System.IO.File.Delete("Properties\\set.txt");
            Create_props();
        } // перезаписывает

        public static string Get_props(int i, string lastname)
        {
            try
            {
                char[] chars = lastname.ToCharArray();
                string[] file = System.IO.File.ReadAllLines("Properties\\set.txt");
                string props = file[i];
                props = props.TrimStart(chars); // удаляет 1 слово
                props = props.Trim(new char[] { ' ', ':', ' ', '[', ']' }); // защита от дибила
                if (lastname == "Token" && !(props.Length == 46)) // мало букв
                {
                    Console.WriteLine("Не коректные данные");
                    Replase_Props();
                }
                return props;
            }
            catch (Exception)
            {
                Exception();
                throw;
            }
        }

        protected static void Exception()
        {
            Console.WriteLine("Не коректные данные");
            Replase_Props();
            Create_props();
        }
    } // класс получения настроек
}
