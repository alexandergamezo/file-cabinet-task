using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Reacts to user commands and executes some commands.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="fileStream">Stream for recording.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Creates records in the List and the Dictionary.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(ParameterObject v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets records from the List and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the number of records in the List.
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
    }
}