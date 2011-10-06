using System;
using System.Threading;
using System.Data.Linq;

namespace X264ConversionWrapper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			LoggerInitializer.Initialize();
			
			try
			{
				X264Converter converter = new X264Converter("/home/grerlrr/sample.wmv");
				converter.Convert();
			}
			catch (SourceInfo.SourceInfoParsingException ex)
			{
				Console.WriteLine(ex.Message);
			}
			
		}
		
	}
}
