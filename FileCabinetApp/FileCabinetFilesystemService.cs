using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.RecordIterator;

namespace FileCabinetApp
{
    /// <summary>
    /// Reacts to user commands and executes some commands.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IEnumerable<FileCabinetRecord>
    {
        private const string SourceFileName = "temp.db";
        private const string DestinationBackupFileName = "cabinet-records.db.bac";
        private readonly string filename;

        private readonly List<FileCabinetRecord> list = new ();

        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private readonly int recordSize = 277;
        private readonly int[] offset = { 0, 2, 6, 126, 246, 250, 254, 258, 260, 276 };
        private readonly Func<BinaryReader, int[], FileCabinetRecord> readBinaryRecord = ReadRecordByBinaryReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="fileStream">Stream for recording.</param>
        /// <param name="validator">Reference to one of the strategy objects.</param>
        /// <param name="filename">Filename.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator, string filename)
        {
            this.fileStream = fileStream;
            this.validator = validator;
            this.filename = filename;
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

                FileCabinetRecord record = ReadRecordByBinaryReader(binaryReader, this.offset);

                this.list.Add(record);

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

            this.GetRecords();
            Console.WriteLine($"{count - this.list.Count} deleted record(s).");
            return count;
        }

        /// <summary>
        /// Changes old record on the new one.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        public void UpdateRecord(int id, ParameterObject v)
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
                    this.UpdateRecord(a.Id, paramobj);
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
        /// Removes a record.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void DeleteRecord(int id)
        {
            int realPosition = this.FindRealPosition(id);

            byte[] recordBytes = new byte[this.offset[1]];
            recordBytes[0] = (byte)(recordBytes[0] | 0b_0000_0100);

            this.fileStream.Seek(realPosition, SeekOrigin.Begin);
            this.fileStream.Write(recordBytes, 0, recordBytes.Length);
            this.fileStream.Flush();
        }

        /// <summary>
        /// Defrags a file.
        /// </summary>
        /// <param name="destinationFileName">The destination file name.</param>
        /// <param name="numNewRecords">The number of new records in a new file.</param>
        /// <param name="numOldRecords">The number of old records in an old file.</param>
        public void DefragFile(string destinationFileName, out int numNewRecords, out int numOldRecords)
        {
            using FileStream newFileStream = File.Open(destinationFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            byte[] readBytes = new byte[this.fileStream.Length];
            this.fileStream.Seek(0, SeekOrigin.Begin);
            this.fileStream.Read(readBytes, 0, readBytes.Length);
            using MemoryStream memoryStream = new (readBytes);
            using BinaryReader binaryReader = new (memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            int offsetShift = 0;
            numNewRecords = 0;
            numOldRecords = (int)this.fileStream.Length / this.recordSize;

            while (memoryStream.Position <= (memoryStream.Length - this.recordSize))
            {
                memoryStream.Seek(offsetShift, SeekOrigin.Begin);

                short reserved = binaryReader.ReadInt16();
                if ((reserved >> 2 & 1) == 1)
                {
                    offsetShift += this.recordSize;
                    continue;
                }

                byte[] newBytesRecord = new byte[this.recordSize];
                Array.Copy(readBytes, offsetShift, newBytesRecord, 0, this.recordSize);

                newFileStream.Write(newBytesRecord, 0, newBytesRecord.Length);

                offsetShift += this.recordSize;
                numNewRecords++;
            }

            newFileStream.Flush();
            this.fileStream.Dispose();
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

            int minId = id;
            long minIdPos = 0;

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

                if (readerId == id)
                {
                    throw new Exception($"Error! Id #{id} exists");
                }
                else if (readerId > id)
                {
                    break;
                }
                else if (readerId < id)
                {
                    minId = readerId;
                    minIdPos = offsetShift;
                }

                offsetShift += this.recordSize;
            }

            if (minId == id)
            {
                using FileStream newFileStream = File.Open(SourceFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                byte[] newBytesRecord = new byte[readBytes.Length + this.recordSize];

                byte[] recordBytes = WriteToBytes(record, this.offset, this.recordSize);
                Array.Copy(recordBytes, 0, newBytesRecord, 0, this.recordSize);
                Array.Copy(readBytes, 0, newBytesRecord, this.recordSize, readBytes.Length);

                newFileStream.Write(newBytesRecord, 0, newBytesRecord.Length);

                newFileStream.Flush();
                newFileStream.Dispose();

                this.fileStream.Dispose();
            }
            else if (minId < id)
            {
                using FileStream newFileStream = File.Open(SourceFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                byte[] newBytesRecord = new byte[readBytes.Length + this.recordSize];
                Array.Copy(readBytes, 0, newBytesRecord, 0, minIdPos + this.recordSize);

                byte[] recordBytes = WriteToBytes(record, this.offset, this.recordSize);
                Array.Copy(recordBytes, 0, newBytesRecord, minIdPos + this.recordSize, this.recordSize);

                Array.Copy(readBytes, minIdPos + this.recordSize, newBytesRecord, minIdPos + (this.recordSize * 2), readBytes.Length - (minIdPos + this.recordSize));

                newFileStream.Write(newBytesRecord, 0, newBytesRecord.Length);

                newFileStream.Flush();
                newFileStream.Dispose();

                this.fileStream.Dispose();
            }

            File.Replace(SourceFileName, this.filename, DestinationBackupFileName);

            return id;
        }

        /// <summary>
        /// Returns fileStream.
        /// </summary>
        /// <returns>fileStream.</returns>
        public FileStream GetFileStream()
        {
            return this.fileStream;
        }

        /// <summary>
        /// Creates a new list with found records.
        /// </summary>
        /// <param name="whatFind">the parameter for finding.</param>
        /// <param name="parameter">firstName, lastName or dateOfBirth.</param>
        /// <returns>the list with found records.</returns>
        public IEnumerable<FileCabinetRecord> GetFindList(string whatFind, string parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException($"Check your input. {nameof(parameter)} can't be null.");
            }

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

            IEnumerator<FileCabinetRecord> b = this.GetEnumerator();

            while (b.MoveNext())
            {
                FileCabinetRecord record = b.Current;

                if (record == null)
                {
                    continue;
                }

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
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemIterator(this, this.recordSize, this.offset, this.readBinaryRecord);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumearator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

        private static FileCabinetRecord ReadRecordByBinaryReader(BinaryReader binaryReader, int[] offset)
        {
            int readerId = binaryReader.ReadInt32();

            char[] firstNameChars = binaryReader.ReadChars(offset[3] - offset[2]);
            StringBuilder strBuilderFirstName = new ();
            foreach (char a in firstNameChars)
            {
                if (char.IsLetterOrDigit(a))
                {
                    strBuilderFirstName.Append(a);
                }
            }

            string readerFirstName = strBuilderFirstName.ToString();

            char[] ch = binaryReader.ReadChars(offset[4] - offset[3]);
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

            FileCabinetRecord record = new ()
            {
                Id = readerId,
                FirstName = readerFirstName,
                LastName = readerLastName,
                DateOfBirth = readerDateOfBirth,
                Property1 = readerProperty1,
                Property2 = readerProperty2,
                Property3 = readerProperty3,
            };

            return record;
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
                if (readerId == id)
                {
                    break;
                }

                offsetShift += this.recordSize;
            }

            return offsetShift;
        }
    }
}