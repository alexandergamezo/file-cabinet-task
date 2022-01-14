using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'list' command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        /// <param name="recordPrinter">Delegate.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
            : base(fileCabinetService)
        {
            this.recordPrinter = recordPrinter;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                this.List();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void List()
        {
            var arr = this.service.GetRecords();
            if (arr.Count == 0)
            {
                Console.WriteLine("The list is empty.");
            }
            else
            {
                this.recordPrinter(arr);
            }
        }
    }
}
