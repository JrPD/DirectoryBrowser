namespace WebApiFirst.Classes
{
	public static class FilesCount
	{
		static FilesCount()
		{
			Small = 0;
			Medium = 0;
			Large = 0;
		}

		public static int Small { get; private set; }

		public static int Medium { get; private set; }

		public static int Large { get; private set; }

		public static void AddSmall()
		{
			Small += 1;
		}

		public static void AddMedium()
		{
			Medium += 1;
		}

		public static void AddLarge()
		{
			Large += 1;
		}

		public static void Clear()
		{
			Small = 0;
			Medium = 0;
			Large = 0;
		}
	}

	public enum FileSizes
	{
		Small = 10,
		Medium = 50,
		Large = 100
	}
}