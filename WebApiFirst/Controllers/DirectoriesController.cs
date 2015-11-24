using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using WebApiFirst.Classes;
using WebApiFirst.Models;


namespace WebApiFirst.Controllers
{
	public class DirectoriesController : ApiController
	{
		// init directories
		public IEnumerable<DirectoryItems> GetDirectories()
		{
			string path = HttpRuntime.AppDomainAppPath;
			var info = new DirectoryInfo(path);

			var manager = new DirManager();
			List<DirectoryItems> directories = manager.GetDirs(info);

			return directories;
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dir">selected directory</param>
		/// <returns>subdirectories with parent directory</returns>
		public IHttpActionResult GetDirectories(string dir)
		{
			lock (DirManager.TraceErrors)
			{
				DirManager.TraceErrors.Clear();
			}

			var manager = new DirManager();
			var info = manager.GetDirInfo(dir);

			var directories = manager.GetDirs(info);

			if (directories.Count == 0)
			{
				return NotFound();
			}

			if (DirManager.TraceErrors.Length != 0)
			{
				return Content(HttpStatusCode.BadRequest, DirManager.TraceErrors.ToString());
			}
			return Ok(directories);
		}
	}
}
