using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'create' command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "select")
            {
                this.Select(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static int FindIdLength(int id)
        {
            int n = 0;
            int div = id;
            while (div != 0)
            {
                div /= 10;
                n++;
            }

            return n;
        }

        private static void FindMaxLength(ref int maxIdLength, ref int maxFirstNameLength, ref int maxLastNameLength, ref int maxDateLength, ref int maxProperty1Length, ref int maxProperty2Length, ref int maxProperty3Length, FileCabinetRecord a)
        {
            int n = FindIdLength(a.Id);

            if (n > maxIdLength)
            {
                maxIdLength = n;
            }

            if (a.FirstName.Length > maxFirstNameLength)
            {
                maxFirstNameLength = a.FirstName.Length;
            }

            if (a.LastName.Length > maxLastNameLength)
            {
                maxLastNameLength = a.LastName.Length;
            }

            if (a.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).Length > maxDateLength)
            {
                maxDateLength = a.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).Length;
            }

            n = FindIdLength(a.Property1);
            if (n > maxProperty1Length)
            {
                maxProperty1Length = n;
            }

            n = FindIdLength((int)a.Property2);
            if (n > maxProperty2Length)
            {
                maxProperty2Length = n;
            }
        }

        private static void PrintRecordInTable(int idwidth, int firstNameWidth, int lastNameWidth, int dateWidth, int property1Width, int property2Width, int property3Width, int id, string firstName, string lastName, string dateOfBirth, string property1, string property2, string property3, char separator, string selectId, string selectFirst, string selectLast, string selectDate, string selectProperty1, string selectProperty2, string selectProperty3)
        {
            Console.Write(separator);

            int firstSpace;
            int lastSpace;

            if (id > 0 && selectId != null)
            {
                firstSpace = idwidth - FindIdLength(id) - 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(id);
                lastSpace = idwidth - FindIdLength(id) - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectId != null)
            {
                firstSpace = (idwidth - 2) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write("Id");
                lastSpace = idwidth - 2 - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectId != null)
            {
                Console.Write(new string('-', idwidth));
                Console.Write(separator);
            }

            if (id > 0 && selectFirst != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(firstName);
                lastSpace = firstNameWidth - firstName.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectFirst != null)
            {
                firstSpace = (firstNameWidth - firstName.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(firstName);
                lastSpace = firstNameWidth - firstName.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectFirst != null)
            {
                Console.Write(new string('-', firstNameWidth));
                Console.Write(separator);
            }

            if (id > 0 && selectLast != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(lastName);
                lastSpace = lastNameWidth - lastName.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectLast != null)
            {
                firstSpace = (lastNameWidth - lastName.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(lastName);
                lastSpace = lastNameWidth - lastName.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectLast != null)
            {
                Console.Write(new string('-', lastNameWidth));
                Console.Write(separator);
            }

            if (id > 0 && selectDate != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(dateOfBirth);
                lastSpace = dateWidth - dateOfBirth.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectDate != null)
            {
                firstSpace = (dateWidth - dateOfBirth.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(dateOfBirth);
                lastSpace = dateWidth - dateOfBirth.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectDate != null)
            {
                Console.Write(new string('-', dateWidth));
                Console.Write(separator);
            }

            if (id > 0 && selectProperty1 != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property1);
                lastSpace = property1Width - property1.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectProperty1 != null)
            {
                firstSpace = (property1Width - property1.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property1);
                lastSpace = property1Width - property1.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectProperty1 != null)
            {
                Console.Write(new string('-', property1Width));
                Console.Write(separator);
            }

            if (id > 0 && selectProperty2 != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property2);
                lastSpace = property2Width - property2.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectProperty2 != null)
            {
                firstSpace = (property2Width - property2.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property2);
                lastSpace = property2Width - property2.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectProperty2 != null)
            {
                Console.Write(new string('-', property2Width));
                Console.Write(separator);
            }

            if (id > 0 && selectProperty3 != null)
            {
                firstSpace = 1;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property3);
                lastSpace = property3Width - property3.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -1 && selectProperty3 != null)
            {
                firstSpace = (property3Width - property3.Length) / 2;
                Console.Write(new string(' ', firstSpace));
                Console.Write(property3);
                lastSpace = property3Width - property3.Length - firstSpace;
                Console.Write(new string(' ', lastSpace));
                Console.Write(separator);
            }
            else if (id == -2 && selectProperty3 != null)
            {
                Console.Write(new string('-', property3Width));
                Console.Write(separator);
            }
        }

        private void Select(AppCommandRequest request)
        {
            string str = request.Parameters;
            char[] ch = { ' ', '=', ',', '\'' };
            string[] paramArr = str.Split(ch, StringSplitOptions.RemoveEmptyEntries);

            SortedDictionary<int, FileCabinetRecord> firstNameDict = new ();
            SortedDictionary<int, FileCabinetRecord> lastNameDict = new ();
            SortedDictionary<int, FileCabinetRecord> sum;

            int startIndex = -1;
            for (int i = 0; i < paramArr.Length; i++)
            {
                if (paramArr[i] == "where")
                {
                    startIndex = i + 1;
                }
            }

            string selectId = null;
            string selectFirst = null;
            string selectLast = null;
            string selectDate = null;
            string selectProperty1 = null;
            string selectProperty2 = null;
            string selectProperty3 = null;

            try
            {
                if (startIndex > 0)
                {
                    for (int i = 0; i < startIndex; i++)
                    {
                        switch (paramArr[i].ToLowerInvariant())
                        {
                            case "id":
                                selectId = "id";
                                break;

                            case "firstname":
                                selectFirst = "firstname";
                                break;

                            case "lastname":
                                selectLast = "lastname";
                                break;

                            case "dateofbirth":
                                selectDate = "dateofbirth";
                                break;

                            case "property1":
                                selectProperty1 = "property1";
                                break;

                            case "property2":
                                selectProperty2 = "property2";
                                break;

                            case "property3":
                                selectProperty3 = "property3";
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < paramArr.Length; i++)
                    {
                        switch (paramArr[i].ToLowerInvariant())
                        {
                            case "id":
                                selectId = "id";
                                break;

                            case "firstname":
                                selectFirst = "firstname";
                                break;

                            case "lastname":
                                selectLast = "lastname";
                                break;

                            case "dateofbirth":
                                selectDate = "dateofbirth";
                                break;

                            case "property1":
                                selectProperty1 = "property1";
                                break;

                            case "property2":
                                selectProperty2 = "property2";
                                break;

                            case "property3":
                                selectProperty3 = "property3";
                                break;
                        }
                    }
                }

                if (selectId == null && selectFirst == null && selectLast == null)
                {
                    throw new Exception("Select one or some of the parameters for display (id, firstname, lastname, dateofbirth, property1, property2, property3).");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            int maxIdLength = 2;
            int maxFirstNameLength = 9;
            int maxLastNameLength = 8;
            int maxDateLength = 11;
            int maxProperty1Length = 9;
            int maxProperty2Length = 9;
            int maxProperty3Length = 9;

            if (startIndex > 0 && (startIndex + 4) < paramArr.Length && paramArr[startIndex + 2] == "or")
            {
                for (int i = startIndex; i < paramArr.Length; i++)
                {
                    switch (paramArr[i].ToLowerInvariant())
                    {
                        case "firstname":
                            IEnumerable<FileCabinetRecord> firstNameCollection = this.service.FindByFirstName(paramArr[i + 1]);
                            foreach (var a in firstNameCollection)
                            {
                                if (a != null)
                                {
                                    firstNameDict.Add(a.Id, a);
                                }

                                FindMaxLength(ref maxIdLength, ref maxFirstNameLength, ref maxLastNameLength, ref maxDateLength, ref maxProperty1Length, ref maxProperty2Length, ref maxProperty3Length, a);
                            }

                            break;

                        case "lastname":
                            if (i + 1 >= paramArr.Length)
                            {
                                throw new ArgumentOutOfRangeException($"Check your input. {paramArr[i]} can't be empty.");
                            }

                            IEnumerable<FileCabinetRecord> lastNameCollection = this.service.FindByLastName(paramArr[i + 1]);
                            foreach (var a in lastNameCollection)
                            {
                                if (a != null)
                                {
                                    lastNameDict.Add(a.Id, a);
                                }

                                FindMaxLength(ref maxIdLength, ref maxFirstNameLength, ref maxLastNameLength, ref maxDateLength, ref maxProperty1Length, ref maxProperty2Length, ref maxProperty3Length, a);
                            }

                            break;
                    }
                }
            }
            else
            {
                var arr = this.service.GetRecords();
                foreach (var a in arr)
                {
                    FindMaxLength(ref maxIdLength, ref maxFirstNameLength, ref maxLastNameLength, ref maxDateLength, ref maxProperty1Length, ref maxProperty2Length, ref maxProperty3Length, a);
                }
            }

            bool ifselect = selectId == null & selectFirst == null & selectLast == null & selectDate == null & selectProperty1 == null & selectProperty2 == null & selectProperty3 == null;
            if (!ifselect)
            {
                int valForHead = -1;
                int valForLine = -2;

                int idwidth = maxIdLength + 2;
                int firstNameWidth = maxFirstNameLength + 2;
                int lastNameWidth = maxLastNameLength + 2;
                int dateWidth = maxDateLength + 2;
                int property1Width = maxProperty1Length + 2;
                int property2Width = maxProperty2Length + 2;
                int property3Width = maxProperty3Length + 2;

                if (startIndex > 0 && (startIndex + 4) < paramArr.Length && paramArr[startIndex + 2] == "or")
                {
                    PrintHead(selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3, valForHead, valForLine, idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width);

                    sum = firstNameDict;
                    foreach (var a in lastNameDict)
                    {
                        if (!sum.ContainsKey(a.Key))
                        {
                            sum.Add(a.Key, a.Value);
                        }
                    }

                    foreach (var a in sum)
                    {
                        PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, a.Key, a.Value.FirstName, a.Value.LastName, a.Value.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), a.Value.Property1.ToString(), a.Value.Property2.ToString(), a.Value.Property3.ToString(), '|', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                        Console.WriteLine();
                    }

                    PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, valForLine, "-", "-", "-", "-", "-", "-", '+', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                    Console.WriteLine();
                }
                else
                {
                    PrintHead(selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3, valForHead, valForLine, idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width);

                    var arr = this.service.GetRecords();
                    foreach (var a in arr)
                    {
                        PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, a.Id, a.FirstName, a.LastName, a.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), a.Property1.ToString(), a.Property2.ToString(), a.Property3.ToString(), '|', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                        Console.WriteLine();
                    }

                    PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, valForLine, "-", "-", "-", "-", "-", "-", '+', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                    Console.WriteLine();
                }
            }

            static void PrintHead(string selectId, string selectFirst, string selectLast, string selectDate, string selectProperty1, string selectProperty2, string selectProperty3, int valForHead, int valForLine, int idwidth, int firstNameWidth, int lastNameWidth, int dateWidth, int property1Width, int property2Width, int property3Width)
            {
                PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, valForLine, "-", "-", "-", "-", "-", "-", '+', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                Console.WriteLine();
                PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, valForHead, "FirstName", "LastName", "DateOfBirth", "Property1", "Property2", "Property3", '|', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                Console.WriteLine();
                PrintRecordInTable(idwidth, firstNameWidth, lastNameWidth, dateWidth, property1Width, property2Width, property3Width, valForLine, "-", "-", "-", "-", "-", "-", '+', selectId, selectFirst, selectLast, selectDate, selectProperty1, selectProperty2, selectProperty3);
                Console.WriteLine();
            }
        }
    }
}
