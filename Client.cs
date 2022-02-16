using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Device.Gpio;

using PiUnit.Client.Functions;

namespace PiUnit.Client
{
	public class Client : IDisposable
	{
		private List<Function> Functions { get; set; }
		private GpioController Controller { get; set; }
		private UdpClient NetworkClient;

		public Client()
		{
			Functions = new List<Function>();

			try
			{
				NetworkClient = new UdpClient(Settings.Global.NetworkHost, Settings.Global.NetworkPort);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			try
			{
				Controller = new GpioController(PinNumberingScheme.Logical);

				Type FunctionEnumType = typeof(Settings.Gpio.FunctionEnum);

				foreach (Settings.Gpio Pin in Settings.Global.GpioList)
				{
					object[] PinFunctionAttributes = FunctionEnumType.GetMember(Pin.Function.ToString())
						.FirstOrDefault(member => member.DeclaringType == FunctionEnumType)
						.GetCustomAttributes(false);

					
					if (Pin.Enabled)
					{
						Settings.Gpio.FunctionReadMemberAttribute ReadFunctionAttribute = (Settings.Gpio.FunctionReadMemberAttribute)PinFunctionAttributes
							.FirstOrDefault(attribute => attribute.GetType() == typeof(Settings.Gpio.FunctionReadMemberAttribute));
						Settings.Gpio.FunctionWriteMemberAttribute WriteFunctionAttribute = (Settings.Gpio.FunctionWriteMemberAttribute)PinFunctionAttributes
							.FirstOrDefault(attribute => attribute.GetType() == typeof(Settings.Gpio.FunctionWriteMemberAttribute));

						if (ReadFunctionAttribute != null)
						{
							Controller.OpenPin(Pin.Value, PinMode.InputPullDown);
							Function function = (Function)Activator.CreateInstance(ReadFunctionAttribute.FunctionType, new object[] { Pin });
							function.SetContext(Controller);
							Functions.Add(function);
						}
						else if (WriteFunctionAttribute != null)
						{
							Controller.OpenPin(Pin.Value, PinMode.Output);
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public bool NetworkLoop()
		{
			if (NetworkClient == null)
			{
				return false;
			}

			//NetworkClient.Send();

			return true;
		}

		public bool GpioLoop()
		{
			if (Controller == null)
			{
				return false;
			}

			foreach (Function function in Functions)
			{
				if (function.GetType().IsSubclassOf(typeof(GpioFunction)))
				{
					if (function.Probe())
					{
						function.DoFunction();
					}
					else if (function is ICancellableFunction cancellableFunction)
					{
						cancellableFunction.CancelFunction();
					}
				}
			}
			return true;
		}

		public void Dispose()
		{
			Controller?.Dispose();
			NetworkClient?.Close();
			NetworkClient?.Dispose();
		}
	}
}
