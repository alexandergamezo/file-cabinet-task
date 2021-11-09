using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reacts to user commands and executes some commands.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// Provides a setter to change a strategy at runtime.
        /// </summary>
        /// <param name="validator">Reference to one of the strategy objects.</param>
        public FileCabinetService(IRecordValidator validator)
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

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = v.FirstName,
                LastName = v.LastName,
                DateOfBirth = v.DateOfBirth,
                Property1 = v.Property1,
                Property2 = v.Property2,
                Property3 = v.Property3,
            };

            this.list.Add(record);

            this.CreateRecordInDictionary(this.firstNameDictionary, v.FirstName);
            this.CreateRecordInDictionary(this.lastNameDictionary, v.LastName);
            this.CreateRecordInDictionary(this.dateOfBirthDictionary, v.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));

            return record.Id;
        }

        /// <summary>
        /// Gets records from the List and puts them into an array.
        /// </summary>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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

            this.EditDictionary(this.firstNameDictionary, v.FirstName, this.list[id - 1].FirstName, record, id);
            this.EditDictionary(this.lastNameDictionary, v.LastName, this.list[id - 1].LastName, record, id);
            this.EditDictionary(this.dateOfBirthDictionary, v.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), this.list[id - 1].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record, id);

            this.list.RemoveAt(id - 1);
            this.list.Insert(id - 1, record);
        }

        /// <summary>
        /// Finds records in the Dictionary by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The array of records found by the <paramref name="firstName"/>.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return FindByKey(this.firstNameDictionary, firstName);
        }

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The array of records which by the <paramref name="lastName"/>.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return FindByKey(this.lastNameDictionary, lastName);
        }

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The array of records found by <paramref name="dateOfBirth"/>.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            return FindByKey(this.dateOfBirthDictionary, dateOfBirth);
        }

        /// <summary>
        /// Finds records in the Dictionary by key.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <returns>The array of records found by <paramref name="currentDictKey"/>.</returns>
        private static FileCabinetRecord[] FindByKey(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string currentDictKey)
        {
            FileCabinetRecord[] arr = Array.Empty<FileCabinetRecord>();
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
                arr = new FileCabinetRecord[nameOfDict[appropriateFormat].Count];
                nameOfDict[appropriateFormat].CopyTo(arr, 0);
                return arr;
            }

            return arr;
        }

        /// <summary>
        /// Changes old record on the new one in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">Name of the Dictionary which has to be changed.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <param name="record">The new record which changes the old record.</param>
        /// <param name="id">Id of old record.</param>
        private void EditDictionary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey, string currentDictKey, FileCabinetRecord record, int id)
        {
            int indexCurrent = nameOfDict[currentDictKey].IndexOf(this.list[id - 1]);
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
