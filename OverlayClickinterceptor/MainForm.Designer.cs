using System.Drawing;
using System.Windows.Forms;

namespace OverlayClickinterceptor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnPlaceOverlay = new Button();
            tbClickMessages = new TextBox();
            lbWindows = new ListBox();
            contextMenuListBox = new ContextMenuStrip(components);
            closeToolStripMenuItem = new ToolStripMenuItem();
            contextMenuListBox.SuspendLayout();
            SuspendLayout();
            // 
            // btnPlaceOverlay
            // 
            btnPlaceOverlay.Location = new Point(12, 12);
            btnPlaceOverlay.Name = "btnPlaceOverlay";
            btnPlaceOverlay.Size = new Size(217, 29);
            btnPlaceOverlay.TabIndex = 0;
            btnPlaceOverlay.Text = "Add New Overlay";
            btnPlaceOverlay.UseVisualStyleBackColor = true;
            btnPlaceOverlay.Click += btnPlaceOverlay_Click;
            // 
            // tbClickMessages
            // 
            tbClickMessages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbClickMessages.Location = new Point(235, 12);
            tbClickMessages.Multiline = true;
            tbClickMessages.Name = "tbClickMessages";
            tbClickMessages.ReadOnly = true;
            tbClickMessages.ScrollBars = ScrollBars.Vertical;
            tbClickMessages.Size = new Size(415, 426);
            tbClickMessages.TabIndex = 1;
            // 
            // lbWindows
            // 
            lbWindows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbWindows.ContextMenuStrip = contextMenuListBox;
            lbWindows.FormattingEnabled = true;
            lbWindows.Location = new Point(12, 54);
            lbWindows.Name = "lbWindows";
            lbWindows.ScrollAlwaysVisible = true;
            lbWindows.Size = new Size(217, 384);
            lbWindows.TabIndex = 2;
            lbWindows.MouseDown += lbWindows_MouseDown;
            // 
            // contextMenuListBox
            // 
            contextMenuListBox.ImageScalingSize = new Size(20, 20);
            contextMenuListBox.Items.AddRange(new ToolStripItem[] { closeToolStripMenuItem });
            contextMenuListBox.Name = "contextMenuListBox";
            contextMenuListBox.Size = new Size(169, 28);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(168, 24);
            closeToolStripMenuItem.Text = "Close Overlay";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(662, 450);
            Controls.Add(lbWindows);
            Controls.Add(tbClickMessages);
            Controls.Add(btnPlaceOverlay);
            Name = "MainForm";
            Text = "Overlay Click Interceptor";
            contextMenuListBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnPlaceOverlay;
        private TextBox tbClickMessages;
        private ListBox lbWindows;
        private ContextMenuStrip contextMenuListBox;
        private ToolStripMenuItem closeToolStripMenuItem;
    }
}
