using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var record = new FileCabinetRecord
            {
                Id = (int)((this.fileStream.Position / this.recordSize) + 1),
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
                memoryStream.Seek(offsetShift + this.offset[1], SeekOrigin.Begin);
                int readerId = binaryReader.ReadInt32();

                char[] firstNameChars = binaryReader.ReadChars(this.offset[3] - this.offset[2]);
                StringBuilder strBuilderFirstName = new ();
                foreach (var a in firstNameChars)
                {
                    if (char.IsLetterOrDigit(a))
                    {
                        strBuilderFirstName.Append(a);
                    }
                }

                string readerFirstName = strBuilderFirstName.ToString();

                char[] ch = binaryReader.ReadChars(this.offset[4] - this.offset[3]);
                StringBuilder strBuilderLastName = new ();
                foreach (var a in ch)
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

            byte[] recordBytes = WriteToBytes(record, this.offset, this.recordSize);
            this.fileStream.Seek((id * this.recordSize) - this.recordSize, SeekOrigin.Begin);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the current state inside a FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Snapshot of object, where the FileCabinetService passes its state to the FileCabinetServiceSnapshot's constructor parameters.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
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
    }
}