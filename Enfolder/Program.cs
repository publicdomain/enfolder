// <copyright file="Program.cs" company="PublicDomain.com">
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
    using System.Threading.Tasks;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// The first instance mutex.
        /// </summary>
        public static Mutex FirstInstanceMutex = null;

        /// <summary>
        /// The file mutex.
        /// </summary>
        public static Mutex FileMutex = null;

        /// <summary>
        /// The file path.
        /// </summary>
        private static string filePath = "EnfolderItems.txt";

        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // Check arguments for context menu start
            if (args.Length > 0)
            {
                // Get file write mutex to write item
                FileMutex = new Mutex(false, @"Local\EnfolderFile");
                FileMutex.WaitOne();
                File.AppendAllLines(filePath, args);
                FileMutex.ReleaseMutex();

                // The first instance flag
                bool firstInstance;

                // Set first instance mutex
                FirstInstanceMutex = new Mutex(true, @"Local\EnfolderFirsInstance", out firstInstance);

                // Act according to instance
                // Check for first instance
                if (firstInstance)
                {
                    // Launch enfolder form
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new EnfolderForm(filePath));

                    // Release the mutex
                    FirstInstanceMutex.ReleaseMutex();
                }
            }
            else // By user
            {
                // Run main form
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

    }
}
