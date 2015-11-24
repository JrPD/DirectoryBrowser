using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFirst.Models
{
	public class DirectoryItems
	{
		public string Path { get; set; }
		public string Name { get; set; }
		public int FilesSmall { get; set; }
		public int FilesMedium { get; set; }
		public int FilesLarge { get; set; }
	}
}