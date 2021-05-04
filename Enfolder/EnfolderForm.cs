using System;
using System.Drawing;
using System.Windows.Forms;

namespace Enfolder
{
    /// <summary>
    /// Description of EnfolderForm.
    /// </summary>
    public partial class EnfolderForm : Form
    {
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
        /// Ons the item added.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        public void OnItemAdded(object sender, EnfolderDataEventArgs args)
        {
            foreach (var item in args.Items)
            {
                this.itemListBox.Items.Add(item);
            }
        }
    }
}
