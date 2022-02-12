using System.Collections.ObjectModel;

namespace FileCabinetApp.RecordIterator
{
    /// <summary>
    /// Declares a methods for IRecordIterator.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Move forward to next element.
        /// </summary>
        /// <returns>next element.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Checks the end of the file.
        /// </summary>
        /// <returns>boolean.</returns>
        public bool HasMore();

        /// <summary>
        /// Creates a new list with found records.
        /// </summary>
        /// <param name="whatFind">the parameter for finding.</param>
        /// <returns>the list with found records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetFindList(string whatFind);
    }
}
