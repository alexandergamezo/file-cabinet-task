using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Property3 Validator.
    /// </summary>
    public class Property3Validator : IRecordValidator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (!char.IsLetter(v.Property3))
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property3 must be more or equal than 'min' value and less or equal than 'max' value (validation-rules.json)");
            }
        }
    }
}
