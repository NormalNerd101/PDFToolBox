import os, sys
from PyQt6.QtWidgets import (
    QApplication, QMainWindow, QLabel, QVBoxLayout, QWidget,
    QTreeView, QSplitter, QFrame, QPushButton
)
from PyQt6.QtGui import QFileSystemModel, QPixmap, QIcon
from PyQt6.QtCore import Qt

# functions
from .dialogs import MergeDialog, SplitDialog, ExtractDialog, InsertDialog


# This class is currently not for use
class OptionsPanel(QWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.initUI()

    def initUI(self):
        self.layout = QVBoxLayout()

        # Set up Left panel for File Explorer
        self.file_explorer = QTreeView()
        self.file_explorer.setFrameShape(QFrame.Shape.StyledPanel)

        # connect to File System Model
        self.model = QFileSystemModel()
        self.model.setRootPath(r"C:/Users/Windows")
        self.file_explorer.setModel(self.model)
        root_index = self.model.index(self.model.rootPath())
        self.file_explorer.setRootIndex(root_index)
        self.file_explorer.setSelectionMode(QTreeView.SelectionMode.MultiSelection)

        self.layout.addWidget(self.file_explorer)
        self.setLayout(self.layout)



class SettingsPanel(QWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.initUI()

    def initUI(self):
        setting_layout = QVBoxLayout()
        setting_layout.setSpacing(0)

        # Label
        setting_label = QLabel("Settings")
        setting_label.setAlignment(Qt.AlignmentFlag.AlignTop)
        setting_label.setStyleSheet("font-size: 20px; font-weight: bold;")
        setting_layout.addWidget(setting_label)

        # Button
        self.merge_btn = QPushButton("Merge PDFs")
        self.split_btn = QPushButton("Split PDFs")
        self.extract_btn = QPushButton("Extract PDFs")
        self.insert_btn = QPushButton("Insert Pages")

        self.merge_btn.setStyleSheet("font-size: 20px; font-weight: bold; padding: 15px; border: 2px solid black;")
        self.split_btn.setStyleSheet("font-size: 20px; font-weight: bold; padding: 15px; border: 2px solid black;")
        self.extract_btn.setStyleSheet("font-size: 20px; font-weight: bold; padding: 15px; border: 2px solid black;")
        self.insert_btn.setStyleSheet("font-size: 20px; font-weight: bold; padding: 15px; border: 2px solid black;")
        self.merge_btn.setCursor(Qt.CursorShape.PointingHandCursor)
        self.split_btn.setCursor(Qt.CursorShape.PointingHandCursor)
        self.extract_btn.setCursor(Qt.CursorShape.PointingHandCursor)
        self.insert_btn.setCursor(Qt.CursorShape.PointingHandCursor)

        setting_layout.addWidget(self.merge_btn, alignment=Qt.AlignmentFlag.AlignCenter)
        setting_layout.addWidget(self.split_btn, alignment=Qt.AlignmentFlag.AlignCenter)
        setting_layout.addWidget(self.extract_btn, alignment=Qt.AlignmentFlag.AlignCenter)
        setting_layout.addWidget(self.insert_btn, alignment=Qt.AlignmentFlag.AlignCenter)
        
        self.merge_btn.clicked.connect(self.merge_pdf)
        self.split_btn.clicked.connect(self.split_pdf)
        self.extract_btn.clicked.connect(self.extract_pdf)
        self.insert_btn.clicked.connect(self.insert_pdf)

        self.setLayout(setting_layout)

    def merge_pdf(self):
        dialog = MergeDialog()
        dialog.exec()

    def split_pdf(self):
        dialog = SplitDialog()
        dialog.exec()
    
    def extract_pdf(self):
        dialog = ExtractDialog()
        dialog.exec()
    
    def insert_pdf(self):
        dialog = InsertDialog()
        dialog.exec()


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()

        self.setWindowTitle("PDF Tools")
        self.setGeometry(1000, 300, 500, 500)
        self.setStyleSheet('background-color: #f3dcd6;')

        if getattr(sys, 'frozen', False):  # If running as .exe
            self.assets_dir = os.path.join(sys._MEIPASS, '../../assets')
        else:
            self.assets_dir = os.path.join(os.path.dirname(os.path.dirname(__file__)), "../assets")

        # Set window icon (logo)
        logo_path = os.path.join(self.assets_dir, "icon.jpg")  # Change filename if needed

        if os.path.exists(logo_path):
            self.setWindowIcon(QIcon(logo_path))
        else:
            print("Logo file not found. Please check the path.")

        # Set up the central widget
        splitter = QSplitter(Qt.Orientation.Horizontal)

        # Left panel: Profile pic
        self.profile_panel = self.create_profile_panel()
        
        # Right panel: Settings
        self.settings_panel = SettingsPanel()

        splitter.addWidget(self.profile_panel)
        splitter.addWidget(self.settings_panel)

        self.setCentralWidget(splitter)

    def create_profile_panel(self):
        """Creates a profile panel with a picture loaded from assets."""
        panel = QWidget()
        layout = QVBoxLayout()

        # Path to profile picture in assets folder
        profile_pic_path = os.path.join(self.assets_dir, "profile_pic.png")

        # Profile picture
        self.profile_pic = QLabel()
        if os.path.exists(profile_pic_path):
            pixmap = QPixmap(profile_pic_path)
            new_width = int(pixmap.width() * 0.6)
            new_height = int(pixmap.height() * 0.6)
            pixmap = pixmap.scaled(new_width, new_height, Qt.AspectRatioMode.KeepAspectRatio)
        else:
            pixmap = QPixmap(100, 100)  # Create empty pixmap if file is missing
            pixmap.fill(Qt.GlobalColor.gray)

        self.profile_pic.setPixmap(pixmap)
        self.profile_pic.setScaledContents(True)

        layout.addWidget(self.profile_pic, alignment=Qt.AlignmentFlag.AlignCenter)
        
        panel.setLayout(layout)
        return panel
