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

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

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
        /// Creates records in the List and the Dictionary.
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

            this.CreateRecordInDictionary(this.firstNameDictionary, v.FirstName);
            this.CreateRecordInDictionary(this.lastNameDictionary, v.LastName);
            this.CreateRecordInDictionary(this.dateOfBirthDictionary, v.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));

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
        /// Changes old record on the new one in the Dictionary and List.
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

            this.EditDictionary(this.firstNameDictionary, v.FirstName, this.list[realPosition].FirstName, record, realPosition);
            this.EditDictionary(this.lastNameDictionary, v.LastName, this.list[realPosition].LastName, record, realPosition);
            this.EditDictionary(this.dateOfBirthDictionary, v.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), this.list[realPosition].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record, realPosition);

            this.list.RemoveAt(realPosition);
            this.list.Insert(realPosition, record);
        }

        /// <summary>
        /// Finds records in the Dictionary by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return FindByKey(this.firstNameDictionary, firstName);
        }

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return FindByKey(this.lastNameDictionary, lastName);
        }

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return FindByKey(this.dateOfBirthDictionary, dateOfBirth);
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

                if (this.list.Count != 0 && this.list[^1].Id >= a.Id)
                {
                    this.EditRecord(a.Id, paramobj);
                    count++;
                }
                else
                {
                    this.list.Add(a);

                    this.CreateRecordInDictionary(this.firstNameDictionary, a.FirstName);
                    this.CreateRecordInDictionary(this.lastNameDictionary, a.LastName);
                    this.CreateRecordInDictionary(this.dateOfBirthDictionary, a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));

                    count++;
                }
            }
        }

        /// <summary>
        /// Finds records in the Dictionary by key.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <returns>The collection of records found by <paramref name="currentDictKey"/>.</returns>
        private static ReadOnlyCollection<FileCabinetRecord> FindByKey(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string currentDictKey)
        {
            List<FileCabinetRecord> onlyList = new ();
            ReadOnlyCollection<FileCabinetRecord> onlyCollection;
            string appropriateFormat;
            if (DateTime.TryParse(currentDictKey, out DateTime appropriateValue))
            {
                string str = appropriateValue.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                appropriateFormat = string.Concat(str[..6].ToUpper(), str[6..].ToLower());
            }
            else
            {
                appropriateFormat = string.Concat(currentDictKey[..1].ToUpper(), currentDictKey[1..].ToLower());
            }

            if (nameOfDict.ContainsKey(appropriateFormat))
            {
                onlyList = nameOfDict[appropriateFormat];
                onlyCollection = new (onlyList);
                return onlyCollection;
            }

            onlyCollection = new (onlyList);
            return onlyCollection;
        }

        /// <summary>
        /// Changes old record on the new one in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">Name of the Dictionary which has to be changed.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <param name="record">The new record which changes the old record.</param>
        /// <param name="realPosition">Id of old record.</param>
        private void EditDictionary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey, string currentDictKey, FileCabinetRecord record, int realPosition)
        {
            int indexCurrent = nameOfDict[currentDictKey].IndexOf(this.list[realPosition]);
            nameOfDict[currentDictKey].RemoveAt(indexCurrent);
            if (nameOfDict[currentDictKey].Count == 0)
            {
                nameOfDict.Remove(currentDictKey);
            }

            if (!nameOfDict.ContainsKey(newDictKey))
            {
                nameOfDict[newDictKey] = new List<FileCabinetRecord> { record };
            }
            else
            {
                nameOfDict[newDictKey].Add(record);
                nameOfDict[newDictKey].Sort(CompareId.CompareWithID);
            }
        }

        /// <summary>
        /// Creates record in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        private void CreateRecordInDictionary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey)
        {
            if (!nameOfDict.ContainsKey(newDictKey))
            {
                nameOfDict[newDictKey] = new List<FileCabinetRecord> { this.list[^1] };
            }
            else
            {
                nameOfDict[newDictKey].Add(this.list[^1]);
            }
        }

        /// <summary>
        /// Contains comparison method.
        /// </summary>
        private static class CompareId
        {
            /// <summary>
            /// Compares two integer instances.
            /// </summary>
            /// <param name="x">First integer instance.</param>
            /// <param name="y">Second integer instance.</param>
            /// <returns>The indication relative values of integer instances.</returns>
            public static int CompareWithID(FileCabinetRecord x, FileCabinetRecord y)
            {
                int val = x.Id.CompareTo(y.Id);
                return (val != 0) ? val : 0;
            }
        }
    }
}
