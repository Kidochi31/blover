global using System;
using Blover.Debugging;
using Blover.Parsing;
using Blover.Scanning;

namespace Blover
{
    internal class Blover
    {
        static Dictionary<string, (string, Action)> Commands = new Dictionary<string, (string, Action)> 
        {
            { "scanner", ("Test the scanner via repl", ScannerTest.ReplTest) },
            { "parser", ("Test the parser via repl", ParserTest.ReplTest) }
        };

        public static void Main(string[] args)
        {
            string IntroText = "Welcome to the Blover Compiler command line tester!\n© 2026 Kidochi";

            Menu menu = new Menu(Commands, IntroText, false);
            menu.RunMenu();
        }
    }
}