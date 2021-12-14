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
        /// Saves the state to the CSV format.
        /// </summary>
        /// <param name="writer">Stream for writing state.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter objFileCabRec = new (writer);

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
    }
}