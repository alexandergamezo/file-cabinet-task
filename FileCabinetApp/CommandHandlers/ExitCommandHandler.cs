using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'exit' command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="action">Delegate.</param>
        public ExitCommandHandler(Action<bool> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "exit")
            {
                this.Exit();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.action(false);
        }
    }
}
