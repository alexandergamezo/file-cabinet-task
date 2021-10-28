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

        /// <summary>
        /// Checks input parameters on the wrong values.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="property1">Property in "short" format.</param>
        /// <param name="property2">Property in "decimal" format.</param>
        /// <param name="property3">Property in "char" format.</param>
        public static void CheckInputParameters(string firstName, string lastName, DateTime dateOfBirth, short property1, decimal property2, char property3)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName), $"{nameof(firstName)} is null or empty");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("All names must be more or equal than 2-letters and less or equal than 60-letters", nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName), $"{nameof(lastName)} is null or empty");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("All names must be more or equal than 2-letters and less or equal than 60-letters", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 01, 01) || dateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Source string has the wrong value", nameof(dateOfBirth));
            }

            if (property1 < short.MinValue || property1 > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(property1), "value is not a <short> number");
            }

            if (property2 < decimal.MinValue || property2 > decimal.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(property2), "value is not a <decimal> number");
            }

            if (!char.IsLetter(property3))
            {
                throw new ArgumentOutOfRangeException(nameof(property3), "value is not a <char> letter");
            }
        }

        /// <summary>
        /// Changes old record on the new one in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">Name of the Dictionary which has to be changed.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <param name="record">The new record which changes the old record.</param>
        /// <param name="id">Id of old record.</param>
        public static void EditDictinary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey, string currentDictKey, FileCabinetRecord record, int id)
        {
            FileCabinetService obj = new ();

            int indexCurrent = nameOfDict[currentDictKey].IndexOf(obj.list[id - 1]);
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
                nameOfDict[newDictKey].Sort(CompareID.CompareWithID);
            }
        }

        /// <summary>
        /// Finds records in the Dictionary by key.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="currentDictKey">Current Dictionary key.</param>
        /// <returns>The array of records found by <paramref name="currentDictKey"/>.</returns>
        public static FileCabinetRecord[] FindByKey(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string currentDictKey)
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
        /// Creates records in the List and the Dictionary.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="property1">Property in "short" format.</param>
        /// <param name="property2">Property in "decimal" format.</param>
        /// <param name="property3">Property in "char" format.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short property1, decimal property2, char property3)
        {
            CheckInputParameters(firstName, lastName, dateOfBirth, property1, property2, property3);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Property1 = property1,
                Property2 = property2,
                Property3 = property3,
            };

            this.list.Add(record);

            this.CreateRecordInDictionary(this.firstNameDictionary, firstName);
            this.CreateRecordInDictionary(this.lastNameDictionary, lastName);
            this.CreateRecordInDictionary(this.dateOfBirthDictionary, dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));

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
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <param name="property1">Property in "short" format.</param>
        /// <param name="property2">Property in "decimal" format.</param>
        /// <param name="property3">Property in "char" format.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short property1, decimal property2, char property3)
        {
            CheckInputParameters(firstName, lastName, dateOfBirth, property1, property2, property3);

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Property1 = property1,
                Property2 = property2,
                Property3 = property3,
            };

            EditDictinary(this.firstNameDictionary, firstName, this.list[id - 1].FirstName, record, id);
            EditDictinary(this.lastNameDictionary, lastName, this.list[id - 1].LastName, record, id);
            EditDictinary(this.dateOfBirthDictionary, dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), this.list[id - 1].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record, id);

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
        /// Creates record in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        public void CreateRecordInDictionary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey)
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
        public static class CompareID
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
