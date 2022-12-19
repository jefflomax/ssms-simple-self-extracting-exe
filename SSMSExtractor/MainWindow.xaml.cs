using System;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using System.IO.Compression;
using ExtractShared;

namespace SSMSExtractor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, ILog
	{
		private ExtractEmbedded _extract;
		private bool _completed = false;
		public MainWindow()
		{
			InitializeComponent();

			var extensionName = GetResource( "extName" );

			var config = new ConfigFromXaml
			(
				extensionName,
				GetResource( "extFolder" ),
				GetResource( "resource" ),
				GetResource( "defaultFolder" )
			);

			_extract = new ExtractEmbedded( config, this );

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

		private void Extract_Click( object sender, RoutedEventArgs e )
		{
			ClearUI();

			if( !ExtractEmbedded.VerifyFolder( extractPath.Text ) )
			{
				var message = $"'{extractPath.Text}' must exist and be a folder";
				MessageBox.Show( message, "Invalid folder" );
				steps.Items.Add( message );
				status.Content = "Failed";

				extractPath.Text = GetResource( "defaultFolder" );
				_extract.DefaultFolder = extractPath.Text;
				return;
			}

			_extract.Extract();

			status.Content = (_extract.Completed)
				? "Done"
				: "Failed" ;
		}

		private void ChoosePath_Click( object sender, RoutedEventArgs e )
		{
			var fp = new WpfCore.FolderPicker.FolderBrowserDialog();
			fp.DefaultFolder = extractPath.Text;
			var dialogResult = fp.ShowDialog();
			if( dialogResult.HasFlag(System.Windows.Forms.DialogResult.OK))
			{
				if( ExtractEmbedded.VerifyFolder( fp.Folder ) )
				{
					extractPath.Text = fp.Folder;
					_extract.DefaultFolder = extractPath.Text;
				}
			}
		}

		public void Log(string message )
		{
			steps.Items.Add( message );
		}

	}
}
