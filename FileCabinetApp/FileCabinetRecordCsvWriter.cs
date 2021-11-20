using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Serializes record to the CSV format.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="writer">Stream for recording.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes a string to the stream.
        /// </summary>
        /// <param name="record">State for recording.</param>
        public void Write(FileCabinetRecord record)
        {
            try
            {
                this.writer.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},{record.Property1},{record.Property2},{record.Property3}");
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc);
            }
        }
    }
}