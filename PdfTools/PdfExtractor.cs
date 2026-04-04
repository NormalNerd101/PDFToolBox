using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace PDFToolBox.PdfTools;

public static class PdfExtractor
{
    /// <summary>
    /// Removes pages within the specified ranges from a PDF and saves the result as a new file.
    /// All page numbers are 1-indexed, inclusive.
    /// </summary>
    public static bool ExtractPages(string sourcePath, IReadOnlyList<(int Start, int End)> ranges)
    {
        try
        {
            // Build set of 0-indexed pages to remove
            var pagesToRemove = new HashSet<int>();
            foreach ((int start, int end) in ranges)
                for (int p = start - 1; p < end; p++)
                    pagesToRemove.Add(p);

            using PdfDocument reader = PdfReader.Open(sourcePath, PdfDocumentOpenMode.Import);
            using PdfDocument writer = new();

            for (int i = 0; i < reader.PageCount; i++)
                if (!pagesToRemove.Contains(i))
                    writer.AddPage(reader.Pages[i]);

            string dir    = Path.GetDirectoryName(sourcePath)!;
            string name   = Path.GetFileName(sourcePath);
            string output = Path.Combine(dir, "cut_" + name);
            writer.Save(output);

            Console.WriteLine($"Successfully cut pages. Saved to: {output}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during page cutting: {ex.Message}");
            return false;
        }
    }
}
