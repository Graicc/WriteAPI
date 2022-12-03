using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace WriteAPI
{
	public static class Hooker
	{
		private const float HookInterval = 5;

		public static readonly Dictionary<GameInterface.Game, GameInterface> Games = new Dictionary<GameInterface.Game, GameInterface>
		{
			{ GameInterface.Game.EchoVR, new EchoVR() },
			{ GameInterface.Game.LoneEcho, new LoneEcho() },
			{ GameInterface.Game.LoneEcho2, new LoneEcho2() },
		};

		public static void Start()
		{
			Thread hookerThread = new Thread(HookerThread);
			hookerThread.Start();
		}

		private static void HookerThread()
		{
			// this doesn't include time to actually perform the hook, but we don't care about precision
			int millis = (int)(HookInterval * 1000 / Games.Count);
			while (true)
			{
				foreach ((GameInterface.Game game, GameInterface gameInterface) in Games)
				{
#if DEBUG
					Console.WriteLine($"Tried hooking {game}");
					Stopwatch sw = Stopwatch.StartNew();
#endif
					gameInterface.Hook();
#if DEBUG
					Console.WriteLine($"Hook time: {sw.ElapsedMilliseconds} ms");
#endif
					Thread.Sleep(millis);
				}
			}
		}
	}
}