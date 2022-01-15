using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Default Property2 Validator.
    /// </summary>
    public class DefaultProperty2Validator
    {
        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.Property2 < decimal.MinValue || v.Property2 > decimal.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property2 value is not a <decimal> number");
            }
        }
    }
}
