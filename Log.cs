using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge
{
    class Log
    {
        public static void WriteLog(string directory, string filename, string message, bool append = true)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string previouslogs = "";

            StreamWriter writer = new StreamWriter(directory + "/" + filename + ".txt", append);
            writer.Write(previouslogs);
            writer.WriteLine(message);

            writer.Flush();
            writer.Dispose();

        }

        public static void ToErrorLog(string message)
        {
            WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Log",
                        DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + "ERROR " + message, true);
        }

        public static void ToLog(string message)
        {
            WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Log",
                        DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + message, true);
        }

        public static void BlankLine()
        {
            WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Log",
                        DateTime.Now.ToString("yyyy-MM-dd"), "\r\n", true);
        }
    }
}
