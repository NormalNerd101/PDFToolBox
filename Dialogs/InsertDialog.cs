using PDFToolBox.PdfTools;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PDFToolBox.Dialogs;

public class InsertDialog : Form
{
    private Label         _infoLabel    = null!;
    private NumericUpDown _pageNumber   = null!;
    private Button        _insertBtn    = null!;

    public InsertDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Insert Pages";
        Size            = new Size(400, 250);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;

        _infoLabel = new Label
        {
            Text = "Enter the page number to insert before, then click 'Insert Pages'.\n" +
                   "You'll be asked to pick:\n  1. The destination PDF\n  2. The PDF whose pages to insert",
            AutoSize  = false,
            Size      = new Size(370, 70),
            Location  = new Point(12, 12),
            Font      = new Font("Segoe UI", 9)
        };

        var numLabel = new Label
        {
            Text     = "Insert before page number (1-indexed):",
            Location = new Point(12, 95),
            AutoSize = true
        };

        _pageNumber = new NumericUpDown
        {
            Minimum  = 1,
            Maximum  = 10000,
            Value    = 1,
            Location = new Point(12, 118),
            Width    = 100
        };

        _insertBtn = new Button
        {
            Text     = "Insert Pages",
            Font     = new Font("Segoe UI", 12, FontStyle.Bold),
            Size     = new Size(366, 48),
            Location = new Point(12, 158),
            Cursor   = Cursors.Hand
        };
        _insertBtn.Click += OnInsert;

        Controls.AddRange(new Control[] { _infoLabel, numLabel, _pageNumber, _insertBtn });
    }

    private void OnInsert(object? sender, EventArgs e)
    {
        string? destFile = PickPdf("Select destination PDF (the file to insert pages into)");
        if (destFile is null) return;

        string? srcFile = PickPdf("Select source PDF (the pages to insert)");
        if (srcFile is null) return;

        // Convert from 1-indexed UI value to 0-indexed position
        int position = (int)_pageNumber.Value - 1;

        bool ok = PdfInserter.InsertPages(destFile, srcFile, position);

        if (ok)
            MessageBox.Show("Pages inserted successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show("Failed to insert pages.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private string? PickPdf(string title)
    {
        using OpenFileDialog ofd = new()
        {
            Title  = title,
            Filter = "PDF Files (*.pdf)|*.pdf"
        };
        return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
    }
}
