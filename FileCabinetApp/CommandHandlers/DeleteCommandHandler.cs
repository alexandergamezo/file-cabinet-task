using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'create' command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "delete")
            {
                this.Delete(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Delete(AppCommandRequest request)
        {
            string str = request.Parameters;
            char[] ch = { ' ', '=' };
            string[] paramArr = str.Split(ch, StringSplitOptions.RemoveEmptyEntries);
            string parameter = paramArr[2].Trim('\'');
            List<int> list = new ();

            switch (paramArr[1].ToLowerInvariant())
            {
                case "id" when paramArr[0] == "where":
                    list.Add(int.Parse(paramArr[2].Trim('\'')));
                    break;

                case "firstname" when paramArr[0] == "where":
                    IEnumerable<FileCabinetRecord> firstNameCollection = this.service.FindByFirstName(parameter);
                    foreach (var a in firstNameCollection)
                    {
                        if (a != null)
                        {
                            list.Add(a.Id);
                        }
                    }

                    break;

                case "lastname" when paramArr[0] == "where":
                    IEnumerable<FileCabinetRecord> lastNameCollection = this.service.FindByLastName(parameter);
                    foreach (var a in lastNameCollection)
                    {
                        if (a != null)
                        {
                            list.Add(a.Id);
                        }
                    }

                    break;

                case "dateofbirth" when paramArr[0] == "where":
                    IEnumerable<FileCabinetRecord> dateOfBirthCollection = this.service.FindByDateOfBirth(parameter);
                    foreach (var a in dateOfBirthCollection)
                    {
                        if (a != null)
                        {
                            list.Add(a.Id);
                        }
                    }

                    break;
            }

            if (list.Count > 1)
            {
                foreach (var a in list)
                {
                    this.service.RemoveRecord(a);
                }

                Console.Write("Records ");
                for (int i = 0; i < list.Count - 1; i++)
                {
                    Console.Write($"#{list[i]}, ");
                }

                Console.WriteLine($"{list[list.Count - 1]} are deleted.");
            }
            else if (list.Count == 1)
            {
                this.service.RemoveRecord(list[0]);
                Console.WriteLine($"Record #{list[0]} is deleted.");
            }
            else
            {
                Console.WriteLine("Check your input. Records aren't found.");
            }
        }
    }
}
