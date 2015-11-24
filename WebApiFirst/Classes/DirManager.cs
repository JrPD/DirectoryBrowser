﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WebApiFirst.Models;

namespace WebApiFirst.Classes
{
	public class DirManager
	{
		public static StringBuilder TraceErrors = new StringBuilder();

		// to recursive function WalkDirectoryTree
		private readonly int _deepLevel;

		public DirManager()
		{
			//set deep level from web.config
			int.TryParse(ConfigurationManager.AppSettings["deepLevel"], out _deepLevel);
			if (_deepLevel==0)
			{
				_deepLevel = 2;
			}
		}

		// concatenate string
		private string MakePath(string dir)
		{
			//todo change this
			return dir.Replace("[", "").Replace("]", "").Replace(@"""", "").Replace(",", "\\");
		}

		public List<DirectoryItems> GetDirs(System.IO.DirectoryInfo info)
		{
			List<DirectoryItems> items = new List<DirectoryItems>();
			DirectoryInfo[] dir = null;
			FileInfo[] files = null;

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
					throw new Exception();
				}
			}
			catch (DirectoryNotFoundException e)
			{
				lock (TraceErrors)
				{
					TraceErrors.AppendLine(e.Message);
				}
			}
			catch (UnauthorizedAccessException e)
			{
				lock (TraceErrors)
				{
					TraceErrors.AppendLine(e.Message);
				}
				// if no access
				info = info.Parent;
				if (info != null) dir = info.GetDirectories();
				if (info != null) files = info.GetFiles("*.*");
			}
			catch (Exception ex)
			{
				lock (TraceErrors)
				{
					TraceErrors.AppendLine("It is a file!");
				}
			}

			// count small, medium, height weight files

			if (info == null)
			{
				return items;
			}
			CountFiles(info);

			// if root, add discs
			if (info.Name.ToLower() == info.Root.Name)
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

			// add directories to list
			if (dir != null)
				items.AddRange(dir.Select(item => new DirectoryItems { Path = item.FullName, Name = item.Name }));

			// add files to list
			if (files != null)
				items.AddRange(files.Select(file => new DirectoryItems { Path = file.FullName, Name = file.Name }));
			return items;
		}

		/// <summary>
		/// resursive obtain information about files in directory and subdirectories
		/// </summary>
		/// <param name="root"></param>
		/// <param name="deepLevel"></param>
		public static void WalkDirectoryTree(DirectoryInfo root, int deepLevel)
		{
			const int mbyte = 1024 * 1024;
			System.IO.FileInfo[] files = null;
			try
			{
				files = root.GetFiles("*.*");
			}
			catch (UnauthorizedAccessException e)
			{
				Debug.WriteLine(e.Message);
			}
			catch (System.IO.DirectoryNotFoundException e)
			{
				Debug.WriteLine(e.Message);
			}
			catch (PathTooLongException e)
			{
				Debug.WriteLine(e.Message);
			}
			if (files == null)
			{
				return;
			}
			deepLevel--;
			foreach (FileInfo file in files)
			{
				double fileSize = file.Length;
				if (fileSize != 0.0)
				{
					fileSize /= mbyte;
					Debug.WriteLine("{0}", fileSize);
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
			if (deepLevel >= 0)
			{
				var subDirs = root.GetDirectories();
				foreach (DirectoryInfo dirInfo in subDirs)
				{
					WalkDirectoryTree(dirInfo, deepLevel);
				}
			}
		}

		public DirectoryInfo GetDirInfo(string dir)
		{
			string path = MakePath(dir);
			return new DirectoryInfo(path);
		}

		/// <summary>
		/// to count amount of files
		/// </summary>
		/// <param name="info"></param>
		private void CountFiles(DirectoryInfo info)
		{
			FilesCount.Clear();
			WalkDirectoryTree(info, _deepLevel);

			Debug.WriteLine("{0} - {1}", "<10", FilesCount.Small);
			Debug.WriteLine("{0} - {1}", "10<X<50", FilesCount.Medium);
			Debug.WriteLine("{0} - {1}", ">50", FilesCount.Large);
		}

	}
}