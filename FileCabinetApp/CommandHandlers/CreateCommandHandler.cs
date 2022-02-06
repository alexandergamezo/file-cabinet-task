﻿using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'create' command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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
                int result = this.service.CreateRecord(paramobj);
                if (result > 0)
                {
                    Console.WriteLine($"Record #{result} is created.");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
