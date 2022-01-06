using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'purge' command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private const string SourceFileName = "temp.db";
        private const string DestinationBackupFileName = "cabinet-records.db.bac";

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "purge")
            {
                Purge();
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Purge()
        {
            Program.fileCabinetService.DefragFile(SourceFileName, out int numNewRecords, out int numOldRecords);
            if (numNewRecords != -1)
            {
                File.Replace(SourceFileName, Program.Filename, DestinationBackupFileName);
                Console.WriteLine($"Data file processing is completed: {numOldRecords - numNewRecords} of {numOldRecords} records were purged.");
                Program.CommandLineParameter(Program.initParams);
            }
        }
    }
}
