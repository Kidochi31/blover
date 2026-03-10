using Blover.Debugging;
using Blover.Zlover.Parsing;
using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal static class ResolverTest
    {
        static Dictionary<string, (string, Action)> Commands = new Dictionary<string, (string, Action)> 
        {
            { "resolve", ("Test the zlover resolver via repl", DeclReplTest) },
            { "resolve-file", ("Test the zlover resolver via opening a file", DeclFileTest) },
        };

        public static void ReplTest()
        {
            string IntroText = "Welcome to the zlover resolver repl!\nEnter where to start parsing, or 'help' for help.";
            Menu menu = new Menu(Commands, IntroText, true);
            menu.RunMenu();
        }


        public static (List<Decl?>, Scanner, bool) ResolveDeclarationAndPrintOnError(string text)
        {
            (List<Parsing.Decl?> tokens, Scanner scanner, bool success) = ParserTest.ParseDeclarationAndPrintOnError(text);
            if (!success)
            {
                Console.WriteLine("\nCannot parse text.");
                Console.WriteLine("");
                return ([], scanner, false);
            }

            success = true;
            Resolver resolver = new Resolver();
            Environment env = new Environment();
            VerifyFunEnvironment verifyEnv = new VerifyFunEnvironment();
            List<Decl?> decls = new();
            foreach(Parsing.Decl? decl in tokens)
            {
                if(decl is not null)
                {
                    try {
                        decls.Add(resolver.ResolveDeclaration(decl, env, verifyEnv));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        success = false;
                    }
                }
            }

            return (decls, scanner, success);
        }

        public static void DeclReplTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the blover declaration parser repl.");
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
                (List<Decl?> statements, _, _) = ResolveDeclarationAndPrintOnError(text);
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
            Console.WriteLine("Welcome to the blover declaration parser file tester.");
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
                    (List<Decl?> statements, _, _) = ResolveDeclarationAndPrintOnError(text);
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
