using System;
using System.IO;

namespace SkyDrive.Client
{
    public class LogHelper
    {
        public static void WriteLog(string matter, string message, string stack)
        {
            DateTime now = DateTime.Now;

            string fileName = string.Format("{0}Logs\\{1}.txt", AppDomain.CurrentDomain.BaseDirectory, now.ToString("yyyyMMdd"));

            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine("==============================================================");
                sw.WriteLine("时间：" + now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine("事件：" + matter);
                sw.WriteLine("内容：" + message);
                sw.WriteLine("追踪：" + stack);

                sw.Flush();
                sw.Close();
            }
        }
    }
}
