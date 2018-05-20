using System;

namespace LoxSharp.Error
{
    class LoxError
    {
        /* TODO: This should really be abstracted out into an abstraction/interface that gets injected, to allow error reporting to other destinations than STDERR (like a linter or whatever). */
        /* That also means this should not be static forever :| */

        // this is a temporary hack. pls fix
        public static bool HadError = false;

        public static void ThrowError(int line, String message) => PrintError(line, "", message);

        public static void ThrowError(String message) => PrintError("", message);

        private static void PrintError(int line, string where, string message)
        {
            Console.Error.WriteLine($"[ERROR] - [line {line}]{where}: {message}");
            HadError = true;
        }

        private static void PrintError(string where, string message)
        {
            Console.Error.WriteLine($"[ERROR]{where}: {message}");
            HadError = true;
        }
    }
}
