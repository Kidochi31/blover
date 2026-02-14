using Blover.Debugging;
using Blover.Parsing;
using Blover.Scanning;

namespace Blover.Parsing
{
    internal static class ParserTest
    {
        static Dictionary<string, (string, Action)> Commands = new Dictionary<string, (string, Action)> 
        {
            { "stmt", ("Test the statement parser via repl", StmtReplTest) },
            { "stmt-file", ("Test the statement parser via opening a file", StmtFileTest) },
        };

        public static void ReplTest()
        {
            string IntroText = "Welcome to the parser repl!\nEnter where to start parsing, or 'help' for help.";
            Menu menu = new Menu(Commands, IntroText, true);
            menu.RunMenu();
        }

        public static (List<Stmt?>, Scanner) ParseStatementAndPrintOnError(string text)
        {
            (List<Token>? tokens, Scanner scanner) = ScannerTest.ScanAndPrintOnError(text);
            if (tokens is null)
            {
                Console.WriteLine("\nCannot parse text.");
                Console.WriteLine("");
                return ([], scanner);
            }

            Parser parser = new Parser(scanner, tokens);
            List<Stmt?> statements = new();
            while (!parser.IsAtEnd())
            {
                Stmt? expression = parser.ParseStatement();
                if (expression is null || parser.Errors.Count > 0)
                {
                    Console.WriteLine("\nParsing errors:");
                    foreach (ParseError error in parser.Errors)
                    {
                        Console.WriteLine(error.VisualMessage(scanner.Lines));
                    }
                    Console.WriteLine("");
                }
                statements.Add(expression);
            }
            

            return (statements, scanner);
        }

        public static void StmtReplTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the statement parser repl.");
            Console.WriteLine("Enter in the text of one or more statements to see parse result.");
            Console.WriteLine("Enter 'quit' to quit.");
            Console.WriteLine("Enter 'menu' to return to the menu.");

            while (true)
            {
                string? text = Repl.GetMultiLineUserInput();
                if (text is null)
                {
                    return;
                }
                (List<Stmt?> statements, _) = ParseStatementAndPrintOnError(text);
                foreach(Stmt? statement in statements){
                    if(statement is null)
                    {
                        Console.WriteLine("Statement: [Invalid Statement]");
                    }
                    else
                    {
                        Console.WriteLine($"Statement: {statement}");
                    }
                }
                Console.WriteLine("");
            }
        }

        public static void StmtFileTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the statement parser file tester.");
            Console.WriteLine("Enter in a file to load and parse.");
            Console.WriteLine("Enter 'quit' to quit.");
            Console.WriteLine("Enter 'menu' to return to the menu.");

            while (true)
            {
                string? file = Repl.GetSingleLineUserInput();
                if (file is null)
                {
                    return;
                }
                try
                {
                    string text = File.ReadAllText(file);
                    (List<Stmt?> statements, _) = ParseStatementAndPrintOnError(text);
                    foreach(Stmt? statement in statements){
                        if(statement is null)
                        {
                            Console.WriteLine("Statement: [Invalid Statement]");
                        }
                        else
                        {
                            Console.WriteLine($"Statement: {statement}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("");
                
                
                
            }
        }
    }
}
