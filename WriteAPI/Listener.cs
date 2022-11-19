using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Linq;

namespace WriteAPI
{
	public static class Listener
	{
		// Spark already uses port 6722 for discord OAuth, so we use the next available port
		private static readonly string[] Prefixes = { "http://127.0.0.1:6723/", "http://localhost:6723/" };

		public static void Start()
		{
			Thread listenerThread = new Thread(ListenerThread);
			listenerThread.Start();
		}

		private static void ListenerThread()
		{
			HttpListener listener = new HttpListener();

			foreach (string prefix in Prefixes)
			{
				listener.Prefixes.Add(prefix);
			}

			listener.Start();

			Console.WriteLine($"Listening on {string.Join(", ", Prefixes)}");

			while (true)
			{
				HttpListenerContext context = listener.GetContext();
				HttpListenerRequest request = context.Request;

#if DEBUG
				Stopwatch sw = Stopwatch.StartNew();
				int debugIndex = 20;
#endif

				if (request.RawUrl != null)
				{
					List<string> parts = request.RawUrl.Split('/').ToList();
					parts.RemoveAll(string.IsNullOrWhiteSpace);

#if DEBUG
					Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
					sw.Restart();
#endif

					switch (parts[0])
					{
						case "echovr":
							Hooker.Games[GameInterface.Game.EchoVR].HandleRequest(context, parts);
							break;
						case "le1":
							Hooker.Games[GameInterface.Game.LoneEcho].HandleRequest(context, parts);
							break;
						case "le2":
							Hooker.Games[GameInterface.Game.LoneEcho2].HandleRequest(context, parts);
							break;
					}
				}
#if DEBUG
				Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
				sw.Restart();
#endif
			}
		}
	}
}