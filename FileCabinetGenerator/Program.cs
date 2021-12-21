using System;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace FileCabinetGenerator
{
    [XmlRoot("records")]
    public class XmlExport
    {     
        [XmlElement("record")]
        public List<XmlRecord> List { get; set; }
        public XmlExport()
        { }

        public XmlExport(List<XmlRecord> list)
        {              
            List = list;
        }
    }
        
    public class XmlRecord
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("name")]
        public Name Name { get; set; }
        
        [XmlElement("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [XmlElement("property1")]
        public string Property1 { get; set; }

        [XmlElement("property2")]
        public string Property2 { get; set; }

        [XmlElement("property3")]
        public string Property3 { get; set; }

        public XmlRecord()
        { }
        public XmlRecord(FileCabinetRecord record)
        {
            Id = record.Id.ToString(CultureInfo.InvariantCulture);
            Name = new Name(record);
            DateOfBirth = record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            Property1 = record.Property1.ToString(CultureInfo.InvariantCulture);
            Property2 = record.Property2.ToString(CultureInfo.InvariantCulture);
            Property3 = record.Property3.ToString(CultureInfo.InvariantCulture);                        
        }
    }

    public class Name
    {
        [XmlAttribute("first")]
        public string First { get; set; }

        [XmlAttribute("last")]
        public string Last { get; set; }

        public Name()
        { }

        public Name(FileCabinetRecord record)
        {
            First = record.FirstName;
            Last = record.LastName;
        }
    }
    
    public static class Program
    {
        private static FileCabinetRecord GenerateData(int id)
        {
            Random rnd = new();

            string[] firstNames = { "Rufus", "Bear", "Dakota", "Fido", "Vanya", "Samuel", "Koani", "Volodya",
                                    "Prince", "Yiska" , "Antonyo", "Kim", "Barbara", "Harry"};
            string firstName = firstNames[rnd.Next(0, firstNames.Length)];

            string[] lastNames = { "Maggie", "Penny", "Saya", "Princess", "Abby", "Laila", "Sadie", "Olivia",
                                   "Starlight", "Talla" , "Fagundes", "Streisand", "Chen", "Potter"};
            string lastName = lastNames[rnd.Next(0, lastNames.Length)];


            int year = rnd.Next(1950, DateTime.Today.Year + 1);
            int month = rnd.Next(1, 13);
            int day = rnd.Next(1, DateTime.DaysInMonth(year, month) + 1);
            DateTime dateOfBirth = new(year, month, day);

            short property1 = (short)rnd.Next(0, (int)short.MaxValue + 1);
            decimal property2 = (decimal)rnd.Next(0, int.MaxValue);

            int startSymbolCode = 65;
            int endSymbolCode = 90;
            char property3 = (char)rnd.Next(startSymbolCode, endSymbolCode + 1);

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Property1 = property1,
                Property2 = property2,
                Property3 = property3,
            };
            return record;
        }
        
        private static void CsvWrite(string path, int id, int amount)
        {
            using StreamWriter writer = new(path);
            writer.WriteLine("Id,First Name,Last Name,Date of Birth,Property1,Property2,Property3");
            int newId = id;
            while (newId <= id + amount - 1)
            {
                FileCabinetRecord record = GenerateData(newId);

                writer.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},{record.Property1},{record.Property2},{record.Property3}");

                newId++;
            }

            string[] fileName = path.Split('\\');
            Console.WriteLine($"{amount} records were written to {fileName[^1]}.");
        }

        private static void XmlWrite(string path, int id, int amount)
        {
            XmlWriterSettings settings = new();
            settings.Indent = true;
            settings.IndentChars = "\t";            

            XmlSerializer serializer = new (typeof(XmlExport));
            XmlWriter writer = XmlWriter.Create(path, settings);
            
            List<XmlRecord> list = new();
            int newId = id;

            while (newId <= id + amount - 1)
            {
                FileCabinetRecord record = GenerateData(newId);
                XmlRecord xmlRecord = new(record);
                list.Add(xmlRecord);
                newId++;
            }

            XmlExport xmlExport = new (list);
            serializer.Serialize(writer, xmlExport);
            writer.Close();
            Console.WriteLine($"{amount} records were written to " + path);
        }

        static void Main(string[] args)
        {
            string path = string.Empty;
            int id;
            int amount;
            string format;

            try
            {
                bool paramVariantOne = args.Length == 4 && args[0][0..14].ToLowerInvariant().Equals("--output-type=") && args[1][0..9].ToLowerInvariant().Equals("--output=") &&
                                       args[2][0..17].ToLowerInvariant().Equals("--records-amount=") && args[3][0..11].ToLowerInvariant().Equals("--start-id=");

                bool paramVariantTwo = args.Length == 8 && args[0].ToLowerInvariant().Equals("-t") && args[2].ToLowerInvariant().Equals("-o") &&
                                       args[4].ToLowerInvariant().Equals("-a") && args[6].ToLowerInvariant().Equals("-i");

                

                if (paramVariantOne)
                {
                    path = args[1][9..];
                    id = int.Parse(args[3][11..]);
                    amount = int.Parse(args[2][17..]);
                    format = args[0][14..];
                    if (format.ToLowerInvariant().Equals("csv"))
                    {
                        CsvWrite(path, id, amount);
                    }

                    if (format.ToLowerInvariant().Equals("xml"))
                    {
                        
                        XmlWrite(path, id, amount);
                    }
                }
                else if (paramVariantTwo)
                {
                    path = args[3];
                    id = int.Parse(args[7]);
                    amount = int.Parse(args[5]);
                    format = args[1];
                    if (format.ToLowerInvariant().Equals("csv"))
                    {
                        CsvWrite(path, id, amount);
                    }

                    if (format.ToLowerInvariant().Equals("xml"))
                    {                        
                        XmlWrite(path, id, amount);
                    }
                }
                else
                {
                    Console.WriteLine("Command line parameter is wrong. Check your input");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Command line parameter is wrong. Check your input.");
                Console.WriteLine(exc);
            }

        }
    }
}