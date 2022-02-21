using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.RecordIterator;

namespace FileCabinetApp
{
    /// <summary>
    /// The IFileCabinetService interface uses for different services.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates records.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        int CreateRecord(ParameterObject v);

        /// <summary>
        /// Gets records from the List and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Defines the number of records in the List.
        /// </summary>
        /// <returns>The number of records.</returns>
        int GetStat();

        /// <summary>
        /// Changes old record on the new one.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        void EditRecord(int id, ParameterObject v);

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds records last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Saves the current state inside a FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Snapshot of object, where the FileCabinetService passes its state to the FileCabinetServiceSnapshot's constructor parameters.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the state from a snapshot object.
        /// </summary>
        /// <param name="snapshot">Snapshot of the object, where saved its state.</param>
        /// <param name="count">Number of records that were imported with definite validation rules.</param>
        void Restore(FileCabinetServiceSnapshot snapshot, out int count);

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void RemoveRecord(int id);

        /// <summary>
        /// Defrags a file.
        /// </summary>
        /// <param name="destinationFileName">The destination file name.</param>
        /// <param name="numNewRecords">The number of new records in a new file.</param>
        /// <param name="numOldRecords">The number of old records in an old file.</param>
        public void DefragFile(string destinationFileName, out int numNewRecords, out int numOldRecords);
    }
}