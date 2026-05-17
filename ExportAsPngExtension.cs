using System;
using System.IO;
using System.Threading.Tasks;
using Pinta.Core;

[assembly: Mono.Addins.Addin ("ExportAsPng", "1.0", Category = "File")]
[assembly: Mono.Addins.AddinName ("Export as PNG")]
[assembly: Mono.Addins.AddinDescription ("Export the current image as a flattened PNG without changing the open document")]
[assembly: Mono.Addins.AddinDependency ("Pinta", PintaCore.AddinCompatVersion)]

namespace ExportAsPng;

[Mono.Addins.Extension]
public sealed class ExportAsPngExtension : IExtension
{
	private Command? export_command;

	public void Initialize ()
	{
		export_command = new Command ("ExportAsPng", "Export as PNG...", null, null, shortcuts: ["<Primary>E"]);
		export_command.Activated += (_, _) => { _ = ExportAsync (); };
		PintaCore.Chrome.Application.AddCommand (export_command);
		PintaCore.Actions.Addins.AddMenuItem (export_command.CreateMenuItem ());
	}

	public void Uninitialize ()
	{
	}

	private async Task ExportAsync ()
	{
		if (!PintaCore.Workspace.HasOpenDocuments)
			return;

		Document document = PintaCore.Workspace.ActiveDocument;
		Gtk.Window parent = PintaCore.Chrome.MainWindow;

		FormatDescriptor? format = PintaCore.ImageFormats.GetFormatByExtension ("png");
		if (format is null || !format.IsExportAvailable ())
			return;

		var fcd = Gtk.FileChooserNative.New (
			"Export as PNG",
			parent,
			Gtk.FileChooserAction.Save,
			"Export",
			"Cancel");

		string baseName = Path.GetFileNameWithoutExtension (document.DisplayName);
		fcd.SetCurrentName ($"{baseName}.png");
		fcd.AddFilter (format.Filter);
		fcd.Filter = format.Filter;

		if (await fcd.RunAsync () != Gtk.ResponseType.Accept)
			return;

		Gio.File file = fcd.GetFile ()!;

		PintaCore.Tools.Commit ();

		try {
			format.Exporter.Export (document, file, parent);
		} catch (Exception e) {
			await PintaCore.Chrome.ShowMessageDialog (parent, "Failed to export image", e.Message);
		}
	}
}
