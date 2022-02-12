using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.RecordIterator
{
    /// <summary>
    /// FilesystemIterator.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private readonly string parameter;
        private readonly int recordSize;
        private readonly int[] offset;
        private readonly FileStream fileStream;
        private readonly Func<BinaryReader, int[], FileCabinetRecord> readBinaryRecord;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="collection">collection.</param>
        /// <param name="parameter">parameter.</param>
        /// <param name="recordSize">recordSize.</param>
        /// <param name="offset">offset.</param>
        /// <param name="readBinaryRecord">Func Delegate.</param>
        public FilesystemIterator(
            FileCabinetFilesystemService collection,
            string parameter,
            int recordSize,
            int[] offset,
            Func<BinaryReader, int[], FileCabinetRecord> readBinaryRecord)
        {
            this.parameter = parameter;
            this.recordSize = recordSize;
            this.offset = offset;
            this.fileStream = collection.GetFileStream();
            this.readBinaryRecord = readBinaryRecord;
        }

        /// <summary>
        /// Move forward to next element.
        /// </summary>
        /// <returns>next element.</returns>
        public FileCabinetRecord GetNext()
        {
            byte[] readBytes = new byte[this.fileStream.Length];
            this.fileStream.Seek(0, SeekOrigin.Begin);
            this.fileStream.Read(readBytes, 0, readBytes.Length);
            using MemoryStream memoryStream = new (readBytes);
            using BinaryReader binaryReader = new (memoryStream);
            memoryStream.Seek(this.position, SeekOrigin.Begin);

            binaryReader.ReadInt16();
            FileCabinetRecord record = this.readBinaryRecord(binaryReader, this.offset);

            return record;
        }

        /// <summary>
        /// Checks the end of the file.
        /// </summary>
        /// <returns>boolean.</returns>
        public bool HasMore()
        {
            if (this.position < this.fileStream.Length)
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
                this.position += this.recordSize;

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
