# PDFToolBox — C# Port

WinForms/.NET 8 rewrite of the original PyQt6 Python tool.

## Requirements
- .NET 8 SDK (https://dotnet.microsoft.com/download)
- Windows (WinForms + Registry context menu)

## Dependencies (auto-restored via NuGet)
- `PDFsharp` 6.1.1

## Build & Run

```bash
dotnet restore
dotnet build -c Release
dotnet run                          # no file selected
dotnet run -- "C:\path\to\file.pdf" # open with a pre-selected PDF
```

## Publish as single EXE

```bash
dotnet publish -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The output EXE will be in `bin/Release/net8.0-windows/win-x64/publish/`.

## Right-click Context Menu

Run the EXE once with the `--register` flag to add "Open with PDF ToolBox"
to the right-click menu for all .pdf files (writes to HKCU — no admin needed):

```bash
PDFToolBox.exe --register
```

Or use **Tools → Register Context Menu** inside the app itself.

To remove it:

```bash
PDFToolBox.exe --unregister
```

## Assets

Place `assets/icon.ico` and `assets/profile_pic.png` next to the EXE (or in
the project's `assets/` folder — they are copied automatically on build).

## Feature mapping from Python

| Python (PyPDF2 / PyQt6)   | C# (PdfSharp / WinForms)         |
|---------------------------|----------------------------------|
| `merge_pdfs()`            | `PdfMerger.MergePdfs()`          |
| `single_split()`          | `PdfSplitter.SingleSplit()`      |
| `multiple_split()`        | `PdfSplitter.MultipleSplit()`    |
| `extract_pages()`         | `PdfExtractor.ExtractPages()`    |
| `insert_pages()`          | `PdfInserter.InsertPages()`      |
| `QSplitter`               | `SplitContainer`                 |
| `QStackedWidget`          | `TabControl`                     |
| `QFileDialog`             | `OpenFileDialog`                 |
| `QMessageBox`             | `MessageBox`                     |
