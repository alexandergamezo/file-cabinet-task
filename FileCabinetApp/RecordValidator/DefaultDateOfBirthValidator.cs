using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default DateOfBirth Validator.
    /// </summary>
    public class DefaultDateOfBirthValidator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.DateOfBirth < new DateTime(1950, 01, 01) || v.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("DateOfBirth string has the wrong value", nameof(v));
            }
        }
    }
}
