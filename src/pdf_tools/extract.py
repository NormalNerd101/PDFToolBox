from typing import List, Tuple
from PyPDF2 import PdfReader, PdfWriter
from pathlib import Path




def cut_pages(
        ranges: List[Tuple[int, int]], 
        pdf_source_path: Path, 
        output_pdf_path: Path
    ) -> bool:
        """
        Remove pages within specified ranges from a PDF.
        
        Args:
            ranges (List[Tuple[int, int]]): Page ranges to remove (1-indexed)
            pdf_source_path (Path): Source PDF file
            output_pdf_path (Path): Output PDF file
        
        Returns:
            bool: True if successful, False otherwise
        """
        try:
            reader = PdfReader(pdf_source_path)
            writer = PdfWriter()

            # Precompute the pages to remove
            pages_to_remove = set(
                page_num
                for start, end in ranges
                for page_num in range(start - 1, end)
            )

            # Add only the pages not in the removal set
            for index, page in enumerate(reader.pages):
                if index not in pages_to_remove:
                    writer.add_page(page)

            # Write the result to the output file
            with output_pdf_path.open("wb") as pdf_file:
                writer.write(pdf_file)

            print(f"Successfully cut pages. Saved new file to: {output_pdf_path}.")
            return True

        except Exception as e:
            print(f"Error during page cutting: {e}")
            return False