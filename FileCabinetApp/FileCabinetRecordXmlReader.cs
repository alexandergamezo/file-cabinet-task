using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FileCabinetGenerator;

namespace FileCabinetApp
{
    /// <summary>
    /// Deserializes records from the XML format.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="reader">Stream for recording.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from the stream.
        /// </summary>
        /// <returns>The list of deserialized records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var serializer = new XmlSerializer(typeof(XmlExport));
            var obj = (XmlExport)serializer.Deserialize(this.reader);

            List<FileCabinetRecord> readIList = new ();

            foreach (var a in obj.List)
            {
                try
                {
                    var record = new FileCabinetRecord
                    {
                        Id = int.Parse(a.Id),
                        FirstName = a.Name.First,
                        LastName = a.Name.Last,
                        DateOfBirth = DateTime.Parse(a.DateOfBirth),
                        Property1 = short.Parse(a.Property1),
                        Property2 = decimal.Parse(a.Property2),
                        Property3 = char.Parse(a.Property3),
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
