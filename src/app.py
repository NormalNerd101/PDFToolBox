import sys, os
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), 'gui/')))
from gui.main_window import *




def main():
    app = QApplication(sys.argv)
    window = MainWindow()
    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    main()