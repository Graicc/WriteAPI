using System;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

namespace WriteAPI
{
	public static class ConfigurationManager
	{
		private const string RemoteConfigPath = @"https://raw.githubusercontent.com/NtsFranz/WriteAPI/master/WriteAPI/config.json";
		private const string ConfigPath = "config.json";

		public struct Config
		{
			public class EchoVRContainer
			{
				public string cameraPosition;
				public string cameraRotation;
			}

			public class LoneEchoContainer
			{
				public string speed;
			}

			public class LoneEcho2Container
			{
				public string speed;
			}

			public EchoVRContainer EchoVR;
			public LoneEchoContainer LoneEcho;
			public LoneEcho2Container LoneEcho2;
		}

		public static Config config;

		public static void UpdateConfig(bool updateConfig = true)
		{
			if (updateConfig)
			{
				TryUpdateConfigFile();
			}

			config = ReadLocalConfig();
		}

		private static void TryUpdateConfigFile()
		{
			Console.WriteLine("Checking for updated config file...");
			using HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = httpClient.GetAsync(RemoteConfigPath).Result;
			if (response.IsSuccessStatusCode)
			{
				FileInfo fileInfo = new FileInfo(ConfigPath);
				FileMode fileMode = fileInfo.Exists ? FileMode.Truncate : FileMode.Create;
				using (Stream output = File.Open(ConfigPath, fileMode))
				{
					response.Content.CopyToAsync(output).Wait();
				}

				Console.WriteLine("Updated config file");
			}
			else
			{
				Console.WriteLine("Could not get remote config file");
			}
		}

		private static Config ReadLocalConfig()
		{
#if DEBUG
			Console.WriteLine("Reading config");
#endif
			string data;

			using (StreamReader reader = File.OpenText(ConfigPath))
			{
				data = reader.ReadToEnd();
			}

			Config localConfig = JsonConvert.DeserializeObject<Config>(data);

#if DEBUG
			Console.WriteLine($"Position address: {localConfig.EchoVR.cameraPosition} | Rotation address: {localConfig.EchoVR.cameraRotation}");
#endif

			return localConfig;
		}
	}
}