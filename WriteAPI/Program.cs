using System;

namespace WriteAPI
{
	public static class Program
	{
		private static void Main(string[] args)
		{
			// --noupdateconfig prevents config from being updated
			bool updateConfig = true;
			if (args.Length > 0)
			{
				if (args[0] == "--noupdateconfig")
				{
					updateConfig = false;
				}
			}

			ConfigurationManager.UpdateConfig(updateConfig);
			Console.WriteLine();

			Hooker.Start();
			Listener.Start();
		}
	}
}