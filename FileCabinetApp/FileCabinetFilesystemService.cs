using System;
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
        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private readonly int recordSize = 279;
        private readonly int[] offset = { 0, 2, 6, 126, 246, 250, 254, 258, 262, 278 };

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

            static byte[] FuncRecordToByte(FileCabinetRecord fileCabinetRecord, int[] offset, int recordSize)
            {
                int writerId = fileCabinetRecord.Id;
                char[] writerFirstName = fileCabinetRecord.FirstName.ToCharArray();
                char[] writerLastName = fileCabinetRecord.LastName.ToCharArray();
                int writerFirstDateOfBirth = fileCabinetRecord.DateOfBirth.Year;
                int writerSecondDateOfBirth = fileCabinetRecord.DateOfBirth.Month;
                int writerThirdDateOfBirth = fileCabinetRecord.DateOfBirth.Day;
                short writerProperty1 = fileCabinetRecord.Property1;
                decimal writerProperty2 = fileCabinetRecord.Property2;
                char writerProperty3 = fileCabinetRecord.Property3;

                byte[] bytes = new byte[recordSize];
                using (MemoryStream memoryStream = new (bytes))
                using (BinaryWriter binaryWriter = new (memoryStream))
                {
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
                    binaryWriter.Write(writerFirstDateOfBirth);

                    binaryWriter.Seek(offset[5], SeekOrigin.Begin);
                    binaryWriter.Write(writerSecondDateOfBirth);

                    binaryWriter.Seek(offset[6], SeekOrigin.Begin);
                    binaryWriter.Write(writerThirdDateOfBirth);

                    binaryWriter.Seek(offset[7], SeekOrigin.Begin);
                    binaryWriter.Write(writerProperty1);

                    binaryWriter.Seek(offset[8], SeekOrigin.Begin);
                    binaryWriter.Write(writerProperty2);

                    binaryWriter.Seek(offset[9], SeekOrigin.Begin);
                    binaryWriter.Write(writerProperty3);
                }

                return bytes;
            }

            byte[] recordBytes = FuncRecordToByte(record, this.offset, this.recordSize);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the number of records in the file.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes old record on the new one in the Dictionary and List.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        public void EditRecord(int id, ParameterObject v)
        {
            throw new NotImplementedException();
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
    }
}