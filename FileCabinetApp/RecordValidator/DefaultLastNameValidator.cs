using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default LastName Validator.
    /// </summary>
    public class DefaultLastNameValidator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
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
    }
}
