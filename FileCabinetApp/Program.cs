using System;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class for this app.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Object reference.
        /// </summary>
        public static IFileCabinetService fileCabinetService;

        /// <summary>
        /// A parameter that defines the work of an app.
        /// </summary>
        public static bool isRunning = true;

        private const string DeveloperName = "Alexander Gamezo";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        /// <summary>
        /// The main method for this app.
        /// </summary>
        /// <param name="args">Application arguments with validation-rules command line parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");

            var commandHandler = CreateCommandHandlers();

            CommandHandler.CommandLineParameter(args);

            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex].ToLower();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                }
                else
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                    commandHandler.Handle(new AppCommandRequest { Command = command, Parameters = parameters });
                }
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var commandHandler = new CommandHandler();
            return commandHandler;
        }
    }
}