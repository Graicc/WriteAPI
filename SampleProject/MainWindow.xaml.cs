using System;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace SampleProject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const string Endpoint = "http://127.0.0.1:6723/";

		static readonly HttpClient client = new HttpClient();
		private static float duration = 3;

		public class CameraTransform
		{
			public Vector3 position = Vector3.Zero;
			public Quaternion rotation = Quaternion.Identity;

			public CameraTransform() { }

			public CameraTransform(Vector3 pos, Quaternion rot)
			{
				position = pos;
				rotation = rot;
			}
		}

		static CameraTransform start = new CameraTransform();
		static CameraTransform end = new CameraTransform();

		static Thread followPathThread = new Thread(FollowPathThread);

		public MainWindow()
		{
			InitializeComponent();

			Closed += new EventHandler(MainWindow_Closed);
		}

		void MainWindow_Closed(object sender, EventArgs e)
		{
			client.Dispose();
		}

		private void SetStart_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				HttpResponseMessage response = client.GetAsync(Endpoint).Result;
				string data = response.Content.ReadAsStringAsync().Result;
				start = JsonConvert.DeserializeObject<CameraTransform>(data);
			} catch (Exception E)
			{
				Console.WriteLine(E.Message);
			}
		}

		private void SetEnd_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				HttpResponseMessage response = client.GetAsync(Endpoint).Result;
				string data = response.Content.ReadAsStringAsync().Result;
				end = JsonConvert.DeserializeObject<CameraTransform>(data);
			} catch (Exception E)
			{
				Console.WriteLine(E.Message);
			}
		}

		private void FollowPath_Click(object sender, RoutedEventArgs e)
		{
			if (!followPathThread.IsAlive)
			{
				float.TryParse(TimeInput.Text, out duration);
				followPathThread = new Thread(FollowPathThread);
				followPathThread.Start();
			}
		}

		static void FollowPathThread()
		{
			DateTime startTime = DateTime.Now;
			DateTime currentTime = startTime;
			float t = 0;
			while (t < 1)
			{
				currentTime = DateTime.Now;
				float elapsed = (currentTime.Ticks - startTime.Ticks) / 10000000f;
				t = elapsed / duration;

				Vector3 newPos = Vector3.Lerp(start.position, end.position, t);
				Quaternion newRot = Quaternion.Slerp(start.rotation, end.rotation, t);

				CameraTransform newTransform = new CameraTransform(newPos, newRot);

				string data = JsonConvert.SerializeObject(newTransform);

				using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), Endpoint))
				{
					request.Content = new StringContent(data);
					client.SendAsync(request).Wait();
				}
			}
		}
	}
}
