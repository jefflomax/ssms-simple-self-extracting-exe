using System;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using System.IO.Compression;
using System.Configuration;

namespace SSMSExtractor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _extension = "";
		private string _resource = "";
		private bool _completed = false;
		public MainWindow()
		{
			InitializeComponent();

			var res = GetResource( "defaultFolder" );
			var extensionName = GetResource( "extName" );
			_extension = GetResource( "extFolder" );
			_resource = GetResource( "resource" );

			extractFolderLabel.Content = extractFolderLabel.Content + extensionName;

			ClearUI();
		}

		private string GetResource( string key )
		{
			return FindResource( key ).ToString() ?? string.Empty;
		}

		private void ClearUI()
		{
			steps.Items.Clear();
			status.Content = "";
		}

		private void extract_Click( object sender, RoutedEventArgs e )
		{
			ClearUI();

			if( ! VerifyFolder( extractPath.Text ) )
			{
				var message = $"'{extractPath.Text}' must exist and be a folder";
				MessageBox.Show( message, "Invalid folder" );
				steps.Items.Add( message );
				status.Content = "Failed";

				extractPath.Text = GetResource( "defaultFolder" );
				return;
			}

			var tempDir = Directory.CreateTempSubdirectory( _extension );
			steps.Items.Add( $"Temp folder {tempDir}" );

			var assembly = this.GetType().Assembly;

			var tempZipFileName = $"{_extension}.zip";
			var tempZipFilePath = Path.Join( tempDir.FullName, tempZipFileName );
			var foundResourceStream = false;
			using( var tempZipFile = File.Create( tempZipFilePath ) )
			{
				using( var stream = assembly.GetManifestResourceStream( _resource ) )
				{
					if( stream != null )
					{
						steps.Items.Add( $"Found resource {_resource}" );
						foundResourceStream = true;
						stream.CopyTo( tempZipFile );
						steps.Items.Add( $"Copied to {tempZipFilePath}" );
					}
					else
					{
						steps.Items.Add( $"Did not find resource {_resource} - check app.config" );
					}
				}
				tempZipFile.Close();

				if( foundResourceStream )
				{
					var extractFilePath = Path.Join
					(
						extractPath.Text,
						_extension
					);
					steps.Items.Add( $"Target path {extractFilePath}" );

					// Delete existing
					if( Directory.Exists( extractFilePath ) )
					{
						steps.Items.Add( $"Deleting existing path and contents" );
						Directory.Delete( extractFilePath, recursive: true );
					}

					steps.Items.Add( $"Extracting {extractFilePath}" );
					ZipFile.ExtractToDirectory
					(
						tempZipFilePath,
						extractFilePath
					);
					_completed = true;
				}

				steps.Items.Add( $"Deleting temp file {tempZipFilePath}" );
				File.Delete( tempZipFilePath );
			}

			steps.Items.Add( $"Deleting temp folder {tempDir.FullName}" );
			tempDir.Delete();
			steps.Items.Add( $"Done" );

			status.Content = (_completed)
				? "Done"
				: "Failed" ;
		}

		private void choosePath_Click( object sender, RoutedEventArgs e )
		{
			var fp = new WpfCore.FolderPicker.FolderBrowserDialog();
			fp.DefaultFolder = extractPath.Text;
			var dialogResult = fp.ShowDialog();
			if( dialogResult.HasFlag(System.Windows.Forms.DialogResult.OK))
			{
				if( VerifyFolder( fp.Folder ) )
				{
					extractPath.Text = fp.Folder;
				}
			}
		}

		private static bool VerifyFolder( string path )
		{
			try
			{
				return File
					.GetAttributes( path )
					.HasFlag( FileAttributes.Directory );
			}
			catch
			{
			}
			return false;
		}
	}
}
