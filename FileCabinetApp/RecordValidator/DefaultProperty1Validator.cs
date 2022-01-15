using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default Property1 Validator.
    /// </summary>
    public class DefaultProperty1Validator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.Property1 < short.MinValue || v.Property1 > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property1 value is not a <short> number");
            }
        }
    }
}
