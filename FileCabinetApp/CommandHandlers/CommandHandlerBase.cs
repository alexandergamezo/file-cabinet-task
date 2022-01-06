using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base handler class.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Сonnects handlers.
        /// </summary>
        /// <param name="handler">The next handler in a chain.</param>
        /// <returns>Returns the next handler in a chain.</returns>
        public ICommandHandler SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
            return handler;
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Command request.</param>
        public virtual void Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
            else
            {
                PrintMissedCommandInfo(request);
            }
        }

        private static void PrintMissedCommandInfo(AppCommandRequest request)
        {
            Console.WriteLine($"There is no '{request.Command}' command.");
            Console.WriteLine();
        }
    }
}
