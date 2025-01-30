from typing import List, Tuple
from pathlib import Path
import os
from PyPDF2 import PdfReader, PdfWriter


def single_split(
    input_file: Path, 
    start: int, 
    end: int
) -> None:
    """
    Split a PDF into a specific page range. works like extraction of pages.
    
    Args:
        input_file (Path): Source PDF file
        output_file (Path): Output PDF file
        start (int): Starting page number (1-indexed)
        end (int): Ending page number (1-indexed)
    """
    reader = PdfReader(input_file)
    writer = PdfWriter()
    
    # Add specified pages to the writer
    for page_num in range(start - 1, end):
        if 0 <= page_num < len(reader.pages):
            writer.add_page(reader.pages[page_num])
        else:
            print(f"Page {page_num} is out of range for this PDF.")
    
    output_file = input_file.with_name(f"{input_file.stem}_split_{start}-{end}.pdf")
    with open(output_file, "wb") as output_file:
        writer.write(output_file)
    print(f"Done splitting: {output_file}")


def multiple_split(
    input_file: Path, 
    ranges: List[Tuple[int, int]]
) -> None:
    """
    Split a PDF into multiple custom page ranges.
    
    Args:
        input_file (Path): Source PDF file
        output_prefix (str): Prefix for output filenames
        ranges (List[Tuple[int, int]]): List of page ranges to extract (1-indexed)
    """
    try:
        dir_path = os.path.dirname(input_file)
        reader = PdfReader(input_file)

        for start, end in ranges:
            if start < 1 or end > len(reader.pages):
                print(f"Invalid range: {start}-{end}. Skipping.")
                continue
            
            writer = PdfWriter()
            for page_num in range(start - 1, end):  
                writer.add_page(reader.pages[page_num])
            
            output_pdf_file = os.path.join(dir_path, f"{input_file.name}-{start}-{end}.pdf")
            with open(output_pdf_file, 'wb') as pdf:
                writer.write(pdf)
            print(f"Done splitting: {output_pdf_file}")
    
    except Exception as err:
        print(f"Error during splitting process: {err}")