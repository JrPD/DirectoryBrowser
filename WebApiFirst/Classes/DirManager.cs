using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebApiFirst.Models;

namespace WebApiFirst.Classes
{
	public class DirManager
	{
		private Log _loger = Log.GetLog();

		private List<DirectoryItems> _dirItems;
		
		public DirManager()
		{
			_dirItems =new List<DirectoryItems>();
		}

		public List<DirectoryItems> GetDirectoriesFromDirectory(DirectoryInfo info)
		{
			DirectoryInfo[] dir = null;
			FileInfo[] files = null;

			if (info == null)
			{
				return null;
			}

			GetFilesAndDirs(ref files, ref dir, info);

			if (files == null)
			{
				return _dirItems;
			}
			// count small, medium, large weight files
			DirectoryWalker.CountFiles(info);

			CheckIfRoot(_dirItems, info);

			// add directories to list
			if (dir != null)
				_dirItems.AddRange(dir.Select(item => new DirectoryItems { Path = item.FullName, Name = item.Name }));

			// add files to list
				_dirItems.AddRange(files.Select(file => new DirectoryItems { Path = file.FullName, Name = file.Name }));
			return _dirItems;
		}
		
		private void GetFilesAndDirs(ref FileInfo[] files, ref DirectoryInfo[] dir, DirectoryInfo info)
		{
			// get derectories
			try
			{
				if (info.Attributes.HasFlag(FileAttributes.Directory))
				{
					files = info.GetFiles("*.*");
					dir = info.GetDirectories();
				}
				else
				{
					_loger.Write("It is a FILE");
				}
			}
			catch (DirectoryNotFoundException e)
			{
				_loger.Write(e.Message);
			}
			catch (UnauthorizedAccessException e)
			{
				_loger.Write(e.Message);

				// if no access
				info = info.Parent;
				if (info != null) dir = info.GetDirectories();
				if (info != null) files = info.GetFiles("*.*");
			}
			catch (Exception e)
			{
				_loger.Write(e.Message);
			}
		}
		
		private void CheckIfRoot(List<DirectoryItems> items, DirectoryInfo info)
		{
			// if root, add discs
			if (info.Name.ToLower() == info.Root.Name.ToLower())
			{
				items.Add(new DirectoryItems()
				{
					Path = "c:\\",
					Name = "C",
					FilesSmall = FilesCount.Small,
					FilesLarge = FilesCount.Large,
					FilesMedium = FilesCount.Medium
				});
				items.Add(new DirectoryItems() { Path = "e:\\", Name = "E" });
			}
			else
			{
				// add parent directory link
				if (info.Parent != null)
					items.Add(new DirectoryItems()
					{
						Path = info.Parent.FullName,
						Name = "../",
						FilesSmall = FilesCount.Small,
						FilesLarge = FilesCount.Large,
						FilesMedium = FilesCount.Medium
					});
			}
		}

		
	}
}