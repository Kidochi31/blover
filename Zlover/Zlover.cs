using Blover.Debugging;
using Blover.Zlover.Parsing;
using Blover.Zlover.Scanning;

namespace Blover.Zlover
{
    internal static class ZloverMenu
    {
        static Dictionary<string, (string, Action)> Commands = new Dictionary<string, (string, Action)> 
        {
            { "scanner", ("Test the Zlover scanner via repl", ScannerTest.ReplTest) },
            { "parser", ("Test the Zlover parser via repl", ParserTest.ReplTest) },
        };

        public static void Run()
        {
            string IntroText = "Welcome to the Zlover Compiler command line tester!\n© 2026 Kidochi";

            Menu menu = new Menu(Commands, IntroText, true);
            menu.RunMenu();
        }
    }
}