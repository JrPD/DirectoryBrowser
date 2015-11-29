using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApiFirst.Classes.Models;

namespace WebApiFirst.Classes
{
	public class DirManager
	{
		//current directory
		private readonly DirectoryInfo _currentDir;

		// list of directories and files
		private List<DirectoryItems> _dirItems;

		private DirectoryInfo[] _dirs;
		private FileInfo[] _files;

		public DirManager(DirectoryInfo info)
		{
			_currentDir = info;
			_dirItems = new List<DirectoryItems>();
		}

		public List<DirectoryItems> GetDirectoriesFromDirectory()
		{
			if (_currentDir == null)
			{
				return null;
			}

			GetFilesAndDirs();

			if (_files == null && _dirs == null)
			{
				return _dirItems;
			}

			// count small, medium, large weight files
			new DirectoryWalker(_currentDir).CountFiles();

			CheckIfRoot();

			// add directories to list
			AddItemsToList();

			return _dirItems;
		}

		private void AddItemsToList()
		{
			if (_dirs != null)
				_dirItems.AddRange(_dirs.Select(item => new DirectoryItems { Path = item.FullName, Name = item.Name }));

			if (_files != null)
				_dirItems.AddRange(_files.Select(file => new DirectoryItems { Path = file.FullName, Name = file.Name }));
		}

		private void GetFilesAndDirs()
		{
			// get derectories
			try
			{
				if (_currentDir.Attributes.HasFlag(FileAttributes.Directory))
				{
					_files = _currentDir.GetFiles("*.*");
					_dirs = _currentDir.GetDirectories();
				}
				else
				{
					Log.GetLog().Write("It is a FILE");
				}
			}
			catch (Exception e)
			{
				Log.GetLog().Write(e.Message);
			}
		}

		private void CheckIfRoot()
		{
			// if root, add discs
			if (_currentDir.Name.ToLower() == _currentDir.Root.Name.ToLower())
			{
				AddRootDiscs();
			}
			// add parent directory link
			else	
			{
				AddParentDirectory();
			}
		}

		private void AddParentDirectory()
		{
			if (_currentDir.Parent != null)
				_dirItems.Add(new DirectoryItems()
				{
					Path = _currentDir.Parent.FullName,
					Name = "../",
					FilesSmall = FilesCount.Small,
					FilesLarge = FilesCount.Large,
					FilesMedium = FilesCount.Medium
				});
		}

		private void AddRootDiscs()
		{
			_dirItems.Add(new DirectoryItems()
			{
				Path = "c:\\",
				Name = "C",
				FilesSmall = FilesCount.Small,
				FilesLarge = FilesCount.Large,
				FilesMedium = FilesCount.Medium
			});
			_dirItems.Add(new DirectoryItems() {Path = "e:\\", Name = "E"});
		}
	}
}