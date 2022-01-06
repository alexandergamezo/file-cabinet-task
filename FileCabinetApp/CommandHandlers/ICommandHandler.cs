namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Declares a methods for building the chain of handlers.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Сonnects handlers.
        /// </summary>
        /// <param name="handler">The next handler in a chain.</param>
        /// <returns>Returns the next handler in a chain.</returns>
        ICommandHandler SetNext(ICommandHandler handler);

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Command request.</param>
        void Handle(AppCommandRequest request);
    }
}