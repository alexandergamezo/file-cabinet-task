using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// The FileCabinetServiceSnapshot contains the infrastructure for saving the FileCabinetService state.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;
        private IReadOnlyCollection<FileCabinetRecord> listLoadFromCsv;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// Constructor which gets FileCabinetService state.
        /// </summary>
        /// <param name="stateList">FileCabinetService state.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> stateList)
        {
            this.records = stateList.ToArray();
        }

        /// <summary>
        /// Gets a list of records from CSV file.
        /// </summary>
        /// <value>
        /// A list of records from CSV file.
        /// </value>
        public IReadOnlyCollection<FileCabinetRecord> Records => this.listLoadFromCsv;

        /// <summary>
        /// Saves the state to the CSV format.
        /// </summary>
        /// <param name="writer">Stream for writing state.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter objFileCabRec = new (writer);

            writer.WriteLine("Id,First Name,Last Name,Date of Birth,Property1,Property2,Property3");

            foreach (var record in this.records)
            {
                objFileCabRec.Write(record);
            }
        }

        /// <summary>
        /// Saves the state to the XML format.
        /// </summary>
        /// <param name="writer">Stream for writing state.</param>
        public void SaveToXmL(StreamWriter writer)
        {
            XmlWriterSettings settings = new ();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using XmlWriter xmlWriter = XmlWriter.Create(writer, settings);
            FileCabinetRecordXmlWriter objFileCabRecXml = new (xmlWriter);

            xmlWriter.WriteStartElement("records");
            foreach (var record in this.records)
            {
                objFileCabRecXml.Write(record);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        /// <summary>
        /// Loads records from the CSV file.
        /// </summary>
        /// <param name="reader">Stream for reading records.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCsvReader fileCabinetRecordCsvReader = new (reader);
            this.listLoadFromCsv = (IReadOnlyCollection<FileCabinetRecord>)fileCabinetRecordCsvReader.ReadAll();
        }
    }
}