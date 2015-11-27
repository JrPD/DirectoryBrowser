using System.IO;

namespace WebApiFirst.Classes
{
	public static class DirInformation
	{
		public static DirectoryInfo GetDirInfo(string dir)
		{
			string path = MakePath(dir);
			return new DirectoryInfo(path);
		}
		// concatenate string
		private static string MakePath(string dir)
		{
			//todo change this
			return dir.Replace("[", "").Replace("]", "").Replace(@"""", "").Replace(",", "\\");
		}
	}
}