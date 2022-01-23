using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// DateOfBirth Validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">The start date.</param>
        /// <param name="to">The end date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="v">Pararmeter object.</param>
        public void ValidateParameters(ParameterObject v)
        {
            if (v.DateOfBirth < this.from || v.DateOfBirth > this.to)
            {
                throw new ArgumentException("DateOfBirth string has the wrong value", nameof(v));
            }
        }
    }
}
