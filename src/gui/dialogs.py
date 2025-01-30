import sys, os
from pathlib import Path
from PyQt6.QtWidgets import QDialog, QVBoxLayout, QLabel, QPushButton, QFileDialog, QMessageBox
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from pdf_tools.merge import merge_pdfs
from pdf_tools.split import single_split, multiple_split


class MergeDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Merge PDFs")
        self.setFixedSize(300, 200)

        layout = QVBoxLayout()

        self.label = QLabel("Select PDFs to merge")
        self.btn_select = QPushButton("Choose Files")
        self.btn_merge = QPushButton("Merge")

        layout.addWidget(self.label)
        layout.addWidget(self.btn_select)
        layout.addWidget(self.btn_merge)

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



class SplitDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Split PDF")
        self.setFixedSize(300, 200)

        layout = QVBoxLayout()

        self.label = QLabel("Select a PDF to split")
        self.btn_select = QPushButton("Choose File")
        self.btn_split = QPushButton("Split")

        layout.addWidget(self.label)
        layout.addWidget(self.btn_select)
        layout.addWidget(self.btn_split)

        self.btn_select.clicked.connect(self.select_file)

        self.setLayout(layout)

    def select_file(self):
        file, _ = QFileDialog.getOpenFileName(self, "Select a PDF", "", "PDF Files (*.pdf)")
        if file:
            self.label.setText(f"Selected: {file}")
            self.file = file  

            print("Selected File:", self.file)
