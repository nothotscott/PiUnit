using System;

using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace PiUnit.Client
{
	class Program
	{
		private static Client Client;
		
		static void Main(string[] args)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			using (FileStream fileStream = File.Open("Settings.xml", FileMode.Open, FileAccess.Read))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				Settings.Global = (Settings)serializer.Deserialize(fileStream);
				Client = new Client();
			}
			
			while (Client.Loop())
			{
				Thread.Sleep(Settings.Global.PollInterval);
			}

			Client.Dispose();
		}
	}
}
