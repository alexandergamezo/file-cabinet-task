using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'purge' command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string SourceFileName = "temp.db";
        private const string DestinationBackupFileName = "cabinet-records.db.bac";
        private readonly string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        /// <param name="filename">Filename.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService, string filename)
            : base(fileCabinetService)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "purge")
            {
                this.Purge();
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Purge()
        {
            this.service.DefragFile(SourceFileName, out int numNewRecords, out int numOldRecords);
            if (numNewRecords != -1)
            {
                File.Replace(SourceFileName, this.filename, DestinationBackupFileName);
                Console.WriteLine($"Data file processing is completed: {numOldRecords - numNewRecords} of {numOldRecords} records were purged.");
                Program.CommandLineParameter(Environment.GetCommandLineArgs());
            }
        }
    }
}
