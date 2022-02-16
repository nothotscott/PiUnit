using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;

using PiUnit.Client.Functions;

namespace PiUnit.Client
{
	[XmlRoot("Settings", Namespace = "https://scott.center/PiUnit")]
	public class Settings
	{
		public static Settings Global;

		public int NetworkSendInterval { get; set; }

		public string NetworkHost { get; set; }

		public int NetworkPort { get; set; }

		public int GpioPollInterval { get; set; }

		public int GpioShutdownTimer { get; set; }

		[XmlArray]
		[XmlArrayItem(ElementName = "Gpio")]
		public List<Gpio> GpioList { get; set; }

		public class Gpio
		{
			[XmlAttribute("function")]
			public FunctionEnum Function { get; set; }

			[XmlAttribute("enabled")]
			public bool Enabled { get; set; }

			[XmlText]
			public int Value { get; set; }

			public enum FunctionEnum
			{
				[XmlEnum]
				[FunctionReadMember(typeof(ShutdownFunction))]
				Shutdown
			}

			[System.AttributeUsage(AttributeTargets.Field)]
			public class FunctionReadMemberAttribute : System.Attribute
			{
				public Type FunctionType { get; set; }

				public FunctionReadMemberAttribute(Type functionType)
				{
					FunctionType = functionType;
				}
			}

			[System.AttributeUsage(AttributeTargets.Field)]
			public class FunctionWriteMemberAttribute : System.Attribute
			{

				public FunctionWriteMemberAttribute()
				{
				}
			}
		}
	}
}
