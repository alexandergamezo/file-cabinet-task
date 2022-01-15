using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Property1 Validator.
    /// </summary>
    public class Property1Validator : IRecordValidator
    {
        private readonly short minValue;
        private readonly short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property1Validator"/> class.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public Property1Validator(short min, short max)
        {
            this.minValue = min;
            this.maxValue = max;
        }

        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.Property1 < this.minValue || v.Property1 > this.maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property1 value is not a <short> number");
            }
        }
    }
}
