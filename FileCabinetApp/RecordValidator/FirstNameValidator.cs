using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// FirstName Validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum length of the first name.</param>
        /// <param name="max">The maximum length of the first name.</param>
        public FirstNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

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

            if (v.FirstName.Length < this.minLength || v.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException("All names must be more or equal than 'min' value and less or equal than 'max' value (validation-rules.json)", nameof(v));
            }
        }
    }
}
