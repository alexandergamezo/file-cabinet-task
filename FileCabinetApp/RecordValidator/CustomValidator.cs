using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// The concrete strategy implements the definite algorithm.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Checks input parameters on the wrong values.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        public void ValidateParameters(ParameterObject v)
        {
            var customFirstNameValidator = new CustomFirstNameValidator();
            customFirstNameValidator.ValidateParameters(v);

            var customLastNameValidator = new CustomLastNameValidator();
            customLastNameValidator.ValidateParameters(v);

            var customDateOfBirthValidator = new CustomDateOfBirthValidator();
            customDateOfBirthValidator.ValidateParameters(v);

            var customProperty1Validator = new CustomProperty1Validator();
            customProperty1Validator.ValidateParameters(v);

            var customProperty2Validator = new CustomProperty2Validator();
            customProperty2Validator.ValidateParameters(v);

            var customProperty3Validator = new CustomProperty3Validator();
            customProperty3Validator.ValidateParameters(v);
        }
    }
}
