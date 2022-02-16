using System;
using System.Collections.Generic;
using System.Text;

using System.Device.Gpio;

namespace PiUnit.Client.Functions
{
	public class ShutdownFunction : GpioFunction, ICancellableFunction
	{
		private DateTime? PendingShutdown;

		public ShutdownFunction(object configureObject) : base(configureObject)
		{
			PendingShutdown = null;
		}

		public override bool Probe()
		{
			return ContextController.Read(3) == PinValue.High;
		}

		public override void DoFunction()
		{
			if (PendingShutdown == null)
			{
				PendingShutdown = DateTime.Now;
				Console.WriteLine("Pending shutdown");
			}
			else if (DateTime.Now - PendingShutdown >= TimeSpan.FromSeconds(Settings.Global.GpioShutdownTimer))
			{
				Console.WriteLine("Shutdown time");
			}
		}

		public void CancelFunction()
		{
			if (PendingShutdown != null)
			{
				PendingShutdown = null;
				Console.WriteLine("Cancel shutdown");
			}
		}
	}
}
