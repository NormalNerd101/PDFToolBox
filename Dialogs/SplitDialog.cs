using PDFToolBox.PdfTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PDFToolBox.Dialogs;

public class SplitDialog : Form
{
    private readonly string? _filepath;

    // Single Split controls
    private NumericUpDown _startPage = null!;
    private NumericUpDown _endPage   = null!;

    // Multiple Split controls
    private TextBox _rangesBox = null!;

    // Common
    private Label  _statusLabel = null!;
    private Button _actionBtn   = null!;
    private TabControl _tabs    = null!;

    public SplitDialog(string? filepath = null)
    {
        _filepath = filepath;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text            = "Split PDF";
        Size            = new Size(380, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;

        _statusLabel = new Label
        {
            Text     = _filepath != null ? $"File: {System.IO.Path.GetFileName(_filepath)}" : "No file selected",
            AutoSize  = false,
            Size      = new Size(350, 30),
            Location  = new Point(12, 10),
            Font      = new Font("Segoe UI", 9)
        };

        // ── Tab Control ──────────────────────────────────────────────
        _tabs = new TabControl
        {
            Location = new Point(12, 48),
            Size     = new Size(345, 170)
        };

        // Single Split tab
        TabPage singleTab = new("Single Split");
        singleTab.Controls.Add(new Label { Text = "From page:", Location = new Point(8, 15), AutoSize = true });
        _startPage = new NumericUpDown { Minimum = 1, Maximum = 10000, Location = new Point(8, 35), Width = 100 };
        singleTab.Controls.Add(_startPage);
        singleTab.Controls.Add(new Label { Text = "To page:", Location = new Point(8, 70), AutoSize = true });
        _endPage = new NumericUpDown { Minimum = 1, Maximum = 10000, Location = new Point(8, 90), Width = 100 };
        singleTab.Controls.Add(_endPage);

        // Multiple Split tab
        TabPage multiTab = new("Multiple Split");
        multiTab.Controls.Add(new Label
        {
            Text     = "Ranges (e.g. 1-2, 5-6):",
            Location = new Point(8, 15),
            AutoSize = true
        });
        _rangesBox = new TextBox
        {
            Location    = new Point(8, 38),
            Width       = 320,
            PlaceholderText = "1-3, 5-7, 10-12"
        };
        multiTab.Controls.Add(_rangesBox);

        _tabs.TabPages.Add(singleTab);
        _tabs.TabPages.Add(multiTab);

        // ── Action Button ────────────────────────────────────────────
        _actionBtn = new Button
        {
            Text     = "Split",
            Font     = new Font("Segoe UI", 12, FontStyle.Bold),
            Size     = new Size(345, 44),
            Location = new Point(12, 232),
            Cursor   = Cursors.Hand
        };
        _actionBtn.Click += OnSplit;

        Controls.AddRange(new Control[] { _statusLabel, _tabs, _actionBtn });
    }

    private void OnSplit(object? sender, EventArgs e)
    {
        string? file = ResolveFile();
        if (file is null) return;

        bool ok;

        if (_tabs.SelectedIndex == 0)
        {
            // Single split
            int start = (int)_startPage.Value;
            int end   = (int)_endPage.Value;
            ok = PdfSplitter.SingleSplit(file, start, end);
        }
        else
        {
            // Multiple split
            var ranges = ParseRanges(_rangesBox.Text);
            if (ranges is null) return;
            ok = PdfSplitter.MultipleSplit(file, ranges);
        }

        ShowResult(ok, "split");
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

    private static void ShowResult(bool ok, string op)
    {
        if (ok)
            MessageBox.Show($"PDFs {op} successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show($"Failed to {op} PDFs.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
