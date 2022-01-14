using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Declares methods for outputting.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Outputs collection to the console.
        /// </summary>
        /// <param name="records">Collection.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var a in records)
            {
                Console.WriteLine($"#{a.Id}, {a.FirstName}, {a.LastName}, {a.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, {a.Property1}, {a.Property2}, {a.Property3}");
            }
        }
    }
}
