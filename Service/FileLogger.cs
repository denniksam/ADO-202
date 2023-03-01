using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Service
{
    internal class FileLogger : ILogger
    {
        private readonly String filename;
        public FileLogger()
        {
            filename = "logs.txt";
        }
        public void Log(string message, string level)
        {
            this.Log(message, level, "", "");
        }

        public void Log(string message, string level, string className, string methodName)
        {
            File.AppendAllText(filename,
                $"{DateTime.Now} | {level} | {message} | {className}.{methodName}\r\n");
        }
    }
}
