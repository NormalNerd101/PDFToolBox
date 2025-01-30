from PyPDF2 import PdfReader, PdfWriter
from pathlib import Path


def insert_pages(
        input_pdf: Path, 
        pages_to_insert: Path, 
        position: int
    ) -> None:
        """
        Insert pages from one PDF into another at a specified position.
        
        Args:
            input_pdf (Path): Original PDF file
            pages_to_insert (Path): PDF with pages to insert
            position (int): Page position to insert at (0-indexed)
            output_pdf (Path): Output PDF file
        """
        try:
            # Read the original PDF and pages to insert
            original_reader = PdfReader(input_pdf)
            insert_reader = PdfReader(pages_to_insert)

            # Ensure the file with pages to insert contains pages
            if len(insert_reader.pages) == 0:
                raise ValueError(f"The file {pages_to_insert} contains no pages.")

            writer = PdfWriter()

            # Add pages up to the insertion point
            for i, page in enumerate(original_reader.pages):
                if i == position:  # Insert here
                    for insert_page in insert_reader.pages:
                        writer.add_page(insert_page)
                writer.add_page(page)

            # If position is beyond the last page, append the insert pages at the end
            if position >= len(original_reader.pages):
                for insert_page in insert_reader.pages:
                    writer.add_page(insert_page)

            # Write the resulting PDF to the output file
            output_pdf = input_pdf.with_name(f"inserted_{input_pdf.name}")
            with open(output_pdf, 'wb') as output_file:
                writer.write(output_file)

            print(f"Pages inserted successfully. Output saved to {output_pdf}")
        
        except Exception as e:
            print(f"An error occurred: {e}")