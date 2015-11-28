using System;
using System.IO;

namespace WebApiFirst.Classes
{
	public static class DirInformation
	{
		public static DirectoryInfo GetDirInfo(string dir)
		{
			try
			{
				string path = MakePath(dir);
				return new DirectoryInfo(path);
			}
			catch (Exception)
			{
				return null;
			}
		}

		// concatenate string
		private static string MakePath(string dir)
		{
			//todo change this
			return dir.Replace("[", "").Replace("]", "").Replace(@"""", "").Replace(",", "\\");
		}
	}
}