using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace PDFToolBox.PdfTools;

public static class PdfSplitter
{
    /// <summary>
    /// Extracts a contiguous page range from a PDF into a new file (1-indexed).
    /// </summary>
    public static bool SingleSplit(string inputFile, int start, int end)
    {
        try
        {
            using PdfDocument reader = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import);
            using PdfDocument writer = new();

            for (int i = start - 1; i < end; i++)
            {
                if (i < 0 || i >= reader.PageCount)
                {
                    Console.WriteLine($"Page {i + 1} is out of range, skipping.");
                    continue;
                }
                writer.AddPage(reader.Pages[i]);
            }

            string dir      = Path.GetDirectoryName(inputFile)!;
            string stem     = Path.GetFileNameWithoutExtension(inputFile);
            string output   = Path.Combine(dir, $"{stem}-{start}-{end}.pdf");
            writer.Save(output);

            Console.WriteLine($"Done splitting: {output}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during splitting: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Splits a PDF into multiple files, one per provided page range (1-indexed, inclusive).
    /// </summary>
    public static bool MultipleSplit(string inputFile, IReadOnlyList<(int Start, int End)> ranges)
    {
        try
        {
            using PdfDocument reader = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import);
            string dir  = Path.GetDirectoryName(inputFile)!;
            string stem = Path.GetFileNameWithoutExtension(inputFile);

            foreach ((int start, int end) in ranges)
            {
                if (start < 1 || end > reader.PageCount)
                {
                    Console.WriteLine($"Invalid range {start}-{end}, skipping.");
                    continue;
                }

                using PdfDocument writer = new();
                for (int i = start - 1; i < end; i++)
                    writer.AddPage(reader.Pages[i]);

                string output = Path.Combine(dir, $"{stem}-{start}-{end}.pdf");
                writer.Save(output);
                Console.WriteLine($"Done splitting: {output}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during splitting: {ex.Message}");
            return false;
        }
    }
}
