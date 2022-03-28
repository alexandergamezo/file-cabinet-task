using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.RecordValidator;
using FileCabinetApp.RecordValidator.Extensions;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class for this app.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// File name.
        /// </summary>
        public const string Filename = "cabinet-records.db";

        /// <summary>
        /// Log name.
        /// </summary>
        private const string Logname = "log.txt";
        private const string DeveloperName = "Alexander Gamezo";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static readonly Action<IEnumerable<FileCabinetRecord>> RecordPrinter = Print;
        private static readonly Action<bool> Action = ChangeIsRunning;
        private static bool isRunning = true;

        /// <summary>
        /// Object reference.
        /// </summary>
        private static IFileCabinetService fileCabinetService;
        private static string commandLineParameter = string.Empty;

        /// <summary>
        /// The main method for this app.
        /// </summary>
        /// <param name="args">Application arguments with validation-rules command line parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");

            CommandLineParameter(args);
            var commandHandler = CreateCommandHandlers();

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

        /// <summary>
        /// Reads parameters from the command line.
        /// </summary>
        /// <param name="args">Array with string parameters.</param>
        public static void CommandLineParameter(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json")
                .Build();

            IRecordValidator defaultValidator = new ValidatorBuilder().CreateDefault(builder);
            IRecordValidator customValidator = new ValidatorBuilder().CreateCustom(builder);

            try
            {
                bool paramVariantOne = (args[0].ToLowerInvariant().Equals("--validation-rules=default") || (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v default")) &&
                                       (args[2].ToLowerInvariant().Equals("--storage=memory") || (args[2] + " " + args[3]).ToLowerInvariant().Equals("-s memory"));

                bool paramVariantTwo = (args[0].ToLowerInvariant().Equals("--validation-rules=default") || (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v default")) &&
                                       (args[2].ToLowerInvariant().Equals("--storage=file") || (args[2] + " " + args[3]).ToLowerInvariant().Equals("-s file"));

                bool paramVariantThree = (args[0].ToLowerInvariant().Equals("--validation-rules=custom") || (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v custom")) &&
                                         (args[2].ToLowerInvariant().Equals("--storage=memory") || (args[2] + " " + args[3]).ToLowerInvariant().Equals("-s memory"));

                bool paramVariantFour = (args[0].ToLowerInvariant().Equals("--validation-rules=custom") || (args[0] + " " + args[1]).ToLowerInvariant().Equals("-v custom")) &&
                                        (args[2].ToLowerInvariant().Equals("--storage=file") || (args[2] + " " + args[3]).ToLowerInvariant().Equals("-s file"));

                bool paramVariantFive = args.Length == 5 && args[4].ToLowerInvariant().Equals("use-stopwatch");

                bool paramVariantSix = args.Length == 6 && args[4].ToLowerInvariant().Equals("use-stopwatch") && args[5].ToLowerInvariant().Equals("use-logger");

                IFileCabinetService fileCabinetBase;

                if (paramVariantOne)
                {
                    Console.WriteLine("Using default validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (defaultValidator);
                    fileCabinetBase = fileCabinetMemoryService;
                    commandLineParameter = "default";
                }
                else if (paramVariantTwo)
                {
                    Console.WriteLine("Using default validation rules. Storage is file.");
                    FileStream fileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Seek(0, SeekOrigin.End);
                    FileCabinetFilesystemService fileCabinetFilesystemService = new (fileStream, defaultValidator, Filename);
                    fileCabinetBase = fileCabinetFilesystemService;
                    commandLineParameter = "default";
                }
                else if (paramVariantThree)
                {
                    Console.WriteLine("Using custom validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (customValidator);
                    fileCabinetBase = fileCabinetMemoryService;
                    commandLineParameter = "custom";
                }
                else if (paramVariantFour)
                {
                    Console.WriteLine("Using custom validation rules. Storage is file.");
                    FileStream fileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Seek(0, SeekOrigin.End);
                    FileCabinetFilesystemService fileCabinetFilesystemService = new (fileStream, customValidator, Filename);
                    fileCabinetBase = fileCabinetFilesystemService;
                    commandLineParameter = "custom";
                }
                else
                {
                    Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                    Console.WriteLine("Using default validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (defaultValidator);
                    fileCabinetBase = fileCabinetMemoryService;
                    commandLineParameter = "default";
                }

                if (paramVariantFive)
                {
                    fileCabinetService = new ServiceMeter(fileCabinetBase);
                }
                else if (paramVariantSix)
                {
                    StreamWriter writer = new (Logname);
                    fileCabinetService = new ServiceLogger(new ServiceMeter(fileCabinetBase), writer);
                }
                else
                {
                    fileCabinetService = fileCabinetBase;
                }
            }
            catch
            {
                Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                Console.WriteLine("Using default validation rules. Storage is memory.");
                FileCabinetMemoryService fileCabinetMemoryService = new (defaultValidator);
                fileCabinetService = fileCabinetMemoryService;
                commandLineParameter = "default";
            }
        }

        /// <summary>
        /// Checks input from the line.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="property1">Property in "short" format.</param>
        /// <param name="property2">Property in "decimal" format.</param>
        /// <param name="property3">Property in "char" format.</param>
        public static void CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3)
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

        /// <summary>
        /// Defines the format and path of the file from parameters.
        /// </summary>
        /// <param name="format">Format of the file.</param>
        /// <param name="path">Path of the file.</param>
        /// <param name="parameters">Parameters.</param>
        public static void CheckInputParameters(out string format, out string path, string parameters)
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

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var exitHandler = new ExitCommandHandler(Action);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, RecordPrinter);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, RecordPrinter);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService, Filename);
            var insertHandler = new InsertCommandHandler(fileCabinetService, Filename);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);

            helpHandler.SetNext(exitHandler);
            exitHandler.SetNext(statHandler);
            statHandler.SetNext(createHandler);
            createHandler.SetNext(listHandler);
            listHandler.SetNext(editHandler);
            editHandler.SetNext(findHandler);
            findHandler.SetNext(exportHandler);
            exportHandler.SetNext(importHandler);
            importHandler.SetNext(removeHandler);
            removeHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(insertHandler);
            insertHandler.SetNext(deleteHandler);
            return helpHandler;
        }

        private static void ChangeIsRunning(bool b)
        {
            isRunning = b;
        }

        private static void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var a in records)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }
    }
}