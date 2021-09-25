using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Memory;

namespace WriteAPI
{
	public static class GameInterface
	{
		const string GameName = "echovr.exe";

		public static CameraTransform CameraTransform
		{
			get
			{
				return GetTransform();
			}
			set
			{
				UpdateTransform(value);
			}
		}

		static Mem mem = new Mem();

		public static void Hook()
		{
			Console.WriteLine("Hooking Echo VR...");

			bool hooked;
			do
			{
				hooked = mem.OpenProcess(GameName);
				if (!hooked)
				{
					Console.WriteLine("Could not find Echo VR process, make sure Echo VR is running");
					Console.WriteLine("Trying again in 5 seconds...");
					System.Threading.Thread.Sleep(5000);
				}
			} while (!hooked);

			Console.WriteLine("Hooked Echo VR");
		}

		static void UpdateTransform(CameraTransform transform)
		{
			transform.rotation = Quaternion.Normalize(transform.rotation);
			WriteVector(ConfigurationManager.config.cameraPositionAddress, transform.position);
			WriteQuaternion(ConfigurationManager.config.cameraRotationAddress, transform.rotation);
#if DEBUG
			Console.WriteLine($"Wrote new camera transform: {transform}");
#endif
		}

		static CameraTransform GetTransform()
		{
			Vector3 position = ReadVector(ConfigurationManager.config.cameraPositionAddress);
			Quaternion rotation = ReadQuaternion(ConfigurationManager.config.cameraRotationAddress);
			return new CameraTransform(position, rotation);
		}

		static string AddressWithOffset(string address, int offset)
		{
			string baseString = address.Substring(0, address.Length - 3);
			string end = address.Substring(address.Length - 2);

			int preoffset = Convert.ToInt32($"0x{end}", 16);
			return baseString + (preoffset + offset).ToString("X2");
		}

		static void WriteVector(string address, Vector3 vector)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", vector.X.ToString());
			mem.WriteMemory(AddressWithOffset(address, 4), "float", vector.Y.ToString());
			mem.WriteMemory(AddressWithOffset(address, 8), "float", vector.Z.ToString());
		}

		static void WriteQuaternion(string address, Quaternion quaternion)
		{
			mem.WriteMemory(AddressWithOffset(address, 0), "float", quaternion.X.ToString());
			mem.WriteMemory(AddressWithOffset(address, 4), "float", quaternion.Y.ToString());
			mem.WriteMemory(AddressWithOffset(address, 8), "float", quaternion.Z.ToString());
			mem.WriteMemory(AddressWithOffset(address, 12), "float", quaternion.W.ToString());
		}

		static Vector3 ReadVector(string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			return new Vector3(x, y, z);
		}

		static Quaternion ReadQuaternion(string address)
		{
			float x = mem.ReadFloat(AddressWithOffset(address, 0), round: false);
			float y = mem.ReadFloat(AddressWithOffset(address, 4), round: false);
			float z = mem.ReadFloat(AddressWithOffset(address, 8), round: false);
			float w = mem.ReadFloat(AddressWithOffset(address, 12), round: false);
			return new Quaternion(x, y, z, w);
		}
	}
}
