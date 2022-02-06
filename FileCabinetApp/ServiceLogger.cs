using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides a logging.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Object reference.</param>
        /// <param name="writer">Stream for writing state.</param>
        public ServiceLogger(IFileCabinetService service, TextWriter writer)
        {
            this.service = service;
            this.writer = writer;
        }

        /// <summary>
        /// Creates records in the List and the Dictionary.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        /// <returns>The id number.</returns>
        public int CreateRecord(ParameterObject v)
        {
            int result = -1;
            try
            {
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Create() with FirstName = '{v.FirstName}', LastName = '{v.LastName}', DateOfBirth = '{v.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}', " +
                    $"Property1 = '{v.Property1}', Property2 = '{v.Property2}', Property3 = '{v.Property3}'");
                result = this.service.CreateRecord(v);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Create() returned '{result}'");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Exception: '{exc}'");
            }
            finally
            {
                this.writer.Flush();
            }

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
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Purge()");
            this.service.DefragFile(destinationFileName, out numNewRecords, out numOldRecords);
            this.writer.Flush();
        }

        /// <summary>
        /// Changes old record on the new one in the Dictionary and List.
        /// </summary>
        /// <param name="id">Id number.</param>
        /// <param name="v">Object with parameters.</param>
        public void EditRecord(int id, ParameterObject v)
        {
            try
            {
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Edit() with FirstName = '{v.FirstName}', LastName = '{v.LastName}', DateOfBirth = '{v.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}', " +
                        $"Property1 = '{v.Property1}', Property2 = '{v.Property2}', Property3 = '{v.Property3}'");
                this.service.EditRecord(id, v);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Edit() updated Record #{id}");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Exception: '{exc}'");
            }
            finally
            {
                this.writer.Flush();
            }
        }

        /// <summary>
        /// Finds records in the Dictionary by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <returns>The collection of records found by <paramref name="dateOfBirth"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Find() by dateofbirth = '{dateOfBirth}'");
            ReadOnlyCollection<FileCabinetRecord> result = this.service.FindByDateOfBirth(dateOfBirth);
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Find() returned '{result.Count}' record(s)");
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Finds records in the Dictionary by first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>The collection of records found by the <paramref name="firstName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Find() by firstname = '{firstName}'");
            ReadOnlyCollection<FileCabinetRecord> result = this.service.FindByFirstName(firstName);
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Find() returned '{result.Count}' record(s)");
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Finds records in the Dictionary by last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>The collection of records which by the <paramref name="lastName"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Find() by lastname = '{lastName}'");
            ReadOnlyCollection<FileCabinetRecord> result = this.service.FindByLastName(lastName);
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Find() returned '{result.Count}' record(s)");
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Gets records from the List and puts them into a collection.
        /// </summary>
        /// <returns>The collection of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling List()");
            ReadOnlyCollection<FileCabinetRecord> result = this.service.GetRecords();
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - List() returned '{result.Count}' record(s)");
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Defines the number of records in the List.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Stat()");
            int result = this.service.GetStat();
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Stat() returned '{result}'");
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Saves the current state inside a FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Snapshot of object, where the FileCabinetService passes its state to the FileCabinetServiceSnapshot's constructor parameters.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Export()");
            FileCabinetServiceSnapshot result = this.service.MakeSnapshot();
            this.writer.Flush();

            return result;
        }

        /// <summary>
        /// Removes a record from the Dictionary and List.
        /// </summary>
        /// <param name="id">Id number.</param>
        public void RemoveRecord(int id)
        {
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Remove()");
                this.service.RemoveRecord(id);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Remove() removed Record #{id}");
                this.writer.Flush();
        }

        /// <summary>
        /// Restores the state from a snapshot object.
        /// </summary>
        /// <param name="snapshot">Snapshot of the object, where saved its state.</param>
        /// <param name="count">Number of records that were imported with definite validation rules.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out int count)
        {
            count = 0;
            try
            {
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Calling Import()");
                this.service.Restore(snapshot, out count);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Import() imported {count} record(s)");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                this.writer.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - Exception: '{exc}'");
            }
            finally
            {
                this.writer.Flush();
            }
        }
    }
}
