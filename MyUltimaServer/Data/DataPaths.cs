using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using MyUltimaServer.Accounting;

namespace MyUltimaServer.Data
{
    public static class DataPaths
    {
        private static readonly string AccountDataLocation = @"\Save\AccountData.xml";
        private static readonly string ServerSettingsLocation = @"\Save\ServerSettings.xml";
        public static string AccountDataFullPath
        {
            get { return Environment.CurrentDirectory + AccountDataLocation; }
        }
        public static string ServerSettingsFullPath
        {
            get { return Environment.CurrentDirectory + ServerSettingsLocation; }
        }
    }
}
