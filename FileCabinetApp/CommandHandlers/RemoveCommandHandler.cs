using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'remove' command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "remove")
            {
                this.Remove(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Remove(AppCommandRequest request)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(request.Parameters) && int.TryParse(request.Parameters, out int enteredId))
            {
                id = enteredId;
            }

            var onlyCollection = this.service.GetRecords();

            if (onlyCollection.Count == 0)
            {
                Console.WriteLine($"#{request.Parameters} record is not found.");
            }

            for (int i = 0; i < onlyCollection.Count; i++)
            {
                if (id == onlyCollection[i].Id)
                {
                    try
                    {
                        this.service.RemoveRecord(id);
                        Console.WriteLine($"Record #{id} is removed.");
                        break;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }

                if (i == onlyCollection.Count - 1 || onlyCollection.Count == 0)
                {
                    Console.WriteLine($"Record #{request.Parameters} doesn't exists.");
                }
            }
        }
    }
}
