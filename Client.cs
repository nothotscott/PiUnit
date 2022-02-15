using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Device.Gpio;

using PiUnit.Client.Functions;

namespace PiUnit.Client
{
	public class Client : IDisposable
	{
		private GpioController Controller { get; set; }
		private List<Function> Functions { get; set; }

		public Client()
		{
			Functions = new List<Function>();

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

		public bool Loop()
		{
			if (Controller != null)
			{
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
			}

			return true;
		}

		public void Dispose()
		{
			Controller?.Dispose();
		}
	}
}
