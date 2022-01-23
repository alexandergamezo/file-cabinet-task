using System;

namespace FileCabinetApp.RecordValidator.Extensions
{
    /// <summary>
    /// Class for extension methods.
    /// </summary>
    public static class RecordValidatorExtension
    {
        /// <summary>
        /// Checks record with default parameters.
        /// </summary>
        /// <param name="validatorBuilder">Object reference.</param>
        /// <returns>A new reference with default validators.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            IRecordValidator result = validatorBuilder.ValidateFirstName(2, 60).ValidateLastName(2, 60).ValidateDateOfBirth(new DateTime(1950, 01, 01), DateTime.Today).ValidateProperty1(short.MinValue, short.MaxValue).ValidateProperty2(decimal.MinValue, decimal.MaxValue).ValidateProperty3().Create();

            return result;
        }

        /// <summary>
        /// Checks record with custom parameters.
        /// </summary>
        /// <param name="validatorBuilder">Object reference.</param>
        /// <returns>A new reference with custom validators.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            IRecordValidator result = validatorBuilder.ValidateFirstName(2, 60).ValidateLastName(2, 60).ValidateDateOfBirth(new DateTime(2000, 01, 01), DateTime.Today).ValidateProperty1(short.MinValue, short.MaxValue).ValidateProperty2(decimal.MinValue, decimal.MaxValue).ValidateProperty3().Create();
            return result;
        }
    }
}
