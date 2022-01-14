using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Declares methods for outputting.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Outputs collection to the console.
        /// </summary>
        /// <param name="records">Collection.</param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
