using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

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

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

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

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return FindByKey(this.firstNameDictionary, firstName);
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return FindByKey(this.lastNameDictionary, lastName);
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            return FindByKey(this.dateOfBirthDictionary, dateOfBirth);
        }

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

        public static class CompareID
        {
            public static int CompareWithID(FileCabinetRecord x, FileCabinetRecord y)
            {
                int val = x.Id.CompareTo(y.Id);
                return (val != 0) ? val : 0;
            }
        }
    }
}
