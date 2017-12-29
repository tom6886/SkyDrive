using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SkyDrive.Client
{
    public class StaticMenus
    {
        private static Dictionary<string, string> list = new Dictionary<string, string>();

        public Dictionary<string, string> List
        {
            get { return list; }
            set { list = value; }
        }

        static StaticMenus _instance;

        public static StaticMenus GetInstance()
        {
            if (_instance == null)
            {
                _instance = new StaticMenus();

                XDocument xml = XDocument.Load("Urls.xml");

                list = xml.Root.Elements("url").ToDictionary(k => k.Element("key").Value, k => k.Element("value").Value);
            }

            return _instance;
        }
    }
}
