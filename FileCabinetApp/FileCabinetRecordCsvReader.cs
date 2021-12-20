using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Deserializes records from the CSV format.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="reader">Stream for recording.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from the stream.
        /// </summary>
        /// <returns>The list of deserialized records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> readIList = new ();

            string line = this.reader.ReadLine();
            while ((line = this.reader.ReadLine()) != null)
            {
                try
                {
                    string[] recordValues = line.Split(',');
                    var record = new FileCabinetRecord
                    {
                        Id = int.Parse(recordValues[0][1..]),
                        FirstName = recordValues[1],
                        LastName = recordValues[2],
                        DateOfBirth = DateTime.Parse(recordValues[3]),
                        Property1 = short.Parse(recordValues[4]),
                        Property2 = decimal.Parse(recordValues[5]),
                        Property3 = char.Parse(recordValues[6]),
                    };
                    readIList.Add(record);
                }
                catch
                {
                    continue;
                }
            }

            return readIList;
        }
    }
}
