using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class Log
    {
        

        public static void OutputExceptionLog(Exception e)
        {
            string dirPath = Startup.Configuration["AppPath:ExceptionPath"];
            string fileName = "ExceptionLog_"+DateTime.Now.ToString("yyyyMMdd")+".txt";
            string fullPath = Path.Combine(dirPath, fileName);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true))
            {
                string output = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + e.Message +
                                "\n <<StackTrace>> : " + e.StackTrace +
                                "\n <<InnerException>> : " + e.InnerException +
                                "\n <<HelpLink>> : " + e.HelpLink+
                                "\n <<TargetSite>> : " + e.TargetSite;
                sw.Write(output);
            }

        }
    }
}
