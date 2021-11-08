namespace FileCabinetApp
{
    /// <summary>
    /// Extends base class with additional individual algorithms.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Returns a reference to the concrete strategy.
        /// </summary>
        /// <returns>New object.</returns>
        protected override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
