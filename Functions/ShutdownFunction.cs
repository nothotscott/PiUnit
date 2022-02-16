using System;
using System.Collections.Generic;
using System.Text;

using System.Device.Gpio;
using System.Runtime.InteropServices;

namespace PiUnit.Client.Functions
{
	public class ShutdownFunction : GpioFunction, ICancellableFunction
	{
		[DllImport("libc.so.6", SetLastError = true)]
		private static extern Int32 reboot(Int32 cmd, IntPtr arg);
		private const Int32 LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;

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
				/*int ret = reboot(LINUX_REBOOT_CMD_POWER_OFF, IntPtr.Zero);
				if (ret == 0 || ret == -1)
				{
					Int32 errno = Marshal.GetLastWin32Error();
					Console.WriteLine("Shutdown error {0}", errno);
				}*/
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
