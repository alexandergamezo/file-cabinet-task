using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// The concrete strategy implements the definite algorithm.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Checks input parameters on the wrong values.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        public void ValidateParameters(ParameterObject v)
        {
            var firstNameValidator = new FirstNameValidator(2, 60);
            firstNameValidator.ValidateParameters(v);

            var lastNameValidator = new LastNameValidator(2, 60);
            lastNameValidator.ValidateParameters(v);

            var dateOfBirthValidator = new DateOfBirthValidator(new DateTime(1950, 01, 01), DateTime.Today);
            dateOfBirthValidator.ValidateParameters(v);

            var property1Validator = new Property1Validator(short.MinValue, short.MaxValue);
            property1Validator.ValidateParameters(v);

            var property2Validator = new Property2Validator(decimal.MinValue, decimal.MaxValue);
            property2Validator.ValidateParameters(v);

            var property3Validator = new Property3Validator();
            property3Validator.ValidateParameters(v);
        }
    }
}
