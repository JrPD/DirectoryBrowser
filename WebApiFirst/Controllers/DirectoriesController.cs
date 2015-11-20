using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using DirectoryItems = WebApiFirst.Models.DirectoryItems;

namespace WebApiFirst.Controllers
{
	public class DirectoriesController : ApiController
	{
		// to recursive function WalkDirectoryTree
		private readonly int _deepLevel;

		private static StringBuilder TraceErrors = new StringBuilder();


		public DirectoriesController()
		{
			//set deep level from web.config
			int.TryParse(ConfigurationManager.AppSettings["deepLevel"], out _deepLevel);
			if (_deepLevel==0)
			{
				_deepLevel = 2;
			}
		}

		// init directories
		public IEnumerable<DirectoryItems> GetAlldirectories()
		{
			string path = HttpRuntime.AppDomainAppPath;
			var info = new System.IO.DirectoryInfo(path);

			List<DirectoryItems> directories = AllDirs(info);

			return directories;
		}

		public List<DirectoryItems> AllDirs(System.IO.DirectoryInfo info)
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

			if (info==null)
			{
				return items;
			}
			CountFiles(info);

			// if root, add discs
			if (info.Name.ToLower() == info.Root.Name)
			{
				items.Add(new DirectoryItems()
				{
					Path = "c:\\", Name = "C",
					FilesSmall = FilesCount.Small,
					FilesHeight = FilesCount.Height,
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
						FilesHeight = FilesCount.Height,
						FilesMedium =  FilesCount.Medium});
			}

			// add directories to list
			if(dir!=null)
				items.AddRange(dir.Select(item => new DirectoryItems { Path = item.FullName, Name = item.Name }));
			
			// add files to list
			if (files!=null)
				items.AddRange(files.Select(file => new DirectoryItems { Path = file.FullName, Name = file.Name }));
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
			Debug.WriteLine("{0} - {1}", "10<X<50", FilesCount.Medium);
			Debug.WriteLine("{0} - {1}", ">50", FilesCount.Height);
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
			lock (TraceErrors)
			{
				TraceErrors.Clear();
			}
			var directories = AllDirs(info);

			if (directories.Count == 0)
			{
				return NotFound();
			}

			
			if (TraceErrors.Length != 0)
			{
				return Content(HttpStatusCode.BadRequest, TraceErrors.ToString());
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
		//todo height - large
		enum FileSizes 
		{
			Small=10,
			Medium =50,
		}
		/// <summary>
		/// resursive obtain information about files in directory and subdirectories
		/// </summary>
		/// <param name="root"></param>
		/// <param name="deepLevel"></param>
		private static void WalkDirectoryTree(DirectoryInfo root, int deepLevel)
		{
			const int mbyte = 1024*1024;
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
				
				if (fileSize < (int) FileSizes.Small)
				{
					FilesCount.AddSmall();
				}
				else if (fileSize < (int)FileSizes.Medium)
				{
					FilesCount.AddMedium();
				}
				else
				{
					FilesCount.AddHeight();
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
	}
}
