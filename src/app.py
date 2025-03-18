import sys, os
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), 'gui/')))
from gui.main_window import *




def main(filepath=None):
    app = QApplication(sys.argv)
    window = MainWindow(filepath)
    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    filepath = sys.argv[1] if len(sys.argv) > 1 else None
    main(filepath)