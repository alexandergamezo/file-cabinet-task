using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.RecordIterator
{
    /// <summary>
    /// MemoryIterator.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private readonly FileCabinetMemoryService collection;
        private readonly string parameter;
        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="collection">collection.</param>
        /// <param name="parameter">parameter.</param>
        public MemoryIterator(FileCabinetMemoryService collection, string parameter)
        {
            this.collection = collection;
            this.parameter = parameter;
        }

        /// <summary>
        /// Move forward to next element.
        /// </summary>
        /// <returns>next element.</returns>
        public FileCabinetRecord GetNext()
        {
            this.position++;
            return this.collection.GetRecords()[this.position];
        }

        /// <summary>
        /// Checks the end of the file.
        /// </summary>
        /// <returns>boolean.</returns>
        public bool HasMore()
        {
            if (this.position < this.collection.GetRecords().Count - 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new list with found records.
        /// </summary>
        /// <param name="whatFind">the parameter for finding.</param>
        /// <returns>the list with found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetFindList(string whatFind)
        {
            bool firstname = whatFind.Equals("firstname");
            bool lastname = whatFind.Equals("lastname");
            bool dateofbirth = whatFind.Equals("dateofbirth");

            List<FileCabinetRecord> onlyList = new ();
            ReadOnlyCollection<FileCabinetRecord> onlyCollection;
            string appropriateFormat;
            if (dateofbirth && DateTime.TryParse(this.parameter, out DateTime appropriateValue))
            {
                string str = appropriateValue.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                appropriateFormat = string.Concat(str[..6].ToUpper(), str[6..].ToLower());
            }
            else
            {
                appropriateFormat = string.Concat(this.parameter[..1].ToUpper(), this.parameter[1..].ToLower());
            }

            while (this.HasMore())
            {
                FileCabinetRecord record = this.GetNext();

                if (firstname && record.FirstName.Equals(appropriateFormat))
                {
                    onlyList.Add(record);
                }

                if (lastname && record.LastName.Equals(appropriateFormat))
                {
                    onlyList.Add(record);
                }

                if (dateofbirth && record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).Equals(appropriateFormat))
                {
                    onlyList.Add(record);
                }
            }

            onlyCollection = new (onlyList);

            return onlyCollection;
        }
    }
}
