namespace DailyQuote
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
            menuStrip = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            openExcelMenuItem = new ToolStripMenuItem();
            fileSeparator1 = new ToolStripSeparator();
            exportProgressMenuItem = new ToolStripMenuItem();
            importProgressMenuItem = new ToolStripMenuItem();
            fileSeparator2 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();
            progressMenu = new ToolStripMenuItem();
            resetProgressMenuItem = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            rootLayout = new TableLayoutPanel();
            optionsPanel = new FlowLayoutPanel();
            showEnglishCheckBox = new CheckBox();
            showUrduCheckBox = new CheckBox();
            orderLabel = new Label();
            orderComboBox = new ComboBox();
            contentHost = new Panel();
            readingPanel = new Panel();
            refLabel = new Label();
            urduTextBox = new DailyQuote.Controls.ReadingTextBox();
            englishTextBox = new DailyQuote.Controls.ReadingTextBox();
            arabicTextBox = new DailyQuote.Controls.ReadingTextBox();
            messagePanel = new TableLayoutPanel();
            messageLabel = new Label();
            messageButton = new Button();
            progressLayout = new TableLayoutPanel();
            progressLabel = new Label();
            progressBar = new ProgressBar();
            buttonsPanel = new FlowLayoutPanel();
            markReadButton = new Button();
            skipButton = new Button();
            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            rootLayout.SuspendLayout();
            optionsPanel.SuspendLayout();
            contentHost.SuspendLayout();
            readingPanel.SuspendLayout();
            messagePanel.SuspendLayout();
            progressLayout.SuspendLayout();
            buttonsPanel.SuspendLayout();
            SuspendLayout();
            //
            // menuStrip
            //
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, progressMenu, helpMenu });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(884, 24);
            menuStrip.TabIndex = 0;
            //
            // fileMenu
            //
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openExcelMenuItem, fileSeparator1, exportProgressMenuItem, importProgressMenuItem, fileSeparator2, exitMenuItem });
            fileMenu.Name = "fileMenu";
            fileMenu.Text = "&File";
            //
            // openExcelMenuItem
            //
            openExcelMenuItem.Name = "openExcelMenuItem";
            openExcelMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openExcelMenuItem.Text = "&Open Excel...";
            openExcelMenuItem.Click += openExcelMenuItem_Click;
            //
            // exportProgressMenuItem
            //
            exportProgressMenuItem.Name = "exportProgressMenuItem";
            exportProgressMenuItem.Text = "&Export Progress...";
            exportProgressMenuItem.Click += exportProgressMenuItem_Click;
            //
            // importProgressMenuItem
            //
            importProgressMenuItem.Name = "importProgressMenuItem";
            importProgressMenuItem.Text = "&Import Progress...";
            importProgressMenuItem.Click += importProgressMenuItem_Click;
            //
            // exitMenuItem
            //
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += exitMenuItem_Click;
            //
            // progressMenu
            //
            progressMenu.DropDownItems.AddRange(new ToolStripItem[] { resetProgressMenuItem });
            progressMenu.Name = "progressMenu";
            progressMenu.Text = "&Progress";
            //
            // resetProgressMenuItem
            //
            resetProgressMenuItem.Name = "resetProgressMenuItem";
            resetProgressMenuItem.Text = "&Reset Progress...";
            resetProgressMenuItem.Click += resetProgressMenuItem_Click;
            //
            // helpMenu
            //
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
            helpMenu.Name = "helpMenu";
            helpMenu.Text = "&Help";
            //
            // aboutMenuItem
            //
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Text = "&About";
            aboutMenuItem.Click += aboutMenuItem_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 639);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(884, 22);
            statusStrip.TabIndex = 2;
            //
            // statusLabel
            //
            statusLabel.Name = "statusLabel";
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            //
            // rootLayout
            //
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(optionsPanel, 0, 0);
            rootLayout.Controls.Add(contentHost, 0, 1);
            rootLayout.Controls.Add(progressLayout, 0, 2);
            rootLayout.Controls.Add(buttonsPanel, 0, 3);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 24);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new Padding(10, 6, 10, 6);
            rootLayout.RowCount = 4;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayout.Size = new Size(884, 615);
            rootLayout.TabIndex = 1;
            //
            // optionsPanel
            //
            optionsPanel.AutoSize = true;
            optionsPanel.Controls.Add(showEnglishCheckBox);
            optionsPanel.Controls.Add(showUrduCheckBox);
            optionsPanel.Controls.Add(orderLabel);
            optionsPanel.Controls.Add(orderComboBox);
            optionsPanel.Dock = DockStyle.Fill;
            optionsPanel.Name = "optionsPanel";
            optionsPanel.TabIndex = 0;
            //
            // showEnglishCheckBox
            //
            showEnglishCheckBox.Anchor = AnchorStyles.Left;
            showEnglishCheckBox.AutoSize = true;
            showEnglishCheckBox.Checked = true;
            showEnglishCheckBox.CheckState = CheckState.Checked;
            showEnglishCheckBox.Margin = new Padding(3, 3, 16, 3);
            showEnglishCheckBox.Name = "showEnglishCheckBox";
            showEnglishCheckBox.TabIndex = 0;
            showEnglishCheckBox.Text = "Show &English";
            showEnglishCheckBox.CheckedChanged += showEnglishCheckBox_CheckedChanged;
            //
            // showUrduCheckBox
            //
            showUrduCheckBox.Anchor = AnchorStyles.Left;
            showUrduCheckBox.AutoSize = true;
            showUrduCheckBox.Checked = true;
            showUrduCheckBox.CheckState = CheckState.Checked;
            showUrduCheckBox.Margin = new Padding(3, 3, 24, 3);
            showUrduCheckBox.Name = "showUrduCheckBox";
            showUrduCheckBox.TabIndex = 1;
            showUrduCheckBox.Text = "Show &Urdu";
            showUrduCheckBox.CheckedChanged += showUrduCheckBox_CheckedChanged;
            //
            // orderLabel
            //
            orderLabel.Anchor = AnchorStyles.Left;
            orderLabel.AutoSize = true;
            orderLabel.Name = "orderLabel";
            orderLabel.TabIndex = 2;
            orderLabel.Text = "Order:";
            //
            // orderComboBox
            //
            orderComboBox.Anchor = AnchorStyles.Left;
            orderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            orderComboBox.Items.AddRange(new object[] { "Sequential (Excel order)", "Random" });
            orderComboBox.Name = "orderComboBox";
            orderComboBox.Size = new Size(180, 23);
            orderComboBox.TabIndex = 3;
            orderComboBox.SelectedIndexChanged += orderComboBox_SelectedIndexChanged;
            //
            // contentHost
            //
            contentHost.BorderStyle = BorderStyle.FixedSingle;
            contentHost.Controls.Add(readingPanel);
            contentHost.Controls.Add(messagePanel);
            contentHost.Dock = DockStyle.Fill;
            contentHost.Margin = new Padding(3, 6, 3, 6);
            contentHost.Name = "contentHost";
            contentHost.TabIndex = 1;
            //
            // readingPanel
            //
            readingPanel.AutoScroll = true;
            readingPanel.BackColor = SystemColors.Window;
            // Docked Top controls stack in reverse order of addition: Arabic, English, Urdu, Ref.
            readingPanel.Controls.Add(refLabel);
            readingPanel.Controls.Add(urduTextBox);
            readingPanel.Controls.Add(englishTextBox);
            readingPanel.Controls.Add(arabicTextBox);
            readingPanel.Dock = DockStyle.Fill;
            readingPanel.Name = "readingPanel";
            readingPanel.Padding = new Padding(24, 16, 24, 16);
            readingPanel.TabIndex = 0;
            //
            // arabicTextBox
            //
            arabicTextBox.BottomGap = 20;
            arabicTextBox.Dock = DockStyle.Top;
            arabicTextBox.Font = new Font("Segoe UI", 26F);
            arabicTextBox.LineSpacing = 1.2F;
            arabicTextBox.Name = "arabicTextBox";
            arabicTextBox.RightToLeft = RightToLeft.Yes;
            arabicTextBox.Size = new Size(834, 60);
            arabicTextBox.TabIndex = 0;
            arabicTextBox.Text = "";
            //
            // englishTextBox
            //
            englishTextBox.BottomGap = 16;
            englishTextBox.Dock = DockStyle.Top;
            englishTextBox.Font = new Font("Segoe UI", 12F);
            englishTextBox.Name = "englishTextBox";
            englishTextBox.RightToLeft = RightToLeft.No;
            englishTextBox.Size = new Size(834, 40);
            englishTextBox.TabIndex = 1;
            englishTextBox.Text = "";
            //
            // urduTextBox
            //
            urduTextBox.BottomGap = 16;
            urduTextBox.Dock = DockStyle.Top;
            urduTextBox.Font = new Font("Segoe UI", 18F);
            urduTextBox.LineSpacing = 1.6F;
            urduTextBox.Name = "urduTextBox";
            urduTextBox.RightToLeft = RightToLeft.Yes;
            urduTextBox.Size = new Size(834, 50);
            urduTextBox.TabIndex = 2;
            urduTextBox.Text = "";
            //
            // refLabel
            //
            refLabel.AutoSize = true;
            refLabel.Dock = DockStyle.Top;
            refLabel.Font = new Font("Segoe UI", 9F);
            refLabel.ForeColor = SystemColors.GrayText;
            refLabel.Name = "refLabel";
            refLabel.Padding = new Padding(0, 4, 0, 0);
            refLabel.TabIndex = 3;
            //
            // messagePanel
            //
            messagePanel.BackColor = SystemColors.Window;
            messagePanel.ColumnCount = 1;
            messagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            messagePanel.Controls.Add(messageLabel, 0, 1);
            messagePanel.Controls.Add(messageButton, 0, 2);
            messagePanel.Dock = DockStyle.Fill;
            messagePanel.Name = "messagePanel";
            messagePanel.RowCount = 4;
            messagePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            messagePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            messagePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            messagePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            messagePanel.TabIndex = 1;
            messagePanel.Visible = false;
            messagePanel.Resize += messagePanel_Resize;
            //
            // messageLabel
            //
            messageLabel.Anchor = AnchorStyles.None;
            messageLabel.AutoSize = true;
            messageLabel.Font = new Font("Segoe UI", 12F);
            messageLabel.Margin = new Padding(3, 3, 3, 16);
            messageLabel.Name = "messageLabel";
            messageLabel.TabIndex = 0;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            //
            // messageButton
            //
            messageButton.Anchor = AnchorStyles.None;
            messageButton.AutoSize = true;
            messageButton.Name = "messageButton";
            messageButton.Padding = new Padding(12, 4, 12, 4);
            messageButton.TabIndex = 1;
            messageButton.UseVisualStyleBackColor = true;
            messageButton.Click += messageButton_Click;
            //
            // progressLayout
            //
            progressLayout.AutoSize = true;
            progressLayout.ColumnCount = 2;
            progressLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            progressLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            progressLayout.Controls.Add(progressLabel, 0, 0);
            progressLayout.Controls.Add(progressBar, 1, 0);
            progressLayout.Dock = DockStyle.Fill;
            progressLayout.Name = "progressLayout";
            progressLayout.RowCount = 1;
            progressLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            progressLayout.TabIndex = 2;
            //
            // progressLabel
            //
            progressLabel.Anchor = AnchorStyles.Left;
            progressLabel.AutoSize = true;
            progressLabel.Margin = new Padding(3, 3, 12, 3);
            progressLabel.Name = "progressLabel";
            progressLabel.TabIndex = 0;
            progressLabel.Text = "Read 0 of 0";
            //
            // progressBar
            //
            progressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(700, 18);
            progressBar.TabIndex = 1;
            //
            // buttonsPanel
            //
            buttonsPanel.AutoSize = true;
            buttonsPanel.Controls.Add(markReadButton);
            buttonsPanel.Controls.Add(skipButton);
            buttonsPanel.Dock = DockStyle.Fill;
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.TabIndex = 3;
            //
            // markReadButton
            //
            markReadButton.AutoSize = true;
            markReadButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            markReadButton.Name = "markReadButton";
            markReadButton.Padding = new Padding(16, 4, 16, 4);
            markReadButton.TabIndex = 0;
            markReadButton.Text = "Mark as Read  (Enter)";
            markReadButton.UseVisualStyleBackColor = true;
            markReadButton.Click += markReadButton_Click;
            //
            // skipButton
            //
            skipButton.AutoSize = true;
            skipButton.Font = new Font("Segoe UI", 10F);
            skipButton.Name = "skipButton";
            skipButton.Padding = new Padding(16, 4, 16, 4);
            skipButton.TabIndex = 1;
            skipButton.Text = "Skip  (S)";
            skipButton.UseVisualStyleBackColor = true;
            skipButton.Click += skipButton_Click;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(884, 661);
            Controls.Add(rootLayout);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(520, 420);
            Name = "MainForm";
            RightToLeft = RightToLeft.No;
            Text = "Arabic Reader";
            FormClosing += MainForm_FormClosing;
            KeyDown += MainForm_KeyDown;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            rootLayout.ResumeLayout(false);
            rootLayout.PerformLayout();
            optionsPanel.ResumeLayout(false);
            optionsPanel.PerformLayout();
            contentHost.ResumeLayout(false);
            readingPanel.ResumeLayout(false);
            readingPanel.PerformLayout();
            messagePanel.ResumeLayout(false);
            messagePanel.PerformLayout();
            progressLayout.ResumeLayout(false);
            progressLayout.PerformLayout();
            buttonsPanel.ResumeLayout(false);
            buttonsPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem openExcelMenuItem;
        private ToolStripSeparator fileSeparator1;
        private ToolStripMenuItem exportProgressMenuItem;
        private ToolStripMenuItem importProgressMenuItem;
        private ToolStripSeparator fileSeparator2;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem progressMenu;
        private ToolStripMenuItem resetProgressMenuItem;
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem aboutMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private TableLayoutPanel rootLayout;
        private FlowLayoutPanel optionsPanel;
        private CheckBox showEnglishCheckBox;
        private CheckBox showUrduCheckBox;
        private Label orderLabel;
        private ComboBox orderComboBox;
        private Panel contentHost;
        private Panel readingPanel;
        private DailyQuote.Controls.ReadingTextBox arabicTextBox;
        private DailyQuote.Controls.ReadingTextBox englishTextBox;
        private DailyQuote.Controls.ReadingTextBox urduTextBox;
        private Label refLabel;
        private TableLayoutPanel messagePanel;
        private Label messageLabel;
        private Button messageButton;
        private TableLayoutPanel progressLayout;
        private Label progressLabel;
        private ProgressBar progressBar;
        private FlowLayoutPanel buttonsPanel;
        private Button markReadButton;
        private Button skipButton;
    }
}
