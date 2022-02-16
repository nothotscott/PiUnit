using System;

using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace PiUnit.Client
{
	class Program
	{
		private static ThreadPriority Priority = ThreadPriority.Highest;
		private static Client Client;

		private static Thread GpioThread;
		private static Thread NetworkThread;
		
		static void Main(string[] args)
		{
			Thread.CurrentThread.Priority = Priority;

			using (FileStream fileStream = File.Open("Settings.xml", FileMode.Open, FileAccess.Read))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				Settings.Global = (Settings)serializer.Deserialize(fileStream);
				Client = new Client();
			}

			GpioThread = new Thread(() =>
			{
				while (Client.GpioLoop())
				{
					Thread.Sleep(Settings.Global.GpioPollInterval);
				}
			});
			GpioThread.Priority = Priority;
			GpioThread.Start();

			NetworkThread = new Thread(() =>
			{
				while (Client.NetworkLoop())
				{
					Thread.Sleep(Settings.Global.NetworkSendInterval);
				}
			});
			NetworkThread.Priority = Priority;
			NetworkThread.Start();
		}
	}
}
