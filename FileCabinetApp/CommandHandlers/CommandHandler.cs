using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handles a request.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private const string SourceFileName = "temp.db";
        private const string DestinationBackupFileName = "cabinet-records.db.bac";
        private const string Filename = "cabinet-records.db";

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
            new string[] { "import", "imports service data from from file in the 'CSV' or 'XML' format", "The 'import' imports service data from file in the 'CSV' or 'XML' format. Formats: 'csv', 'xml'." },
            new string[] { "remove", "removes a record", "The 'remove' removes a record." },
            new string[] { "purge", "defragments a file", "The 'purge' defragments a file." },
        };

        private static string commandLineParameter = string.Empty;
        private static string[] initParams = new string[4];

        /// <summary>
        /// Reads parameters from the command line.
        /// </summary>
        /// <param name="args">Array with string parameters.</param>
        public static void CommandLineParameter(string[] args)
        {
            initParams = args;

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

                if (paramVariantOne)
                {
                    Console.WriteLine("Using default validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (new DefaultValidator());
                    Program.fileCabinetService = fileCabinetMemoryService;
                    commandLineParameter = "default";
                }
                else if (paramVariantTwo)
                {
                    Console.WriteLine("Using default validation rules. Storage is file.");
                    FileStream fileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Seek(0, SeekOrigin.End);
                    FileCabinetFilesystemService fileCabinetFilesystemService = new (fileStream, new DefaultValidator());
                    Program.fileCabinetService = fileCabinetFilesystemService;
                    commandLineParameter = "default";
                }
                else if (paramVariantThree)
                {
                    Console.WriteLine("Using custom validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (new CustomValidator());
                    Program.fileCabinetService = fileCabinetMemoryService;
                    commandLineParameter = "custom";
                }
                else if (paramVariantFour)
                {
                    Console.WriteLine("Using custom validation rules. Storage is file.");
                    FileStream fileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Seek(0, SeekOrigin.End);
                    FileCabinetFilesystemService fileCabinetFilesystemService = new (fileStream, new CustomValidator());
                    Program.fileCabinetService = fileCabinetFilesystemService;
                    commandLineParameter = "custom";
                }
                else
                {
                    Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                    Console.WriteLine("Using default validation rules. Storage is memory.");
                    FileCabinetMemoryService fileCabinetMemoryService = new (new DefaultValidator());
                    Program.fileCabinetService = fileCabinetMemoryService;
                    commandLineParameter = "default";
                }
            }
            catch
            {
                Console.WriteLine("Validation-rules command line parameter is wrong. Check your input.");
                Console.WriteLine("Using default validation rules. Storage is memory.");
                FileCabinetMemoryService fileCabinetMemoryService = new (new DefaultValidator());
                Program.fileCabinetService = fileCabinetMemoryService;
                commandLineParameter = "default";
            }
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Command request.</param>
        /// <returns>Method for executing a request.</returns>
        public override object Handle(AppCommandRequest request)
        {
            switch (request.Command)
            {
                case "help":
                    PrintHelp(request);
                    break;
                case "exit":
                    Exit();
                    break;
                case "stat":
                    Stat();
                    break;
                case "create":
                    Create();
                    break;
                case "list":
                    List();
                    break;
                case "edit":
                    Edit(request);
                    break;
                case "find":
                    Find(request);
                    break;
                case "export":
                    Export(request);
                    break;
                case "import":
                    Import(request);
                    break;
                case "remove":
                    Remove(request);
                    break;
                case "purge":
                    Purge();
                    break;
                default:
                    PrintMissedCommandInfo(request);
                    break;
            }

            return base.Handle(request);
        }

        private static void PrintHelp(AppCommandRequest request)
        {
            if (!string.IsNullOrEmpty(request.Parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], request.Parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{request.Parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static void Stat()
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create()
        {
            try
            {
                CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                Console.WriteLine($"Record #{Program.fileCabinetService.CreateRecord(paramobj)} is created.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static void List()
        {
            var arr = Program.fileCabinetService.GetRecords();
            if (arr.Count == 0)
            {
                Console.WriteLine("The list is empty.");
            }
            else
            {
                Show(arr);
            }
        }

        private static void Edit(AppCommandRequest request)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(request.Parameters) && int.TryParse(request.Parameters, out int enteredId))
            {
                id = enteredId;
            }

            var onlyCollection = Program.fileCabinetService.GetRecords();

            if (onlyCollection.Count == 0)
            {
                Console.WriteLine($"#{request.Parameters} record is not found.");
            }

            for (int i = 0; i < onlyCollection.Count; i++)
            {
                if (id == onlyCollection[i].Id)
                {
                    try
                    {
                        CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                        ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                        Program.fileCabinetService.EditRecord(id, paramobj);
                        Console.WriteLine($"Record #{id} is updated.");
                        break;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }

                if (i == onlyCollection.Count - 1)
                {
                    Console.WriteLine($"#{request.Parameters} record is not found.");
                }
            }
        }

        private static void Find(AppCommandRequest request)
        {
            string[] inputs = request.Parameters.Split(' ', 2);
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

        private static void Export(AppCommandRequest request)
        {
            string paramFormat;
            string paramPath = string.Empty;
            try
            {
                CheckInputParameters(out string f, out string p, request.Parameters);
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
                Program.fileCabinetService.MakeSnapshot().SaveToCsv(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }

            void SaveToXmlFormat(string path)
            {
                StreamWriter writer = new (path);
                Program.fileCabinetService.MakeSnapshot().SaveToXmL(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }
        }

        private static void Import(AppCommandRequest request)
        {
            string paramFormat;
            string paramPath = string.Empty;
            try
            {
                CheckInputParameters(out string f, out string p, request.Parameters);
                paramFormat = f;
                paramPath = p;

                if (File.Exists(paramPath))
                {
                    FindAppropriateFunc();
                }
                else
                {
                    Console.WriteLine($"Import error: file {paramPath} is not exist.");
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                Console.WriteLine($"Import failed: can't open file {paramPath}. {exc}.");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc}.");
            }

            void FindAppropriateFunc()
            {
                if (paramFormat == "csv")
                {
                    ImportFromCsvFormat(paramPath);
                }

                if (paramFormat == "xml")
                {
                    ImportFromXmlFormat(paramPath);
                }
            }

            void ImportFromCsvFormat(string path)
            {
                StreamReader reader = new (new FileStream(path, FileMode.Open));
                FileCabinetServiceSnapshot snapshot = new ();
                snapshot.LoadFromCsv(reader);
                Program.fileCabinetService.Restore(snapshot, out int count);

                reader.Close();
                Console.WriteLine($"{count} records were imported from {paramPath}.");
            }

            void ImportFromXmlFormat(string path)
            {
                StreamReader reader = new (new FileStream(path, FileMode.Open));
                FileCabinetServiceSnapshot snapshot = new ();
                snapshot.LoadFromXml(reader);
                Program.fileCabinetService.Restore(snapshot, out int count);

                reader.Close();
                Console.WriteLine($"{count} records were imported from {paramPath}.");
            }
        }

        private static void Remove(AppCommandRequest request)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(request.Parameters) && int.TryParse(request.Parameters, out int enteredId))
            {
                id = enteredId;
            }

            var onlyCollection = Program.fileCabinetService.GetRecords();

            if (onlyCollection.Count == 0)
            {
                Console.WriteLine($"#{request.Parameters} record is not found.");
            }

            for (int i = 0; i < onlyCollection.Count; i++)
            {
                if (id == onlyCollection[i].Id)
                {
                    try
                    {
                        Program.fileCabinetService.RemoveRecord(id);
                        Console.WriteLine($"Record #{id} is removed.");
                        break;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }

                if (i == onlyCollection.Count - 1 || onlyCollection.Count == 0)
                {
                    Console.WriteLine($"Record #{request.Parameters} doesn't exists.");
                }
            }
        }

        private static void Purge()
        {
            Program.fileCabinetService.DefragFile(SourceFileName, out int numNewRecords, out int numOldRecords);
            if (numNewRecords != -1)
            {
                File.Replace(SourceFileName, Filename, DestinationBackupFileName);
                Console.WriteLine($"Data file processing is completed: {numOldRecords - numNewRecords} of {numOldRecords} records were purged.");
                CommandLineParameter(initParams);
            }
        }

        private static void CheckInputParameters(out string format, out string path, string parameters)
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

        private static void FindAppropriateMethod(string[] originalParameters, string command, string parameter)
        {
            bool checkParameterFirstName = originalParameters[0].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterLastName = originalParameters[1].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterDateOfBirth = originalParameters[2].Equals(command, StringComparison.InvariantCultureIgnoreCase);

            if (checkParameterFirstName)
            {
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = Program.fileCabinetService.FindByFirstName(parameter);
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
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = Program.fileCabinetService.FindByLastName(parameter);
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
                    ReadOnlyCollection<FileCabinetRecord> onlyCollection = Program.fileCabinetService.FindByDateOfBirth(parameter.Trim('"'));
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

        private static void Show(ReadOnlyCollection<FileCabinetRecord> onlyCollection)
        {
            foreach (var a in onlyCollection)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }

        private static void PrintMissedCommandInfo(AppCommandRequest request)
        {
            Console.WriteLine($"There is no '{request.Command}' command.");
            Console.WriteLine();
        }
    }
}
