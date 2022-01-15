using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default FirstName Validator.
    /// </summary>
    public class DefaultFirstNameValidator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
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
