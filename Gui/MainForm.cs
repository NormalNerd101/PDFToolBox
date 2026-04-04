using PDFToolBox.Dialogs;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PDFToolBox.Gui;

public class MainForm : Form
{
    private readonly string? _filepath;

    public MainForm(string? filepath = null)
    {
        _filepath = filepath;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "PDF Tools";
        Size            = new Size(620, 520);
        MinimumSize     = new Size(500, 400);
        BackColor       = Color.FromArgb(243, 220, 214);   // #f3dcd6
        StartPosition   = FormStartPosition.CenterScreen;

        // Window icon
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "icon.ico");
        if (File.Exists(iconPath)) Icon = new Icon(iconPath);

        // ── Main Menu (context-menu registration) ─────────────────────────
        var menuStrip = new MenuStrip();
        var toolsMenu = new ToolStripMenuItem("Tools");
        var registerItem   = new ToolStripMenuItem("Register Context Menu");
        var unregisterItem = new ToolStripMenuItem("Unregister Context Menu");
        registerItem.Click   += (_, _) =>
        {
            ContextMenuRegistrar.Register();
            MessageBox.Show("PDF ToolBox added to the PDF right-click menu.", "Done",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        unregisterItem.Click += (_, _) =>
        {
            ContextMenuRegistrar.Unregister();
            MessageBox.Show("PDF ToolBox removed from the PDF right-click menu.", "Done",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        toolsMenu.DropDownItems.Add(registerItem);
        toolsMenu.DropDownItems.Add(unregisterItem);
        menuStrip.Items.Add(toolsMenu);
        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);

        // ── SplitContainer (mirrors QSplitter) ───────────────────────────
        var splitter = new SplitContainer
        {
            Width            = 620,  // <--- ADD THIS LINE FIRST!
            Dock             = DockStyle.Fill,
            Orientation      = Orientation.Vertical,
            Panel1MinSize    = 120,
            Panel2MinSize    = 240,
            SplitterDistance = 230,  
            BackColor        = Color.FromArgb(243, 220, 214)
        };

        // ── Left: Profile picture ─────────────────────────────────────────
        var profileBox = new PictureBox
        {
            Dock     = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Padding  = new Padding(8)
        };

        string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "profile_pic.png");
        if (File.Exists(picPath))
            profileBox.Image = Image.FromFile(picPath);
        else
        {
            // Gray placeholder
            var bmp = new Bitmap(120, 120);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Gray);
            profileBox.Image = bmp;
        }

        splitter.Panel1.Controls.Add(profileBox);

        // ── Right: Feature buttons ────────────────────────────────────────
        var rightPanel = new TableLayoutPanel
        {
            Dock       = DockStyle.Fill,
            RowCount   = 5,
            ColumnCount = 1,
            BackColor  = Color.Transparent,
            Padding    = new Padding(10, 4, 10, 10)
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));   // title
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var titleLabel = new Label
        {
            Text      = "Features",
            Font      = new Font("Segoe UI", 16, FontStyle.Bold),
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        Button MakeBtn(string text) => new()
        {
            Text      = text,
            Font      = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock      = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(220, 190, 182),
            Cursor    = Cursors.Hand,
            Margin    = new Padding(0, 4, 0, 4)
        };

        var mergeBtn   = MakeBtn("Merge PDFs");
        var splitBtn   = MakeBtn("Split PDFs");
        var extractBtn = MakeBtn("Cut PDFs");
        var insertBtn  = MakeBtn("Insert Pages");

        mergeBtn.Click   += (_, _) => new MergeDialog().ShowDialog(this);
        splitBtn.Click   += (_, _) => new SplitDialog(_filepath).ShowDialog(this);
        extractBtn.Click += (_, _) => new ExtractDialog(_filepath).ShowDialog(this);
        insertBtn.Click  += (_, _) => new InsertDialog().ShowDialog(this);

        rightPanel.Controls.Add(titleLabel,  0, 0);
        rightPanel.Controls.Add(mergeBtn,    0, 1);
        rightPanel.Controls.Add(splitBtn,    0, 2);
        rightPanel.Controls.Add(extractBtn,  0, 3);
        rightPanel.Controls.Add(insertBtn,   0, 4);

        splitter.Panel2.Controls.Add(rightPanel);

        Controls.Add(splitter);
    }
}
