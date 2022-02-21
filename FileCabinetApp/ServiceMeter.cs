using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a set of methods that determine the execution time.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Object reference.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Creates records.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(ParameterObject v)
        {
            Stopwatch time = new ();
            time.Start();

            int result = this.service.CreateRecord(v);

            time.Stop();
            Console.WriteLine($"Create method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Defrags a file.
        /// </summary>
        /// <param name="destinationFileName">The destination file name.</param>
        /// <param name="numNewRecords">The number of new records in a new file.</param>
        /// <param name="numOldRecords">The number of old records in an old file.</param>
        public void DefragFile(string destinationFileName, out int numNewRecords, out int numOldRecords)
        {
            Stopwatch time = new ();
            time.Start();

            this.service.DefragFile(destinationFileName, out numNewRecords, out numOldRecords);

            time.Stop();
            Console.WriteLine($"Purge method execution duration is {time.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Changes old record on the new one.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        public void EditRecord(int id, ParameterObject v)
        {
            Stopwatch time = new ();
            time.Start();

            this.service.EditRecord(id, v);

            time.Stop();
            Console.WriteLine($"Edit method execution duration is {time.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            Stopwatch time = new ();
            time.Start();

            IEnumerable<FileCabinetRecord> result = this.service.FindByDateOfBirth(dateOfBirth);

            time.Stop();
            Console.WriteLine($"Find method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            Stopwatch time = new ();
            time.Start();

            IEnumerable<FileCabinetRecord> result = this.service.FindByFirstName(firstName);

            time.Stop();
            Console.WriteLine($"Find method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            Stopwatch time = new ();
            time.Start();

            IEnumerable<FileCabinetRecord> result = this.service.FindByLastName(lastName);

            time.Stop();
            Console.WriteLine($"Find method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Gets records from the List and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            Stopwatch time = new ();
            time.Start();

            ReadOnlyCollection<FileCabinetRecord> result = this.service.GetRecords();

            time.Stop();
            Console.WriteLine($"List method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Defines the number of records in the List.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            Stopwatch time = new ();
            time.Start();

            int result = this.service.GetStat();

            time.Stop();
            Console.WriteLine($"Stat method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Saves the current state inside a FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Snapshot of object, where the FileCabinetService passes its state to the FileCabinetServiceSnapshot's constructor parameters.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            Stopwatch time = new ();
            time.Start();

            FileCabinetServiceSnapshot result = this.service.MakeSnapshot();

            time.Stop();
            Console.WriteLine($"Export method execution duration is {time.ElapsedTicks} ticks.");

            return result;
        }

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void RemoveRecord(int id)
        {
            Stopwatch time = new ();
            time.Start();

            this.service.RemoveRecord(id);

            time.Stop();
            Console.WriteLine($"Remove method execution duration is {time.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Restores the state from a snapshot object.
        /// </summary>
        /// <param name="snapshot">Snapshot of the object, where saved its state.</param>
        /// <param name="count">Number of records that were imported with definite validation rules.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out int count)
        {
            Stopwatch time = new ();
            time.Start();

            this.service.Restore(snapshot, out count);

            time.Stop();
            Console.WriteLine($"Import method execution duration is {time.ElapsedTicks} ticks.");
        }
    }
}
