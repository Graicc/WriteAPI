using System;
using System.Linq;
using System.Threading;

namespace WriteAPI
{
	public static class Program
	{
		public static string game = "echovr";

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

				if (args.Contains("--game"))
				{
					string gameArg = args[args.ToList().IndexOf("--game") + 1];
					game = gameArg switch
					{
						"echovr" => gameArg,
						"loneecho" => gameArg,
						"loneecho2" => gameArg,
						_ => game
					};
				}
			}

			ConfigurationManager.UpdateConfig(updateConfig);
			Console.WriteLine();

			// Constantly checks to see if still hooked
			// new Thread(GameInterface.Hook).Start();

			Listener.Start();
		}
	}
}