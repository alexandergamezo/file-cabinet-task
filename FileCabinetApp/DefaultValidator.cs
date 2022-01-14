using System;

namespace FileCabinetApp
{
    /// <summary>
    /// The concrete strategy implements the definite algorithm.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Checks input parameters on the wrong values.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        public void ValidateParameters(ParameterObject v)
        {
            ValidateFirstName(v);
            ValidateLastName(v);
            ValidateDateOfBirth(v);
            ValidateProperty1(v);
            ValidateProperty2(v);
            ValidateProperty3(v);
        }

        private static void ValidateProperty3(ParameterObject v)
        {
            if (!char.IsLetter(v.Property3))
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property3 value is not a <char> letter");
            }
        }

        private static void ValidateProperty2(ParameterObject v)
        {
            if (v.Property2 < decimal.MinValue || v.Property2 > decimal.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property2 value is not a <decimal> number");
            }
        }

        private static void ValidateProperty1(ParameterObject v)
        {
            if (v.Property1 < short.MinValue || v.Property1 > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property1 value is not a <short> number");
            }
        }

        private static void ValidateDateOfBirth(ParameterObject v)
        {
            if (v.DateOfBirth < new DateTime(1950, 01, 01) || v.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("DateOfBirth string has the wrong value", nameof(v));
            }
        }

        private static void ValidateLastName(ParameterObject v)
        {
            if (string.IsNullOrEmpty(v.LastName))
            {
                throw new ArgumentNullException(nameof(v), $"{nameof(v)} is null or empty");
            }

            if (v.LastName.Length < 2 || v.LastName.Length > 60)
            {
                throw new ArgumentException(
                    "All names must be more or equal than 2-letters and less or equal than 60-letters", nameof(v));
            }
        }

        private static void ValidateFirstName(ParameterObject v)
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
        }
    }
}
