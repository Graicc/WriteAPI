using System;
using System.Numerics;
using Memory;

namespace WriteAPI
{
	public static class GameInterface
	{
		private static string gameExe = "echovr.exe";
		private static string gameName = "Echo VR";

		public static CameraTransform CameraTransform
		{
			get => GetTransform();
			set => UpdateTransform(value);
		}

		public static float LoneEchoSpeed => GetLoneEchoSpeed();
		public static float LoneEcho2Speed => GetLoneEcho2Speed();

		private static readonly Mem echoVRMem = new Mem();
		private static readonly Mem loneEchoMem = new Mem();
		private static readonly Mem loneEcho2Mem = new Mem();

		public static void Hook()
		{
			gameName = Program.game switch
			{
				"echovr" => "Echo VR",
				"loneecho" => "Lone Echo",
				"loneecho2" => "Lone Echo 2",
			};
			gameExe = Program.game switch
			{
				"echovr" => "echovr.exe",
				"loneecho" => "loneecho.exe",
				"loneecho2" => "loneecho2.exe",
			};
			
			Console.WriteLine($"Hooking {gameName}...");

			bool hooked = false;
			while (true)
			{
				if (!echoVRMem.OpenProcess(gameExe))
				{
					if (hooked)
					{
						Console.WriteLine($"Could not find {gameName} process, make sure {gameName} is running");
					}

					hooked = false;
				}
				else
				{
					if (!hooked)
					{
						Console.WriteLine($"Found {gameName} Process");
					}
					hooked = true;
				}
				System.Threading.Thread.Sleep(1000);
			}
		}

		private static void UpdateTransform(CameraTransform transform)
		{
			transform.rotation = Quaternion.Normalize(transform.rotation);
			WriteVector(echoVRMem, ConfigurationManager.config.cameraPositionAddress, transform.position);
			WriteQuaternion(echoVRMem, ConfigurationManager.config.cameraRotationAddress, transform.rotation);
#if DEBUG
			Console.WriteLine($"Wrote new camera transform: {transform}");
#endif
		}

		static CameraTransform GetTransform()
		{
			Vector3 position = ReadVector(echoVRMem, ConfigurationManager.config.cameraPositionAddress);
			Quaternion rotation = ReadQuaternion(echoVRMem, ConfigurationManager.config.cameraRotationAddress);
			return new CameraTransform(position, rotation);
		}

		private static float GetLoneEchoSpeed()
		{
			if (loneEchoMem.OpenProcess("loneecho"))
			{
				return loneEchoMem.ReadFloat(ConfigurationManager.config.loneEchoSpeedAddress, round: false);
			}

			return -1;
		}
		private static float GetLoneEcho2Speed()
		{
			if (loneEcho2Mem.OpenProcess("loneecho2"))
			{
				return loneEcho2Mem.ReadFloat(ConfigurationManager.config.loneEcho2SpeedAddress, round: false);
			}

			return -1;
		}


		private static string AddressWithOffset(string address, int offset)
		{
			int preOffset = Convert.ToInt32(address, 16);
			return address[..^3] + (preOffset + offset).ToString("X2");
		}
		
		private static void WriteVector(Mem mem, string address, Vector3 vector)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", vector.X.ToString());
			mem.WriteMemory(AddressWithOffset(address, 4), "float", vector.Y.ToString());
			mem.WriteMemory(AddressWithOffset(address, 8), "float", vector.Z.ToString());
		}

		private static void WriteQuaternion(Mem mem, string address, Quaternion quaternion)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", quaternion.X.ToString());
			mem.WriteMemory(AddressWithOffset(address, 4), "float", quaternion.Y.ToString());
			mem.WriteMemory(AddressWithOffset(address, 8), "float", quaternion.Z.ToString());
			mem.WriteMemory(AddressWithOffset(address, 12), "float", quaternion.W.ToString());
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
