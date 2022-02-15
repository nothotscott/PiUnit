using System;
using System.Collections.Generic;
using System.Text;

using System.Device.Gpio;

namespace PiUnit.Client.Functions
{
	public interface ICancellableFunction
	{
		public void CancelFunction();
	}

	public abstract class Function
	{
		protected object ConfigurationObject { get; set; }
		protected object ContextObject { get; set; }

		public Function(object configurationObject)
		{
			ConfigurationObject = configurationObject;
		}

		protected Function()
		{
		}

		public virtual void SetContext(object contextObject)
		{
			ContextObject = contextObject;
		}

		public abstract bool Probe();

		public abstract void DoFunction();
	}

	public abstract class GpioFunction : Function
	{
		protected Settings.Gpio ConfigurationGpio { get { return (Settings.Gpio)ConfigurationObject; } set { ConfigurationObject = value; } }
		protected GpioController ContextController { get { return (GpioController)ContextObject; } set { ContextObject = value; } }

		public GpioFunction(object configureObject) : base(configureObject)
		{

		}
	}
}
