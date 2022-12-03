using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;

namespace WriteAPI
{
	public class EchoVR : GameInterface
	{
		public EchoVR()
		{
			game = Game.EchoVR;
		}

		public override Transform GetCameraTransform()
		{
			Vector3 position = ReadVector(mem, ConfigurationManager.config.EchoVR.cameraPosition);
			Quaternion rotation = ReadQuaternion(mem, ConfigurationManager.config.EchoVR.cameraRotation);

			return new Transform(position, rotation);
		}

		public override void SetCameraTransform(Transform transform)
		{
			transform.rotation = Quaternion.Normalize(transform.rotation);
			WriteVector(mem, ConfigurationManager.config.EchoVR.cameraPosition, transform.position);
			WriteQuaternion(mem, ConfigurationManager.config.EchoVR.cameraRotation, transform.rotation);
#if DEBUG
			Console.WriteLine($"Wrote new camera transform: {transform}");
#endif
		}

		public override float GetSpeed()
		{
			throw new NotImplementedException();
		}

		public override void HandleRequest(HttpListenerContext context, List<string> parts)
		{
			if (parts[1] == "camera_transform")
			{
				switch (context.Request.HttpMethod)
				{
					case "GET":
						GetCameraTransform(context);
						break;
					case "POST":
						SetCameraTransform(context);
						break;
				}
			}
		}

		private void GetCameraTransform(HttpListenerContext context)
		{
#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
			int debugIndex = 10;
#endif

			Transform transform = GetCameraTransform();
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				ContractResolver = QuaternionContractResolver.Instance
			};

#if DEBUG
			Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
			sw.Restart();
#endif

			string data = JsonConvert.SerializeObject(transform, settings) + "\n";

#if DEBUG
			Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
			sw.Restart();
#endif

			HttpListenerResponse response = context.Response;
			response.AddHeader("Content-Type", "application/json; charset=utf-8");

			byte[] buffer = Encoding.UTF8.GetBytes(data);
			response.ContentLength64 = buffer.Length;

#if DEBUG
			Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
			sw.Restart();
#endif

			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();

#if DEBUG
			Console.WriteLine($"{debugIndex++}\t{sw.ElapsedTicks}");
			sw.Restart();
#endif
		}

		private void SetCameraTransform(HttpListenerContext context)
		{
			string data;
			using (StreamReader reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
			{
				data = reader.ReadToEnd();
			}

			Transform transform;
			HttpListenerResponse response = context.Response;

			try
			{
				transform = JsonConvert.DeserializeObject<Transform>(data);
			}
			catch (JsonException e)
			{
				Console.WriteLine("Invalid POST request");
#if DEBUG
				Console.WriteLine(e.Message);
#endif
				response.StatusCode = 400;
				response.OutputStream.Close();
				return;
			}

			if (transform == null)
			{
				Console.WriteLine("Invalid POST request");
				response.StatusCode = 400;
				response.OutputStream.Close();
				return;
			}

			SetCameraTransform(transform);

			GetCameraTransform(context);
		}
	}
}