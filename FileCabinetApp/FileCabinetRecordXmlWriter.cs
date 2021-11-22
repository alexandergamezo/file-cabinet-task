using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Serializes record to the XML format.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// Constructor which gets the stream.
        /// </summary>
        /// <param name="writer">Stream for recording.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
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
                this.writer.WriteStartElement("record");
                this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));

                this.writer.WriteStartElement("name");
                this.writer.WriteAttributeString("first", record.FirstName);
                this.writer.WriteAttributeString("last", record.LastName);
                this.writer.WriteEndElement();

                this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                this.writer.WriteElementString("property1", record.Property1.ToString(CultureInfo.InvariantCulture));
                this.writer.WriteElementString("property2", record.Property2.ToString(CultureInfo.InvariantCulture));
                this.writer.WriteElementString("property3", record.Property3.ToString(CultureInfo.InvariantCulture));

                this.writer.WriteEndElement();
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc);
            }
        }
    }
}