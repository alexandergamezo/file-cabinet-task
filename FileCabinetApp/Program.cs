using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

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
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export", "exports service data into a file in the 'CSV' or 'XML' format", "The 'export' exports service data into a file in the 'CSV' or 'XML' format. Formats: 'csv', 'xml'." },
        };

        private static FileCabinetService fileCabinetService;

        private static string commandLineParameter = string.Empty;

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

            var onlyCollection = fileCabinetService.GetRecords();
            if (id > 0 && id <= onlyCollection.Count)
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

        private static void Export(string parameters)
        {
            string paramFormat;
            string paramPath = string.Empty;
            try
            {
                CheckInputParameters(out string f, out string p);
                paramFormat = f;
                paramPath = p;

                if (!File.Exists(paramPath))
                {
                    FindAppropriateFunc();
                }
                else
                {
                    Console.Write($"File is exist - rewrite {paramPath}? [Y/n] ");
                    string answer = Console.ReadLine();
                    if (answer.ToLowerInvariant() == "y")
                    {
                        FindAppropriateFunc();
                    }
                    else if (answer.ToLowerInvariant() == "n")
                    {
                        Console.WriteLine($"Export failed: file {paramPath} was not rewrited.");
                    }
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                Console.WriteLine($"Export failed: can't open file {paramPath}. {exc}.");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc}.");
            }

            void FindAppropriateFunc()
            {
                if (paramFormat == "csv")
                {
                    SaveToCsvFormat(paramPath);
                }

                if (paramFormat == "xml")
                {
                    SaveToXmlFormat(paramPath);
                }
            }

            void SaveToCsvFormat(string path)
            {
                StreamWriter writer = new (path);
                fileCabinetService.MakeSnapshot().SaveToCsv(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }

            void SaveToXmlFormat(string path)
            {
                StreamWriter writer = new (path);
                fileCabinetService.MakeSnapshot().SaveToXmL(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }

            void CheckInputParameters(out string format, out string path)
            {
                string[] inputs = parameters.Split(' ', 2);
                const int recordFormat = 0;
                const int fileName = 1;

                string[] originalParameters = { "csv", "xml" };

                format = inputs[recordFormat].ToLowerInvariant();
                bool checkFormat = originalParameters[0].Equals(format, StringComparison.InvariantCultureIgnoreCase) ||
                                   originalParameters[1].Equals(format, StringComparison.InvariantCultureIgnoreCase);
                if (!checkFormat)
                {
                    throw new ArgumentOutOfRangeException(nameof(format), "Check your format.");
                }

                path = string.Empty;
                try
                {
                    path = inputs[fileName];
                }
                catch (IndexOutOfRangeException exc)
                {
                    Console.WriteLine("Add your filename(path). {0}", exc.Message);
                }
            }
        }

        private static void FindAppropriateMethod(string[] originalParameters, string command, string parameter)
        {
            bool checkParameterFirstName = originalParameters[0].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterLastName = originalParameters[1].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterDateOfBirth = originalParameters[2].Equals(command, StringComparison.InvariantCultureIgnoreCase);

            if (checkParameterFirstName)
            {
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = fileCabinetService.FindByFirstName(parameter);
                if (onlyCollection.Count == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    Show(onlyCollection);
                }
            }

            if (checkParameterLastName)
            {
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = fileCabinetService.FindByLastName(parameter);
                if (onlyCollection.Count == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    Show(onlyCollection);
                }
            }

            if (checkParameterDateOfBirth)
            {
                try
                {
                    ReadOnlyCollection<FileCabinetRecord> onlyCollection = fileCabinetService.FindByDateOfBirth(parameter.Trim('"'));
                    if (onlyCollection.Count == 0)
                    {
                        Console.WriteLine("No results.");
                    }
                    else
                    {
                        Show(onlyCollection);
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
            Tuple<bool, string, string> StringConverter(string str)
            {
                bool process = true;
                string appropriateStr = str;
                foreach (var a in str)
                {
                    if (!char.IsLetter(a))
                    {
                        process = false;
                        break;
                    }
                }

                var result = new Tuple<bool, string, string>(process, str, appropriateStr);
                return result;
            }

            Tuple<bool, string, DateTime> DateConverter(string str)
            {
                bool process = DateTime.TryParse(str, out DateTime appropriateDate);
                var result = new Tuple<bool, string, DateTime>(process, str, appropriateDate);
                return result;
            }

            Tuple<bool, string, short> ShortConverter(string str)
            {
                bool process = short.TryParse(str, out short appropriateShort);
                var result = new Tuple<bool, string, short>(process, str, appropriateShort);
                return result;
            }

            Tuple<bool, string, decimal> DecimalConverter(string str)
            {
                bool process = decimal.TryParse(str, out decimal appropriateDate);
                var result = new Tuple<bool, string, decimal>(process, str, appropriateDate);
                return result;
            }

            Tuple<bool, string, char> CharConverter(string str)
            {
                bool process = true;
                char appropriateChar;
                if (str.Length != 1 && !char.IsLetter(str[0]))
                {
                    process = false;
                    appropriateChar = ' ';
                }
                else
                {
                    appropriateChar = str[0];
                }

                var result = new Tuple<bool, string, char>(process, str, appropriateChar);
                return result;
            }

            Func<string, Tuple<bool, string, string>> stringConverter = StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateConverter = DateConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = DecimalConverter;
            Func<string, Tuple<bool, string, char>> charConverter = CharConverter;

            Func<string, Tuple<bool, string>> firstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator;
            Func<short, Tuple<bool, string>> shortValidator;
            Func<decimal, Tuple<bool, string>> decimalValidator;
            Func<char, Tuple<bool, string>> charValidator;

            if (commandLineParameter == "custom")
            {
                Tuple<bool, string> FirstNameValidator(string firstname)
                {
                    bool process = true;
                    if (string.IsNullOrEmpty(firstname) || (firstname.Length < 2 || firstname.Length > 60))
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, firstname);
                    return result;
                }

                Tuple<bool, string> LastNameValidator(string lastname)
                {
                    bool process = true;
                    if (string.IsNullOrEmpty(lastname) || (lastname.Length < 2 || lastname.Length > 60))
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, lastname);
                    return result;
                }

                Tuple<bool, string> DateOfBirthValidator(DateTime date)
                {
                    bool process = true;
                    string appropriateDate = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    if (date < new DateTime(2000, 01, 01) || date > DateTime.Today)
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, appropriateDate);
                    return result;
                }

                Tuple<bool, string> ShortValidator(short shortValue)
                {
                    bool process = false;
                    try
                    {
                        if (shortValue > short.MinValue && shortValue < short.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property1 value is not a <short> number");
                    }

                    var result = new Tuple<bool, string>(process, shortValue.ToString());
                    return result;
                }

                Tuple<bool, string> DecimalValidator(decimal decimalValue)
                {
                    bool process = false;
                    try
                    {
                        if (decimalValue > decimal.MinValue && decimalValue < decimal.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property2 value is not a <decimal> number");
                    }

                    var result = new Tuple<bool, string>(process, decimalValue.ToString());
                    return result;
                }

                Tuple<bool, string> CharValidator(char charValue)
                {
                    bool process = false;
                    try
                    {
                        if (charValue > char.MinValue && charValue < char.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property3 value is not a <char> letter");
                    }

                    var result = new Tuple<bool, string>(process, charValue.ToString());
                    return result;
                }

                firstNameValidator = FirstNameValidator;
                lastNameValidator = LastNameValidator;
                dateOfBirthValidator = DateOfBirthValidator;
                shortValidator = ShortValidator;
                decimalValidator = DecimalValidator;
                charValidator = CharValidator;
            }
            else
            {
                Tuple<bool, string> FirstNameValidator(string firstname)
                {
                    bool process = true;
                    if (string.IsNullOrEmpty(firstname) || (firstname.Length < 2 || firstname.Length > 60))
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, firstname);
                    return result;
                }

                Tuple<bool, string> LastNameValidator(string lastname)
                {
                    bool process = true;
                    if (string.IsNullOrEmpty(lastname) || (lastname.Length < 2 || lastname.Length > 60))
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, lastname);
                    return result;
                }

                Tuple<bool, string> DateOfBirthValidator(DateTime date)
                {
                    bool process = true;
                    string appropriateDate = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    if (date < new DateTime(1950, 01, 01) || date > DateTime.Today)
                    {
                        process = false;
                    }

                    var result = new Tuple<bool, string>(process, appropriateDate);
                    return result;
                }

                Tuple<bool, string> ShortValidator(short shortValue)
                {
                    bool process = false;
                    try
                    {
                        if (shortValue > short.MinValue && shortValue < short.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property1 value is not a <short> number");
                    }

                    var result = new Tuple<bool, string>(process, shortValue.ToString());
                    return result;
                }

                Tuple<bool, string> DecimalValidator(decimal decimalValue)
                {
                    bool process = false;
                    try
                    {
                        if (decimalValue > decimal.MinValue && decimalValue < decimal.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property2 value is not a <decimal> number");
                    }

                    var result = new Tuple<bool, string>(process, decimalValue.ToString());
                    return result;
                }

                Tuple<bool, string> CharValidator(char charValue)
                {
                    bool process = false;
                    try
                    {
                        if (charValue > char.MinValue && charValue < char.MaxValue)
                        {
                            process = true;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Property3 value is not a <char> letter");
                    }

                    var result = new Tuple<bool, string>(process, charValue.ToString());
                    return result;
                }

                firstNameValidator = FirstNameValidator;
                lastNameValidator = LastNameValidator;
                dateOfBirthValidator = DateOfBirthValidator;
                shortValidator = ShortValidator;
                decimalValidator = DecimalValidator;
                charValidator = CharValidator;
            }

            Console.Write("First name: ");
            firstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            lastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            dateOfBirth = ReadInput(dateConverter, dateOfBirthValidator);

            Console.Write("Property1 <short>: ");
            property1 = ReadInput(shortConverter, shortValidator);

            Console.Write("Property2 <decimal>: ");
            property2 = ReadInput(decimalConverter, decimalValidator);

            Console.Write("Property3 <char>: ");
            property3 = ReadInput(charConverter, charValidator);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static void CommandLineParameter(string[] args)
        {
            if (args.Length == 0 ||
                (args.Length == 1 && args[0].ToLowerInvariant().Equals("--validation-rules=default")) ||
                (args.Length == 2 && (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v default")))
            {
                Console.WriteLine("Using default validation rules.");
                fileCabinetService = new (new DefaultValidator());
                commandLineParameter = "default";
            }
            else if ((args.Length == 1 && args[0].ToLowerInvariant().Equals("--validation-rules=custom")) ||
                     (args.Length == 2 && (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v custom")))
            {
                Console.WriteLine("Using custom validation rules.");
                fileCabinetService = new (new CustomValidator());
                commandLineParameter = "custom";
            }
            else
            {
                Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                Console.WriteLine("Using default validation rules.");
                fileCabinetService = new (new DefaultValidator());
                commandLineParameter = "default";
            }
        }

        private static void Show(ReadOnlyCollection<FileCabinetRecord> onlyCollection)
        {
            foreach (var a in onlyCollection)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }
    }
}