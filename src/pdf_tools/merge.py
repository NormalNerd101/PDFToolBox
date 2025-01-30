from typing import List
from pathlib import Path
from PyPDF2 import PdfReader, PdfWriter, errors




def merge_pdfs(
    pdf_files: List[Path], 
    out_pdf_path: Path
) -> bool:
    """
    Merge PDF files while preserving existing bookmarks.
    
    Args:
        pdf_files (List[Path]): List of PDF files to merge
        out_pdf_path (Path): Output path for merged PDF
    
    Returns:
        bool: True if merge successful, False otherwise
    """
    try:
        writer = PdfWriter()
        page_num_counter = 0
        
        # Merge files
        for file in pdf_files:
            try:
                pdf_reader = PdfReader(file)
                
                # Handle encrypted PDFs
                if pdf_reader.is_encrypted:
                    try:
                        pdf_reader.decrypt('')
                    except errors.PdfReadError:
                        print(f"Error decrypting file {file}: Unsupported encryption")
                        raise errors.PdfReadError()
                        continue
                
                num_pages = len(pdf_reader.pages)
                
                if num_pages == 0:
                    print(f"Warning: {file} has no pages, skipping")
                    continue
                
                # Add bookmark for the file
                filename = file.stem
                writer.add_outline_item(filename, page_num_counter)
                
                # Add pages
                for page in pdf_reader.pages:
                    writer.add_page(page)
                page_num_counter += num_pages
                
            except Exception as e:
                print(f"Error processing file {file}: {e}")
                continue

        # Ensure output directory exists
        out_pdf_path.parent.mkdir(parents=True, exist_ok=True)
        
        # Write merged PDF to output
        with open(out_pdf_path, 'wb') as pdf_file:
            writer.write(pdf_file)
        
        print(f"Successfully merged PDF to: {out_pdf_path}")
        return True
        
    except Exception as e:
        print(f"Critical error during merge: {e}")
        return False

