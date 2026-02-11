
namespace Blover.Debugging{
    public class Menu
    {
        Dictionary<string, (string HelpText, Action Command)> Commands;
        bool IsSubMenu;
        string InvalidText;
        string IntroText;

        bool ReturnBack = false;

        public Menu(Dictionary<string, (string HelpText, Action Command)> commands, string introText, bool isSubMenu)
        {
            IntroText = introText;
            Commands = commands;
            IsSubMenu = isSubMenu;
            InvalidText = isSubMenu ? "Invalid command. Enter 'help' for help, 'menu' to go back, or 'quit' to quit."
                                    : "Invalid command. Enter 'help' for help or 'quit' to quit.";
            if (IsSubMenu)
            {
                Commands["menu"] = ("Return to the previous menu", () => {ReturnBack = true;});
            }
            Commands["quit"] = ("Quit the program", () => {Environment.Exit(0); });
            Commands["help"] = ("See a list of comands", Help);
        }

        public void RunMenu()
        {
            while (true)
            {
                Console.WriteLine(IntroText);
                while (true)
                {
                    Console.WriteLine($"");
                    Console.Write("Enter a command: ");
                    string? command = Console.ReadLine();
                    if (command is null || command == "")
                    {
                        continue;
                    }
                    if (Commands.ContainsKey(command))
                    {
                        Commands[command].Command();
                        if(command == "help"){
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    Console.WriteLine(InvalidText);
                }
                if (ReturnBack)
                {
                    ReturnBack = false;
                    return;
                }
            }
        }

        void Help()
        {
            Console.WriteLine($"");
            Console.WriteLine($"COMMANDS");
            Console.WriteLine($"~~~~~~~~");
            foreach ((string command, (string description, _)) in Commands)
            {
                Console.WriteLine($"{command}: {description}");
            }
            Console.WriteLine($"");
        }
    }
}