using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'import' command.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "export")
            {
                this.Import(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Import(AppCommandRequest request)
        {
            string paramFormat;
            string paramPath = string.Empty;
            try
            {
                Program.CheckInputParameters(out string f, out string p, request.Parameters);
                paramFormat = f;
                paramPath = p;

                if (File.Exists(paramPath))
                {
                    FindAppropriateFunc();
                }
                else
                {
                    Console.WriteLine($"Import error: file {paramPath} is not exist.");
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                Console.WriteLine($"Import failed: can't open file {paramPath}. {exc}.");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc}.");
            }

            void FindAppropriateFunc()
            {
                if (paramFormat == "csv")
                {
                    ImportFromCsvFormat(paramPath);
                }

                if (paramFormat == "xml")
                {
                    ImportFromXmlFormat(paramPath);
                }
            }

            void ImportFromCsvFormat(string path)
            {
                StreamReader reader = new (new FileStream(path, FileMode.Open));
                FileCabinetServiceSnapshot snapshot = new ();
                snapshot.LoadFromCsv(reader);
                this.service.Restore(snapshot, out int count);

                reader.Close();
                Console.WriteLine($"{count} records were imported from {paramPath}.");
            }

            void ImportFromXmlFormat(string path)
            {
                StreamReader reader = new (new FileStream(path, FileMode.Open));
                FileCabinetServiceSnapshot snapshot = new ();
                snapshot.LoadFromXml(reader);
                this.service.Restore(snapshot, out int count);

                reader.Close();
                Console.WriteLine($"{count} records were imported from {paramPath}.");
            }
        }
    }
}
