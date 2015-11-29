using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using WebApiFirst.Classes;
using WebApiFirst.Classes.Models;

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

			var manager = new DirManager(new DirectoryInfo(path));
			List<DirectoryItems> directories = manager.GetDirectoriesFromDirectory();

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

			var manager = new DirManager(DirInformation.GetDirInfo(dir));

			var directories = manager.GetDirectoriesFromDirectory();

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