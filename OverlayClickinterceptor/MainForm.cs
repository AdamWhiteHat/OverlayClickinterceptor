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
            newForm.FormClosing += NewForm_FormClosing;
            newForm.OverlayClicked += Overlay_Clicked;
            newForm.Show(this);

        }

        private void NewForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            OverlayMouseInfo closingForm = sender as OverlayMouseInfo;
            if (closingForm == null)
            {
                return;
            }

            closingForm.FormClosing -= NewForm_FormClosing;
            closingForm.OverlayClicked -= Overlay_Clicked;

            int indexToRemove = lbWindows.Items.IndexOf(closingForm);
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

        private void lbWindows_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lbWindows.SelectedIndex = lbWindows.IndexFromPoint(e.X, e.Y);
            }
        }

        private void Overlay_Clicked(object sender, MouseEventArgs e)
        {
            OverlayMouseInfo overlayForm = sender as OverlayMouseInfo;
            if (overlayForm == null)
            {
                return;
            }

            tbClickMessages.AppendText($"{overlayForm.Name}.MouseClicked: ({e.X}, {e.Y})" + Environment.NewLine);
        }
    }
}
