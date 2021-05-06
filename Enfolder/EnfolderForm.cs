using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Enfolder
{
    /// <summary>
    /// Description of EnfolderForm.
    /// </summary>
    public partial class EnfolderForm : Form
    {
        /// <summary>
        /// The file and folder counts.
        /// </summary>
        int fileCount = 0, directoryCount = 0;

        /// <summary>
        /// The file path.
        /// </summary>
        private string filePath = string.Empty;

        /// <summary>
        /// The length of the read file.
        /// </summary>
        private long readFileLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Enfolder.EnfolderForm"/> class.
        /// </summary>
        public EnfolderForm(string filePath)
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Set file path
            this.filePath = filePath;
        }

        /// <summary>
        /// Handles the subfolder button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSubfolderButtonClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the browse button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            // TODO Add code
        }

        /// <summary>
        /// Handles the enfolder form form closing event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEnfolderFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // Get file write mutex to remove frmo disk
            Mutex fileMutex = new Mutex(false, @"Local\EnfolderFile");
            fileMutex.WaitOne();
            File.Delete(this.filePath); // Remove it
            fileMutex.ReleaseMutex();
        }

        /// <summary>
        /// Handles the item timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnItemTimerTick(object sender, EventArgs e)
        {
            // Stop the timer
            this.itemTimer.Stop();

            // Get file by mutex
            Mutex fileMutex = new Mutex(false, @"Local\EnfolderFile");
            fileMutex.WaitOne();

            // Set file info
            FileInfo fileInfo = new FileInfo(this.filePath);

            // Checks (file exists, same as previous read file length
            if (!fileInfo.Exists || this.readFileLength == fileInfo.Length)
            {
                // Halt flow after releasing mutex
                goto ReleaseMutextAndExit;
            }

            // Items from file
            string[] itemsFromFile;

            try
            {
                // Set items from file
                itemsFromFile = File.ReadAllLines(this.filePath, Encoding.UTF8);
            }
            catch
            {
                // Halt flow after releasing mutex
                goto ReleaseMutextAndExit;
            }

            // Save last read length
            this.readFileLength = fileInfo.Length;

            // Iterate backwards, until there's a previous item match
            for (int i = itemsFromFile.Length - 1; i >= 0; i--)
            {
                // Set current item
                string item = itemsFromFile[i];

                // Check for previous presence
                if (this.itemListBox.Items.Contains(item))
                {
                    // Update count release mutext and exit label
                    goto UpdateCountReleaseMutextAndExit;
                }

                // Add to list
                this.itemListBox.Items.Add(item);

                // Raise file or directory conut
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    // Directory
                    this.directoryCount++;
                }
                else
                {
                    // File
                    this.fileCount++;
                }
            }

        // Update count release mutext and exit label
        UpdateCountReleaseMutextAndExit:

            // Update labels
            this.fileCountToolStripStatusLabel.Text = fileCount.ToString();
            this.directoryCountToolStripStatusLabel.Text = directoryCount.ToString();

        // Release mutext and exit label
        ReleaseMutextAndExit:

            // Release mutex
            fileMutex.ReleaseMutex();

            // Re-enable timer
            this.itemTimer.Start();
        }
    }
}
