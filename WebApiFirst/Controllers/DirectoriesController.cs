using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Directory = WebApiFirst.Models.Directory;

namespace WebApiFirst.Controllers
{
	public class DirectoriesController : ApiController
	{
		// to recursive function WalkDirectoryTree
		private readonly int _deepLevel;

		public DirectoriesController()
		{
			//set deep level from web.config
			_deepLevel = Convert.ToInt32(ConfigurationManager.AppSettings["deepLevel"]);
			if (_deepLevel==0)
			{
				_deepLevel = 1;
			}
		}

		// init directories
		public IEnumerable<Directory> GetAlldirectories()
		{
			string path = HttpRuntime.AppDomainAppPath;
			var info = new System.IO.DirectoryInfo(path);

			List<Directory> directories= AllDirs(info);

			return directories;
		}

		public List<Directory> AllDirs(System.IO.DirectoryInfo info)
		{
			List<Directory> items = new List<Directory>();
			DirectoryInfo[] dir = null;
			FileInfo[] files = null;

			// get derectories
			try
			{
				if (info.Attributes == FileAttributes.Directory)
				{
					files = info.GetFiles("*.*");
					dir = info.GetDirectories();
				}
				else
				{
					throw new UnauthorizedAccessException();
				}
			}
			catch (DirectoryNotFoundException)
			{
				//throw new UnauthorizedAccessException();
			}
			catch (UnauthorizedAccessException)
			{
				// if no access
				info = info.Parent;
				if (info != null) dir = info.GetDirectories();
				if (info != null) files = info.GetFiles("*.*");
			}

			// count small, medium, height weight files

			if (info==null)
			{
				return items;
			}
			CountFiles(info);

			// if root, add discs
			if (info.Name.ToLower() == info.Root.Name)
			{
				items.Add(new Directory()
				{
					Path = "c:\\", Name = "C",
					FilesSmall = FilesCount.Small,
					FilesHeight = FilesCount.Height,
					FilesMedium = FilesCount.Medium
				});
				items.Add(new Directory() {Path = "e:\\", Name = "E"});
			}
			else
			{
				// add parent directory link
				if (info.Parent != null)
					items.Add(new Directory() {Path = info.Parent.FullName, Name = "../",
						FilesSmall = FilesCount.Small,
						FilesHeight = FilesCount.Height,
						FilesMedium =  FilesCount.Medium});
			}

			// add directories to list
			if(dir!=null)
				items.AddRange(dir.Select(item => new Directory() {Path = item.FullName, Name = item.Name}));
			
			// add files to list
			if (files!=null)
				items.AddRange(files.Select(file => new Directory() {Path = file.FullName, Name = file.Name}));
			return items;
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
			Debug.WriteLine("{0} - {1}", "10<50<100", FilesCount.Medium);
			Debug.WriteLine("{0} - {1}", ">100", FilesCount.Height);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir">selected directory</param>
		/// <returns>subdirectories with parent directory</returns>
		public IHttpActionResult GetSubDirectories(string dir)
		{
			string path = MakePath(dir);
			var info = new System.IO.DirectoryInfo(path);

			var directories = AllDirs(info);

			if (directories.Count == 0)
			{
				return NotFound();
			}
			return Ok(directories);
		}

		private string MakePath(string dir)
		{
			//todo change this
			return dir.Replace("[", "").Replace("]", "").Replace(@"""", "").Replace(",", "\\");
		}

		public  static class FilesCount
		{
			static FilesCount()
			{
				Small = 0;
				Medium = 0;
				Height = 0;
			}

			public static int Small { get; set; }

			public static int Medium { get; set; }

			public static int Height { get; set; }


			public static void AddSmall()
			{
				Small += 1;
		
			}
			public static void AddMedium()
			{
				Medium += 1;
			}
			public static void AddHeight()
			{
				Height += 1;
			}

			public static void Clear()
			{
				Small = 0;
				Medium = 0;
				Height = 0;
			}
		}

		/// <summary>
		/// resursive obtain information about files in directory and subdirectories
		/// </summary>
		/// <param name="root"></param>
		/// <param name="deepLevel"></param>
		private static void WalkDirectoryTree(DirectoryInfo root, int deepLevel)
		{

			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;
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
				//string fileFullName = file.FullName;
				double fileSize = Convert.ToDouble(file.Length);
				if (fileSize != 0)
				{
					fileSize /= 1024;
				}
				Debug.WriteLine("{0}", fileSize);
				fileSize /= 1024;
				if (fileSize < 10)
				{
					FilesCount.AddSmall();
				}
				else if (fileSize < 50)
				{
					FilesCount.AddMedium();
				}
				else
				{
					FilesCount.AddHeight();

				}
			}
			if (deepLevel <= 0)
			{
				subDirs = root.GetDirectories();
				foreach (DirectoryInfo dirInfo in subDirs)
				{
					WalkDirectoryTree(dirInfo, deepLevel);
				}
			}
		}
	}
}
