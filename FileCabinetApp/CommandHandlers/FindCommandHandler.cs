using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'find' command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        /// <param name="recordPrinter">Delegate.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
            : base(fileCabinetService)
        {
            this.recordPrinter = recordPrinter;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "find")
            {
                this.Find(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Find(AppCommandRequest request)
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
                this.FindAppropriateMethod(originalParameters, command, parameter);
            }
        }

        private void FindAppropriateMethod(string[] originalParameters, string command, string parameter)
        {
            bool checkParameterFirstName = originalParameters[0].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterLastName = originalParameters[1].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            bool checkParameterDateOfBirth = originalParameters[2].Equals(command, StringComparison.InvariantCultureIgnoreCase);

            if (checkParameterFirstName)
            {
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = (ReadOnlyCollection<FileCabinetRecord>)this.service.FindByFirstName(parameter);
                if (onlyCollection.Count == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    this.recordPrinter(onlyCollection);
                }
            }

            if (checkParameterLastName)
            {
                ReadOnlyCollection<FileCabinetRecord> onlyCollection = (ReadOnlyCollection<FileCabinetRecord>)this.service.FindByLastName(parameter);
                if (onlyCollection.Count == 0)
                {
                    Console.WriteLine("No results.");
                }
                else
                {
                    this.recordPrinter(onlyCollection);
                }
            }

            if (checkParameterDateOfBirth)
            {
                try
                {
                    ReadOnlyCollection<FileCabinetRecord> onlyCollection = (ReadOnlyCollection<FileCabinetRecord>)this.service.FindByDateOfBirth(parameter.Trim('"'));
                    if (onlyCollection.Count == 0)
                    {
                        Console.WriteLine("No results.");
                    }
                    else
                    {
                        this.recordPrinter(onlyCollection);
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
