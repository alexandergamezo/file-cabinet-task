using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short property1, decimal property2, char property3)
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

            this.list.RemoveAt(id - 1);
            this.list.Insert(id - 1, record);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> findResults = this.list.FindAll(FuncFindFirstName);

            bool FuncFindFirstName(FileCabinetRecord record)
            {
                if (record.FirstName.ToLower() == firstName.ToLower())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return findResults.ToArray();
        }
    }
}
