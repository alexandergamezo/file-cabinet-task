using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'exit' command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "exit")
            {
                Exit();
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting an application...");
            Program.IsRunning = false;
        }
    }
}
