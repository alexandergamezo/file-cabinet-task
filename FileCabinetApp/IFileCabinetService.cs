using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// The IFileCabinetService interface uses for different services.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates records in the List and the Dictionary.
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
        /// Changes old record on the new one in the Dictionary and List.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        void EditRecord(int id, ParameterObject v);

        /// <summary>
        /// Finds records in the Dictionary by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

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
    }
}