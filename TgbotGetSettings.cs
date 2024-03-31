namespace tgbot
{
    internal class ALO_Props
    {
        public static string Get_props(int i, string lastname)
        {
          char[] chars = lastname.ToCharArray();
          string[] file = System.IO.File.ReadAllLines("Properties\\set.txt");
          string props = file[i];
          props = props.TrimStart(chars);
          props = props.Trim(new char[] { ' ', ':', ' ', '[', ']' });
          return props;
        }
    }
}
