using System;
using System.IO;
using System.Globalization;

namespace FileCabinetGenerator
{
    static class Program
    {
        private static void CsvWrite(string path, int id, int amount)
        {
            using StreamWriter writer = new(path);
            while (id <= amount)
            {
                FileCabinetRecord record = GenerateData(id);

                writer.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},{record.Property1},{record.Property2},{record.Property3}");

                id++;
            }

            string[] fileName = path.Split('\\');
            Console.WriteLine($"{amount} records were written to {fileName[^1]}.");
        }

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

        static void Main(string[] args)
        {
            string path = string.Empty;
            int id;
            int amount;
            string format;

            try
            {
                bool paramVariantOne = args[0][0..14].ToLowerInvariant().Equals("--output-type=") && args[1][0..9].ToLowerInvariant().Equals("--output=") &&
                                       args[2][0..17].ToLowerInvariant().Equals("--records-amount=") && args[3][0..11].ToLowerInvariant().Equals("--start-id=");

                bool paramVariantTwo = args[0].ToLowerInvariant().Equals("-t") && args[2].ToLowerInvariant().Equals("-o") &&
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