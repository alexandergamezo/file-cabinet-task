using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'stat' command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "stat")
            {
                this.Stat();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Stat()
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }
    }
}
