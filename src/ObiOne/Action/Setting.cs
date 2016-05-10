using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class Setting
    {
        public static string SQLITE_PATH { get { return Startup.Configuration["AppPath:SqlitePath"]; } }
        public static string PICT_PATH { get { return Startup.Configuration["AppPath:PictPath"]; } }
        public static string PICT_TEMP_PATH { get { return Startup.Configuration["AppPath:PictTempPath"]; } }

    }
}
