using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Reacts to user commands and executes some commands.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new ();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new ();

        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private readonly int recordSize = 277;
        private readonly int[] offset = { 0, 2, 6, 126, 246, 250, 254, 258, 260, 276 };

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="fileStream">Stream for recording.</param>
        /// <param name="validator">Reference to one of the strategy objects.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Creates records in the file.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(ParameterObject v)
        {
            this.validator.ValidateParameters(v);

            int recordId = 1;
            if (this.fileStream.Length != 0)
            {
                this.GetRecords();
                recordId = this.list[^1].Id + 1;
            }

            var record = new FileCabinetRecord
            {
                Id = recordId,
                FirstName = v.FirstName,
                LastName = v.LastName,
                DateOfBirth = v.DateOfBirth,
                Property1 = v.Property1,
                Property2 = v.Property2,
                Property3 = v.Property3,
            };

            byte[] recordBytes = WriteToBytes(record, this.offset, this.recordSize);
            this.fileStream.Write(recordBytes, 0, recordBytes.Length);
            this.fileStream.Flush();

            return record.Id;
        }

        /// <summary>
        /// Gets records from the file and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            if (this.list.Count != 0)
            {
                this.list.Clear();
                this.firstNameDictionary.Clear();
                this.lastNameDictionary.Clear();
                this.dateOfBirthDictionary.Clear();
            }

            byte[] readBytes = new byte[this.fileStream.Length];
            this.fileStream.Seek(0, SeekOrigin.Begin);
            this.fileStream.Read(readBytes, 0, readBytes.Length);
            using MemoryStream memoryStream = new (readBytes);
            using BinaryReader binaryReader = new (memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            int offsetShift = 0;
            while (memoryStream.Position <= (memoryStream.Length - this.recordSize))
            {
                memoryStream.Seek(offsetShift, SeekOrigin.Begin);

                short reserved = binaryReader.ReadInt16();
                if ((reserved >> 2 & 1) == 1)
                {
                    offsetShift += this.recordSize;
                    continue;
                }

                int readerId = binaryReader.ReadInt32();

                char[] firstNameChars = binaryReader.ReadChars(this.offset[3] - this.offset[2]);
                StringBuilder strBuilderFirstName = new ();
                foreach (char a in firstNameChars)
                    {
                        if (char.IsLetterOrDigit(a))
                        {
                            strBuilderFirstName.Append(a);
                        }
                    }

                string readerFirstName = strBuilderFirstName.ToString();

                char[] ch = binaryReader.ReadChars(this.offset[4] - this.offset[3]);
                StringBuilder strBuilderLastName = new ();
                foreach (char a in ch)
                    {
                        if (char.IsLetterOrDigit(a))
                        {
                            strBuilderLastName.Append(a);
                        }
                    }

                string readerLastName = strBuilderLastName.ToString();

                int readerYearDateOfBirth = binaryReader.ReadInt32();
                int readerMonthDateOfBirth = binaryReader.ReadInt32();
                int readerDayDateOfBirth = binaryReader.ReadInt32();

                string dateOfBirth = string.Concat(readerMonthDateOfBirth, "/", readerDayDateOfBirth, "/", readerYearDateOfBirth);
                DateTime readerDateOfBirth = DateTime.Parse(dateOfBirth);

                short readerProperty1 = binaryReader.ReadInt16();
                decimal readerProperty2 = binaryReader.ReadDecimal();
                char readerProperty3 = binaryReader.ReadChar();

                var record = new FileCabinetRecord
                    {
                        Id = readerId,
                        FirstName = readerFirstName,
                        LastName = readerLastName,
                        DateOfBirth = readerDateOfBirth,
                        Property1 = readerProperty1,
                        Property2 = readerProperty2,
                        Property3 = readerProperty3,
                    };

                this.list.Add(record);

                this.CreateRecordInDictionary(this.firstNameDictionary, readerFirstName);
                this.CreateRecordInDictionary(this.lastNameDictionary, readerLastName);
                this.CreateRecordInDictionary(this.dateOfBirthDictionary, readerDateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));

                offsetShift += this.recordSize;
            }

            ReadOnlyCollection<FileCabinetRecord> onlyCollection = new (this.list);
            return onlyCollection;
        }

        /// <summary>
        /// Defines the number of records in the file.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            int count = (int)this.fileStream.Length / this.recordSize;
            return count;
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

            int realPosition = this.FindRealPosition(id);

            byte[] recordBytes = WriteToBytes(record, this.offset, this.recordSize);
            this.fileStream.Seek(realPosition, SeekOrigin.Begin);
            this.fileStream.Write(recordBytes, 0, recordBytes.Length);
            this.fileStream.Flush();
        }

        /// <summary>
        /// Finds records in the Dictionary by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.GetRecords();
            return FindByKey(this.firstNameDictionary, firstName);
        }

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.GetRecords();
            return FindByKey(this.lastNameDictionary, lastName);
        }

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.GetRecords();
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
            foreach (FileCabinetRecord a in snapshot.Records)
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

                this.GetRecords();

                bool exist = false;
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (a.Id == this.list[i].Id)
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    this.EditRecord(a.Id, paramobj);
                    count++;
                }
                else
                {
                    byte[] recordBytes = WriteToBytes(a, this.offset, this.recordSize);
                    this.fileStream.Write(recordBytes, 0, recordBytes.Length);
                    this.fileStream.Flush();

                    count++;
                }
            }
        }

        /// <summary>
        /// Removes a record from the Dictionary and List.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void RemoveRecord(int id)
        {
            int realPosition = this.FindRealPosition(id);

            byte[] recordBytes = new byte[this.offset[1]];
            recordBytes[0] = (byte)(recordBytes[0] | 0b_0000_0100);

            this.fileStream.Seek(realPosition, SeekOrigin.Begin);
            this.fileStream.Write(recordBytes, 0, recordBytes.Length);
            this.fileStream.Flush();
        }

        /// <summary>
        /// Converts record in bytes.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="offset">An array of offset values for the record properties in the file.</param>
        /// <param name="recordSize">Record size.</param>
        /// <returns>Array with bytes.</returns>
        private static byte[] WriteToBytes(FileCabinetRecord record, int[] offset, int recordSize)
        {
            int writerId = record.Id;
            char[] writerFirstName = record.FirstName.ToCharArray();
            char[] writerLastName = record.LastName.ToCharArray();
            int writerYearDateOfBirth = record.DateOfBirth.Year;
            int writerMonthDateOfBirth = record.DateOfBirth.Month;
            int writerDayDateOfBirth = record.DateOfBirth.Day;
            short writerProperty1 = record.Property1;
            decimal writerProperty2 = record.Property2;
            char writerProperty3 = record.Property3;

            byte[] writeBytes = new byte[recordSize];
            using MemoryStream memoryStream = new (writeBytes);
            using BinaryWriter binaryWriter = new (memoryStream);
            binaryWriter.Seek(offset[1], SeekOrigin.Begin);
            binaryWriter.Write(writerId);

            binaryWriter.Seek(offset[2], SeekOrigin.Begin);
            UnicodeEncoding unicode = new ();
            byte[] firstNameBytes = unicode.GetBytes(writerFirstName);
            binaryWriter.Write(firstNameBytes);

            binaryWriter.Seek(offset[3], SeekOrigin.Begin);
            byte[] lastNameBytes = unicode.GetBytes(writerLastName);
            binaryWriter.Write(lastNameBytes);

            binaryWriter.Seek(offset[4], SeekOrigin.Begin);
            binaryWriter.Write(writerYearDateOfBirth);

            binaryWriter.Seek(offset[5], SeekOrigin.Begin);
            binaryWriter.Write(writerMonthDateOfBirth);

            binaryWriter.Seek(offset[6], SeekOrigin.Begin);
            binaryWriter.Write(writerDayDateOfBirth);

            binaryWriter.Seek(offset[7], SeekOrigin.Begin);
            binaryWriter.Write(writerProperty1);

            binaryWriter.Seek(offset[8], SeekOrigin.Begin);
            binaryWriter.Write(writerProperty2);

            binaryWriter.Seek(offset[9], SeekOrigin.Begin);
            binaryWriter.Write(writerProperty3);

            return writeBytes;
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

        private int FindRealPosition(int id)
        {
            byte[] readBytes = new byte[this.fileStream.Length];
            this.fileStream.Seek(0, SeekOrigin.Begin);
            this.fileStream.Read(readBytes, 0, readBytes.Length);
            using MemoryStream memoryStream = new (readBytes);
            using BinaryReader binaryReader = new (memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            int offsetShift = 0;
            int readerId = -1;
            while (memoryStream.Position <= (memoryStream.Length - this.recordSize))
            {
                memoryStream.Seek(offsetShift, SeekOrigin.Begin);

                short reserved = binaryReader.ReadInt16();
                if ((reserved >> 2 & 1) == 1)
                {
                    offsetShift += this.recordSize;
                    continue;
                }

                readerId = binaryReader.ReadInt32();
                if (readerId == id)
                {
                    break;
                }

                offsetShift += this.recordSize;
            }

            return offsetShift;
        }

        /// <summary>
        /// Creates record in the Dictionary.
        /// </summary>
        /// <param name="nameOfDict">The name of the Dictionary in which searches records by key.</param>
        /// <param name="newDictKey">New Dictionary key.</param>
        private void CreateRecordInDictionary(Dictionary<string, List<FileCabinetRecord>> nameOfDict, string newDictKey)
        {
            string appropriateNewDictKey = string.Concat(newDictKey[..1].ToUpper(), newDictKey[1..].ToLower());

            if (!nameOfDict.ContainsKey(appropriateNewDictKey))
            {
                nameOfDict[appropriateNewDictKey] = new List<FileCabinetRecord> { this.list[^1] };
            }
            else
            {
                nameOfDict[appropriateNewDictKey].Add(this.list[^1]);
            }
        }
    }
}