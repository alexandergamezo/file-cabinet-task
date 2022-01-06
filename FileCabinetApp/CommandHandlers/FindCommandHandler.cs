using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'find' command.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "find")
            {
                Find(request);
            }
            else
            {
                base.Handle(request);
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
                    Program.Show(onlyCollection);
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
                    Program.Show(onlyCollection);
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
                        Program.Show(onlyCollection);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"The '{originalParameters[2]}' parameter has the wrong format.");
                }
            }
        }
    }
}
