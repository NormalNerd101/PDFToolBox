using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace PDFToolBox.PdfTools;

public static class PdfInserter
{
    /// <summary>
    /// Inserts all pages from <paramref name="pagesToInsertPath"/> into <paramref name="inputPdf"/>
    /// at the given 0-indexed <paramref name="position"/>.
    /// If position >= total pages the inserted pages are appended at the end.
    /// </summary>
    public static bool InsertPages(string inputPdf, string pagesToInsertPath, int position)
    {
        try
        {
            using PdfDocument original = PdfReader.Open(inputPdf, PdfDocumentOpenMode.Import);
            using PdfDocument insert   = PdfReader.Open(pagesToInsertPath, PdfDocumentOpenMode.Import);

            if (insert.PageCount == 0)
                throw new InvalidOperationException($"{pagesToInsertPath} contains no pages.");

            using PdfDocument writer = new();

            // Pages before insertion point
            int insertAt = Math.Min(position, original.PageCount);
            for (int i = 0; i < insertAt; i++)
                writer.AddPage(original.Pages[i]);

            // Inserted pages
            foreach (PdfPage page in insert.Pages)
                writer.AddPage(page);

            // Remaining original pages
            for (int i = insertAt; i < original.PageCount; i++)
                writer.AddPage(original.Pages[i]);

            string dir    = Path.GetDirectoryName(inputPdf)!;
            string stem   = Path.GetFileNameWithoutExtension(inputPdf);
            string output = Path.Combine(dir, $"Inserted_PageNum-{position + 1}_{stem}.pdf");
            writer.Save(output);

            Console.WriteLine($"Pages inserted successfully. Saved to: {output}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting pages: {ex.Message}");
            return false;
        }
    }
}
