using System;
using System.Diagnostics;
using System.Text;

namespace Utilities
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Write(message);
        }

        public static void Log(Exception e)
        {
            var builder = new StringBuilder();
            builder.Append("Exception: ");
            builder.AppendLine(e.Message);
            builder.AppendLine(e.StackTrace);
            Write(builder.ToString());
        }

        public static void Log(object o)
        {
            Write(o.ToString());
        }

        private static void Write(string s)
        {
//            if (Debugger.IsAttached) Debug.WriteLine(s);

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " - " + s + "\n");
            
        }

    }
}
