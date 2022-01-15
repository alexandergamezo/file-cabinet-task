using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Custom DateOfBirth Validator.
    /// </summary>
    public class CustomDateOfBirthValidator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.DateOfBirth < new DateTime(2000, 01, 01) || v.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("DateOfBirth string has the wrong value and has to more than 2000-01-01", nameof(v));
            }
        }
    }
}
