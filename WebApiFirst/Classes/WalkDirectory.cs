using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace WebApiFirst.Classes
{
	public class DirectoryWalker
	{
		private readonly int _deepLevel;
		private FileInfo[] _files;
		private DirectoryInfo _root;

		public DirectoryWalker(DirectoryInfo info)
		{
			_root = info;

			//set deep level from web.config
			int.TryParse(ConfigurationManager.AppSettings["deepLevel"], out _deepLevel);
			if (_deepLevel == 0)
			{
				_deepLevel = 2;
			}
		}

		/// <summary>
		/// resursive obtain information about files in directory and subdirectories
		/// </summary>
		/// <param name="root"></param>
		/// <param name="deepLevel"></param>
		private void WalkDirectoryTree(DirectoryInfo root, int deepLevel)
		{
			_files = Getfiles();

			if (_files == null)
			{
				return;
			}

			CountFileWeight();

			deepLevel--;
			if (deepLevel < 0) return;

			WalkSubDirectories(root, deepLevel);
		}

		private void WalkSubDirectories(DirectoryInfo root, int deepLevel)
		{
			try
			{
				var subDirs = root.GetDirectories();

				foreach (var dirInfo in subDirs)
				{
					WalkDirectoryTree(dirInfo, deepLevel);
				}
			}
			catch (Exception)
			{
			}
		}

		private void CountFileWeight()
		{
			const int mbyte = 1024 * 1024;

			foreach (var file in _files)
			{
				double fileSize = file.Length;
				if (fileSize != 0.0)
				{
					fileSize /= mbyte;
				}

				if (fileSize < (int)FileSizes.Small)
				{
					FilesCount.AddSmall();
				}
				else if (fileSize < (int)FileSizes.Medium)
				{
					FilesCount.AddMedium();
				}
				else
				{
					FilesCount.AddLarge();
				}
			}
		}

		private FileInfo[] Getfiles()
		{
			try
			{
				if (_root != null)
				{
					return _root.GetFiles("*.*");
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return null;
		}

		/// <summary>
		/// to count amount of files
		/// </summary>
		/// <param name="info"></param>
		public void CountFiles()
		{
			FilesCount.Clear();
			WalkDirectoryTree(_root, _deepLevel);
		}
	}
}