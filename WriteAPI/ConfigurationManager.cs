using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

namespace WriteAPI
{
	public static class ConfigurationManager
	{
		const string RemoteConfigPath = @"https://raw.githubusercontent.com/Graicc/WriteAPI/master/WriteAPI/config.json";
		const string ConfigPath = "config.json";

		public struct Config
		{
			public string cameraPositionAddress;
			public string cameraRotationAddress;
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

		static void TryUpdateConfigFile()
		{
			Console.WriteLine("Checking for updated config file...");
			using (var httpClient = new HttpClient()) {
				var response = httpClient.GetAsync(RemoteConfigPath).Result;
				if (response.IsSuccessStatusCode)
				{
					using (Stream output = File.OpenWrite(ConfigPath))
					{
						response.Content.CopyToAsync(output).Wait();
					}
					Console.WriteLine("Updated config file");
				} else
				{
					Console.WriteLine("Could not get remote config file");
				}
			}
		}

		static Config ReadLocalConfig()
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
			Console.WriteLine($"Position address: {localConfig.cameraPositionAddress} | Rotation address: {localConfig.cameraRotationAddress}");
#endif

			return localConfig;
		}
	}
}
