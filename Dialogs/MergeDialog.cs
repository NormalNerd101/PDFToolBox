using PDFToolBox.PdfTools;
using System.Windows.Forms;

namespace PDFToolBox.Dialogs;

public class MergeDialog : Form
{
    private Label  _label;
    private Button _chooseBtn;

    public MergeDialog()
    {
        Text          = "Merge PDFs";
        Size          = new System.Drawing.Size(320, 180);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox   = false;
        StartPosition = FormStartPosition.CenterParent;

        _label = new Label
        {
            Text      = "Select PDFs to merge",
            AutoSize  = true,
            Location  = new System.Drawing.Point(12, 20)
        };

        _chooseBtn = new Button
        {
            Text     = "Choose Files",
            Font     = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold),
            Size     = new System.Drawing.Size(280, 50),
            Location = new System.Drawing.Point(12, 60),
            Cursor   = Cursors.Hand
        };
        _chooseBtn.Click += OnChooseFiles;

        Controls.AddRange(new Control[] { _label, _chooseBtn });
    }

    private void OnChooseFiles(object? sender, System.EventArgs e)
    {
        using OpenFileDialog ofd = new()
        {
            Title      = "Select PDFs to merge",
            Filter     = "PDF Files (*.pdf)|*.pdf",
            Multiselect = true
        };

        if (ofd.ShowDialog() != DialogResult.OK) return;

        _label.Text = $"Selected: {ofd.FileNames.Length} files";

        bool ok = PdfMerger.MergePdfs(ofd.FileNames);

        if (ok)
            MessageBox.Show("PDFs merged successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show("Failed to merge PDFs.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
