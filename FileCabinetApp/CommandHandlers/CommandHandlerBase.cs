using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base handler class.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        /// Сonnects handlers.
        /// </summary>
        /// <param name="handler">The next handler in a chain.</param>
        /// <returns>Returns the next handler in a chain.</returns>
        public ICommandHandler SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
            return handler;
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Command request.</param>
        public virtual void Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
            else
            {
                PrintMissedCommandInfo(request);
                FindSimilarCommands(request);
            }
        }

        private static void PrintMissedCommandInfo(AppCommandRequest request)
        {
            Console.WriteLine($"There is no '{request.Command}' command. See 'help'.");
            Console.WriteLine();
        }

        private static void FindSimilarCommands(AppCommandRequest request)
        {
            string[][] helpMessages = new HelpCommandHandler().GetHelpMessages();
            List<string> list = new ();

            foreach (string[] message in helpMessages)
            {
                if (message[0].StartsWith(request.Command.ToLowerInvariant()))
                {
                    list.Add(message[0]);
                }
            }

            if (list.Count == 1)
            {
                Console.WriteLine("The most similar command is");
                Console.WriteLine($"         {list[0]}");
            }

            if (list.Count > 1)
            {
                Console.WriteLine("The most similar commands are");
                foreach (var a in list)
                {
                    Console.WriteLine($"         {a}");
                }
            }

            int num = 0;
            string command = string.Empty;
            if (list.Count == 0)
            {
                foreach (string[] message in helpMessages)
                {
                    int numMessageLetters = 0;
                    for (int i = 0; i < message[0].Length; i++)
                    {
                        for (int j = 0; j < request.Command.Length; j++)
                        {
                            if (request.Command[j] == message[0][i])
                            {
                                numMessageLetters++;
                                continue;
                            }
                        }
                    }

                    if (numMessageLetters > num)
                    {
                        num = numMessageLetters;
                        command = message[0];
                    }
                }

                Console.WriteLine("The most similar command is");
                Console.WriteLine($"         {command}");
            }
        }
    }
}
