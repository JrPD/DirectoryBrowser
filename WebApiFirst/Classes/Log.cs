using System.Text;

namespace WebApiFirst.Classes
{
	public class Log
	{
		private static Log _instance;

		private static object syncLock = new object();

		private StringBuilder _logger = new StringBuilder();

		protected Log()
		{
		}

		public static Log GetLog()
		{
			if (_instance == null)
			{
				lock (syncLock)
				{
					if (_instance == null)
					{
						_instance = new Log();
					}
				}
			}
			return _instance;
		}

		public void Write(string message)
		{
			lock (syncLock)
			{
				_logger.AppendLine(message);
			}
		}

		public override string ToString()
		{
			return _logger.ToString();
		}

		public void Clear()
		{
			_logger.Clear();
		}
	}
}