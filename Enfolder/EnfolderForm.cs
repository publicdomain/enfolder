﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

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
        /// The directory list.
        /// </summary>
        private List<string> directoryList = new List<string>();

        /// <summary>
        /// The file list.
        /// </summary>
        private List<string> fileList = new List<string>();

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
            // Trigger subfolder
            this.Subfolder(Environment.CurrentDirectory);
        }

        /// <summary>
        /// Handles the browse button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            // Set folder browser dialog result by showing
            DialogResult folderBrowserDialogResultdialog = this.folderBrowserDialog.ShowDialog();

            // Check there's something
            if (folderBrowserDialogResultdialog == DialogResult.OK && !string.IsNullOrWhiteSpace(this.folderBrowserDialog.SelectedPath))
            {
                // Trigger subfolder
                this.Subfolder(this.folderBrowserDialog.SelectedPath);
            }
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

                    // Add to directory list
                    this.directoryList.Add(item);
                }
                else
                {
                    // File
                    this.fileCount++;

                    // Add to file list
                    this.fileList.Add(item);
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

        /// <summary>
        /// Subfolder the specified baseDirectory.
        /// </summary>
        /// <param name="baseDirectory">Base directory.</param>
        private void Subfolder(string baseDirectory)
        {
            // Done flag
            bool done = false;

            // Naming loop
            while (true)
            {
                // Declare destination directory
                string destinationDirectory = string.Empty;

                // Get directory name
                string directoryName = Interaction.InputBox("Please enter subfolder name to create:", "Subfolder", $"Enfolder_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");

                // Check there's something
                if (directoryName.Length > 0)
                {
                    // Set destination directory
                    destinationDirectory = Path.Combine(baseDirectory, directoryName);

                    // Check it is new
                    if (Directory.Exists(destinationDirectory))
                    {
                        // Ask user to enter new directory name
                        if (MessageBox.Show("Directory already exists. Would you like to retry?", "Existing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            // Try again. Trigger another while iteration
                            continue;
                        }
                        else
                        {
                            // User said no. Halt flow
                            return;
                        }
                    }

                    // Enfolder
                    this.Enfolder(destinationDirectory);

                    // Toggle done flag
                    done = true;

                    // Exit while
                    break;
                }
                else
                {
                    // Halt flow
                    return;
                }
            }

            // Check if done
            if (done)
            {
                // Disable buttons
                this.subfolderButton.Enabled = false;
                this.browseButton.Enabled = false;

                // Exit program
                Application.Exit();
            }
        }

        /// <summary>
        /// Enfolder the specified destinationDirectory.
        /// </summary>
        /// <param name="destinationDirectory">Destination directory.</param>
        private void Enfolder(string destinationDirectory)
        {
            // Consolidate file and directory conut
            int itemCount = this.fileCount + this.directoryCount;

            // Create destination directory
            Directory.CreateDirectory(destinationDirectory);

            // Make progressbar visible
            this.enfolderToolStripProgressBar.Visible = true;

            // Processed items
            int processedItems = 0;

            // Declare error list
            List<string> errorList = new List<string>();

            // Process directories
            foreach (var directory in this.directoryList)
            {
                // Set destination path
                string destinationPath = Path.Combine(destinationDirectory, new DirectoryInfo(directory).Name);

                try
                {
                    // Move to passed destination directory
                    Directory.Move(directory, destinationPath);
                }
                catch (Exception exception)
                {
                    // Add exception to error list
                    errorList.Add(
                    $"From: {directory}{Environment.NewLine}To: {destinationPath}{Environment.NewLine}Message: {exception.Message}{Environment.NewLine}{Environment.NewLine}"
                        );
                }

                // Raise processed items
                processedItems++;

                // Update progress bar
                this.enfolderToolStripProgressBar.Value = ((processedItems / itemCount) * 100);
            }

            // Process files
            foreach (var file in this.fileList)
            {
                // Set destination path
                string destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(file));

                try
                {
                    // Move to passed destination directory
                    File.Move(file, destinationPath);
                }
                catch (Exception exception)
                {
                    // Add exception to error list
                    errorList.Add(
                    $"From: {file}{Environment.NewLine}To: {destinationPath}{Environment.NewLine}Message: {exception.Message}{Environment.NewLine}{Environment.NewLine}"
                        );
                }

                // Raise processed items
                processedItems++;

                // Update progress bar
                this.enfolderToolStripProgressBar.Value = ((processedItems / itemCount) * 100);
            }

            // Handle error list
            if (errorList.Count > 0)
            {
                // Save to file
                File.AppendAllLines("EnfolderErrorLog.txt", errorList);

                // Advise user
                MessageBox.Show($"Error count: {errorList.Count} error{(errorList.Count > 1 ? "s" : string.Empty)}.{Environment.NewLine}{Environment.NewLine}Please check EnfolderErrorLog.txt for detailed information.", "Enfolder operation had errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
