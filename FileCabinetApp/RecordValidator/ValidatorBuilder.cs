using System;
using System.Collections.Generic;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Builder class.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <param name="min">The minimum length of the first name.</param>
        /// <param name="max">The maximum length of the first name.</param>
        /// <returns>Object reference.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <param name="min">The minimum length of the last name.</param>
        /// <param name="max">The maximum length of the last name.</param>
        /// <returns>Object reference.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <param name="from">The start date.</param>
        /// <param name="to">The end date.</param>
        /// <returns>A new reference with necessary validators.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new reference with necessary validators.</returns>
        public ValidatorBuilder ValidateProperty1(short min, short max)
        {
            this.validators.Add(new Property1Validator(min, max));
            return this;
        }

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new reference with necessary validators.</returns>
        public ValidatorBuilder ValidateProperty2(decimal min, decimal max)
        {
            this.validators.Add(new Property2Validator(min, max));
            return this;
        }

        /// <summary>
        /// Adds a new reference of validator in the list of validators.
        /// </summary>
        /// <returns>A new reference with necessary validators.</returns>
        public ValidatorBuilder ValidateProperty3()
        {
            this.validators.Add(new Property3Validator());
            return this;
        }

        /// <summary>
        /// Makes a new reference with necessary validators.
        /// </summary>
        /// <returns>A new reference with necessary validators.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
