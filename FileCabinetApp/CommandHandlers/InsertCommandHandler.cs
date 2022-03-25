using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'insert' command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string SourceFileName = "temp.db";
        private const string DestinationBackupFileName = "cabinet-records.db.bac";
        private readonly string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        /// <param name="filename">Filename.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService, string filename)
            : base(fileCabinetService)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "insert")
            {
                this.Insert(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Insert(AppCommandRequest request)
        {
            string requestStr = request.Parameters;
            int paramStartIndex = 1;
            int paramEndIndex = requestStr.IndexOf(')');
            int valStartIndex = requestStr.IndexOf(" values (") + 9;
            int valEndIndex = requestStr.Length - 1;

            string paramStr = string.Empty;
            string valStr = string.Empty;

            if (requestStr[0] == '(' && requestStr[^1] == ')')
            {
                paramStr = requestStr[paramStartIndex .. paramEndIndex];
                valStr = requestStr[valStartIndex .. valEndIndex];
            }
            else
            {
                throw new Exception("Check your input");
            }

            int id = 0;
            string firstName = string.Empty;
            string lastName = string.Empty;
            DateTime dateOfBirth = DateTime.MinValue;
            short property1 = 0;
            decimal property2 = 0;
            char property3 = '\0';

            string[] paramArr = paramStr.Split(',');
            string[] valArr = valStr.Split(',');

            try
            {
                for (int i = 0; i < paramArr.Length; i++)
                {
                    switch (paramArr[i].Trim(' '))
                    {
                        case "id":
                            id = int.Parse(valArr[i].Trim('\'', ' '));
                            break;
                        case "firstname":
                            firstName = valArr[i].Trim('\'', ' ');
                            break;
                        case "lastname":
                            lastName = valArr[i].Trim('\'', ' ');
                            break;
                        case "dateofbirth":
                            dateOfBirth = DateTime.Parse(valArr[i].Trim('\'', ' '));
                            break;
                        case "property1":
                            property1 = short.Parse(valArr[i].Trim('\'', ' '));
                            break;
                        case "property2":
                            property2 = decimal.Parse(valArr[i].Trim('\'', ' '));
                            break;
                        case "property3":
                            property3 = char.Parse(valArr[i].Trim('\'', ' '));
                            break;
                    }
                }

                ParameterObject v = new (firstName, lastName, dateOfBirth, property1, property2, property3);

                if (id == 0)
                {
                    int result = this.service.CreateRecord(v);
                    if (result > 0)
                    {
                        Console.WriteLine($"Record #{result} is inserted.");
                    }
                }
                else
                {
                    int result = this.service.Insert(id, v);
                    if (result > 0)
                    {
                        File.Replace(SourceFileName, this.filename, DestinationBackupFileName);
                        Console.WriteLine($"Record #{result} is inserted.");

                        string[] str = Environment.GetCommandLineArgs();
                        Program.Main(str[1..]);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc.Message}");
            }
        }
    }
}
