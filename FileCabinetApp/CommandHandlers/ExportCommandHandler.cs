using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'export' command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Object reference.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Export(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Export(AppCommandRequest request)
        {
            string paramFormat;
            string paramPath = string.Empty;
            try
            {
                Program.CheckInputParameters(out string f, out string p, request.Parameters);
                paramFormat = f;
                paramPath = p;

                if (!File.Exists(paramPath))
                {
                    FindAppropriateFunc();
                }
                else
                {
                    Console.Write($"File is exist - rewrite {paramPath}? [Y/n] ");
                    string answer = Console.ReadLine();
                    if (answer.ToLowerInvariant() == "y")
                    {
                        FindAppropriateFunc();
                    }
                    else if (answer.ToLowerInvariant() == "n")
                    {
                        Console.WriteLine($"Export failed: file {paramPath} was not rewrited.");
                    }
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                Console.WriteLine($"Export failed: can't open file {paramPath}. {exc}.");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Check your input. {exc}.");
            }

            void FindAppropriateFunc()
            {
                if (paramFormat == "csv")
                {
                    SaveToCsvFormat(paramPath);
                }

                if (paramFormat == "xml")
                {
                    SaveToXmlFormat(paramPath);
                }
            }

            void SaveToCsvFormat(string path)
            {
                StreamWriter writer = new (path);
                this.service.MakeSnapshot().SaveToCsv(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }

            void SaveToXmlFormat(string path)
            {
                StreamWriter writer = new (path);
                this.service.MakeSnapshot().SaveToXmL(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }
        }
    }
}
