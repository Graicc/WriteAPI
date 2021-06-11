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
		public static readonly string[] Prefixes = { "http://127.0.0.1:6723/", "http://localhost:6723/" };

		public static void Start()
		{
			Thread listenerThread = new Thread(ListenerThread);
			listenerThread.Start();
		}

		static void ListenerThread()
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

				if (request.HttpMethod == "GET")
				{
					ProcessGet(context);
				}
				else if (request.HttpMethod == "POST")
				{
					ProcessPost(context);
				}
			}
		}

		static void ProcessGet(HttpListenerContext context)
		{
			CameraTransform transform = GameInterface.CameraTransform;
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				ContractResolver = QuaternionContractResolver.Instance
			};

			string data = JsonConvert.SerializeObject(transform, settings) + "\n";

			HttpListenerResponse response = context.Response;

			byte[] buffer = Encoding.UTF8.GetBytes(data);
			response.ContentLength64 = buffer.Length;

			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}

		static void ProcessPost(HttpListenerContext context)
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
			catch (JsonException E)
			{
				Console.WriteLine("Invalid POST request");
#if DEBUG
				Console.WriteLine(E.Message);
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
			return;
		}
	}
}
