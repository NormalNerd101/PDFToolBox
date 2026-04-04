using Microsoft.Win32;
using System;
using System.IO;

namespace PDFToolBox;

/// <summary>
/// Registers / unregisters a Windows Explorer right-click context menu entry
/// for .pdf files under HKEY_CURRENT_USER — no administrator rights required.
///
/// Registry path written:
///   HKCU\Software\Classes\SystemFileAssociations\.pdf\shell\PDFToolBox
///   HKCU\Software\Classes\SystemFileAssociations\.pdf\shell\PDFToolBox\command
/// </summary>
public static class ContextMenuRegistrar
{
    private const string MenuLabel   = "Open with PDF ToolBox";
    private const string RegistryKey = @"Software\Classes\SystemFileAssociations\.pdf\shell\PDFToolBox";

    public static void Register()
    {
        // Full path to this executable
        string exePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "PDFToolBox.exe");

        using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RegistryKey);
        if (key is null) throw new InvalidOperationException("Could not open registry key.");

        key.SetValue(string.Empty, MenuLabel);
        key.SetValue("Icon", $"\"{exePath}\",0");   // use the exe's embedded icon

        using RegistryKey? cmd = key.CreateSubKey("command");
        if (cmd is null) throw new InvalidOperationException("Could not create command key.");

        // %1 receives the path to the right-clicked file
        cmd.SetValue(string.Empty, $"\"{exePath}\" \"%1\"");
    }

    public static void Unregister()
    {
        Registry.CurrentUser.DeleteSubKeyTree(RegistryKey, throwOnMissingSubKey: false);
    }
}
