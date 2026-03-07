using Blover.Debugging;

namespace Blover.Zlover.Scanning
{
    internal class ScannerTest
    {
        public static (List<Token>?, Scanner) ScanAndPrintOnError(string text)
        {
            Scanner scanner = new Scanner(text);
            List<Token> tokens = scanner.ScanTokens();
            List<ScanError> errors = scanner.Errors;

            if (errors.Count > 0)
            {
                Console.WriteLine("\nScanning errors:");
                foreach (ScanError error in errors)
                {
                    Console.WriteLine(error.VisualMessage(scanner.Lines));
                }
                Console.WriteLine("");
                return (null, scanner);
            }

            return (tokens, scanner);
        }

        public static void ReplTest()
        {
            Console.WriteLine("");
            Console.WriteLine("Welcome to the zlover scanner repl.");
            Console.WriteLine("Enter in text to see the tokens/errors.");
            Console.WriteLine("Press enter to create a new line, press enter twice to see result.");
            Console.WriteLine("Enter 'quit' to quit.");
            Console.WriteLine("Enter 'menu' to return to the menu.");

            while (true){
                string? text = Repl.GetMultiLineUserInput();
                if (text is null)
                {
                    return;
                }
                (List<Token>? tokens, _) = ScanAndPrintOnError(text);
                if(tokens is null)
                {
                    continue;
                }
                Console.WriteLine("\nNo scanning errors.");
                foreach (Token token in tokens)
                {
                    Console.WriteLine(token);
                }
            }
        }
    }
}
