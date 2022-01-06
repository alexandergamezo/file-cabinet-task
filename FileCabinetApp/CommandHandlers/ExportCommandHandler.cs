using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the 'export' command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handlers a request.
        /// </summary>
        /// <param name="request">Request.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command == "export")
            {
                Export(request);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Export(AppCommandRequest request)
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
                Program.fileCabinetService.MakeSnapshot().SaveToCsv(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }

            void SaveToXmlFormat(string path)
            {
                StreamWriter writer = new (path);
                Program.fileCabinetService.MakeSnapshot().SaveToXmL(writer);
                writer.Close();
                Console.WriteLine($"All records are exported to file {paramPath}.");
            }
        }
    }
}
