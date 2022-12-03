using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Numerics;
using Memory;

namespace WriteAPI
{
	public abstract class GameInterface
	{
		public enum Game
		{
			EchoVR,
			LoneEcho,
			LoneEcho2
		}

		private static readonly Dictionary<Game, string> exes = new Dictionary<Game, string>()
		{
			{ Game.EchoVR, "echovr.exe" },
			{ Game.LoneEcho, "loneecho.exe" },
			{ Game.LoneEcho2, "loneecho2.exe" },
		};

		public Game game;
		public Mem mem = new Mem();
		public bool hooked;


		protected void HookIfNotHooked()
		{
			if (!hooked) Hook();
		}

		public void Hook()
		{
			bool hookedNow = mem.OpenProcess(exes[game]);
			if (hookedNow && !hooked)
			{
				Console.WriteLine($"Found {game} Process");
			}

			hooked = hookedNow;
		}

		// These are methods that could be common for all game types, but there is no reason not to implement methods
		// at the game level and cast to the appropriate game in the Listener
		public abstract Transform GetCameraTransform();
		public abstract void SetCameraTransform(Transform t);
		public abstract float GetSpeed();
		public abstract void HandleRequest(HttpListenerContext context, List<string> parts);


		private static string AddressWithOffset(string address, int offset)
		{
			string baseString = address[..^3];
			string end = address[^2..];

			int preOffset = Convert.ToInt32($"0x{end}", 16);
			return baseString + (preOffset + offset).ToString("X2");
		}

		protected static void WriteVector(Mem mem, string address, Vector3 vector)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", vector.X.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 4), "float", vector.Y.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 8), "float", vector.Z.ToString(CultureInfo.InvariantCulture));
		}

		protected static void WriteQuaternion(Mem mem, string address, Quaternion quaternion)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", quaternion.X.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 4), "float", quaternion.Y.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 8), "float", quaternion.Z.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 12), "float", quaternion.W.ToString(CultureInfo.InvariantCulture));
		}

		protected static Vector3 ReadVector(Mem mem, string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			return new Vector3(x, y, z);
		}

		protected static Quaternion ReadQuaternion(Mem mem, string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			float w = mem.ReadFloat(AddressWithOffset(address, 12), round: false);
			return new Quaternion(x, y, z, w);
		}
	}
}