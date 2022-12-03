using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WriteAPI
{
	public class LoneEcho : GameInterface
	{
		public LoneEcho()
		{
			game = Game.LoneEcho;
		}

		public override Transform GetCameraTransform()
		{
			throw new NotImplementedException();
		}

		public override void SetCameraTransform(Transform t)
		{
			throw new NotImplementedException();
		}

		public override float GetSpeed()
		{
			if (!hooked) return -1;
			return mem.ReadFloat(ConfigurationManager.config.LoneEcho.speed, round: false);
		}

		public override void HandleRequest(HttpListenerContext context, List<string> parts)
		{
			if (parts[1] == "speed")
			{
				string data = "{\"speed\": " + GetSpeed() + "}";

				HttpListenerResponse response = context.Response;
				response.AddHeader("Content-Type", "application/json; charset=utf-8");

				byte[] buffer = Encoding.UTF8.GetBytes(data);
				response.ContentLength64 = buffer.Length;

				response.OutputStream.Write(buffer, 0, buffer.Length);
				response.OutputStream.Close();
			}
		}
	}
}