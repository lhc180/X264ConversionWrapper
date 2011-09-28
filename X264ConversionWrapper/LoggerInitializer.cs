using System;
using System.Reflection;
using log4net.Config;

namespace X264ConversionWrapper
{
	public static class LoggerInitializer
	{
		private static bool initialized = false;
		public const String DEFAULT_LOGGER_CONFIG_FILE = "log.conf"; 
		
		public static void Initialize()
		{
			if (initialized) return;
			
			Assembly asm = Assembly.GetExecutingAssembly();
			
			String path = System.IO.Path.GetDirectoryName(asm.Location);
			
			path = System.IO.Path.Combine(path, DEFAULT_LOGGER_CONFIG_FILE);
			
			XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(path));
		}
	}
}

