using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'list' command.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                List();
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void List()
        {
            var arr = Program.fileCabinetService.GetRecords();
            if (arr.Count == 0)
            {
                Console.WriteLine("The list is empty.");
            }
            else
            {
                Program.Show(arr);
            }
        }
    }
}
