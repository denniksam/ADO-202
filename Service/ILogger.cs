using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Service
{
    internal interface ILogger
    {
        void Log(String message, String level);
        void Log(String message, String level, String className, String methodName);
    }
}
