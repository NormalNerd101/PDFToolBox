from pathlib import Path   
from typing import List
from PyPDF2 import PdfReader, PdfWriter



def get_bookmarks(bookmark_list: list, reader: PdfReader) -> dict:
        """
        Convert bookmark list to dictionary mapping page numbers to titles.
        
        Args:
            bookmark_list (list): List of bookmarks
            reader (PdfReader): PDF reader object
        
        Returns:
            dict: Dictionary of page numbers to bookmark titles
        """
        result = {}
        if not bookmark_list:
            return result
        
        for item in bookmark_list:
            if isinstance(item, list):
                # Recursively process nested bookmark lists
                nested_bookmarks = get_bookmarks(item, reader)
                result.update(nested_bookmarks)
            else:
                try:
                    title = item.title
                    page_num = item.page
                    
                    if isinstance(page_num, (int, float)):
                        result[page_num] = title
                except Exception as e:
                    print(f"Warning: Could not process bookmark {item}: {e}")
        
        return result



def insert_bookmarks(
        bookmark_names: List[str], 
        bookmark_page_nums: List[int], 
        pdf_source_file: Path, 
        out_pdf_file: Path
    ) -> bool:
        """
        Insert bookmarks into a PDF file.
        
        Args:
            bookmark_names (List[str]): List of bookmark titles
            bookmark_page_nums (List[int]): Corresponding page numbers
            pdf_source_file (Path): Source PDF file
            out_pdf_file (Path): Output PDF file
        
        Returns:
            bool: True if successful, False otherwise
        """
        try:
            # Validate input lists
            if len(bookmark_names) != len(bookmark_page_nums):
                print("Error: Bookmark names and page numbers lists must have the same length.")
                return False

            reader = PdfReader(pdf_source_file)
            writer = PdfWriter()
            
            bookmark_data = {
                num_page - 1: name for name, num_page in zip(bookmark_names, bookmark_page_nums)
            }
            
            for page_num, page in enumerate(reader.pages):
                if page_num in bookmark_data:
                    writer.add_outline_item(bookmark_data[page_num], page_num)
                writer.add_page(page)
            
            with open(out_pdf_file, 'wb') as pdf_file:
                writer.write(pdf_file)
            
            print(f"Successfully added bookmarks. Saved new file to: {out_pdf_file}.")
            return True
        
        except Exception as e:
            print(f"Critical error during bookmark insertion: {e}")
            return False


