using System;
using System.Windows.Forms;
using PDFToolBox.Gui;

namespace PDFToolBox;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Handle context menu registration flags (run once to set up / tear down)
        if (args.Length > 0 && args[0] == "--register")
        {
            ContextMenuRegistrar.Register();
            MessageBox.Show(
                "PDF ToolBox has been added to the right-click context menu for PDF files.",
                "Registered", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (args.Length > 0 && args[0] == "--unregister")
        {
            ContextMenuRegistrar.Unregister();
            MessageBox.Show(
                "PDF ToolBox has been removed from the right-click context menu.",
                "Unregistered", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        // A PDF file path may be passed when launched from context menu
        string? filepath = args.Length > 0 ? args[0] : null;
        Application.Run(new MainForm(filepath));
    }
}
