# ExportAsPng

A Pinta add-in that adds **Add-ins → Export as PNG...**. Exports a flattened PNG to disk without changing the currently open document. Unlike *Save As*, your original file (e.g. an ORA with multiple layers) stays open and unchanged after the export.

## Prerequisites

- [Pinta](https://www.pinta-project.com/) 3.1 or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Build

```
dotnet build -c Release
```

The output DLL will be at:

```
bin\Release\net10.0\ExportAsPng.dll
```

## Install

1. Copy **only** `ExportAsPng.dll` to the same directory as the Pinta executable.

2. Restart Pinta.

3. The menu item appears under **Add-ins → Export as PNG...** (shortcut: Ctrl+E)

> You do not need to copy `Pinta.Core.dll` or `Mono.Addins.dll` - these are already loaded by Pinta itself.

## Uninstall

Delete `ExportAsPng.dll` from the Pinta executable directory and restart Pinta.

## How it works

- Uses Pinta's existing PNG exporter (`GdkPixbufFormat`), which flattens layers into a temporary surface internally. The document's layers are never modified.
- Does **not** update `document.File`, `document.FileType`, or the undo history, so the window title and dirty state are unchanged after export.
