using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'edit' command.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "edit")
            {
                Edit(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Edit(AppCommandRequest request)
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
                        Program.CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                        ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                        Program.fileCabinetService.EditRecord(id, paramobj);
                        Console.WriteLine($"Record #{id} is updated.");
                        break;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }

                if (i == onlyCollection.Count - 1)
                {
                    Console.WriteLine($"#{request.Parameters} record is not found.");
                }
            }
        }
    }
}
