using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'edit' command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "update")
            {
                this.Update(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Update(AppCommandRequest request)
        {
            char[] ch = { ' ', '=', ',' };
            string[] paramArr = request.Parameters.Split(ch, StringSplitOptions.RemoveEmptyEntries);

            int id = -1;
            string firstName = string.Empty;
            string lastName = string.Empty;
            DateTime dateOfBirth = DateTime.MinValue;
            short property1 = 0;
            decimal property2 = 0;
            char property3 = '\0';

            try
            {
                for (int i = 1; i < paramArr.Length; i++)
                {
                    switch (paramArr[i].ToLowerInvariant())
                    {
                        case "id":
                            id = int.Parse(paramArr[i + 1].Trim('\''));
                            break;
                        case "firstname":
                            firstName = paramArr[i + 1].Trim('\'');
                            break;
                        case "lastname":
                            lastName = paramArr[i + 1].Trim('\'');
                            break;
                        case "dateofbirth":
                            dateOfBirth = DateTime.Parse(paramArr[i + 1].Trim('\''));
                            break;
                        case "property1":
                            property1 = short.Parse(paramArr[i + 1].Trim('\''));
                            break;
                        case "property2":
                            property2 = decimal.Parse(paramArr[i + 1].Trim('\''));
                            break;
                        case "property3":
                            property3 = char.Parse(paramArr[i + 1].Trim('\''));
                            break;
                    }
                }

                ParameterObject v = new (firstName, lastName, dateOfBirth, property1, property2, property3);

                if (id >= 0)
                {
                    this.service.UpdateRecord(id, v);
                    Console.WriteLine($"Record #{id} is updated.");
                }

                if (id < 0)
                {
                    List<int> listAfterWhere = this.FindIdCoincidences(paramArr, firstName, lastName);
                    this.EditRecordWithoutId(v, listAfterWhere);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc.Message}");
            }
        }

        private void EditRecordWithoutId(ParameterObject v, List<int> listAfterWhere)
        {
            if (listAfterWhere.Count > 1)
            {
                foreach (var a in listAfterWhere)
                {
                    this.service.UpdateRecord(a, v);
                }

                Console.Write("Records ");
                for (int i = 0; i < listAfterWhere.Count - 1; i++)
                {
                    Console.Write($"#{listAfterWhere[i]}, ");
                }

                Console.WriteLine($"{listAfterWhere[listAfterWhere.Count - 1]} are updated.");
            }
            else if (listAfterWhere.Count == 1)
            {
                this.service.UpdateRecord(listAfterWhere[0], v);
                Console.WriteLine($"Record #{listAfterWhere[0]} is updated.");
            }
            else
            {
                Console.WriteLine("Check your input. Records aren't found.");
            }
        }

        private List<int> FindIdCoincidences(string[] paramArr, string firstName, string lastName)
        {
            int k = 0;
            for (int i = 1; i < paramArr.Length; i++)
            {
                if (paramArr[i] == "where")
                {
                    k = i;
                }
            }

            List<int> listAfterWhere = new ();
            List<int> idfirstname = new ();
            List<int> idlastname = new ();

            for (int i = k + 1; i < paramArr.Length; i++)
            {
                switch (paramArr[i].ToLowerInvariant())
                {
                    case "firstname" when k != 0:
                        IEnumerable<FileCabinetRecord> firstNameCollection = this.service.FindByFirstName(firstName);
                        foreach (var a in firstNameCollection)
                        {
                            if (a != null)
                            {
                                idfirstname.Add(a.Id);
                            }
                        }

                        break;

                    case "lastname" when k != 0:
                        IEnumerable<FileCabinetRecord> lastNameCollection = this.service.FindByLastName(lastName);
                        foreach (var a in lastNameCollection)
                        {
                            if (a != null)
                            {
                                idlastname.Add(a.Id);
                            }
                        }

                        break;
                }
            }

            for (int i = 0; i < idfirstname.Count; i++)
            {
                for (int m = 0; m < idlastname.Count; m++)
                {
                    if (idfirstname[i] == idlastname[m])
                    {
                        listAfterWhere.Add(idfirstname[i]);
                    }
                }
            }

            return listAfterWhere;
        }
    }
}