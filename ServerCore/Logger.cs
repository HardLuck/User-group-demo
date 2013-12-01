using System;
using System.Runtime.CompilerServices;

namespace ServerCore
{
    public static class Logger
    {
        public static void PrintException(Exception ex, [CallerMemberName]string memberName = "")
        {
            Console.WriteLine("{0}: \n '{1}' \n '{2}'", memberName, ex.Message, ex.StackTrace);
        }
    }
}
