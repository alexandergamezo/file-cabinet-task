using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'create' command.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "create")
            {
                this.Create();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Create()
        {
            try
            {
                Program.CheckInputFromLine(out string firstName, out string lastName, out DateTime dateOfBirth, out short property1, out decimal property2, out char property3);
                ParameterObject paramobj = new (firstName, lastName, dateOfBirth, property1, property2, property3);
                Console.WriteLine($"Record #{this.service.CreateRecord(paramobj)} is created.");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
