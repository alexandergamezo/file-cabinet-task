namespace FileCabinetApp
{
    /// <summary>
    /// The IRecordValidator interface is common to all concrete strategies.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        ///  Declares a method used for executing the strategy.
        /// </summary>
        /// <param name="v">Object with parameters.</param>
        public void ValidateParameters(ParameterObject v);
    }
}
