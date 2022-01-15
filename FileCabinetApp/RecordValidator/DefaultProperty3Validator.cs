using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default Property3 Validator.
    /// </summary>
    public class DefaultProperty3Validator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (!char.IsLetter(v.Property3))
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property3 value is not a <char> letter");
            }
        }
    }
}
