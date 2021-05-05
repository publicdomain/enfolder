using System;
using System.Drawing;
using System.IO;
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
        /// Initializes a new instance of the <see cref="T:Enfolder.EnfolderForm"/> class.
        /// </summary>
        public EnfolderForm()
        {
            InitializeComponent();
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
            // TODO Add code
        }

        /// <summary>
        /// Handles the item timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnItemTimerTick(object sender, EventArgs e)
        {
            // TODO Add code
        }
    }
}
