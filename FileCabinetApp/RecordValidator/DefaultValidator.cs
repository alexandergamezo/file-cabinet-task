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
            var defaultFirstNameValidator = new DefaultFirstNameValidator();
            defaultFirstNameValidator.ValidateParameters(v);

            var defaultLastNameValidator = new DefaultLastNameValidator();
            defaultLastNameValidator.ValidateParameters(v);

            var defaultDateOfBirthValidator = new DefaultDateOfBirthValidator();
            defaultDateOfBirthValidator.ValidateParameters(v);

            var defaultProperty1Validator = new DefaultProperty1Validator();
            defaultProperty1Validator.ValidateParameters(v);

            var defaultProperty2Validator = new DefaultProperty2Validator();
            defaultProperty2Validator.ValidateParameters(v);

            var defaultProperty3Validator = new DefaultProperty3Validator();
            defaultProperty3Validator.ValidateParameters(v);
        }
    }
}
