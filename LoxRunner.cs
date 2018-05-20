using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LoxSharp.Lexing;
using LoxSharp.Tokens;
using LoxSharp.Error;

namespace LoxSharp
{
    class LoxRunner
    {
        private static bool _exitPrompt = false;
        private static int _exitCode = 0;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: loxsharp [script]");
            }
            else if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    RunFile(args[0]);
                }
                else
                {
                    LoxError.ThrowError("[ERROR] The specified script could not be found.");
                    _exitCode = 1;
                }
            }
            else
            {
                RunPrompt();
            }

            Environment.Exit(_exitCode);
        }

        private static void RunFile(string scriptPath)
        {
            byte[] scriptContent = File.ReadAllBytes(scriptPath);
            Run(System.Text.Encoding.Default.GetString(scriptContent));

            if (LoxError.HadError)
                _exitCode = 1;
        }

        private static void RunPrompt()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(HaltPrompt);

            while (!_exitPrompt)
            {
                LoxError.HadError = false;
                Console.Write("> ");
                Run(Console.ReadLine());
            }
        }

        private static void HaltPrompt(object sender, ConsoleCancelEventArgs e)
        {
            _exitPrompt = true;
            _exitCode = 1;
        }

        private static void Run(string scriptSource)
        {
            Lexer l = new Lexer(scriptSource);
            List<Token> tokens = l.ScanTokens();

            foreach (Token t in tokens)
            {
                Console.WriteLine(t);
            }
        }
    }
}
