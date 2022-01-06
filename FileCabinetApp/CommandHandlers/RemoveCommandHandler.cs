using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'remove' command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "remove")
            {
                Remove(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Remove(AppCommandRequest request)
        {
            int id = -1;
            if (!string.IsNullOrEmpty(request.Parameters) && int.TryParse(request.Parameters, out int enteredId))
            {
                id = enteredId;
            }

            var onlyCollection = Program.fileCabinetService.GetRecords();

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
                        Program.fileCabinetService.RemoveRecord(id);
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
