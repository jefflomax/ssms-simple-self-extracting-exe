using System;
using System.Collections.Generic;
using System.Text;

namespace ExtractShared
{
	public interface IConfigureExtractor
	{
		public string ExtName { get; }
		public string ExtFolder { get; }
		public string Resource { get; }
		public string DefaultFolder { get; }
	}
}
