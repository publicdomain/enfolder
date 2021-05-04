﻿// <copyright file="Program.cs" company="PublicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>
// <auto-generated />

namespace Enfolder
{
    // Directives
    using System;
    using System.IO;
    using System.IO.Pipes;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO.MemoryMappedFiles;
    using System.Collections.Generic;

    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// The mutex.
        /// </summary>
        private static Mutex mutex = null;

        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // Check arguments for context menu start
            if (args.Length > 0)
            {
                // The first instance flag
                bool firstInstance;

                // Set mutext
                mutex = new Mutex(true, @"Local\Enfolder", out firstInstance);

                // Check for a secondary instance
                if (!firstInstance)
                {
                    // Set the named pipe client
                    NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "EnfolderIPC", PipeDirection.InOut);

                    // Connect named pipe client
                    namedPipeClientStream.Connect();

                    // Set binary formatter
                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    // Process/serialize current arguments
                    binaryFormatter.Serialize(namedPipeClientStream, args);

                    // Close the named pipe client
                    namedPipeClientStream.Close();

                    // Halt flow
                    return;
                }

                // First instance. Run 
                try
                {
                    // Process current arguments
                    ProcessArguments(args);

                    // Set the named pipe server
                    NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream("EnfolderIPC", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                    // Prepare server for connections
                    namedPipeServerStream.BeginWaitForConnection(new AsyncCallback(ConnectionHandler), namedPipeServerStream);

                    // Launch 
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var enfolderForm = new EnfolderForm())

                    {
                        EnfolderData.OnItemAdded += enfolderForm.OnItemAdded;

                        Application.Run(enfolderForm);
                    }
                }
                finally
                {
                    // Release the mutex
                    mutex.ReleaseMutex();
                }
            }
            else // By user
            {
                // Run it
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        /// <summary>
        /// Connections the handler.
        /// </summary>
        /// <param name="result">Result.</param>
        static void ConnectionHandler(IAsyncResult result)
        {
            // Set named pipe server
            NamedPipeServerStream namedPipeServerStream = result.AsyncState as NamedPipeServerStream;

            // Wait for connection
            namedPipeServerStream.EndWaitForConnection(result);

            // Set binary formatter
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            // Set the arguments
            string[] arguments = binaryFormatter.Deserialize(namedPipeServerStream) as string[];

            // Process the arguments
            ProcessArguments(arguments);

            // Close the server
            namedPipeServerStream.Close();

            // Re-set the named pipe server
            namedPipeServerStream = new NamedPipeServerStream("EnfolderIPC", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

            // Prepare for connection again
            namedPipeServerStream.BeginWaitForConnection(new AsyncCallback(ConnectionHandler), namedPipeServerStream);
        }

        /// <summary>
        /// Processes the arguments.
        /// </summary>
        /// <param name="args">Arguments.</param>
        static void ProcessArguments(string[] args)
        {

            EnfolderData.AddItems(new List<string>(args));
        }

    }
}
