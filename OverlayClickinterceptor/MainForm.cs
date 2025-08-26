using System;
using System.Windows.Forms;
using OverlayClickinterceptor.Subordinant;

namespace OverlayClickinterceptor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.FormClosing += MainForm_FormClosing;
            lbWindows.DisplayMember = "Name";
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            GlobalHooks.Dispose();
        }

        private void btnPlaceOverlay_Click(object sender, EventArgs e)
        {
            int newIdNum =  lbWindows.Items.Count + 1;

            OverlayMouseInfo newForm = new OverlayMouseInfo();
            newForm.Name = $"OverlayForm{newIdNum}";
            lbWindows.Items.Add(newForm);
            newForm.FormClosed += NewForm_FormClosed;
            this.Cursor = Cursors.Cross;
            newForm.Show(this);
        }

        private void NewForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            Form closedForm = sender as Form;
            if (closedForm == null)
            {
                return;
            }

            int indexToRemove = lbWindows.Items.IndexOf(closedForm);
            if (indexToRemove == -1)
            {
                return;
            }

            lbWindows.Items.RemoveAt(indexToRemove);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbWindows.SelectedIndex == -1)
            {
                return;
            }

            Form toClose = (Form)lbWindows.SelectedItem;
            toClose.Close();
        }
    }
}
