using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace PDFToolBox.PdfTools;

public static class PdfMerger
{
    /// <summary>
    /// Merges a list of PDF files into a single output file saved alongside the first file.
    /// Each source file gets a named bookmark at its first page (mirrors Python behaviour).
    /// </summary>
    public static bool MergePdfs(IReadOnlyList<string> pdfFiles)
    {
        try
        {
            using PdfDocument output = new();
            int pageCounter = 0;

            foreach (string filePath in pdfFiles)
            {
                try
                {
                    using PdfDocument input = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                    if (input.PageCount == 0)
                    {
                        Console.WriteLine($"Warning: {filePath} has no pages, skipping.");
                        continue;
                    }

                    // Add bookmark for this file at its first page
                    string bookmarkTitle = Path.GetFileNameWithoutExtension(filePath);
                    output.Outlines.Add(bookmarkTitle, output.Pages.Count > 0
                        ? output.Pages[pageCounter]
                        : null!);   // null-safe: PdfSharp accepts null target for root outlines

                    foreach (PdfPage page in input.Pages)
                        output.AddPage(page);

                    pageCounter += input.PageCount;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {filePath}: {ex.Message}");
                }
            }

            string directory = Path.GetDirectoryName(pdfFiles[0])!;
            string outputPath = Path.Combine(directory, "merged_pdf_file.pdf");
            output.Save(outputPath);

            Console.WriteLine($"Successfully merged PDFs to: {outputPath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical error during merge: {ex.Message}");
            return false;
        }
    }
}
