using static System.Console;
using ExtractShared;
using SSMSExtractorConsole;

var logger = new Logger();

WriteLine( "SSMS Self-extracting Extension installer" );
WriteLine( "Must run with elevated priviledge as it deletes a folder" );
WriteLine( @"in C:\Program Files (x86)" );

var extract = new ExtractEmbedded
(
	new ConfigureExtractor(),
	logger
);

WriteLine( "Press enter to accept SSMS extensions folder:" );
WriteLine( extract.DefaultFolder );
WriteLine( "Or enter extensions fully qualified path (excluding extension folder name)" );
var extensionPath = ReadLine();

if( !string.IsNullOrEmpty( extensionPath ) )
{
	var path = extensionPath.Trim();
	if( ExtractEmbedded.VerifyFolder( path ) )
	{
		extract.DefaultFolder = path;
	}
	else
	{
		logger.Log( $"'{path}' must exist and be a fully qualified path" );
		Environment.Exit( -1 );
	}
}

extract.Extract();

WriteLine( "Press any key" );
ReadKey();
