using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Extends base class with additional individual algorithms.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Checks input parameters on the wrong values.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        protected override void ValidateParameters(ParameterObject v)
        {
            if (string.IsNullOrEmpty(v.FirstName))
            {
                throw new ArgumentNullException(nameof(v), $"{nameof(v)} is null or empty");
            }

            if (v.FirstName.Length < 2 || v.FirstName.Length > 60)
            {
                throw new ArgumentException(
                    "All names must be more or equal than 2-letters and less or equal than 60-letters", nameof(v));
            }

            if (string.IsNullOrEmpty(v.LastName))
            {
                throw new ArgumentNullException(nameof(v), $"{nameof(v)} is null or empty");
            }

            if (v.LastName.Length < 2 || v.LastName.Length > 60)
            {
                throw new ArgumentException(
                    "All names must be more or equal than 2-letters and less or equal than 60-letters", nameof(v));
            }

            if (v.DateOfBirth < new DateTime(2000, 01, 01) || v.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("DateOfBirth string has the wrong value and has to more than 2000-01-01", nameof(v));
            }

            if (v.Property1 < short.MinValue || v.Property1 > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property1 value is not a <short> number");
            }

            if (v.Property2 < decimal.MinValue || v.Property2 > decimal.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property2 value is not a <decimal> number");
            }

            if (!char.IsLetter(v.Property3))
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property3 value is not a <char> letter");
            }
        }
    }
}
