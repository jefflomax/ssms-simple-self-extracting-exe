using ExtractShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSExtractor
{
	public class ConfigFromXaml : IConfigureExtractor
	{
		public ConfigFromXaml
		(
			string extName,
			string extFolder,
			string resource,
			string defaultFolder
		)
		{
			ExtName = extName;
			ExtFolder = extFolder;
			Resource = resource;
			DefaultFolder = defaultFolder;
		}

		public string ExtName { get; }
		public string ExtFolder { get; }
		public string Resource { get; }
		public string DefaultFolder { get; }
	}
}
