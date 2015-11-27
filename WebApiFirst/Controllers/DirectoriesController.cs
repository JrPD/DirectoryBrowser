using System.Collections.Generic;
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
		private Log _loger = Log.GetLog();

		// init directories
		public IEnumerable<DirectoryItems> GetDirectories()
		{
			_loger.Clear();
			string path = HttpRuntime.AppDomainAppPath;
			var info = new DirectoryInfo(path);

			var manager = new DirManager();
			List<DirectoryItems> directories = manager.GetDirectoriesFromDirectory(info);

			return directories;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dir">selected directory</param>
		/// <returns>subdirectories with parent directory</returns>
		public IHttpActionResult GetDirectories(string dir)
		{
			_loger.Clear();
			var manager = new DirManager();
			var info = DirInformation.GetDirInfo(dir);

			var directories = manager.GetDirectoriesFromDirectory(info);

			if (_loger.ToString().Length != 0)
			{
				return Content(HttpStatusCode.BadRequest, _loger.ToString());
			}

			if (directories.Count == 0)
			{
				return NotFound();
			}
			return Ok(directories);
		}
	}
}