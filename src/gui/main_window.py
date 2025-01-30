import sys
from PyQt6.QtWidgets import (
    QApplication, QMainWindow, QLabel, QVBoxLayout, QWidget,
    QTreeView, QSplitter, QFrame
)
from PyQt6.QtGui import QFileSystemModel
from PyQt6.QtCore import Qt


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()

        self.setWindowTitle("PDF Tools")
        self.setGeometry(500, 300, 1000, 600)

        # Set up central widget
        splitter = QSplitter(Qt.Orientation.Horizontal)

        # Set up Left panel for File Explorer
        self.file_explorer = QTreeView()
        self.file_explorer.setFrameShape(QFrame.Shape.StyledPanel)

        # connect to File System Model
        self.model = QFileSystemModel()
        self.model.setRootPath(r"C:\Users\Windows")
        self.file_explorer.setModel(self.model)
        root_index = self.model.index(self.model.rootPath())
        self.file_explorer.setRootIndex(root_index)
        self.file_explorer.setSelectionMode(QTreeView.SelectionMode.MultiSelection)

        # Add the file explorer to the left panel
        splitter.addWidget(self.file_explorer)


        # Set up Right panel for feature options
        setting_panel = QWidget()
        setting_layout = QVBoxLayout()
        setting_label = QLabel("Settings") 
        setting_layout.addWidget(setting_label)
        setting_panel.setLayout(setting_layout)
        splitter.addWidget(setting_panel)

        self.setCentralWidget(splitter)


def main():
    app = QApplication(sys.argv)
    window = MainWindow()

    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    main()