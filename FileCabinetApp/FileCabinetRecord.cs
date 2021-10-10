using System;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Property1 { get; set; }

        public decimal Property2 { get; set; }

        public char Property3 { get; set; }
    }
}
