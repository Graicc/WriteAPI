using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace WriteAPI
{
	public static class Listener
	{
		// Spark already uses port 6722 for discord OAuth, so we use the next available port
		private static readonly string[] Prefixes = {"http://127.0.0.1:6723/", "http://localhost:6723/"};

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

				switch (request.RawUrl)
				{
					case "/camera_transform":
						switch (request.HttpMethod)
						{
							case "GET":
								ProcessGet(context);
								break;
							case "POST":
								ProcessPost(context);
								break;
						}
						break;
					case "/le1/speed":
						ReturnSpeed(context, 1);
						break;
					case "/le2/speed":
						ReturnSpeed(context, 2);
						break;
				}
			}
		}

		private static void ProcessGet(HttpListenerContext context)
		{
			CameraTransform transform = GameInterface.CameraTransform;
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				ContractResolver = QuaternionContractResolver.Instance
			};

			string data = JsonConvert.SerializeObject(transform, settings) + "\n";

			HttpListenerResponse response = context.Response;
			response.AddHeader("Content-Type", "application/json; charset=utf-8");

			byte[] buffer = Encoding.UTF8.GetBytes(data);
			response.ContentLength64 = buffer.Length;

			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}

		private static void ProcessPost(HttpListenerContext context)
		{
			string data;
			using (StreamReader reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
			{
				data = reader.ReadToEnd();
			}

			CameraTransform transform;
			HttpListenerResponse response = context.Response;

			try
			{
				transform = JsonConvert.DeserializeObject<CameraTransform>(data);
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

			GameInterface.CameraTransform = transform;

			ProcessGet(context);
		}
		
		private static void ReturnSpeed(HttpListenerContext context, int loneEchoVersion)
		{
			switch (loneEchoVersion)
			{
				case 1:
				{
					string data = "{\"speed\": " + GameInterface.LoneEchoSpeed + "}";

					HttpListenerResponse response = context.Response;
					response.AddHeader("Content-Type", "application/json; charset=utf-8");

					byte[] buffer = Encoding.UTF8.GetBytes(data);
					response.ContentLength64 = buffer.Length;

					response.OutputStream.Write(buffer, 0, buffer.Length);
					response.OutputStream.Close();
					break;
				}
				case 2:
				{
					string data = "{\"speed\": " + GameInterface.LoneEcho2Speed + "}";

					HttpListenerResponse response = context.Response;
					response.AddHeader("Content-Type", "application/json; charset=utf-8");

					byte[] buffer = Encoding.UTF8.GetBytes(data);
					response.ContentLength64 = buffer.Length;

					response.OutputStream.Write(buffer, 0, buffer.Length);
					response.OutputStream.Close();
					break;
				}
			}
		}
	}
}