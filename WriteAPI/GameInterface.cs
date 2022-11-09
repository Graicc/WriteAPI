using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Memory;

namespace WriteAPI
{
	public static class GameInterface
	{
		public static CameraTransform CameraTransform
		{
			get => GetTransform();
			set => UpdateTransform(value);
		}

		public static float LoneEchoSpeed => GetLoneEchoSpeed();
		public static float LoneEcho2Speed => GetLoneEcho2Speed();

		private enum Game
		{
			EchoVR,
			LoneEcho,
			LoneEcho2
		}

		private static readonly Dictionary<Game, Mem> mems = new Dictionary<Game, Mem>()
		{
			{ Game.EchoVR, new Mem() },
			{ Game.LoneEcho, new Mem() },
			{ Game.LoneEcho2, new Mem() },
		};

		private static readonly Dictionary<Game, bool> hooked = new Dictionary<Game, bool>()
		{
			{ Game.EchoVR, false },
			{ Game.LoneEcho, false },
			{ Game.LoneEcho2, false },
		};

		private static readonly Dictionary<Game, string> exes = new Dictionary<Game, string>()
		{
			{ Game.EchoVR, "echovr.exe" },
			{ Game.LoneEcho, "loneecho.exe" },
			{ Game.LoneEcho2, "loneecho2.exe" },
		};

		private static void HookIfNotHooked(Game game)
		{
			if (!hooked[game]) Hook(game);
		}

		private static void Hook(Game game)
		{
			bool hookedNow = mems[game].OpenProcess(exes[game]);
			if (hookedNow && !hooked[game])
			{
				Console.WriteLine($"Found {game} Process");
			}

			hooked[game] = hookedNow;
		}

		private static void UpdateTransform(CameraTransform transform)
		{
			HookIfNotHooked(Game.EchoVR);

			transform.rotation = Quaternion.Normalize(transform.rotation);
			WriteVector(mems[Game.EchoVR], ConfigurationManager.config.cameraPositionAddress, transform.position);
			WriteQuaternion(mems[Game.EchoVR], ConfigurationManager.config.cameraRotationAddress, transform.rotation);
#if DEBUG
			Console.WriteLine($"Wrote new camera transform: {transform}");
#endif

			// if we became unhooked, rehook for the next request
			Task.Run(() => { Hook(Game.EchoVR); });
		}

		private static CameraTransform GetTransform()
		{
			HookIfNotHooked(Game.EchoVR);

			Vector3 position = ReadVector(mems[Game.EchoVR], ConfigurationManager.config.cameraPositionAddress);
			Quaternion rotation = ReadQuaternion(mems[Game.EchoVR], ConfigurationManager.config.cameraRotationAddress);

			// if we became unhooked, rehook for the next request
			Task.Run(() => { Hook(Game.EchoVR); });
			
			return new CameraTransform(position, rotation);
		}

		private static float GetLoneEchoSpeed()
		{
			HookIfNotHooked(Game.LoneEcho);

			if (mems[Game.LoneEcho].OpenProcess("loneecho"))
			{
				return mems[Game.LoneEcho].ReadFloat(ConfigurationManager.config.loneEchoSpeedAddress, round: false);
			}

			return -1;
		}

		private static float GetLoneEcho2Speed()
		{
			HookIfNotHooked(Game.LoneEcho2);

			if (mems[Game.LoneEcho2].OpenProcess("loneecho2"))
			{
				return mems[Game.LoneEcho2].ReadFloat(ConfigurationManager.config.loneEcho2SpeedAddress, round: false);
			}

			return -1;
		}


		private static string AddressWithOffset(string address, int offset)
		{
			string baseString = address[..^3];
			string end = address[^2..];

			int preOffset = Convert.ToInt32($"0x{end}", 16);
			return baseString + (preOffset + offset).ToString("X2");
		}

		private static void WriteVector(Mem mem, string address, Vector3 vector)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", vector.X.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 4), "float", vector.Y.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 8), "float", vector.Z.ToString(CultureInfo.InvariantCulture));
		}

		private static void WriteQuaternion(Mem mem, string address, Quaternion quaternion)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", quaternion.X.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 4), "float", quaternion.Y.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 8), "float", quaternion.Z.ToString(CultureInfo.InvariantCulture));
			mem.WriteMemory(AddressWithOffset(address, 12), "float", quaternion.W.ToString(CultureInfo.InvariantCulture));
		}

		private static Vector3 ReadVector(Mem mem, string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			return new Vector3(x, y, z);
		}

		private static Quaternion ReadQuaternion(Mem mem, string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			float w = mem.ReadFloat(AddressWithOffset(address, 12), round: false);
			return new Quaternion(x, y, z, w);
		}
	}
}