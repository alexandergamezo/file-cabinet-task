using System;

namespace FileCabinetApp.RecordValidator
{/// <summary>
 /// Property2 Validator.
 /// </summary>
    public class Property2Validator : IRecordValidator
    {
        private readonly decimal minValue;
        private readonly decimal maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property2Validator"/> class.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public Property2Validator(decimal min, decimal max)
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
            if (v.Property2 < this.minValue || v.Property2 > this.maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(v), "Property2 value is not a <decimal> number");
            }
        }
    }
}
