﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Alexander Gamezo";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a record", "The 'create' creates a record." },
            new string[] { "list", "returns a list of records", "The 'list' returns a list of records." },
            new string[] { "edit", "edits a record", "The 'edit' edits a record." },
            new string[] { "find", "finds and returns a list of records", "The 'find' finds and returns a list of records." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
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
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            try
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Date of birth: ");
                DateTime dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                Console.Write("Property1 <short>: ");
                short property1 = short.Parse(Console.ReadLine());

                Console.Write("Property2 <decimal>: ");
                decimal property2 = decimal.Parse(Console.ReadLine());

                Console.Write("Property3 <char>: ");
                char property3 = char.Parse(Console.ReadLine());

                Console.WriteLine($"Record #{fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, property1, property2, property3)} is created.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private static void List(string parameters)
        {
            var arr = Program.fileCabinetService.GetRecords();
            foreach (var a in arr)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }

        private static void Edit(string parameters)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(parameters) && int.TryParse(parameters, out int num))
            {
                id = num;
            }

            var arr = Program.fileCabinetService.GetRecords();
            if (id > 0 && id <= arr.Length)
            {
                try
                {
                    Console.Write("First name: ");
                    string firstName = Console.ReadLine();

                    Console.Write("Last name: ");
                    string lastName = Console.ReadLine();

                    Console.Write("Date of birth: ");
                    DateTime dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    Console.Write("Property1 <short>: ");
                    short property1 = short.Parse(Console.ReadLine());

                    Console.Write("Property2 <decimal>: ");
                    decimal property2 = decimal.Parse(Console.ReadLine());

                    Console.Write("Property3 <char>: ");
                    char property3 = char.Parse(Console.ReadLine());

                    fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, property1, property2, property3);
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
            string command = inputs[commandIndex];
            string parameter = string.Empty;
            try
            {
                parameter = inputs[parameterIndex].Trim('"');
                Console.WriteLine("parameter: " + parameter);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Add your parameter(s)");
            }

            string firstName = "firstname";

            if (firstName.Equals(command, StringComparison.InvariantCultureIgnoreCase))
            {
                FileCabinetRecord[] arr = fileCabinetService.FindByFirstName(parameter);
                foreach (var a in arr)
                {
                    Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
                }
            }
        }
    }
}