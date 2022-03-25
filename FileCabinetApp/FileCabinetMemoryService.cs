using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reacts to user commands and executes some commands.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// Provides a setter to change a strategy at runtime.
        /// </summary>
        /// <param name="validator">Reference to one of the strategy objects.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates records in the List.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(ParameterObject v)
        {
            this.validator.ValidateParameters(v);
            FileCabinetRecord record;

            if (this.list.Count == 0)
            {
                record = new FileCabinetRecord
                {
                    Id = 1,
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    DateOfBirth = v.DateOfBirth,
                    Property1 = v.Property1,
                    Property2 = v.Property2,
                    Property3 = v.Property3,
                };
            }
            else
            {
                record = new FileCabinetRecord
                {
                    Id = this.list[^1].Id + 1,
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    DateOfBirth = v.DateOfBirth,
                    Property1 = v.Property1,
                    Property2 = v.Property2,
                    Property3 = v.Property3,
                };
            }

            this.list.Add(record);

            return record.Id;
        }

        /// <summary>
        /// Gets records from the List and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ReadOnlyCollection<FileCabinetRecord> onlyCollection = new (this.list);
            return onlyCollection;
        }

        /// <summary>
        /// Defines the number of records in the List.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Changes old record on the new one in the List.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        public void EditRecord(int id, ParameterObject v)
        {
            this.validator.ValidateParameters(v);

            int realPosition = 0;
            for (int i = 0; i < this.list.Count; i++)
            {
                if (id == this.list[i].Id)
                {
                    realPosition = i;
                }
            }

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = v.FirstName,
                LastName = v.LastName,
                DateOfBirth = v.DateOfBirth,
                Property1 = v.Property1,
                Property2 = v.Property2,
                Property3 = v.Property3,
            };

            this.list.RemoveAt(realPosition);
            this.list.Insert(realPosition, record);
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return this.GetFindList("firstname", firstName);
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return this.GetFindList("lastname", lastName);
        }

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return this.GetFindList("dateofbirth", dateOfBirth);
        }

        /// <summary>
        /// Saves the current state inside a FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Snapshot of object, where the FileCabinetService passes its state to the FileCabinetServiceSnapshot's constructor parameters.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list);
        }

        /// <summary>
        /// Restores the state from a snapshot object.
        /// </summary>
        /// <param name="snapshot">Snapshot of the object, where saved its state.</param>
        /// <param name="count">Number of records that were imported with definite validation rules.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out int count)
        {
            count = 0;
            foreach (var a in snapshot.Records)
            {
                ParameterObject paramobj = new (a.FirstName, a.LastName, a.DateOfBirth, a.Property1, a.Property2, a.Property3);

                try
                {
                    this.validator.ValidateParameters(paramobj);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Error! Id #{a.Id}: {exc}");
                    continue;
                }

                bool exist = false;
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (a.Id == this.list[i].Id)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    this.list.Add(a);
                    count++;
                }
                else
                {
                    this.EditRecord(a.Id, paramobj);
                    count++;
                }
            }
        }

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void RemoveRecord(int id)
        {
            this.list.RemoveAt(this.FindRealPosition(id));
        }

        /// <summary>
        /// Defrags a file.
        /// </summary>
        /// <param name="destinationFileName">The destination file name.</param>
        /// <param name="numNewRecords">The number of new records in a new file.</param>
        /// <param name="numOldRecords">The number of old records in an old file.</param>
        public void DefragFile(string destinationFileName, out int numNewRecords, out int numOldRecords)
        {
            numNewRecords = -1;
            numOldRecords = -1;
            Console.WriteLine("'Purge' isn't available for parameter '--storage=memory'");
        }

        /// <summary>
        /// Creates a new list with found records.
        /// </summary>
        /// <param name="whatFind">the parameter for finding.</param>
        /// <param name="parameter">firstName, lastName or dateOfBirth.</param>
        /// <returns>the list with found records.</returns>
        public IEnumerable<FileCabinetRecord> GetFindList(string whatFind, string parameter)
        {
            bool firstname = whatFind.Equals("firstname");
            bool lastname = whatFind.Equals("lastname");
            bool dateofbirth = whatFind.Equals("dateofbirth");

            string appropriateFormat;
            if (dateofbirth && DateTime.TryParse(parameter, out DateTime appropriateValue))
            {
                string str = appropriateValue.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                appropriateFormat = string.Concat(str[..6].ToUpper(), str[6..].ToLower());
            }
            else
            {
                appropriateFormat = string.Concat(parameter[..1].ToUpper(), parameter[1..].ToLower());
            }

            foreach (var record in this.list)
            {
                if (firstname && record.FirstName.Equals(appropriateFormat))
                {
                    yield return record;
                }
                else if (lastname && record.LastName.Equals(appropriateFormat))
                {
                    yield return record;
                }
                else if (dateofbirth && record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).Equals(appropriateFormat))
                {
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Inserts records.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int Insert(int id, ParameterObject v)
        {
            this.validator.ValidateParameters(v);

            int minPos = -1;
            foreach (var a in this.list)
            {
                if (a.Id == id)
                {
                    throw new Exception($"Error! Id #{id} exists");
                }

                if (a.Id < id && minPos < a.Id)
                {
                    minPos = a.Id;
                }
            }

            FileCabinetRecord record = new ()
            {
                    Id = id,
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    DateOfBirth = v.DateOfBirth,
                    Property1 = v.Property1,
                    Property2 = v.Property2,
                    Property3 = v.Property3,
            };

            int realPosition = minPos > 0 ? this.FindRealPosition(minPos) : minPos;
            this.list.Insert(realPosition + 1, record);

            return id;
        }

        private int FindRealPosition(int id)
        {
            int realPosition = 0;
            for (int i = 0; i < this.list.Count; i++)
            {
                if (id == this.list[i].Id)
                {
                    realPosition = i;
                }
            }

            return realPosition;
        }
    }
}
