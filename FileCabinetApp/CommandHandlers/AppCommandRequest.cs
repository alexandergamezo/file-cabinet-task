namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Application command request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Command.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Parameters.
        /// </value>
        public string Parameters { get; set; }
    }
}
