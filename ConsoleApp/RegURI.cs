using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    static class RegURI
    {
        public const string UriScheme = "medialan";
        public const string FriendlyName = "Media Lan Indexer";

        public static void RegisterUriScheme()
        {
            Console.WriteLine("Registering URI Scheme ...");
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
            {
                string applicationLocation = typeof(RegURI).Assembly.Location;

                key.SetValue("", "URL:" + FriendlyName);
                key.SetValue("URL Protocol", "");

                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("", applicationLocation + ",1");
                }

                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Registering URI Scheme ... [Success]");
        }
    }
}
