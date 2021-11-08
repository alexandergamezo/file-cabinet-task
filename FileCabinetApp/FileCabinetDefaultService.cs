namespace FileCabinetApp
{
    /// <summary>
    /// Extends base class with default algorithms.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Returns a reference to the concrete strategy.
        /// </summary>
        /// <returns>New object.</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
