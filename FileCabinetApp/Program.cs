using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class for this app.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Alexander Gamezo";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a record", "The 'create' creates a record." },
            new string[] { "list", "returns a list of records", "The 'list' returns a list of records." },
            new string[] { "edit", "edits a record", "The 'edit' edits a record." },
            new string[] { "find", "finds and returns a list of records", "The 'find' finds and returns a list of records. Parameters: 'firstname', 'lastname', 'dateofbirth'." },
        };

        private static FileCabinetService fileCabinetService;

        private static bool isRunning = true;

        /// <summary>
        /// The main method for this app.
        /// </summary>
        /// <param name="args">Application arguments with validation-rules command line parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");

            CommandLineParameter(args);

            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            try
            {
                CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                Console.WriteLine($"Record #{fileCabinetService.CreateRecord(paramobj)} is created.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static void List(string parameters)
        {
            var arr = fileCabinetService.GetRecords();
            Show(arr);
        }

        private static void Edit(string parameters)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(parameters) && int.TryParse(parameters, out int enteredId))
            {
                id = enteredId;
            }

            var arr = fileCabinetService.GetRecords();
            if (id > 0 && id <= arr.Length)
            {
                try
                {
                    CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                    ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                    fileCabinetService.EditRecord(id, paramobj);
                    Console.WriteLine($"Record #{id} is updated.");
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
            else
            {
                Console.WriteLine($"#{parameters} record is not found.");
            }
        }

        private static void Find(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2);
            const int commandIndex = 0;
            const int parameterIndex = 1;

            string[] originalParameters = { "firstname", "lastname", "dateofbirth" };

            string command = inputs[commandIndex];
            bool checkCommand = originalParameters[0].Equals(command, StringComparison.InvariantCultureIgnoreCase) ||
                                originalParameters[1].Equals(command, StringComparison.InvariantCultureIgnoreCase) ||
                                originalParameters[2].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            if (!checkCommand)
            {
                Console.WriteLine("Check your command.");
            }

            string parameter = string.Empty;
            try
            {
                parameter = inputs[parameterIndex].Trim('"');
            }
            catch (IndexOutOfRangeException exc)
            {
                Console.WriteLine("Add your parameter(s). {0}", exc.Message);
            }

            bool checkParameter = checkCommand && !string.IsNullOrEmpty(parameter) && !string.IsNullOrWhiteSpace(parameter);
            if (checkParameter)
            {
                FindAppropriateMethod(originalParameters, command, parameter);
            }
        }

        private static void FindAppropriateMethod(string[] originalParameters, string command, string parameter)
        {
            bool checkParameterFirstName = originalParameters[0].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterLastName = originalParameters[1].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterDateOfBirth = originalParameters[2].Equals(command, StringComparison.InvariantCultureIgnoreCase);

            if (checkParameterFirstName)
            {
                FileCabinetRecord[] arr = fileCabinetService.FindByFirstName(parameter);
                if (arr.Length == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    Show(arr);
                }
            }

            if (checkParameterLastName)
            {
                FileCabinetRecord[] arr = fileCabinetService.FindByLastName(parameter);
                if (arr.Length == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    Show(arr);
                }
            }

            if (checkParameterDateOfBirth)
            {
                try
                {
                    FileCabinetRecord[] arr = fileCabinetService.FindByDateOfBirth(parameter.Trim('"'));
                    if (arr.Length == 0)
                    {
                        Console.WriteLine("No results.");
                    }
                    else
                    {
                        Show(arr);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"The '{originalParameters[2]}' parameter has the wrong format.");
                }
            }
        }

        private static void CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3)
        {
            Console.Write("First name: ");
            firstName = Console.ReadLine();

            Console.Write("Last name: ");
            lastName = Console.ReadLine();

            Console.Write("Date of birth: ");
            dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

            Console.Write("Property1 <short>: ");
            property1 = short.Parse(Console.ReadLine());

            Console.Write("Property2 <decimal>: ");
            property2 = decimal.Parse(Console.ReadLine());

            Console.Write("Property3 <char>: ");
            property3 = char.Parse(Console.ReadLine());
        }

        private static void CommandLineParameter(string[] args)
        {
            if (args.Length == 0 ||
                (args.Length == 1 && args[0].ToLowerInvariant().Equals("--validation-rules=default")) ||
                (args.Length == 2 && (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v default")))
            {
                Console.WriteLine("Using default validation rules.");
                fileCabinetService = new FileCabinetDefaultService();
            }
            else if ((args.Length == 1 && args[0].ToLowerInvariant().Equals("--validation-rules=custom")) ||
                     (args.Length == 2 && (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v custom")))
            {
                Console.WriteLine("Using custom validation rules.");
                fileCabinetService = (FileCabinetService)new FileCabinetCustomService();
            }
            else
            {
                Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                Console.WriteLine("Using default validation rules.");
                fileCabinetService = new FileCabinetDefaultService();
            }
        }

        private static void Show(FileCabinetRecord[] arr)
        {
            foreach (var a in arr)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }
    }
}