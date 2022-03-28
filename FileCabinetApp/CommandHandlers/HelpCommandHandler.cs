using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'help' command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

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
            new string[] { "insert", "inserts a record with new values", "The 'insert' inserts a record with new values." },
            new string[] { "delete", "deletes records using the given criteria", "The 'delete' deletes records using the given criteria." },
        };

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "help")
            {
                PrintHelp(request);
            }
            else
            {
                base.Handle(request);
            }
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
    }
}
