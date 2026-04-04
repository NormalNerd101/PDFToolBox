using PDFToolBox.PdfTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PDFToolBox.Dialogs;

public class ExtractDialog : Form
{
    private readonly string? _filepath;
    private Label   _statusLabel = null!;
    private TextBox _rangesBox   = null!;
    private Button  _cutBtn      = null!;

    public ExtractDialog(string? filepath = null)
    {
        _filepath = filepath;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Cut Pages";
        Size            = new Size(360, 230);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;

        _statusLabel = new Label
        {
            Text     = _filepath != null ? $"File: {System.IO.Path.GetFileName(_filepath)}" : "Select PDF to cut pages",
            AutoSize  = false,
            Size      = new Size(330, 30),
            Location  = new Point(12, 12),
            Font      = new Font("Segoe UI", 9)
        };

        var rangeLabel = new Label
        {
            Text     = "Ranges to cut (e.g. 1-2, 5-6):",
            Location = new Point(12, 52),
            AutoSize = true
        };

        _rangesBox = new TextBox
        {
            Location        = new Point(12, 76),
            Width           = 320,
            PlaceholderText = "1-3, 5-7"
        };

        _cutBtn = new Button
        {
            Text     = "Choose File / Cut Pages",
            Font     = new Font("Segoe UI", 12, FontStyle.Bold),
            Size     = new Size(330, 48),
            Location = new Point(12, 118),
            Cursor   = Cursors.Hand
        };
        _cutBtn.Click += OnCut;

        Controls.AddRange(new Control[] { _statusLabel, rangeLabel, _rangesBox, _cutBtn });
    }

    private void OnCut(object? sender, EventArgs e)
    {
        string? file = ResolveFile();
        if (file is null) return;

        var ranges = ParseRanges(_rangesBox.Text);
        if (ranges is null) return;

        bool ok = PdfExtractor.ExtractPages(file, ranges);

        if (ok)
            MessageBox.Show("Pages cut successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show("Failed to cut pages.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private string? ResolveFile()
    {
        if (_filepath is not null)
        {
            _statusLabel.Text = $"File: {System.IO.Path.GetFileName(_filepath)}";
            return _filepath;
        }

        using OpenFileDialog ofd = new() { Filter = "PDF Files (*.pdf)|*.pdf" };
        if (ofd.ShowDialog() != DialogResult.OK) return null;
        _statusLabel.Text = $"File: {System.IO.Path.GetFileName(ofd.FileName)}";
        return ofd.FileName;
    }

    private List<(int, int)>? ParseRanges(string input)
    {
        var result = new List<(int, int)>();
        try
        {
            foreach (string part in input.Split(',', StringSplitOptions.TrimEntries))
            {
                string[] lr = part.Split('-');
                result.Add((int.Parse(lr[0].Trim()), int.Parse(lr[1].Trim())));
            }
            return result;
        }
        catch
        {
            MessageBox.Show("Invalid range format. Use: 1-3, 5-7", "Parse Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
    }
}
