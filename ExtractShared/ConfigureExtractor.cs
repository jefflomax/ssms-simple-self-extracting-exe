namespace ExtractShared
{
	public class ConfigureExtractor : IConfigureExtractor
	{
		public string ExtName => "Source Control Deep Links";
		public string ExtFolder => "SourceControlDeepLinks";
		public string Resource => "SSMSExtractorConsole.Archive.SourceControlDeepLinks-0.9.0.zip";
		public string DefaultFolder => @"C:\Program Files (x86)\Microsoft SQL Server Management Studio 18\Common7\IDE\Extensions";
	}
}
