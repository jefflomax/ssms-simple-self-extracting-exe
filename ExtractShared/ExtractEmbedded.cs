using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.IO;


namespace ExtractShared
{
	public class ExtractEmbedded
	{
		private bool _completed = false;
		private IConfigureExtractor _config;
		private string _extensionsFolder;
		private ILog _log;
		public ExtractEmbedded
		(
			IConfigureExtractor configureExtractor,
			ILog logger
		)
		{
			_config = configureExtractor;
			_extensionsFolder = _config.DefaultFolder;
			_log = logger;
		}

		public string DefaultFolder
		{
			get { return _extensionsFolder; }
			set { _extensionsFolder = value; }
		}

		public void Extract()
		{
			var assembly = this.GetType().Assembly;

			var extFolder = _config.ExtFolder;
			var tempDir = Directory.CreateTempSubdirectory( extFolder );
			_log.Log( $"Temp folder {tempDir}" );

			var tempZipFileName = $"{extFolder}.zip";
			var tempZipFilePath = Path.Join( tempDir.FullName, tempZipFileName );
			var foundResourceStream = false;
			using( var tempZipFile = File.Create( tempZipFilePath ) )
			{
				using( var stream = assembly.GetManifestResourceStream( _config.Resource ) )
				{
					if( stream != null )
					{
						_log.Log( $"Found resource {_config.Resource}" );
						foundResourceStream = true;
						stream.CopyTo( tempZipFile );
						_log.Log( $"Copied to {tempZipFilePath}" );
					}
					else
					{
						_log.Log( $"Did not find resource {_config.Resource}" );
					}
				}
				tempZipFile.Close();

				if( foundResourceStream )
				{
					var extractFilePath = Path.Join
					(
						_extensionsFolder,
						_config.ExtFolder
					);
					_log.Log( $"Target path {extractFilePath}" );

					// Delete existing
					if( Directory.Exists( extractFilePath ) )
					{
						_log.Log( $"Deleting existing path and contents" );
						Directory.Delete( extractFilePath, recursive: true );
					}

					_log.Log( $"Extracting {extractFilePath}" );
					ZipFile.ExtractToDirectory
					(
						tempZipFilePath,
						extractFilePath
					);
					_completed = true;
				}

				_log.Log( $"Deleting temp file {tempZipFilePath}" );
				File.Delete( tempZipFilePath );
			}

			_log.Log( $"Deleting temp folder {tempDir.FullName}" );
			tempDir.Delete();

			var status = (Completed)
				? "Done"
				: "Failed";
			_log.Log( status );
		}

		public bool Completed => _completed;

		public static bool VerifyFolder( string path )
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
