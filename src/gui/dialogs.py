import sys, os
from pathlib import Path
from PyQt6.QtWidgets import (
    QDialog, QVBoxLayout, QLabel, QPushButton, QFileDialog, QMessageBox, QStackedWidget, QWidget, QLineEdit
)
from PyQt6.QtGui import QIntValidator
from PyQt6.QtCore import pyqtSignal

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from pdf_tools import single_split, multiple_split, merge_pdfs, extract_pages, insert_pages

class MergeDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Merge PDFs")
        self.setFixedSize(300, 200)

        layout = QVBoxLayout()

        self.label = QLabel("Select PDFs to merge")
        self.btn_select = QPushButton("Choose Files")
        self.btn_select.setStyleSheet("font-size: 16px; font-weight: bold; padding : 10px;")
        layout.addWidget(self.label)
        layout.addWidget(self.btn_select)

        self.btn_select.clicked.connect(self.merge_pdfs_catalog)

        self.setLayout(layout)

    def merge_pdfs_catalog(self):
        files, _ = QFileDialog.getOpenFileNames(self, "Select PDFs", "", "PDF Files (*.pdf)")
        if files:
            self.label.setText(f"Selected: {len(files)} files")
            pdf_files = [Path(file) for file in files]
            success = merge_pdfs(pdf_files)
            if success:
                QMessageBox.information(self, "Success", "PDFs merged successfully!")
            else:
                QMessageBox.warning(self, "Error", "Failed to merge PDFs.")


class DoubleClickButton(QPushButton):
    doubleClicked = pyqtSignal()  # Custom signal for double-click

    def __init__(self, text):
        super().__init__(text)

    def mouseDoubleClickEvent(self, event):
        self.doubleClicked.emit()  # Emit the double-click signal


class SplitDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Split PDF")
        self.setGeometry(800, 300, 300, 200)

        layout = QVBoxLayout()

        self.label = QLabel("Double-click to select PDF from the FE")
        layout.addWidget(self.label)

        # --- MODE SWITCH BUTTONS ---
        self.single_split_btn = DoubleClickButton("Single Split")
        self.multiple_split_btn = DoubleClickButton("Multiple Split")

        self.single_split_btn.clicked.connect(lambda: self.switch_mode(0))
        self.multiple_split_btn.clicked.connect(lambda: self.switch_mode(1))

        # --- SINGLE SPLIT WIDGET ---
        self.single_split_widget = QWidget()
        single_split_layout = QVBoxLayout()
        
        single_split_layout.addWidget(QLabel("From this Page number : "))
        self.start_pageNum = QLineEdit()
        self.start_pageNum.setValidator(QIntValidator(1, 10000))
        single_split_layout.addWidget(self.start_pageNum)
        single_split_layout.addWidget(QLabel("To this Page number : "))
        self.end_pageNum = QLineEdit()
        self.end_pageNum.setValidator(QIntValidator(1, 10000))
        single_split_layout.addWidget(self.end_pageNum)

        self.single_split_widget.setLayout(single_split_layout)

        
        # --- MULTIPLE SPLIT WIDGET ---
        self.multiple_split_widget = QWidget()
        multiple_split_layout = QVBoxLayout()

        multiple_split_layout.addWidget(QLabel("Enter the page ranges to split (e.g : 1-2, 5-6, ...): "))
        self.page_ranges = QLineEdit()
        multiple_split_layout.addWidget(self.page_ranges)

        self.multiple_split_widget.setLayout(multiple_split_layout)
    

        # --- ADD FUNCTIONS TO BUTTONS ---
        self.single_split_btn.doubleClicked.connect(self.single_split_catalog)
        self.multiple_split_btn.doubleClicked.connect(self.multi_split_catalog)

        # --- STACKED WIDGET ---
        self.stacked_widget = QStackedWidget()
        self.stacked_widget.addWidget(self.single_split_widget)
        self.stacked_widget.addWidget(self.multiple_split_widget)

        # --- ADD WIDGETS TO LAYOUT ---
        layout.addWidget(self.single_split_btn)
        layout.addWidget(self.multiple_split_btn)
        layout.addWidget(self.stacked_widget)

        self.switch_mode(0)

        self.setLayout(layout)
    

    def switch_mode(self, index):
        self.stacked_widget.setCurrentIndex(index)


    def single_split_catalog(self):
        file, _ = QFileDialog.getOpenFileName(self, "Select PDF", "", "PDF Files (*.pdf)")
        if file:
            file = Path(file)
            self.label.setText(f"Selected: {file}")
            start = int(self.start_pageNum.text())
            end = int(self.end_pageNum.text())
            success = single_split(file, start, end)
            if success:
                QMessageBox.information(self, "Success", "PDFs split successfully!")
            else:
                QMessageBox.warning(self, "Error", "Failed to split PDFs.")
    

    def multi_split_catalog(self):
        file, _ = QFileDialog.getOpenFileName(self, "Select PDFs", "", "PDF Files (*.pdf)")
        if file:
            file = Path(file)
            self.label.setText(f"Selected: {file}.")
            ranges = [tuple(map(int, r.split("-"))) for r in self.page_ranges.text().split(",")]
            success = multiple_split(file, ranges)
            if success:
                QMessageBox.information(self, "Success", "PDFs split successfully!")
            else:
                QMessageBox.warning(self, "Error", "Failed to split PDFs.")


class ExtractDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Cut Pages")
        self.setGeometry(800, 300, 300, 200)

        layout = QVBoxLayout()

        self.label = QLabel("Select PDF to extract pages")
        layout.addWidget(self.label)

        self.extract_btn = QPushButton("Choose File")
        self.extract_btn.setStyleSheet("font-size: 16px; font-weight: bold; padding : 10px;")

        layout.addWidget(QLabel("Ranges to cut (e.g : 1-2, 5-6, ...): "))
        self.page_ranges = QLineEdit()
        
        self.extract_btn.clicked.connect(self.extract_pages_catalog)

        layout.addWidget(self.page_ranges)
        layout.addWidget(self.extract_btn)

        self.setLayout(layout)

    def extract_pages_catalog(self):
        file, _ = QFileDialog.getOpenFileName(self, "Select PDF", "", "PDF Files (*.pdf)")
        if file:
            file = Path(file)
            self.label.setText(f"Selected: {file}")
            ranges = [tuple(map(int, r.split("-"))) for r in self.page_ranges.text().split(",")]
            success = extract_pages(file, ranges)
            if success:
                QMessageBox.information(self, "Success", "PDFs cut successfully!")
            else:
                QMessageBox.warning(self, "Error", "Failed to cut PDFs.")


class InsertDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Insert Pages")
        self.setGeometry(200, 200, 200, 200)

        layout = QVBoxLayout()

        self.label = QLabel("After enter the page number to start inserting. clicked 'Insert Pages', then choose the source file and the source pages.")
        layout.addWidget(self.label)

        layout.addWidget(QLabel("Insert from page number (0-indexed) : "))
        self.insertPageNumber = QLineEdit()
        self.insertPageNumber.setValidator(QIntValidator(1, 10000))

        self.insert_btn = QPushButton("Insert Pages")
        self.insert_btn.setStyleSheet("font-size: 15px; font-weight: bold; padding : 5px;")

        self.insert_btn.clicked.connect(self.insert_catalog)

        layout.addWidget(self.insertPageNumber)
        layout.addWidget(self.insert_btn)

        self.setLayout(layout)

    def select_catalog(self):
        file, _ = QFileDialog.getOpenFileName(self, "SELECT PDF", "", "PDF Files (*.pdf)")
        if file:
            return Path(file)
        return None

    def insert_catalog(self):
        source_file = self.select_catalog()
        source_pages = self.select_catalog()
        
        if source_file and source_pages:
            insert_page = int(self.insertPageNumber.text())
            success = insert_pages(source_file, source_pages, insert_page)
            if success:
                QMessageBox.information(self, "Success", "Pages inserted successfully!")
            else:
                QMessageBox.warning(self, "Error", "Failed to insert pages.")












