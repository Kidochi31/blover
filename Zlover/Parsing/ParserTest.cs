using Blover.Debugging;
using Blover.Zlover.Parsing;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Parsing
{
    internal static class ParserTest
    {
        static Dictionary<string, (string, Action)> Commands = new Dictionary<string, (string, Action)> 
        {
            { "stmt", ("Test the zlover statement parser via repl", StmtReplTest) },
            { "stmt-file", ("Test the zlover statement parser via opening a file", StmtFileTest) },
            { "decl", ("Test the zlover declaration parser via repl", DeclReplTest) },
            { "decl-file", ("Test the zlover declaration parser via opening a file", DeclFileTest) },
        };

        public static void ReplTest()
        {
            string IntroText = "Welcome to the zlover parser repl!\nEnter where to start parsing, or 'help' for help.";
            Menu menu = new Menu(Commands, IntroText, true);
            menu.RunMenu();
        }

        public static (List<Stmt?>, Scanner, bool) ParseStatementAndPrintOnError(string text)
        {
            (List<Token>? tokens, Scanner scanner) = ScannerTest.ScanAndPrintOnError(text);
            if (tokens is null)
            {
                Console.WriteLine("\nCannot parse text.");
                Console.WriteLine("");
                return ([], scanner, false);
            }

            bool success = true;
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
                    success = false;
                }
                statements.Add(expression);
            }
            

            return (statements, scanner, success);
        }

        public static (List<Decl?>, Scanner, bool) ParseDeclarationAndPrintOnError(string text)
        {
            (List<Token>? tokens, Scanner scanner) = ScannerTest.ScanAndPrintOnError(text);
            if (tokens is null)
            {
                Console.WriteLine("\nCannot parse text.");
                Console.WriteLine("");
                return ([], scanner, false);
            }

            bool success = true;

            Parser parser = new Parser(scanner, tokens);
            List<Decl?> declarations = new();
            while (!parser.IsAtEnd())
            {
                Decl? expression = parser.ParseDeclaration();
                if (parser.Errors.Count > 0)
                {
                    Console.WriteLine("\nParsing errors:");
                    foreach (ParseError error in parser.Errors)
                    {
                        Console.WriteLine(error.VisualMessage(scanner.Lines));
                        success = false;
                    }
                    Console.WriteLine("");
                    
                }
                declarations.Add(expression);
            }
            

            return (declarations, scanner, success);
        }

        public static void StmtReplTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the zlover statement parser repl.");
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
                (List<Stmt?> statements, _, _) = ParseStatementAndPrintOnError(text);
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
            Console.WriteLine("Welcome to the zlover statement parser file tester.");
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
                    (List<Stmt?> statements, _, _) = ParseStatementAndPrintOnError(text);
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

        public static void DeclReplTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the zlover declaration parser repl.");
            Console.WriteLine("Enter in the text of one or more declarations to see parse result.");
            Console.WriteLine("Enter 'quit' to quit.");
            Console.WriteLine("Enter 'menu' to return to the menu.");

            while (true)
            {
                string? text = Repl.GetMultiLineUserInput();
                if (text is null)
                {
                    return;
                }
                (List<Decl?> statements, _, _) = ParseDeclarationAndPrintOnError(text);
                foreach(Decl? statement in statements){
                    if(statement is null)
                    {
                        Console.WriteLine("Declaration: [Invalid Statement]");
                    }
                    else
                    {
                        Console.WriteLine($"Declaration: {statement}");
                    }
                }
                Console.WriteLine("");
            }
        }

        public static void DeclFileTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the zlover declaration parser file tester.");
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
                    (List<Decl?> statements, _, _) = ParseDeclarationAndPrintOnError(text);
                    foreach(Decl? statement in statements){
                        if(statement is null)
                        {
                            Console.WriteLine("Declaration: [Invalid Statement]");
                        }
                        else
                        {
                            Console.WriteLine($"Declaration: {statement}");
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
